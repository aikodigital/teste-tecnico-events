using System.Globalization;
using System.Text.Json;
using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.MessageGenerator;

internal class MessageFactory
{
    private const double BaseLatitude = -21.2050;
    private const double BaseLongitude = -47.8100;

    private static readonly long[] EquipmentStateIds = [101, 102, 103, 104, 105, 106];
    private static readonly long[] LocationIds = [501, 502, 503, 504];
    private static readonly string[] BeaconModels = ["BX-200", "BX-300", "KBPro"];
    private static readonly string[] BeaconFirmwares = ["1.6.2", "1.7.0", "1.7.5"];
    private static readonly string[] BeaconHardwares = ["rev2", "rev3"];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly Random _random = new();
    private readonly string _siteId;
    private readonly MdtMessageType[] _types;
    private readonly bool _invalid;
    private readonly Equipment[] _fleet;

    public MessageFactory(string siteId, MdtMessageType[] types, bool invalid, int fleetSize = 10)
    {
        _siteId = siteId;
        _types = types;
        _invalid = invalid;
        _fleet = Enumerable.Range(0, fleetSize).Select(CreateEquipment).ToArray();
    }

    public MdtMessage Create()
    {
        var equipment = Pick(_fleet);
        var type = Pick(_types);
        var now = DateTime.UtcNow;

        equipment.Move(_random);

        string payload = type switch
        {
            MdtMessageType.Status => Status(equipment, now),
            MdtMessageType.MachineState => MachineState(equipment, now),
            MdtMessageType.Beacon => Beacon(equipment, now),
            _ => throw new NotSupportedException($"Tipo {type} não suportado."),
        };

        if (_invalid)
        {
            payload = payload[..(payload.Length / 2)];
        }

        return new MdtMessage
        {
            Type = type,
            Id = equipment.NextMessageId(),
            Date = now,
            Payload = payload,
            SiteId = _siteId,
            DeviceId = equipment.DeviceId,
            CourierDeviceId = null,
            DeviceAddress = equipment.DeviceAddress,
            FirstBootTs = equipment.FirstBootTs,
            EquipmentId = equipment.Id,
            ReceivedAt = now,
        };
    }

    private Equipment CreateEquipment(int index)
    {
        return new Equipment
        {
            Id = 1001 + index,
            DeviceId = Convert.ToHexString(RandomBytes(8)).ToLowerInvariant(),
            DeviceAddress = MacAddress(),
            BeaconAddress = MacAddress(),
            FirstBootTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _random.Next(0, 3_600_000),
            OperatorId = 2001 + _random.Next(0, 20),
            Latitude = BaseLatitude + ((_random.NextDouble() - 0.5) * 0.05),
            Longitude = BaseLongitude + ((_random.NextDouble() - 0.5) * 0.05),
            Bearing = _random.Next(0, 360),
            Speed = _random.Next(0, 25),
            EquipmentStateId = Pick(EquipmentStateIds),
            MachineState = _random.Next(0, 4),
            Hourmeter = _random.Next(500, 12_000),
            BatteryLevel = _random.Next(40, 101),
        };
    }

    private string Status(Equipment equipment, DateTime now)
    {
        bool isWorking = equipment.Speed > 1;
        bool hasFix = Chance(0.95);

        equipment.BatteryLevel = Math.Clamp(equipment.BatteryLevel + _random.Next(-1, 2), 5, 100);

        return Json(new
        {
            LocationId = Chance(0.8) ? Pick(LocationIds) : (long?)null,
            OperatorId = Chance(0.9) ? equipment.OperatorId : (long?)null,
            equipment.EquipmentStateId,
            MessageQueueSize = Chance(0.8) ? 0 : _random.Next(1, 200),
            Location = new
            {
                IsValid = hasFix,
                Lon = Math.Round(equipment.Longitude, 7),
                Lat = Math.Round(equipment.Latitude, 7),
                Alt = Math.Round(540 + (_random.NextDouble() * 30), 1),
                Acc = Math.Round(hasFix ? 2 + (_random.NextDouble() * 6) : 50 + (_random.NextDouble() * 100), 1),
                Brg = Math.Round(equipment.Bearing, 1),
                Spd = Math.Round(equipment.Speed / 3.6, 2),
                Hdop = Math.Round(0.6 + (_random.NextDouble() * 1.2), 2),
                Vdop = Math.Round(0.8 + (_random.NextDouble() * 1.5), 2),
                Motion = Math.Round(_random.NextDouble() * (isWorking ? 3 : 0.3), 3),
                IsCharging = true,
                Ts = new DateTimeOffset(now).ToUnixTimeMilliseconds(),
                Src = 1,
                Method = hasFix ? "gps" : "network",
            },
            SignalStrength = (double)_random.Next(-110, -60),
            equipment.BatteryLevel,
            Powered = Chance(0.95),
            IsWorking = isWorking,
            IsIdle = !isWorking && equipment.MachineState > 0,
            MotionAverage = Math.Round(_random.NextDouble() * (isWorking ? 3 : 0.3), 3),
            VirtualHourmeter = Math.Round(equipment.Hourmeter, 3),
        });
    }

