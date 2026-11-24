using Aiko.EventConsumer.Events;
using Microsoft.Extensions.Options;

namespace Aiko.EventConsumer.Infrastructure.ClickHouse.Adapters;

internal class StatusEventAdapter(IOptions<ClickHouseOptions> options)
    : ClickHouseAdapter<StatusEvent>(options, "status_event")
{
    protected override void SetupMappings()
    {
        Map("location_id", e => e.LocationId);
        Map("operator_id", e => e.OperatorId);
        Map("equipment_state_id", e => e.EquipmentStateId);
        Map("message_queue_size", e => e.MessageQueueSize);
        Map("location", e => e.Location);
        Map("longitude", e => e.Longitude);
        Map("latitude", e => e.Latitude);
        Map("accuracy", e => e.Accuracy);
        Map("is_valid", e => e.IsValid);
        Map("signal_strength", e => e.SignalStrength);
        Map("battery_level", e => e.BatteryLevel);
        Map("powered", e => e.Powered);
        Map("is_working", e => e.IsWorking);
        Map("is_idle", e => e.IsIdle);
        Map("motion_average", e => e.MotionAverage);
        Map("virtual_hourmeter", e => e.VirtualHourmeter);
    }
}
