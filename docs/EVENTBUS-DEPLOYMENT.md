# Event Bus Deployment Runbook

The event bus lets HR.API publish integration events (time-clock punches,
submitted timesheets) that Finance.API consumes to drive payroll. It has two
runtime providers, selected by the `EventBus:Provider` configuration key:

| Provider    | Transport                         | When to use                                    |
| ----------- | --------------------------------- | ---------------------------------------------- |
| `InProcess` | In-memory dispatch (single host)  | Default. Same-process only; no cross-service.  |
| `Broker`    | RabbitMQ via MassTransit          | Cross-process. Local self-host **or** Amazon MQ. |

`InProcess` is the default in `appsettings.json`, so existing deployments are
unaffected until you explicitly opt in to `Broker`.

## Configuration keys (`EventBus:RabbitMq`)

| Key          | Env var                              | Default            | Notes                                   |
| ------------ | ------------------------------------ | ------------------ | --------------------------------------- |
| `Host`       | `EventBus__RabbitMq__Host`           | `localhost`        | Hostname only, no scheme/port.          |
| `Port`       | `EventBus__RabbitMq__Port`           | 5672 (5671 if SSL) | AMQP port.                              |
| `UseSsl`     | `EventBus__RabbitMq__UseSsl`         | `false`            | `true` for Amazon MQ (TLS 1.2, amqps).  |
| `VirtualHost`| `EventBus__RabbitMq__VirtualHost`    | `/`                |                                         |
| `Username`   | `EventBus__RabbitMq__Username`       | `guest`            | Inject from secrets in production.      |
| `Password`   | `EventBus__RabbitMq__Password`       | `guest`            | Inject from secrets in production.      |
| `RetryLimit` | `EventBus__RabbitMq__RetryLimit`     | 5                  | Exponential retry before `<queue>_error` DLQ. |

Failed messages are retried with exponential backoff; once the limit is
exhausted MassTransit routes them to the RabbitMQ dead-letter queue
`<queue>_error` for inspection.

---

## Local / fallback: self-hosted RabbitMQ (durable)

`docker-compose.eventbus.yml` runs a `rabbitmq:3-management` broker with a
persistent volume (`rabbitmq_data` → `/var/lib/rabbitmq`) so queued messages and
definitions survive restarts, plus HR.API and Finance.API in `Broker` mode.

```powershell
# Optional: set credentials (otherwise dev defaults are used)
copy .env.example .env   # then edit RABBITMQ_USER / RABBITMQ_PASSWORD

docker compose --env-file .env -f docker-compose.eventbus.yml up --build
```

- RabbitMQ management UI: http://localhost:15672
- HR.API: http://localhost:5041
- Finance.API: http://localhost:5051

Smoke test: punch **"Clock out for the day"** in the app's time clock → a
`TimesheetSubmitted` message appears in RabbitMQ and Finance's
`TimesheetSubmittedHandler` ingests it for payroll.

---

## Production: Amazon MQ for RabbitMQ (managed, TLS)

`docker-compose.eventbus.prod.yml` does **not** run a broker. It expects an
existing Amazon MQ for RabbitMQ broker and connects over TLS (`amqps`, port
5671). All host/credentials come from the environment.

### 1. Provision the broker (one time)
Create an **Amazon MQ for RabbitMQ** broker in the AWS console/IaC. Note the
AMQPS endpoint host (without scheme/port), e.g.
`b-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.mq.us-east-1.amazonaws.com`.

### 2. Store credentials in Secrets Manager
Keep the broker username/password in AWS Secrets Manager (or SSM Parameter
Store). Do **not** commit them.

### 3. Provide env vars at deploy time
Populate a gitignored `.env` (from Secrets Manager) or set them directly in your
ECS task definition / systemd unit:

```
MQ_HOST=b-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.mq.us-east-1.amazonaws.com
MQ_USERNAME=<broker-user>
MQ_PASSWORD=<broker-password>
MQ_VHOST=/
MQ_RETRY_LIMIT=5
```

### 4. Launch
```bash
docker compose --env-file .env -f docker-compose.eventbus.prod.yml up --build -d
```

The compose file fails fast if `MQ_HOST`, `MQ_USERNAME`, or `MQ_PASSWORD` are
missing, so a misconfigured deploy won't silently fall back to defaults.

### Network / security
- Open the Amazon MQ security group to your app subnet on **5671** only.
- TLS is enforced by `UseSsl=true` (TLS 1.2); traffic is encrypted in transit.
- Prefer running the broker in the same VPC as the API hosts.

---

## Verifying the end-to-end flow

1. Both HR.API and Finance.API start cleanly (check logs for a MassTransit
   "Bus started" line and no connection errors).
2. In the broker, exchanges/queues for the integration events appear after the
   first publish.
3. A time-clock end-of-day punch produces a `TimesheetSubmitted` message that
   Finance consumes.
4. Poison messages accumulate in `<queue>_error` — monitor these.
