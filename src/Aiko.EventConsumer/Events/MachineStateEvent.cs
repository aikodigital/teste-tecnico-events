using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Events;

public record MachineStateEvent : Event
{
    public required int MachineState { get; init; }

    public override MdtMessageType Type => MdtMessageType.MachineState;
}
