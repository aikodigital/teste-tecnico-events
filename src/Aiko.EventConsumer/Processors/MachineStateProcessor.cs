using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Events;
using Aiko.EventConsumer.Messages.Payloads;

namespace Aiko.EventConsumer.Processors;

internal class MachineStateProcessor : BaseMessageProcessor<MachineStatePayload, MachineStateEvent>
{
    protected override IEnumerable<MachineStateEvent> ProcessInternal(MdtMessage message, MachineStatePayload payload)
    {
        yield return new MachineStateEvent
        {
            Hash = message.Hash(),
            Date = payload.MachineStateChangeDate ?? message.Date,
            SiteId = message.SiteId,
            EquipmentId = message.EquipmentId,
            MachineState = payload.MachineState,
        };
    }
}
