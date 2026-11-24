using Aiko.EventConsumer.Processors.Parsing;
using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Events;
using Aiko.EventConsumer.Messages.Payloads;

namespace Aiko.EventConsumer.Processors;

internal class BeaconProcessor : BaseMessageProcessor<BeaconPayload, BeaconEvent>
{
    protected override IEnumerable<BeaconEvent> ProcessInternal(MdtMessage message, BeaconPayload payload)
    {
        var parser = new BeaconEventParser(
            message.SiteId,
            message.EquipmentId,
            message.Hash(),
            payload.Value);

        return parser.Parse();
    }
}
