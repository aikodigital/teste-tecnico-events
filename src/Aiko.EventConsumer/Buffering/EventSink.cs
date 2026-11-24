using System.Threading.Channels;
using Aiko.EventConsumer.Events;

namespace Aiko.EventConsumer.Buffering;

internal class EventSink : IEventSink
{
    private readonly Channel<TrackedMessage> _channel = Channel.CreateUnbounded<TrackedMessage>(
        new UnboundedChannelOptions { SingleReader = true });

    public ChannelReader<TrackedMessage> Reader => _channel.Reader;

    public async Task AddAsync(IReadOnlyList<Event> events, CancellationToken cancellationToken = default)
    {
        var message = new TrackedMessage(
            events[0].Type,
            events,
            new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));

        await _channel.Writer.WriteAsync(message, cancellationToken);
        await message.AckTcs.Task;
    }
}
