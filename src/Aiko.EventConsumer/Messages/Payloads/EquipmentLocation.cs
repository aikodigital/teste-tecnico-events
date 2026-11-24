namespace Aiko.EventConsumer.Messages.Payloads;

public record EquipmentLocation
{
    public bool IsValid { get; init; }
    public double Lon { get; init; }
    public double Lat { get; init; }
    public double Alt { get; init; }
    public double? Acc { get; init; }
    public double? Brg { get; init; }
    public double? Spd { get; init; }
    public double? Hdop { get; init; }
    public double? Vdop { get; init; }
    public double? Motion { get; init; }
    public bool IsCharging { get; init; }
    public long Ts { get; init; }
    public int Src { get; init; }
    public string? Method { get; init; }
}