    private string MachineState(Equipment equipment, DateTime now)
    {
        equipment.MachineState = _random.Next(0, 4);

        return Json(new
        {
            MachineStateChangeDate = now,
            equipment.MachineState,
        });
    }

    /// <summary>
    /// Formato colunar: a primeira linha traz o instante inicial (t0, em ms) e a quantidade de
    /// avistamentos (n); cada linha seguinte é uma propriedade com um valor por avistamento.
    /// As colunas "date" e "seen_at" são deslocamentos em ms a partir de t0.
    /// </summary>
    private string Beacon(Equipment equipment, DateTime now)
    {
        const int WindowMs = 10_000;

        int rows = _random.Next(1, 9);
        long t0 = new DateTimeOffset(now).ToUnixTimeMilliseconds() - WindowMs;

        var sightings = Enumerable.Range(0, rows)
            .Select(i => CreateSighting(equipment, WindowMs - ((rows - 1 - i) * 250)))
            .ToArray();

        var lines = new List<string>
        {
            $"t0={t0},n={rows}",
            Column("source_equipment_id", rows, _ => equipment.Id.ToString(CultureInfo.InvariantCulture)),
            Column("source_position_lat", rows, _ => Number(equipment.Latitude, "0.0000000")),
            Column("source_position_lon", rows, _ => Number(equipment.Longitude, "0.0000000")),
            Column("date", rows, i => sightings[i].DateOffsetMs.ToString(CultureInfo.InvariantCulture)),
            Column("seen_at", rows, i => sightings[i].SeenAtOffsetMs.ToString(CultureInfo.InvariantCulture)),
            Column("target_beacon", rows, i => sightings[i].Address),
            Column("target_beacon_name", rows, i => sightings[i].Name),
            Column("target_beacon_voltage", rows, i => sightings[i].Voltage),
            Column("target_beacon_firmware", rows, i => sightings[i].Firmware),
            Column("target_beacon_model", rows, i => sightings[i].Model),
            Column("target_beacon_hardware", rows, i => sightings[i].Hardware),
            Column("target_beacon_temperature", rows, i => sightings[i].Temperature),
            Column("target_equipment_id", rows, i => sightings[i].TargetEquipmentId),
            Column("target_equipment_state_id", rows, i => sightings[i].TargetEquipmentStateId),
            Column("target_equipment_bearing", rows, i => sightings[i].TargetEquipmentBearing),
            Column("target_equipment_speed", rows, i => sightings[i].TargetEquipmentSpeed),
            Column("target_position_lat", rows, i => sightings[i].TargetPositionLat),
            Column("target_position_lon", rows, i => sightings[i].TargetPositionLon),
            Column("target_attachment_id", rows, _ => null),
            Column("target_attachment_fraction_id", rows, _ => null),
            Column("signal_strength", rows, i => sightings[i].SignalStrength),
            Column("view_count", rows, i => sightings[i].ViewCount),
            Column("repeater_mode", rows, i => sightings[i].RepeaterMode),
            Column("accelerometer_mode", rows, i => sightings[i].AccelerometerMode),
            Column("indirect_event", rows, i => sightings[i].IndirectEvent),
            Column("indirect_source_address", rows, i => sightings[i].IndirectSourceAddress),
            Column("machine_state", rows, i => sightings[i].MachineState),
            Column("battery", rows, i => sightings[i].Battery),
            Column("accelerometer_axis_x", rows, i => sightings[i].AxisX),
            Column("accelerometer_axis_y", rows, i => sightings[i].AxisY),
            Column("accelerometer_axis_z", rows, i => sightings[i].AxisZ),
        };

        return Json(new { Value = string.Join('\n', lines) + "\n" });
    }

