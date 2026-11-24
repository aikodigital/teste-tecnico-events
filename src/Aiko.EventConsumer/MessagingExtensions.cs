using Aiko.EventConsumer.Buffering;
using Aiko.EventConsumer.Consumers;
using Aiko.EventConsumer.Events;
using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Aiko.EventConsumer;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<EventSink>();
        services.AddSingleton<IEventSink>(sp => sp.GetRequiredService<EventSink>());
        services.AddHostedService<EventSinkWriter>();

        services.AddScoped<IMessageConsumer<MdtMessage>, MdtMessageConsumer>();

        services.Add<BeaconEvent, BeaconProcessor>();
        services.Add<MachineStateEvent, MachineStateProcessor>();
        services.Add<StatusEvent, StatusProcessor>();
        return services;
    }

    private static void Add<TEvent, TProcessor>(this IServiceCollection services)
        where TEvent : Event
        where TProcessor : class, IMessageProcessor
    {
        var eventInstance = (TEvent)Activator.CreateInstance(typeof(TEvent))!;
        MdtMessageType messageType = eventInstance.Type;

        services.AddKeyedSingleton<IMessageProcessor, TProcessor>(messageType);
    }
}
