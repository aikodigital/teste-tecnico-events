using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Events;

public abstract record Event
{
    public required ulong Hash { get; init; }

    public required DateTime Date { get; init; }

    public required string SiteId { get; init; }

    public required long EquipmentId { get; init; }

    public abstract MdtMessageType Type { get; }
}
