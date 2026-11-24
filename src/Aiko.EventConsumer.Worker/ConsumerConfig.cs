using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Worker;

internal record ConsumerConfig
{
    public string Exchange { get; init; } = default!;
    public string Queue { get; init; } = default!;
    public string[] SiteIds { get; init; } = [];
    public MdtMessageType[] MessageTypes { get; init; } = [];
}