    private Sighting CreateSighting(Equipment source, int dateOffsetMs)
    {
        var target = Chance(0.5) ? Pick(_fleet.Where(e => e != source).ToArray()) : null;
        bool hasDetails = Chance(0.5);
        bool accelerometer = Chance(0.3);
        bool indirect = Chance(0.25);
        string address = target?.BeaconAddress ?? MacAddress();

        return new Sighting
        {
            DateOffsetMs = dateOffsetMs,
            SeenAtOffsetMs = dateOffsetMs - _random.Next(0, 3000),
            Address = address,
            Name = Chance(0.7) ? $"BCN_{address.Replace(":", string.Empty)[^5..]}" : null,
            Voltage = hasDetails ? Integer(_random.Next(2800, 3301)) : null,
            Firmware = hasDetails ? Pick(BeaconFirmwares) : null,
            Model = hasDetails ? Pick(BeaconModels) : null,
            Hardware = hasDetails ? Pick(BeaconHardwares) : null,
            Temperature = hasDetails ? Number(18 + (_random.NextDouble() * 20), "0.0") : null,
            TargetEquipmentId = target is null ? null : Integer(target.Id),
            TargetEquipmentStateId = target is null ? null : Integer(target.EquipmentStateId),
            TargetEquipmentBearing = target is null ? null : Number(target.Bearing, "0.0"),
            TargetEquipmentSpeed = target is null ? null : Number(target.Speed, "0.0"),
            TargetPositionLat = target is null ? null : Number(target.Latitude, "0.0000000"),
            TargetPositionLon = target is null ? null : Number(target.Longitude, "0.0000000"),
            SignalStrength = Integer(_random.Next(-95, -35)),
            ViewCount = Integer(_random.Next(1, 6)),
            RepeaterMode = Boolean(Chance(0.2)),
            AccelerometerMode = Boolean(accelerometer),
            IndirectEvent = Boolean(indirect),
            IndirectSourceAddress = indirect ? MacAddress() : null,
            MachineState = target is null ? null : Integer(target.MachineState),
            Battery = Chance(0.6) ? Integer(_random.Next(20, 101)) : null,
            AxisX = accelerometer ? Integer(_random.Next(-1024, 1025)) : null,
            AxisY = accelerometer ? Integer(_random.Next(-1024, 1025)) : null,
            AxisZ = accelerometer ? Integer(_random.Next(-1024, 1025)) : null,
        };
    }

    private static string Column(string name, int rows, Func<int, string?> value)
    {
        return name + "," + string.Join(',', Enumerable.Range(0, rows).Select(i => value(i) ?? string.Empty));
    }

    private static string Json(object payload) => JsonSerializer.Serialize(payload, JsonOptions);

    private static string Number(double value, string format) => value.ToString(format, CultureInfo.InvariantCulture);

    private static string Integer(long value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Boolean(bool value) => value ? "true" : "false";

    private bool Chance(double probability) => _random.NextDouble() < probability;

    private T Pick<T>(T[] options) => options[_random.Next(options.Length)];

    private byte[] RandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        _random.NextBytes(bytes);
        return bytes;
    }

    private string MacAddress() => string.Join(':', RandomBytes(6).Select(b => b.ToString("X2")));

    private record Sighting
    {
        public required int DateOffsetMs { get; init; }
        public required int SeenAtOffsetMs { get; init; }
        public required string Address { get; init; }
        public string? Name { get; init; }
        public string? Voltage { get; init; }
        public string? Firmware { get; init; }
        public string? Model { get; init; }
        public string? Hardware { get; init; }
        public string? Temperature { get; init; }
        public string? TargetEquipmentId { get; init; }
        public string? TargetEquipmentStateId { get; init; }
        public string? TargetEquipmentBearing { get; init; }
        public string? TargetEquipmentSpeed { get; init; }
        public string? TargetPositionLat { get; init; }
        public string? TargetPositionLon { get; init; }
        public required string SignalStrength { get; init; }
        public required string ViewCount { get; init; }
        public required string RepeaterMode { get; init; }
        public required string AccelerometerMode { get; init; }
        public required string IndirectEvent { get; init; }
        public string? IndirectSourceAddress { get; init; }
        public string? MachineState { get; init; }
        public string? Battery { get; init; }
        public string? AxisX { get; init; }
        public string? AxisY { get; init; }
        public string? AxisZ { get; init; }
    }
}
