# Event-Driven Timekeeping → Payroll — Work-In-Progress Handoff

> Status: **Code complete, solution build PASSES (verified).** Do **not** commit/push
> until the full smoke test passes. The user explicitly paused commits ("let this fester").

## Goal

Build up the in-process event bus so the footer time clock drives a real workflow:

> "Each action publishes an event and HR timekeeping records. From HR, timesheets flow
> to Finance. At the end of the day, Finance should only have to 'run payroll'."

## Architecture Decisions (confirmed with user)

- Cross-module event contracts live in `BusinessAsUsual.Core/Events/Integration`.
- Frontend clock → **HR.API** endpoint (frontend is a separate process; the in-process
  bus is per-host, so cross-process delivery goes over HTTP to the API host).
- Persistence is **in-memory but structured for a later EF swap** (ConcurrentDictionary
  singleton stores), mirroring the existing `FinanceDataStore` pattern.
- Reuse the existing bus pattern (`AddInProcessEventBus`,
  `AddIntegrationEventHandler<TEvent, THandler>`, `IIntegrationEventHandler<T>`), exactly
  like the existing CRM→Finance `OpportunityWonIntegrationEvent` flow.

## End-to-End Flow

1. Employee punches the clock (start-day / break / lunch / return / end-day / extra).
2. Frontend `TimeClock.razor` POSTs to **HR.API** `POST /api/hr/timeclock/punch`.
3. `TimekeepingService` records the punch into the open daily `Timesheet` (source of
   truth) and publishes `TimeClockPunchedIntegrationEvent` for every punch.
4. On an **end-day** punch, HR totals worked hours (breaks/lunch excluded), closes the
   timesheet, and publishes `TimesheetSubmittedIntegrationEvent`.
5. Finance's `TimesheetSubmittedHandler` ingests it into `PayrollDataStore` as a
   **pending** `ReceivedTimesheet` (idempotent by HR timesheet id). Nothing is paid.
6. At end of period, Finance calls `POST /api/finance/payroll/run` → `RunPayrollAsync`
   rolls all unprocessed timesheets into a `PayRun` (grouped per employee,
   `hours * hourly rate`). The Finance.Web **Pay Runs** page has the "Run Payroll" button.

## Files Added

### BusinessAsUsual.Core
- `Events/Integration/TimeClockPunchedIntegrationEvent.cs` — `hr.timeclock.punched`
- `Events/Integration/TimesheetSubmittedIntegrationEvent.cs` — `hr.timesheet.submitted`

### HR
- `HR.Domain/Entities/TimeEntry.cs`
- `HR.Domain/Entities/Timesheet.cs` (+ `TimesheetStatus` enum: Open/Submitted)
- `HR.Application/DTOs/PunchRequest.cs`
- `HR.Application/DTOs/TimesheetDto.cs` (+ `TimeEntryDto`)
- `HR.Application/Services/TimekeepingDataStore.cs` (singleton, in-memory)
- `HR.Application/Services/ITimekeepingService.cs`
- `HR.Application/Services/TimekeepingService.cs` (records punch, publishes events,
  computes worked hours, submits on end-day)
- `HR.API/Controllers/TimeClockController.cs` — `POST punch`, `GET timesheets`,
  `GET timesheets/{id}`

### Finance
- `Finance.Domain/Entities/ReceivedTimesheet.cs`
- `Finance.Domain/Entities/PayRun.cs` (+ `PayRunLine`, `PayRunStatus` enum)
- `Finance.Application/DTOs/PayrollDtos.cs` (`PendingTimesheetDto`, `PayRunDto`,
  `PayRunLineDto`)
- `Finance.Application/Services/PayrollDataStore.cs` (singleton, in-memory,
  `DefaultHourlyRate = 25m`)
- `Finance.Application/Services/IPayrollService.cs`
- `Finance.Application/Services/PayrollService.cs` (ingest idempotent, `RunPayrollAsync`)
- `Finance.Application/Events/TimesheetSubmittedHandler.cs`
- `Finance.API/Controllers/PayrollController.cs` — `GET pending`, `GET pay-runs`,
  `GET pay-runs/{id}`, `POST run`

## Files Modified

