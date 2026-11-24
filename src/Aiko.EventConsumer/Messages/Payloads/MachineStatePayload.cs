namespace Aiko.EventConsumer.Messages.Payloads;

internal record MachineStatePayload() : Payload(MdtMessageType.MachineState)
{
    public DateTime? MachineStateChangeDate { get; init; }

    public required int MachineState { get; init; }
}
