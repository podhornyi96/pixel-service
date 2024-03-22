using Services.Common;
using Solo.BuildingBlocks.EventBus.Abstractions;
using StoreService.IntegrationEvents.Events;
using StoreService.IntegrationEvents.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceDefaults(builder.Configuration);

builder.Services.AddTransient<ImageTrackedIntegrationEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<ImageTrackedIntegrationEvent, ImageTrackedIntegrationEventHandler>();

app.Run();