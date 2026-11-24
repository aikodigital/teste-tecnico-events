using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Events;
using Aiko.EventConsumer.Messages.Payloads;
using System.Text.Json;

namespace Aiko.EventConsumer.Processors;

internal abstract class BaseMessageProcessor<TPayload, TEvent> : IMessageProcessor
    where TEvent : Event
    where TPayload : Payload
{
    private readonly JsonSerializerOptions _options;

    public BaseMessageProcessor()
    {
        _options = CreateOptions();
    }

    public IEnumerable<Event> Process(MdtMessage message)
    {
        var payload = JsonSerializer.Deserialize<TPayload>(message.Payload, _options)!;
        return ProcessInternal(message, payload);
    }

    protected abstract IEnumerable<TEvent> ProcessInternal(MdtMessage message, TPayload payload);

    protected virtual JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }
}
