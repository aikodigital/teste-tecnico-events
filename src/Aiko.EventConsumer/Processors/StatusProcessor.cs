using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Events;
using Aiko.EventConsumer.Messages.Payloads;

namespace Aiko.EventConsumer.Processors;

internal class StatusProcessor : BaseMessageProcessor<StatusPayload, StatusEvent>
{
    protected override IEnumerable<StatusEvent> ProcessInternal(MdtMessage message, StatusPayload payload)
    {
        yield return new StatusEvent
        {
            Hash = message.Hash(),
            Date = message.Date,
            SiteId = message.SiteId,
            EquipmentId = message.EquipmentId,
            LocationId = payload.LocationId,
            OperatorId = payload.OperatorId,
            EquipmentStateId = payload.EquipmentStateId,
            MessageQueueSize = payload.MessageQueueSize,
            Location = payload.Location,
            Longitude = payload.Location?.Lon,
            Latitude = payload.Location?.Lat,
            Accuracy = payload.Location?.Acc,
            IsValid = payload.Location?.IsValid ?? false,
            SignalStrength = payload.SignalStrength,
            BatteryLevel = payload.BatteryLevel,
            Powered = payload.Powered,
            IsWorking = payload.IsWorking,
            IsIdle = payload.IsIdle,
            MotionAverage = payload.MotionAverage,
            VirtualHourmeter = payload.VirtualHourmeter,
        };
    }
}
