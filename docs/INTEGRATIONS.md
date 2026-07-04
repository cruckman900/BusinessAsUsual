# 🔗 Integrations

> **How BusinessAsUsual connects to the outside world.**
> This document defines the contracts for API keys, webhooks, and connectors that
> let external systems integrate with BAU. It maps to the Platform `Integrations`
> module (API Keys / Webhooks / Connectors).

**Status:** 🟢 Canonical · Contracts stabilize as the Integrations module is built (🗓️ planned — see [MODULE_STATUS.md](MODULE_STATUS.md)).

---

## 🧩 Integration Surfaces

| Surface | Direction | Use Case |
|---------|-----------|----------|
| **REST APIs** | Inbound | External systems call BAU module APIs |
| **Webhooks** | Outbound | BAU notifies external systems of events |
| **Connectors** | Bi-directional | Prebuilt integrations (accounting, payments, email) |
| **API Keys** | Auth | Scoped credentials for machine access |

---

## 🔑 API Keys

- Keys are **scoped** to specific modules/permissions and are **revocable**.
- Keys are shown once on creation, then stored hashed.
- Every key is attributable to a tenant and an owner.
- Enforce **rate limits** per key.

```http
GET /api/crm/leads
Authorization: Bearer bau_live_xxxxxxxxxxxxxxxxxxxx
X-Tenant-Id: {tenantId}
```

> Security details in [SECURITY.md](SECURITY.md).

---

## 📤 Webhooks (Outbound Events)

BAU emits webhooks when domain events occur. Subscribers register an endpoint URL
and select the events they care about.

### Delivery Contract

- **Transport:** HTTPS POST, JSON body.
- **Retries:** Exponential backoff on non-2xx (e.g., 5 attempts).
- **Ordering:** Not guaranteed; use `id` + `occurredAt` to sequence.
- **Idempotency:** Consumers must dedupe on `id`.

### Signature Verification

Each request is signed so consumers can verify authenticity:

```http
POST https://subscriber.example.com/hooks/bau
Content-Type: application/json
X-BAU-Event: crm.lead.created
X-BAU-Delivery: 5f3b...c9
X-BAU-Signature: sha256=<hmac_of_body_with_shared_secret>
```

### Standard Event Envelope

```json
{
  "id": "evt_01HXYZ...",
  "type": "crm.lead.created",
  "tenantId": "tnt_123",
  "occurredAt": "2025-01-01T12:00:00Z",
  "data": {
	"leadId": "lead_456",
	"name": "Acme Corp",
	"status": "New"
  }
}
```

### Event Naming Convention

`{module}.{entity}.{action}` — for example:

| Event | Fires When |
|-------|------------|
| `crm.lead.created` | A new lead is added |
| `crm.opportunity.won` | An opportunity is marked won |
| `hr.employee.onboarded` | An employee completes onboarding |
| `financial.invoice.paid` | An invoice is fully paid |

---

## 🔌 Connectors

Prebuilt, configurable integrations to common third-party systems. Each connector
implements a common lifecycle so they're consistent to build and operate.

| Connector Category | Examples |
|--------------------|----------|
| Accounting | QuickBooks, Xero |
| Payments | Stripe, PayPal (tokenized — see PCI note in [SECURITY.md](SECURITY.md)) |
| Email / Comms | SMTP, SendGrid, Twilio (SMS) |
| Storage | S3, Azure Blob |
| Identity | OIDC / SAML providers |

### Connector Contract

A connector should expose:

1. **Configure** — credentials + settings (stored as secrets).
2. **Test** — validate connectivity before enabling.
3. **Sync** — pull/push data on a schedule or trigger.
4. **Map** — field mapping between BAU entities and the external system.
5. **Health** — status + last-sync + error surface.

---

## 🧭 AI Integration

The AI microservice exposes its own internal contract for module intelligence
(`/api/ai/embeddings/upsert`, `/query`, `/summarize`, `/predict/lead-score`).
See [MICROSERVICEARCHITECTUREOVERVIEW.md](MICROSERVICEARCHITECTUREOVERVIEW.md)
and [AI-PriorityQueue.md](AI-PriorityQueue.md).

---

## 📋 Integration Developer Checklist

- [ ] Authenticate with a scoped API key or service token.
- [ ] Enforce tenant scoping on inbound calls.
- [ ] Sign outbound webhooks (HMAC).
- [ ] Make webhook consumers idempotent.
- [ ] Store connector credentials as secrets, never in code.
- [ ] Provide a `Test` action before enabling a connector.
- [ ] Rate-limit and log all integration traffic.

---

_See also: [SECURITY.md](SECURITY.md) · [ARCHITECTURE.md](ARCHITECTURE.md) · [PORT_REGISTRY.md](PORT_REGISTRY.md)_
