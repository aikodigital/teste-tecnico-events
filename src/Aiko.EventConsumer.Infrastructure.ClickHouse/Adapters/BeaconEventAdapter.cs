using Aiko.EventConsumer.Events;
using Microsoft.Extensions.Options;

namespace Aiko.EventConsumer.Infrastructure.ClickHouse.Adapters;

internal class BeaconEventAdapter(IOptions<ClickHouseOptions> options)
    : ClickHouseAdapter<BeaconEvent>(options, "beacon_event")
{
    protected override void SetupMappings()
    {
        Map("source_equipment_id", e => e.SourceEquipmentId);
        Map("source_position_lat", e => e.SourcePositionLat);
        Map("source_position_lon", e => e.SourcePositionLon);
        Map("seen_at", e => e.SeenAt);
        Map("target_beacon", e => e.TargetBeacon);
        Map("target_beacon_name", e => e.TargetBeaconName);
        Map("target_beacon_voltage", e => e.TargetBeaconVoltage);
        Map("target_beacon_firmware", e => e.TargetBeaconFirmware);
        Map("target_beacon_model", e => e.TargetBeaconModel);
        Map("target_beacon_hardware", e => e.TargetBeaconHardware);
        Map("target_beacon_temperature", e => e.TargetBeaconTemperature);
        Map("target_equipment_id", e => e.TargetEquipmentId);
        Map("target_equipment_state_id", e => e.TargetEquipmentStateId);
        Map("target_equipment_bearing", e => e.TargetEquipmentBearing);
        Map("target_equipment_speed", e => e.TargetEquipmentSpeed);
        Map("target_position_lat", e => e.TargetPositionLat);
        Map("target_position_lon", e => e.TargetPositionLon);
        Map("target_attachment_id", e => e.TargetAttachmentId);
        Map("target_attachment_fraction_id", e => e.TargetAttachmentFractionId);
        Map("signal_strength", e => e.SignalStrength);
        Map("view_count", e => e.ViewCount);
        Map("repeater_mode", e => e.RepeaterMode);
        Map("accelerometer_mode", e => e.AccelerometerMode);
        Map("indirect_event", e => e.IndirectEvent);
        Map("indirect_source_address", e => e.IndirectSourceAddress);
        Map("machine_state", e => e.MachineState);
        Map("battery", e => e.Battery);
        Map("accelerometer_axis_x", e => e.AccelerometerAxisX);
        Map("accelerometer_axis_y", e => e.AccelerometerAxisY);
        Map("accelerometer_axis_z", e => e.AccelerometerAxisZ);
    }
}
