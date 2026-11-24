using Aiko.EventConsumer.Messages.Payloads;

using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Events;

public record StatusEvent : Event
{
    public long? LocationId { get; init; }
    public long? OperatorId { get; init; }
    public long? EquipmentStateId { get; init; }
    public int MessageQueueSize { get; init; }
    public EquipmentLocation? Location { get; init; }
    public double? Longitude { get; init; }
    public double? Latitude { get; init; }
    public double? Accuracy { get; init; }
    public bool IsValid { get; init; }
    public double? SignalStrength { get; init; }
    public int BatteryLevel { get; init; }
    public bool Powered { get; init; }
    public bool IsWorking { get; init; }
    public bool IsIdle { get; init; }
    public double MotionAverage { get; init; }
    public double VirtualHourmeter { get; init; }

    public override MdtMessageType Type => MdtMessageType.Status;
}
