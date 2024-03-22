using PixelTestTask.IntegrationEvents;
using Solo.BuildingBlocks.EventBus.Abstractions;

namespace PixelTestTask.Services.Pixel;

public class PixelService : IPixelService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventBus _eventBus;

    public PixelService(IHttpContextAccessor httpContextAccessor, IEventBus eventBus)
    {
        _httpContextAccessor = httpContextAccessor;
        _eventBus = eventBus;
    }

    public void TrackImage()
    {
        _eventBus.Publish(new ImageTrackedIntegrationEvent
        {
            Referrer = _httpContextAccessor.HttpContext?.Request.Headers.Referer.ToString(),
            UserAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString(),
            VisitorIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
        });
    }
    
}