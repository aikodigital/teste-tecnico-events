namespace Aiko.EventConsumer.Messages.Payloads;

internal record StatusPayload() : Payload(MdtMessageType.Status)
{
    public long? LocationId { get; init; }
    public long? OperatorId { get; init; }
    public long? EquipmentStateId { get; init; }
    public int MessageQueueSize { get; init; }
    public EquipmentLocation? Location { get; init; }
    public double? SignalStrength { get; init; }
    public int BatteryLevel { get; init; }
    public bool Powered { get; init; }
    public bool IsWorking { get; init; }
    public bool IsIdle { get; init; }
    public double MotionAverage { get; init; }
    public double VirtualHourmeter { get; init; }
}
