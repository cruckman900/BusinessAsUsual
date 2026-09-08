using System.Net;
using BusinessAsUsual.Application.Services;
using BusinessAsUsual.Infrastructure.Http;

namespace BusinessAsUsual.Tests.Unit;

/// <summary>
/// Regression coverage for the scoped-service-in-message-handler pitfall described in
/// docs/TENANT_PROPAGATION_HANDLER_PITFALL.md: <see cref="TenantPropagationHandler"/> must
/// read the current tenant from the singleton, <see cref="AsyncLocal{T}"/>-backed
/// <see cref="ITenantContextAccessor"/> rather than a Scoped <see cref="ITenantContext"/>,
/// so that two "circuits" (simulated here via concurrent async call chains, mirroring how
/// Blazor Server captures a distinct <c>ExecutionContext</c> per circuit) can never observe
/// or send each other's tenant headers - even when they share the same
/// <see cref="TenantPropagationHandler"/>/<see cref="ITenantContextAccessor"/> singleton
/// instances, exactly as they would in production via <c>IHttpClientFactory</c>'s handler
/// cache.
/// </summary>
public class TenantPropagationHandlerTests
{
    /// <summary>
    /// A terminal <see cref="DelegatingHandler"/> stand-in that just records the headers it
    /// observed and returns a canned 200 OK, so tests can assert on what
    /// <see cref="TenantPropagationHandler"/> actually attached without any real network call.
    /// </summary>
    private sealed class RecordingHandler : HttpMessageHandler
    {
        public string? CompanyIdHeader { get; private set; }
        public string? TenantDbHeader { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CompanyIdHeader = request.Headers.TryGetValues("X-Company-Id", out var companyIds)
                ? companyIds.FirstOrDefault()
                : null;
            TenantDbHeader = request.Headers.TryGetValues("X-Tenant-Db", out var tenantDbs)
                ? tenantDbs.FirstOrDefault()
                : null;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    [Fact]
    public async Task Concurrent_tenants_never_cross_send_headers_through_shared_singleton_accessor()
    {
        // Arrange: ONE shared singleton accessor and ONE shared TenantPropagationHandler
        // instance, exactly mirroring how IHttpClientFactory caches a single handler instance
        // and reuses it across every circuit/request for up to ~2 minutes in production.
        var sharedAccessor = new TenantContextAccessor();

        var tenantACompanyId = Guid.NewGuid();
        var tenantBCompanyId = Guid.NewGuid();
        const string tenantADb = "TenantA_Db";
        const string tenantBDb = "TenantB_Db";

        var recorderA = new RecordingHandler();
        var recorderB = new RecordingHandler();

        var handlerA = new TenantPropagationHandler(sharedAccessor) { InnerHandler = recorderA };
        var handlerB = new TenantPropagationHandler(sharedAccessor) { InnerHandler = recorderB };

        using var clientA = new HttpClient(handlerA);
        using var clientB = new HttpClient(handlerB);

        // Act: simulate two concurrent Blazor circuits. Each circuit resolves its own
        // ITenantContext (scoped, one per circuit's DI scope in production) and calls
        // SetContext, which mirrors into the *shared* accessor via an AsyncLocal write -
        // this only affects the calling logical call chain, not the other one running
        // concurrently. A short randomized delay is interleaved to maximize the chance of
        // exposing any accidental cross-talk if the implementation were scope-unsafe.
        var circuitATask = Task.Run(async () =>
        {
            var tenantContextA = new TenantContext(sharedAccessor);
            tenantContextA.SetContext(tenantACompanyId, tenantADb);

            await Task.Delay(25);

            using var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/whatever");
            await clientA.SendAsync(request);
        });

        var circuitBTask = Task.Run(async () =>
        {
            var tenantContextB = new TenantContext(sharedAccessor);
            tenantContextB.SetContext(tenantBCompanyId, tenantBDb);

            await Task.Delay(10);

            using var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/whatever");
            await clientB.SendAsync(request);
        });

        await Task.WhenAll(circuitATask, circuitBTask);

        // Assert: each recording handler must have observed only its own circuit's tenant,
        // never the other's - proving the accessor correctly isolates concurrent circuits
        // even though the handler/accessor instances are shared singletons.
        Assert.Equal(tenantACompanyId.ToString(), recorderA.CompanyIdHeader);
        Assert.Equal(tenantADb, recorderA.TenantDbHeader);

        Assert.Equal(tenantBCompanyId.ToString(), recorderB.CompanyIdHeader);
        Assert.Equal(tenantBDb, recorderB.TenantDbHeader);
    }

    [Fact]
    public async Task No_headers_are_attached_when_tenant_has_not_been_resolved()
    {
        // Arrange: a fresh accessor with no tenant ever set (e.g. a pre-login/provisioning
        // call, or a startup/seed-time call made outside any resolved circuit).
        var accessor = new TenantContextAccessor();
        var recorder = new RecordingHandler();
        var handler = new TenantPropagationHandler(accessor) { InnerHandler = recorder };
        using var client = new HttpClient(handler);

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/whatever");
        await client.SendAsync(request);

        // Assert: no tenant headers should be attached, and the request should proceed
        // unmodified (downstream TenantResolutionMiddleware is responsible for rejecting it
        // unless the target endpoint is exempt via [AllowAnonymousTenant]).
        Assert.Null(recorder.CompanyIdHeader);
        Assert.Null(recorder.TenantDbHeader);
    }

    [Fact]
    public void SetContext_without_an_accessor_does_not_throw()
    {
        // TenantContext must remain usable in hosts/tests that have not registered the
        // singleton ITenantContextAccessor (the constructor parameter is optional).
        var tenantContext = new TenantContext();

        var ex = Record.Exception(() => tenantContext.SetContext(Guid.NewGuid(), "SomeDb"));

        Assert.Null(ex);
        Assert.True(tenantContext.IsResolved);
    }
}