- `HR.Application/HR.Application.csproj` — added `BusinessAsUsual.Core` project ref +
  `Microsoft.Extensions.DependencyInjection.Abstractions` and `...Logging.Abstractions`.
- `HR.API/Program.cs` — added `using BusinessAsUsual.Core.Events;` and registered
  `AddInProcessEventBus()`, `AddSingleton<TimekeepingDataStore>()`,
  `AddScoped<ITimekeepingService, TimekeepingService>()`.
- `Finance.API/Program.cs` — registered `AddSingleton<PayrollDataStore>()`,
  `AddScoped<IPayrollService, PayrollService>()`, and
  `AddIntegrationEventHandler<TimesheetSubmittedIntegrationEvent, TimesheetSubmittedHandler>()`.
- `Finance.Web/Components/Pages/PayRuns.razor` — replaced the "coming soon" scaffold with
  a real page: pending timesheets list, recent pay runs, and a **Run Payroll** button that
  calls the Finance.API over `HttpClient`.

## Design Note (avoids a double-count bug)

There is intentionally **no** HR `TimeClockPunchedHandler`. `TimekeepingService` writes the
punch to the store synchronously *before* publishing, so it is the single source of truth.
A handler that re-recorded the same event would duplicate entries. (Plan step for this was
deliberately skipped.)

## Remaining / TODO (start here in the new dialog)

1. **Build the whole solution and fix any compile errors.** The build tool calls were
   getting canceled in the previous session, so nothing here has been compiler-verified yet.
   Likely things to double-check:
   - `Finance.API/Program.cs` has the needed `using` for
	 `BusinessAsUsual.Core.Events.Integration` and `Finance.Application.Events` /
	 `Finance.Application.Services` / `Finance.Application.DTOs`.
   - `HR.API` resolves `AddInProcessEventBus` (comes from `BusinessAsUsual.Core.Events`,
	 transitively referenced via HR.Application → Core).
   - Finance.Web can see `Finance.Application.DTOs` (it already injects Finance services, so
	 the project ref exists) and that `HttpClient` is injectable there.
2. **Wire `TimeClock.razor` to actually POST punches to HR.API.** ✅ DONE.
   - Added `HrService:Url` (`http://localhost:5041`) to `frontend/BusinessAsUsual.Web`
	 `appsettings.json` and registered a named `HttpClient` "HrApi" in `Program.cs`,
	 mirroring the existing "AiApi" pattern.
   - `TimeClock.razor` now injects `IHttpClientFactory` + `ILogger`, each dialog action
	 carries `PunchAction`/`IsClockIn` metadata, and `SelectAction` fires
	 `SendPunchAsync` → `POST api/hr/timeclock/punch`. Failures are logged, not thrown
	 (local UI state stays authoritative for the widget).
   - Employee identity uses a placeholder stub (`EMP-0001` / "Demo Employee") until auth
	 is wired in.
3. Confirm the Finance.Web `HttpClient` base address points at Finance.API. ✅ DONE.
   - Root cause: `PayRuns.razor` injected a **bare `HttpClient`**, which Blazor Server does
     not provide from DI (would throw at runtime). It genuinely needs HTTP because the
     `PayrollDataStore` + `TimesheetSubmittedHandler` live in the **Finance.API** process,
     not the frontend host (unlike `Invoices.razor`, which uses in-process mock services).
   - Fix: added `FinanceService:Url` (`http://localhost:5007`) config + a named
     `HttpClient` "FinanceApi" in `Program.cs`, and refactored `PayRuns.razor` to resolve
     it via `IHttpClientFactory` for both loads and the Run Payroll POST.
4. Run the HR.API + Finance.API + frontend together and smoke-test the full chain:
   punch → HR timesheet → end-day → Finance pending → Run Payroll → pay run appears.
   - RESOLVED (design): HR.API (5041) and Finance.API (5007) are separate hosts, each with
     its own in-process bus, so `TimesheetSubmittedIntegrationEvent` could not cross. We
     added a **broker-backed `IEventBus`** (RabbitMQ via MassTransit) selectable by config
     so the event now travels HR → RabbitMQ → Finance cross-process. See "Cross-Process
     Event Bus" below. Smoke test in **Broker** mode to validate end-to-end.
