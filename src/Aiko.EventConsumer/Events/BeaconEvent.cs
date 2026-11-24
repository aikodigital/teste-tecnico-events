using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Events;

public record BeaconEvent : Event
{
    public long? SourceEquipmentId { get; init; }
    public double? SourcePositionLat { get; init; }
    public double? SourcePositionLon { get; init; }
    public DateTime SeenAt { get; init; }
    public string? TargetBeacon { get; init; }
    public string? TargetBeaconName { get; init; }
    public int? TargetBeaconVoltage { get; init; }
    public string? TargetBeaconFirmware { get; init; }
    public string? TargetBeaconModel { get; init; }
    public string? TargetBeaconHardware { get; init; }
    public float? TargetBeaconTemperature { get; init; }
    public long? TargetEquipmentId { get; init; }
    public long? TargetEquipmentStateId { get; init; }
    public float? TargetEquipmentBearing { get; init; }
    public float? TargetEquipmentSpeed { get; init; }
    public double? TargetPositionLat { get; init; }
    public double? TargetPositionLon { get; init; }
    public long? TargetAttachmentId { get; init; }
    public long? TargetAttachmentFractionId { get; init; }
    public int SignalStrength { get; init; }
    public int ViewCount { get; init; }
    public bool RepeaterMode { get; init; }
    public bool AccelerometerMode { get; init; }
    public bool IndirectEvent { get; init; }
    public string? IndirectSourceAddress { get; init; }
    public int? MachineState { get; init; }
    public int? Battery { get; init; }
    public int? AccelerometerAxisX { get; init; }
    public int? AccelerometerAxisY { get; init; }
    public int? AccelerometerAxisZ { get; init; }

    public override MdtMessageType Type => MdtMessageType.Beacon;
}
