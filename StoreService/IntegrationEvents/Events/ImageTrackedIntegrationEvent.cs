using Solo.BuildingBlocks.EventBus.Events;

namespace StoreService.IntegrationEvents.Events;

public record ImageTrackedIntegrationEvent : IntegrationEvent
{
    public string? Referrer { get; set; }
    public string? UserAgent { get; set; }
    public string? VisitorIp { get; set; }
}