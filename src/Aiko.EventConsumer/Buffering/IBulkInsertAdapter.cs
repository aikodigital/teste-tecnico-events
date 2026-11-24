using Aiko.EventConsumer.Events;

namespace Aiko.EventConsumer.Buffering;

public interface IBulkInsertAdapter
{
    Task<long> BulkInsertAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default);
}