5. Only after a green build + smoke test: commit & push (user will give the go-ahead).

## Cross-Process Event Bus (RabbitMQ / MassTransit)

### Why
`AddInProcessEventBus` is per-host; HR publishes and Finance consumes in different
processes, so the in-process channel bus can never deliver HR→Finance. A real broker
bridges the hosts. RabbitMQ was chosen because it runs locally (Docker) and maps to
**Amazon MQ for RabbitMQ** on AWS with no code change — only host/creds differ via config.

### What was added
- `BusinessAsUsual.Core.csproj` — `MassTransit` + `MassTransit.RabbitMQ` (8.5.2). Pinned
  to the last free OSS 8.x line; **MassTransit v9+ requires a commercial license**, so do
  not let a restore bump these to 9.x.
- `Events/MassTransitEventBus.cs`
  - `MassTransitEventBus : IEventBus` — publishes via `IPublishEndpoint` (publishers
    unchanged).
  - `IntegrationEventConsumer<TEvent> : IConsumer<TEvent>` — generic adapter that resolves
    and invokes every registered `IIntegrationEventHandler<TEvent>` in a DI scope. Handler
    exceptions bubble up so MassTransit's retry/DLQ pipeline engages.
- `Events/EventBusServiceCollectionExtensions.cs`
  - `AddEventBus(configuration, builder)` — reads `EventBus:Provider`
    (`InProcess` default | `Broker`). Broker mode wires MassTransit/RabbitMQ with
    `UseMessageRetry(Exponential(...))`; in-process mode keeps the channel dispatcher.
  - `EventBusBuilder.AddHandler<TEvent, THandler>()` — registers the handler + (once per
    event type) the bridging consumer.
  - Legacy `AddInProcessEventBus` / `AddIntegrationEventHandler` kept for back-compat
    (CRM.API, Finance.Tests).
- `HR.API/Program.cs` — `AddEventBus(config, _ => {})` (publisher only).
- `Finance.API/Program.cs` — `AddEventBus(config, bus => { AddHandler<OpportunityWon...>();
  AddHandler<TimesheetSubmitted...>(); })`.
- `appsettings.json` (HR.API + Finance.API) — `EventBus` section (`Provider` +
  `RabbitMq` host/vhost/user/pass/RetryLimit). Default `InProcess`.

### Retry + Dead-Letter behavior
- Exponential backoff: retryLimit=5, min 1s, max 5m, delta 2s (configurable via
  `EventBus:RabbitMq:RetryLimit`).
- On exhaustion MassTransit moves the message to the RabbitMQ **`<queue>_error`** queue —
  this is the durable DLQ. Poison messages are inspectable there and are NOT lost.
- Idempotency: `TimesheetSubmittedHandler` already dedupes by HR timesheet id, which is
  required because broker delivery is at-least-once.

### Running locally (Broker mode)
1. Start RabbitMQ:
   `docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management`
   (management UI at http://localhost:15672, guest/guest).
2. Set `EventBus:Provider = Broker` in HR.API and Finance.API (appsettings or env
   `EventBus__Provider=Broker`).
3. Run HR.API + Finance.API + frontend and smoke-test the chain.

### AWS (later)
Point `EventBus:RabbitMq:Host` at the Amazon MQ for RabbitMQ broker endpoint and supply
its credentials. No code change. (MassTransit uses AMQPS on 5671 for Amazon MQ; add the
port/ssl host form when wiring that up.)

### Future work (documented, NOT built)
- **Admin DLQ replay UI**: surface `<queue>_error` messages in the Admin project, allow an
  operator to force data corrections, then re-publish / re-run the event chain manually.
  This is the "after all retries exhausted, Admin can force changes and replay" requirement.

## Reference Patterns (existing, mirror these)

- Publisher: `services/CRM/CRM.Application/Services/MockOpportunityAndCustomerService.cs`
- Consumer: `services/Finance/Finance.Application/Events/OpportunityWonHandler.cs`
- Bus registration: `services/Finance/Finance.API/Program.cs`
- In-memory store: `services/Finance/Finance.Application/Services/FinanceDataStore.cs`
