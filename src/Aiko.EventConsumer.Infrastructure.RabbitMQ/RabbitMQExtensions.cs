using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;

namespace Aiko.EventConsumer.Infrastructure.RabbitMQ;

public static class RabbitMQExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, string connectionString)
    {
        services.AddEasyNetQ(connectionString)
            .UseAlwaysNackWithoutRequeueConsumerErrorStrategy();
        return services;
    }

    public static IServiceCollection AddRabbitMqConsumer<T>(
        this IServiceCollection services,
        Action<RabbitMqConsumerOptions<T>>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.AddHostedService<RabbitMqConsumerHostedService<T>>();
        return services;
    }
}
