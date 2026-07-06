using CRM.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register CRM services (using mock implementations for now)
builder.Services.AddScoped<ILeadService, MockLeadService>();
builder.Services.AddScoped<IOpportunityService, MockOpportunityService>();
builder.Services.AddScoped<ICustomerService, MockCustomerService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed so WebApplicationFactory<Program> can bootstrap the API in functional tests.
public partial class Program { }

