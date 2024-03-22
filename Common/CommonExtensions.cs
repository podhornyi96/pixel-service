using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Solo.BuildingBlocks.EventBus;
using Solo.BuildingBlocks.EventBus.Abstractions;
using Solo.BuildingBlocks.EventBusRabbitMQ;

namespace Services.Common;


public static class CommonExtensions
{
    
    public static IServiceCollection AddServiceDefaults(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMqEventBus(configuration);
    
        return services;
    }

    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBusSection = configuration.GetSection("EventBus");
        
        Console.WriteLine("TEST username: " + eventBusSection["UserName"]); // TODO: remove
        
        var subscriptionClientName = eventBusSection.GetRequiredValue("SubscriptionClientName");
            
        Console.WriteLine("SubscriptionClientName: " + subscriptionClientName);

        if (!eventBusSection.Exists())
        {
            return services;
        }
        
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetRequiredConnectionString("EventBus"),
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(eventBusSection["UserName"]))
            {
                factory.UserName = eventBusSection["UserName"];
            }

            if (!string.IsNullOrEmpty(eventBusSection["Password"]))
            {
                factory.Password = eventBusSection["Password"];
            }

            var retryCount = eventBusSection.GetValue("RetryCount", 5);

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = eventBusSection.GetRequiredValue("SubscriptionClientName");
            
            Console.WriteLine("SubscriptionClientName: " + subscriptionClientName);
            
            var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            var retryCount = eventBusSection.GetValue("RetryCount", 5);

            return new EventBusRabbitMQ(rabbitMqPersistentConnection, logger, sp, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
        });
        
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        return services;
    }
    
}