using LMS.Application;
using LMS.Infrastructure;
using BusinessAsUsual.Core.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add event bus
builder.Services.AddInProcessEventBus();

// Add LMS services
builder.Services.AddLMSApplication();
builder.Services.AddLMSInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
