using Aiko.EventConsumer.Processors.Parsing;
using Aiko.EventConsumer.Messages;
using Xunit;

namespace Aiko.EventConsumer.Tests.Parsing;

public class BeaconEventParserTests
{
    private const string Payload =
        "t0=1774208705106,n=50\n" +
        "source_equipment_id,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389,22434389\n" +
        "source_position_lat,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025,-19.23025\n" +
        "source_position_lon,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675,-44.873721666666675\n" +
        "date,10004,10004,10004,10005,10005,10005,10005,10006,10006,10006,10007,10007,10007,10007,10007,10007,10008,10008,10008,10008,10008,10008,10009,10009,10009,10009,10009,10009,10009,10010,10010,10010,10010,10010,10010,10010,10011,10011,10011,10011,10011,10011,10011,10012,10012,10012,10012,10013,10013,10013\n" +
        "seen_at,7762,7762,7762,7762,7762,8574,8574,8574,8574,8574,7184,7184,7184,7184,7184,7820,7820,7820,7820,7820,5093,5093,5093,5093,5093,8240,8240,8240,8240,8240,8240,7017,8574,1371,1371,1371,1371,1371,-3950,-3950,-3950,-3950,-3950,-3309,4667,4667,4667,4667,4667,3073\n" +
        "target_beacon,BC:57:29:23:75:DC,2372b7,237360,237607,2374a9,BC:57:29:23:73:60,777777,6b6b6d,6e2e63,6d0000,BC:57:29:23:74:A9,237360,2375dc,23726b,237490,BC:57:29:23:74:90,23726b,2374a9,000000,237360,BC:57:29:23:72:27,777777,6b6b6d,6e2e63,6d0000,BC:57:29:23:72:B7,777777,6b6b6d,6e2e63,6d0000,BC:57:29:23:72:B7,BC:57:29:23:72:6B,BC:57:29:23:73:60,BC:57:29:23:76:07,777777,6b6b6d,6e2e63,6d0000,BC:57:29:23:73:71,237447,18ab9d,23757b,23748d,BC:57:29:23:74:2D,BC:57:29:23:74:8D,23757b,237607,237371,23742d,BC:57:29:23:75:7B\n" +
        "target_beacon_name,AIKO_V175DC,,,,,AIKO_V77360,,,,,AIKO_V174A9,,,,,AIKO_V17490,,,,,KBPro_289,,,,,AIKO_V772B7,,,,,AIKO_V772B7,AIKO_V1726B,AIKO_V77360,KBPro_1281,,,,,AIKO_V17371,,,,,AIKO_V7742D,AIKO_V1748D,,,,,AIKO_V7757B\n" +
        "target_beacon_voltage,3300,3125,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_beacon_firmware,1.7.5,1.7.5,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_beacon_model,AIKO_V17,KBPro,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_beacon_hardware,rev3,rev2,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_beacon_temperature,23.5,24.1,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_equipment_id,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_equipment_state_id,7,8,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_equipment_bearing,180.5,90.0,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_equipment_speed,12.3,0.0,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_position_lat,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_position_lon,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_attachment_id,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "target_attachment_fraction_id,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "signal_strength,0,-39,-53,-68,-69,0,46,99,111,1,0,-24,-50,-57,-59,0,-31,-50,-57,-58,0,46,99,111,1,0,46,99,111,1,0,0,0,0,46,99,111,1,0,-45,-54,-58,-62,-83,0,-25,-51,-60,-65,0\n" +
        "view_count,0,1,1,1,1,0,1,1,1,1,0,1,1,1,1,0,1,1,1,1,0,1,1,1,1,0,1,1,1,1,0,0,0,0,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0\n" +
        "repeater_mode,true,false,false,false,false,true,false,false,false,false,true,false,false,false,false,true,false,false,false,false,true,false,false,false,false,true,false,false,false,false,true,false,true,true,false,false,false,false,true,false,false,false,false,true,true,false,false,false,false,true\n" +
        "accelerometer_mode,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false\n" +
        "indirect_event,false,true,true,true,true,false,true,true,true,true,false,true,true,true,true,false,true,true,true,true,false,true,true,true,true,false,true,true,true,true,false,false,false,false,true,true,true,true,false,true,true,true,true,false,false,true,true,true,true,false\n" +
        "indirect_source_address,,BC:57:29:23:75:DC,BC:57:29:23:75:DC,BC:57:29:23:75:DC,BC:57:29:23:75:DC,,BC:57:29:23:73:60,BC:57:29:23:73:60,BC:57:29:23:73:60,BC:57:29:23:73:60,,BC:57:29:23:74:A9,BC:57:29:23:74:A9,BC:57:29:23:74:A9,BC:57:29:23:74:A9,,BC:57:29:23:74:90,BC:57:29:23:74:90,BC:57:29:23:74:90,BC:57:29:23:74:90,,BC:57:29:23:72:27,BC:57:29:23:72:27,BC:57:29:23:72:27,BC:57:29:23:72:27,,BC:57:29:23:72:B7,BC:57:29:23:72:B7,BC:57:29:23:72:B7,BC:57:29:23:72:B7,,,,,BC:57:29:23:76:07,BC:57:29:23:76:07,BC:57:29:23:76:07,BC:57:29:23:76:07,,BC:57:29:23:73:71,BC:57:29:23:73:71,BC:57:29:23:73:71,BC:57:29:23:73:71,,,BC:57:29:23:74:8D,BC:57:29:23:74:8D,BC:57:29:23:74:8D,BC:57:29:23:74:8D,\n" +
        "machine_state,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "battery,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "accelerometer_axis_x,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "accelerometer_axis_y,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n" +
        "accelerometer_axis_z,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\n";

