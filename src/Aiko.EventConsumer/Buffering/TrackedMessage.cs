using Aiko.EventConsumer.Events;
using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Buffering;

internal record TrackedMessage(MdtMessageType EventType, IReadOnlyList<Event> Events, TaskCompletionSource AckTcs);
