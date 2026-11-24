using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Events;

namespace Aiko.EventConsumer.Processors;

internal interface IMessageProcessor
{
    IEnumerable<Event> Process(MdtMessage message);
}