    private const string SiteId = "site-a";
    private const long EquipmentId = 12345;
    private const ulong Hash = 12345;

    [Fact]
    public void Parse_ReturnsCorrectNumberOfEvents()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var events = parser.Parse().ToArray();

        Assert.Equal(50, events.Length);
    }

    [Fact]
    public void Parse_FirstEvent_HasCorrectBaseFields()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Equal(SiteId, first.SiteId);
        Assert.Equal(EquipmentId, first.EquipmentId);
        Assert.Equal(Hash + 1, first.Hash);
        Assert.Equal(MdtMessageType.Beacon, first.Type);
    }

    [Fact]
    public void Parse_FirstEvent_HasCorrectSourceFields()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Equal(22434389L, first.SourceEquipmentId);
        Assert.Equal(-19.23025, first.SourcePositionLat);
        Assert.Equal(-44.873721666666675, first.SourcePositionLon);
    }

    [Fact]
    public void Parse_FirstEvent_HasCorrectTargetFields()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Equal("BC:57:29:23:75:DC", first.TargetBeacon);
        Assert.Equal("AIKO_V175DC", first.TargetBeaconName);
        Assert.Null(first.TargetEquipmentId);
        Assert.Null(first.TargetPositionLat);
        Assert.Null(first.TargetPositionLon);
        Assert.Null(first.TargetAttachmentId);
        Assert.Null(first.TargetAttachmentFractionId);
    }

    [Fact]
    public void Parse_FirstEvent_HasCorrectTargetBeaconMetadata()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Equal(3300, first.TargetBeaconVoltage);
        Assert.Equal("1.7.5", first.TargetBeaconFirmware);
        Assert.Equal("AIKO_V17", first.TargetBeaconModel);
        Assert.Equal("rev3", first.TargetBeaconHardware);
        Assert.Equal(23.5f, first.TargetBeaconTemperature);
    }

    [Fact]
    public void Parse_FirstEvent_HasCorrectTargetEquipmentKinematics()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Equal(7L, first.TargetEquipmentStateId);
        Assert.Equal(180.5f, first.TargetEquipmentBearing);
        Assert.Equal(12.3f, first.TargetEquipmentSpeed);
    }

    [Fact]
    public void Parse_FirstEvent_HasCorrectSignalFields()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Equal(0, first.SignalStrength);
        Assert.Equal(0, first.ViewCount);
        Assert.True(first.RepeaterMode);
        Assert.False(first.AccelerometerMode);
        Assert.False(first.IndirectEvent);
        Assert.Null(first.MachineState);
        Assert.Null(first.Battery);
    }

    [Fact]
    public void Parse_SecondEvent_IsIndirectWithSource()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var second = parser.Parse().Skip(1).First();

        Assert.True(second.IndirectEvent);
        Assert.Equal("BC:57:29:23:75:DC", second.IndirectSourceAddress);
        Assert.Equal(-39, second.SignalStrength);
        Assert.Equal(1, second.ViewCount);
        Assert.False(second.RepeaterMode);
    }

    [Fact]
    public void Parse_HashIncrementsPerRow()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var events = parser.Parse().Take(3).ToArray();

        Assert.Equal(Hash + 1, events[0].Hash);
        Assert.Equal(Hash + 2, events[1].Hash);
        Assert.Equal(Hash + 3, events[2].Hash);
    }

    [Fact]
    public void Parse_NullableFieldsAreNull_WhenEmpty()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var first = parser.Parse().First();

        Assert.Null(first.AccelerometerAxisX);
        Assert.Null(first.AccelerometerAxisY);
        Assert.Null(first.AccelerometerAxisZ);
    }

    [Fact]
    public void Parse_TargetFieldsAreNull_WhenEmpty()
    {
        var parser = new BeaconEventParser(SiteId, EquipmentId, Hash, Payload);

        var sparse = parser.Parse().Skip(2).First();

        Assert.Null(sparse.TargetBeaconVoltage);
        Assert.Null(sparse.TargetBeaconFirmware);
        Assert.Null(sparse.TargetBeaconModel);
        Assert.Null(sparse.TargetBeaconHardware);
        Assert.Null(sparse.TargetBeaconTemperature);
        Assert.Null(sparse.TargetEquipmentStateId);
        Assert.Null(sparse.TargetEquipmentBearing);
        Assert.Null(sparse.TargetEquipmentSpeed);
    }
}
