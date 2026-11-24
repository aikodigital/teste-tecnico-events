namespace Aiko.EventConsumer.Messages.Payloads;

internal record BeaconPayload() : Payload(MdtMessageType.Beacon)
{
    public required string Value { get; init; }
}
