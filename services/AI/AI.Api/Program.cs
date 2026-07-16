using System.Threading.RateLimiting;
using AI.Api.Configuration;
using AI.Api.Services;
using AI.Api.Services.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// --- AI configuration + services ---
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));

// Company validation (paid-tier gate) against the provisioning master DB.
builder.Services.AddSingleton<SqlCompanyDirectory>();

if (isDevelopment)
{
    // Dev-only: configured test company ids unlock the paid tier without a database,
    // while unknown ids still fall through to the real SQL directory.
    builder.Services.AddSingleton<ICompanyDirectory>(sp => new TestCompanyDirectory(
        sp.GetRequiredService<IOptions<AiOptions>>(),
        sp.GetRequiredService<ILogger<TestCompanyDirectory>>(),
        sp.GetRequiredService<SqlCompanyDirectory>()));
}
else
{
    builder.Services.AddSingleton<ICompanyDirectory>(sp => sp.GetRequiredService<SqlCompanyDirectory>());
}

// Build both tier clients once at startup from config. Either may be null when unconfigured.
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    IChatClient? demoClient = GitHubModelsClientFactory.TryCreate(
        options.Demo, loggerFactory.CreateLogger("Ai.Demo"));

    IChatClient? paidClient = null;
    if (isDevelopment && options.Dev.UseStubPaidClient)
    {
        // Dev-only: verify paid routing end-to-end without calling Bedrock (zero AWS cost).
        paidClient = new StubChatClient();
        loggerFactory.CreateLogger("Ai.Paid")
            .LogWarning("Paid tier is using the DEVELOPMENT stub client. No real model is being called.");
    }
    else if (string.Equals(options.Paid.Provider, "Bedrock", StringComparison.OrdinalIgnoreCase))
    {
        paidClient = new BedrockChatClient(options.Paid.Region, options.Paid.Model);
    }

    return new AiClientRegistry(demoClient, paidClient);
});

builder.Services.AddScoped<IAiChatService, AiChatService>();

// --- Rate limiting: cap AI calls per client IP to guard against abuse/cost ---
builder.Services.AddRateLimiter(rl =>
{
    rl.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rl.AddPolicy("ai-chat", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRouting();
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("ai-chat");

// Lightweight health/status endpoint (not rate-limited) so you can confirm at a glance
// which AI tiers are configured and whether the paid-tier company gate is available.
app.MapGet("/health", (AiClientRegistry clients, IOptions<AiOptions> aiOptions) =>
{
    var demoConfigured = clients.DemoClient is not null;
    var paidConfigured = clients.PaidClient is not null;
    var companyGateConfigured =
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING"));

    var dev = aiOptions.Value.Dev;
    var stubActive = isDevelopment && dev.UseStubPaidClient;

    return Results.Ok(new
    {
        status = "healthy",
        tiers = new
        {
            demo = new
            {
                provider = "GitHubModels",
                model = aiOptions.Value.Demo.Model,
                configured = demoConfigured,
                hint = demoConfigured
                    ? null
                    : "Set the AI_DEMO_APIKEY env var (Ai__Demo__ApiKey) to a GitHub Models token to enable the free demo model."
            },
            paid = new
            {
                provider = stubActive ? "DevStub" : "Bedrock",
                configured = paidConfigured
            }
        },
        companyGate = new { source = "AWS_SQL_CONNECTION_STRING", configured = companyGateConfigured },
        dev = new
        {
            enabled = isDevelopment,
            stubPaidClient = stubActive,
            testCompanyIds = isDevelopment ? dev.TestCompanyIds : Array.Empty<string>()
        },
        timestamp = DateTimeOffset.UtcNow
    });
});

app.Run();

// Exposed so WebApplicationFactory<Program> can bootstrap the API in functional tests.
public partial class Program { }
