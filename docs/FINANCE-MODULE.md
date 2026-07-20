# Finance Module Guide

The Finance module is a full vertical slice built with the same playbook as CRM and HR.
It also demonstrates the new in-process **event bus** so modules can react to each
other's domain events (CRM "opportunity won" -> Finance "draft invoice created").

## 1. Project layout

```
services/Finance/
  Finance.Domain/        # Entities + enums (Invoice, InvoiceLineItem, Payment, ...)
  Finance.Application/    # DTOs, service interfaces, mock services, mappers,
						  # module registration, and the OpportunityWonHandler
  Finance.Contracts/      # Mobile UI specification types (list/detail/form/chart)
  Finance.API/            # ASP.NET Core controllers, mobile ui-spec, startup, Dockerfile
  Finance.Web/            # Blazor + MudBlazor dashboard/invoices/payments/reports
  Finance.Tests/          # Unit, integration, and functional tests
```

Dependency direction: `Domain -> Application -> (API | Web)`. Contracts and
`BusinessAsUsual.Core` are shared and referenced where needed.

## 2. Ports

| Concern                | Local dev | Heavy compose (host) |
|------------------------|-----------|----------------------|
| Finance.API            | 5007      | 5006 -> 80           |
| Finance.Web            | 5008      | 5009 -> 80           |

Web talks to the API using `FinanceApi__Url`. In compose this is the internal
DNS name `http://finance-api:80`; locally it defaults to `http://localhost:5007`.

## 3. The event bus

Located in `BusinessAsUsual.Core/Events`:

- `IntegrationEvent` — base contract (`EventId`, `OccurredOnUtc`, `EventType`).
- `IEventBus` — publish-only abstraction: `PublishAsync<TEvent>(evt)`.
- `IIntegrationEventHandler<TEvent>` — consumer contract: `HandleAsync(evt, ct)`.
- `InProcessEventBus` + `EventBusDispatcher` — a `Channel<IntegrationEvent>`-backed
  bus plus a background service that dispatches each event to all registered
  handlers inside a fresh DI scope.
- `EventBusServiceCollectionExtensions` — DI helpers:
  - `AddInProcessEventBus()` registers the channel, `IEventBus`, and the dispatcher.
  - `AddIntegrationEventHandler<TEvent, THandler>()` registers a handler.

### Publish (CRM side)

`MockOpportunityService` optionally takes an `IEventBus`. When an opportunity
transitions to **won**, it publishes an `OpportunityWonIntegrationEvent`. CRM.API
enables this by calling `AddInProcessEventBus()` in `Program.cs`.

### Consume (Finance side)

`OpportunityWonHandler` implements
`IIntegrationEventHandler<OpportunityWonIntegrationEvent>` and creates a draft
invoice linked back to the CRM opportunity via `SourceModule = "crm"` and
`SourceReferenceId = OpportunityId`. Finance.API wires it with:

```csharp
builder.Services.AddInProcessEventBus();
builder.Services.AddIntegrationEventHandler<OpportunityWonIntegrationEvent, OpportunityWonHandler>();
```

### Important limitation

The current bus is **in-process only**. Publishing in CRM.API does not cross the
process boundary into Finance.API. The integration test
(`Finance.Tests/Integration/OpportunityWonFlowTests.cs`) proves the flow *within a
single process*. To deliver events *across services* you need a real transport
(e.g. RabbitMQ, Azure Service Bus, AWS SNS/SQS). The abstraction is intentionally
transport-agnostic, so swapping in a broker means adding a new `IEventBus`
implementation + a subscriber bridge — the handlers stay unchanged.

## 4. Deployment

- `docker-compose.heavy.yml` — adds `finance-api` (5006) and `finance-web` (5009)
  next to CRM/HR, sharing the RDS connection string and Module Registry URL.
- `docker-compose.microservices.yml` — adds `finance-api` behind the gateway with
  the shared health check.
- `.github/workflows/deploy-heavy.yml` — the build loop now includes
  `finance-api finance-web` so both images are built and started on deploy.

## 5. Extending it yourself

1. Add a new entity/DTO in `Finance.Domain` / `Finance.Application`.
2. Expose it via a controller in `Finance.API` and a page in `Finance.Web`.
3. To react to another module's event: create an `IIntegrationEventHandler<T>`
   and register it with `AddIntegrationEventHandler<T, THandler>()`.
4. To emit an event: inject `IEventBus` and call `PublishAsync(new MyEvent { ... })`.
5. Add tests mirroring `Finance.Tests` (unit for services, functional for the API,
   integration for event flows).

## 6. Validation

```
dotnet build BusinessAsUsual.sln
dotnet test services/Finance/Finance.Tests/Finance.Tests.csproj
```

All Finance tests pass (9/9); the only solution-wide warnings are pre-existing
CS1591 XML-comment warnings in `frontend/BusinessAsUsual.Web`.
