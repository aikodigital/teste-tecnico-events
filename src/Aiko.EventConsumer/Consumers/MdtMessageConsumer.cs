using Aiko.EventConsumer.Buffering;
using Aiko.EventConsumer.Processors;
using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aiko.EventConsumer.Consumers;

internal class MdtMessageConsumer : IMessageConsumer<MdtMessage>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventSink _sink;
    private readonly ILogger<MdtMessageConsumer> _logger;

    public MdtMessageConsumer(
        IServiceProvider serviceProvider,
        IEventSink sink,
        ILogger<MdtMessageConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _sink = sink;
        _logger = logger;
    }

    public async Task ConsumeAsync(
        MdtMessage message,
        CancellationToken cancellationToken = default)
    {
        var processor = _serviceProvider.GetKeyedService<IMessageProcessor>(message.Type);
        if (processor is null)
        {
            _logger.LogDebug("No processor for message type {Type}, skipping", message.Type);
            return;
        }

        List<Event> events;
        try
        {
            events = processor.Process(message).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Processor failed for message type {Type}, id {Id}", message.Type, message.Id);
            throw;
        }

        if (events.Count == 0)
        {
            return;
        }

        await _sink.AddAsync(events, cancellationToken);
    }
}
