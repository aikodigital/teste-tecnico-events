using Aiko.EventConsumer.Buffering;
using Aiko.EventConsumer.Events;
using ClickHouse.Driver;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Aiko.EventConsumer.Infrastructure.ClickHouse.Adapters;

internal abstract class ClickHouseAdapter<T> : IBulkInsertAdapter
    where T : Event
{
    private readonly List<(string Column, Func<T, object?> Getter)> _mappings = [];
    private readonly string _connectionString;
    private readonly string _table;

    private static readonly ResiliencePipeline RetryPipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
        })
        .Build();

    private string[] _columns = [];
    private Func<T, object?>[] _getters = [];

    protected ClickHouseAdapter(IOptions<ClickHouseOptions> options, string table)
    {
        _connectionString = options.Value.ConnectionString;
        _table = table;

        Map("date", e => e.Date);
        Map("site_id", e => e.SiteId);
        Map("equipment_id", e => e.EquipmentId);
        Map("hash", e => e.Hash);

        SetupMappings();

        _columns = _mappings.Select(m => m.Column).ToArray();
        _getters = _mappings.Select(m => m.Getter).ToArray();
    }

    protected virtual void SetupMappings() { }

    protected void Map(string column, Func<T, object?> getter)
    {
        _mappings.Add((column, getter));
    }

    private object?[] GetRow(T item)
    {
        var row = new object?[_getters.Length];
        for (int i = 0; i < _getters.Length; i++)
        {
            row[i] = _getters[i](item);
        }

        return row;
    }

    public async Task<long> BulkInsertAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default)
    {
        var rows = events.Cast<T>().Select(GetRow).ToList();

        return await RetryPipeline.ExecuteAsync(async ct =>
        {
            using var client = new ClickHouseClient(_connectionString);
            return await client.InsertBinaryAsync(
                table: _table,
                columns: _columns,
                rows: rows,
                new InsertOptions
                {
                    MaxDegreeOfParallelism = 1,
                });
        }, cancellationToken);
    }
}
