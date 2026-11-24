using Aiko.EventConsumer.Events;

namespace Aiko.EventConsumer.Buffering;

internal interface IEventSink
{
    Task AddAsync(IReadOnlyList<Event> events, CancellationToken cancellationToken = default);
}
