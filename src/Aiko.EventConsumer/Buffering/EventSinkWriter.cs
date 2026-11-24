using System.Threading.Channels;
using Aiko.EventConsumer.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aiko.EventConsumer.Buffering;

internal class EventSinkWriter : BackgroundService
{
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(2);
    private const int MaxCount = 1000;
    private const int MaxParallelInserts = 4;

    private readonly ChannelReader<TrackedMessage> _reader;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventSinkWriter> _logger;

    public EventSinkWriter(
        EventSink sink,
        IServiceProvider serviceProvider,
        ILogger<EventSinkWriter> logger)
    {
        _reader = sink.Reader;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var batch = await ReadBatchAsync(stoppingToken);
            await InsertAsync(batch);
        }

        var remaining = new List<TrackedMessage>();
        while (_reader.TryRead(out var message))
        {
            remaining.Add(message);
        }

        await InsertAsync(remaining);
    }

    private async Task<List<TrackedMessage>> ReadBatchAsync(CancellationToken stoppingToken)
    {
        var batch = new List<TrackedMessage>();
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        try
        {
            batch.Add(await _reader.ReadAsync(stoppingToken));
            deadline.CancelAfter(FlushInterval);

            while (batch.Count < MaxCount)
            {
                batch.Add(await _reader.ReadAsync(deadline.Token));
            }
        }
        catch (OperationCanceledException)
        {
        }

        return batch;
    }

    private async Task InsertAsync(List<TrackedMessage> batch)
    {
        var groups = batch.GroupBy(m => m.EventType);
        var options = new ParallelOptions { MaxDegreeOfParallelism = MaxParallelInserts };

        try
        {
            await Parallel.ForEachAsync(groups, options, async (group, _) =>
            {
                await InsertGroupAsync(group.Key, group.ToList());
            });

            foreach (var message in batch)
            {
                message.AckTcs.TrySetResult();
            }
        }
        catch (Exception ex)
        {
            foreach (var message in batch)
            {
                message.AckTcs.TrySetException(ex);
            }
        }
    }

    private async Task InsertGroupAsync(MdtMessageType eventType, List<TrackedMessage> messages)
    {
        try
        {
            var adapter = _serviceProvider.GetKeyedService<IBulkInsertAdapter>(eventType);
            if (adapter is null)
            {
                _logger.LogWarning("[Flush] No adapter for event type {EventType}, skipping {Count} messages",
                    eventType, messages.Count);
                return;
            }

            var inserted = await adapter.BulkInsertAsync(messages.SelectMany(m => m.Events));
            _logger.LogInformation("[Flush {EventType}] Insert OK. {EventCount} events from {MessageCount} messages.",
                eventType, inserted, messages.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Flush {EventType}] Insert FAILED. Nacking {MessageCount} messages.",
                eventType, messages.Count);
        }
    }
}
