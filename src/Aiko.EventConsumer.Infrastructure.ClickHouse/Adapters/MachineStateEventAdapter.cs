using Aiko.EventConsumer.Events;
using Microsoft.Extensions.Options;

namespace Aiko.EventConsumer.Infrastructure.ClickHouse.Adapters;

internal class MachineStateEventAdapter(IOptions<ClickHouseOptions> options)
    : ClickHouseAdapter<MachineStateEvent>(options, "machine_state_event")
{
    protected override void SetupMappings()
    {
        Map("machine_state", e => e.MachineState);
    }
}
