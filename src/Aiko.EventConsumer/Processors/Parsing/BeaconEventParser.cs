using Aiko.EventConsumer.Events;

namespace Aiko.EventConsumer.Processors.Parsing;

internal class BeaconEventParser(string siteId, long equipmentId, ulong hash, string value)
    : Parser<BeaconEvent>(siteId, equipmentId, hash, value)
{
    protected override BeaconEvent ParseRow(int index)
    {
        return new BeaconEvent
        {
            Hash = Hash + (ulong)(index + 1),
            Date = GetDateTime("date")!.Value,
            SiteId = SiteId,
            EquipmentId = EquipmentId,
            SourceEquipmentId = GetLong("source_equipment_id"),
            SourcePositionLat = GetDouble("source_position_lat"),
            SourcePositionLon = GetDouble("source_position_lon"),
            SeenAt = GetDateTime("seen_at") ?? default,
            TargetBeacon = GetString("target_beacon"),
            TargetBeaconName = GetString("target_beacon_name"),
            TargetBeaconVoltage = GetInt("target_beacon_voltage"),
            TargetBeaconFirmware = GetString("target_beacon_firmware"),
            TargetBeaconModel = GetString("target_beacon_model"),
            TargetBeaconHardware = GetString("target_beacon_hardware"),
            TargetBeaconTemperature = GetFloat("target_beacon_temperature"),
            TargetEquipmentId = GetLong("target_equipment_id"),
            TargetEquipmentStateId = GetLong("target_equipment_state_id"),
            TargetEquipmentBearing = GetFloat("target_equipment_bearing"),
            TargetEquipmentSpeed = GetFloat("target_equipment_speed"),
            TargetPositionLat = GetDouble("target_position_lat"),
            TargetPositionLon = GetDouble("target_position_lon"),
            TargetAttachmentId = GetLong("target_attachment_id"),
            TargetAttachmentFractionId = GetLong("target_attachment_fraction_id"),
            SignalStrength = GetInt("signal_strength") ?? 0,
            ViewCount = GetInt("view_count") ?? 0,
            RepeaterMode = GetBool("repeater_mode") ?? false,
            AccelerometerMode = GetBool("accelerometer_mode") ?? false,
            IndirectEvent = GetBool("indirect_event") ?? false,
            IndirectSourceAddress = GetString("indirect_source_address"),
            MachineState = GetInt("machine_state"),
            Battery = GetInt("battery"),
            AccelerometerAxisX = GetInt("accelerometer_axis_x"),
            AccelerometerAxisY = GetInt("accelerometer_axis_y"),
            AccelerometerAxisZ = GetInt("accelerometer_axis_z"),
        };
    }
}
