using Solo.BuildingBlocks.EventBus.Abstractions;
using StoreService.IntegrationEvents.Events;

namespace StoreService.IntegrationEvents.Handlers;

public class ImageTrackedIntegrationEventHandler : IIntegrationEventHandler<ImageTrackedIntegrationEvent>
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IConfiguration _configuration;
    
    private static readonly object lockObject = new object();

    public ImageTrackedIntegrationEventHandler(IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
    }

    public Task Handle(ImageTrackedIntegrationEvent @event)
    {
        var configFilePath = _configuration.GetValue<string>("FilePath");
        var filePath = Path.Combine(_hostEnvironment.ContentRootPath, configFilePath).ToLower();

        var content = $"{@event.CreationDate:O}|{GetValueOrDefault(@event.Referrer)}|{GetValueOrDefault(@event.UserAgent)}|{GetValueOrDefault(@event.VisitorIp)}";
        
        lock (lockObject)
        {
            using var writer = File.AppendText(filePath);
            
            writer.WriteLineAsync(content);
        }
        
        return Task.CompletedTask;
    }

    private static string GetValueOrDefault(string? value) => string.IsNullOrEmpty(value) ? "null" : value;

}