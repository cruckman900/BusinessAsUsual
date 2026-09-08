# Pitfall: Scoped services inside `IHttpClientFactory` message handlers

**Status:** Fixed. This document exists so the mistake isn't reintroduced.

## Summary

`TenantPropagationHandler` (`backend/BusinessAsUsual.Infrastructure/Http/TenantPropagationHandler.cs`)
is a `DelegatingHandler` attached to every tenant-enforced named/typed `HttpClient` via
`.AddHttpMessageHandler<TenantPropagationHandler>()`. Its job is simple: attach the current
tenant's `X-Company-Id` / `X-Tenant-Db` / `X-User-Id` headers to every outgoing request, so
cross-service calls (e.g. the Blazor Web shell calling Inventory/Sales/Services/HR APIs)
don't need to hand-wire tenant headers at every call site.

The first implementation injected `ITenantContext` (a **Scoped** service) directly into the
handler's constructor. This is unsafe and must never be done again.

## Why it's unsafe

`IHttpClientFactory` does **not** construct `DelegatingHandler` instances using the DI scope
of the code that calls `CreateClient()` / `CreateClient<T>()`. Instead, it:

1. Builds and caches a handler *pipeline* per named/typed client.
2. Reuses that cached pipeline for **every** call to that client for the configured handler
   lifetime (default ~2 minutes - see `SetHandlerLifetime`).
3. Constructs the handlers (and resolves their constructor dependencies) from its own
   **internal, factory-managed DI scope** - not the scope of whichever HTTP request or
   Blazor circuit happens to be calling it at the time.

If a message handler's constructor depends on a Scoped service, that service is resolved
**once**, when the handler pipeline is built/recycled - not once per logical caller. In this
app, `ITenantContext` is Scoped per-request in the module APIs and per-circuit in the Blazor
Web shell (see `TenantContextCircuitHandler`). A message handler holding onto a Scoped
`ITenantContext` therefore captures **whichever tenant happened to be active the first time
the handler was constructed**, and then keeps silently reusing that same tenant for every
subsequent circuit/request until the pipeline recycles.

**Practical impact:** in a multi-tenant system, this is not just a bug - it's a cross-tenant
data leak. User A's tenant headers could be sent on behalf of User B's requests, and vice
versa, entirely silently (no exception, no log, nothing to signal the mistake at runtime).

Microsoft's own guidance confirms this is a known/expected footgun of the factory's design;
it is not specific to this app. See:
<https://learn.microsoft.com/aspnet/core/fundamentals/http-requests#httpclient-and-lifetime-management>.

## The fix: an ambient, `AsyncLocal`-backed accessor

Instead of injecting the Scoped `ITenantContext` into the handler, we introduced
`ITenantContextAccessor` (`backend/BusinessAsUsual.Application/Services/TenantContextAccessor.cs`):

- Registered as a **Singleton**.
- Stores the current tenant as an immutable `TenantSnapshot` in a static
  `AsyncLocal<TenantSnapshot?>`.
- `TenantContext.SetContext(...)` mirrors its resolved value into the accessor every time it
  is called (the constructor takes an optional `ITenantContextAccessor?`, defaulting to
  `null` so `TenantContext` stays usable in hosts/tests that don't register the accessor).
- `TenantPropagationHandler` reads `ITenantContextAccessor.Current` instead of injecting
  `ITenantContext` directly.

This works because `AsyncLocal<T>` flows with the **logical call chain** (the captured
`ExecutionContext`), not with any particular DI scope or physical thread. Blazor Server
captures a distinct `ExecutionContext` per circuit and restores it for every
render/event/JS-interop callback dispatched on that circuit, so two concurrent circuits can
share the exact same singleton `TenantContextAccessor`/`TenantPropagationHandler` instances
and still never observe each other's tenant. The same reasoning applies to concurrent HTTP
requests in the module APIs.

This is the same underlying primitive ASP.NET Core itself uses for `IHttpContextAccessor`.

## Rules going forward

1. **Never inject a Scoped or Transient-with-scoped-dependencies service directly into a
   `DelegatingHandler`** registered via `AddHttpMessageHandler<T>()`. Assume the handler
   instance will outlive and be shared across many unrelated callers.
2. If a message handler needs "current request/circuit" state, expose that state through a
   **Singleton accessor backed by `AsyncLocal<T>`**, updated by whatever Scoped service owns
   the real state (mirroring the `IHttpContextAccessor` pattern).
3. When adding a new tenant-enforced named/typed `HttpClient` in
   `frontend/BusinessAsUsual.Web/Program.cs` or any service's `Program.cs`, always register
   `ITenantContextAccessor` as a singleton alongside the scoped `ITenantContext`, and attach
   `.AddHttpMessageHandler<TenantPropagationHandler>()` to the client.
4. Regression coverage: `BusinessAsUsual.Tests/Unit/TenantPropagationHandlerTests.cs` spins
   up two concurrent "circuits" sharing the same singleton accessor/handler instances and
   asserts neither ever observes the other's tenant headers. Keep this test green - it is the
   canary for this exact bug class.

## Files involved

| File | Role |
|---|---|
| `backend/BusinessAsUsual.Application/Services/TenantContextAccessor.cs` | `ITenantContextAccessor` / `TenantSnapshot` - the `AsyncLocal` ambient accessor. |
| `backend/BusinessAsUsual.Application/Services/TenantContext.cs` | Scoped `ITenantContext`; mirrors into the accessor on `SetContext`. |
| `backend/BusinessAsUsual.Infrastructure/Http/TenantPropagationHandler.cs` | The `DelegatingHandler`; reads from the accessor only. |
| `BusinessAsUsual.Tests/Unit/TenantPropagationHandlerTests.cs` | Regression test for concurrent-tenant isolation. |
| `frontend/BusinessAsUsual.Web/Program.cs`, and each module API's `Program.cs` / `DependencyInjection.cs` | Registers `ITenantContextAccessor` as a singleton alongside the scoped `ITenantContext`. |
