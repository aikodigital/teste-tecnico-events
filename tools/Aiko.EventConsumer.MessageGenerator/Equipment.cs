namespace Aiko.EventConsumer.MessageGenerator;

internal class Equipment
{
    private long _lastMessageId;

    public required long Id { get; init; }
    public required string DeviceId { get; init; }
    public required string DeviceAddress { get; init; }
    public required string BeaconAddress { get; init; }
    public required long FirstBootTs { get; init; }
    public required long OperatorId { get; init; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Bearing { get; set; }
    public double Speed { get; set; }
    public long EquipmentStateId { get; set; }
    public int MachineState { get; set; }
    public double Hourmeter { get; set; }
    public int BatteryLevel { get; set; }

    public long NextMessageId() => ++_lastMessageId;

    public void Move(Random random)
    {
        Bearing = (Bearing + random.Next(-15, 16) + 360) % 360;
        Speed = Math.Clamp(Speed + (random.NextDouble() * 4) - 2, 0, 35);

        double meters = Speed / 3.6;
        double radians = Bearing * Math.PI / 180;

        Latitude += meters * Math.Cos(radians) / 111_320;
        Longitude += meters * Math.Sin(radians) / (111_320 * Math.Cos(Latitude * Math.PI / 180));
        Hourmeter += 0.001;
    }
}
