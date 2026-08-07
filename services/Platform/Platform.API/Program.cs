using Platform.Application;
using Platform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowShell", policy =>
    {
        policy.WithOrigins("http://localhost:5139", "https://localhost:7139")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Platform layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowShell");
app.UseAuthorization();
app.MapControllers();

app.Run();
