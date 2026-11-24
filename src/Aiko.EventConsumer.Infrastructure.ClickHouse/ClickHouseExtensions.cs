using Aiko.EventConsumer.Buffering;
using Aiko.EventConsumer.Infrastructure.ClickHouse.Adapters;
using Aiko.EventConsumer.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Aiko.EventConsumer.Infrastructure.ClickHouse;

public static class ClickHouseExtensions
{
    public static IServiceCollection AddClickHouse(
        this IServiceCollection services,
        Action<ClickHouseOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        services.AddKeyedSingleton<IBulkInsertAdapter, MachineStateEventAdapter>(MdtMessageType.MachineState);
        services.AddKeyedSingleton<IBulkInsertAdapter, BeaconEventAdapter>(MdtMessageType.Beacon);
        services.AddKeyedSingleton<IBulkInsertAdapter, StatusEventAdapter>(MdtMessageType.Status);

        return services;
    }
}
