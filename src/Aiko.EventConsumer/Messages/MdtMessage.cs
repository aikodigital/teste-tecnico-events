using System.IO.Hashing;
using System.Text;

namespace Aiko.EventConsumer.Messages;

public record MdtMessage
{
    public required MdtMessageType Type { get; init; }

    public required long Id { get; init; }

    public short LowDataId => (short)(Id % 256);

    public required DateTime Date { get; init; }

    public required string Payload { get; init; }

    public required string SiteId { get; init; }

    public required string DeviceId { get; init; }

    public required string? CourierDeviceId { get; init; }

    public required string DeviceAddress { get; init; }

    public required long FirstBootTs { get; init; }

    public required long EquipmentId { get; init; }

    public required DateTime ReceivedAt { get; init; }

    public ulong Hash()
    {
        return XxHash64.HashToUInt64([
            ..BitConverter.GetBytes(EquipmentId),
            ..Encoding.UTF8.GetBytes(DeviceId),
            ..BitConverter.GetBytes(FirstBootTs),
            ..BitConverter.GetBytes(Id),
        ]);
    }
}
