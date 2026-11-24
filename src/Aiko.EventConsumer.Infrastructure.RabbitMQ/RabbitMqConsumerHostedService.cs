using System.Text.Json;
using Aiko.EventConsumer.Consumers;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aiko.EventConsumer.Infrastructure.RabbitMQ;

internal class RabbitMqConsumerHostedService<T> : IHostedService
{
    private readonly IBus _bus;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqConsumerOptions<T> _options;
    private readonly ILogger<RabbitMqConsumerHostedService<T>> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private IAsyncDisposable? _subscription;

    public RabbitMqConsumerHostedService(
        IBus bus,
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqConsumerOptions<T>> options,
        ILogger<RabbitMqConsumerHostedService<T>> logger)
    {
        _bus = bus;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting RabbitMQ consumer for {MessageType} on exchange {Exchange}, queue {Queue}, routing key {@RoutingKeys}",
            typeof(T).Name, _options.Exchange, _options.Queue, _options.RoutingKeys);

        var exchange = await _bus.Advanced.ExchangeDeclareAsync(
            _options.Exchange,
            c => c.WithType("topic").AsDurable(true),
            cancellationToken: cancellationToken);

        var dlq = await _bus.Advanced.QueueDeclareAsync(
            _options.Queue + ".dlq",
            c => c.AsDurable(true),
            cancellationToken: cancellationToken);

        var queue = await _bus.Advanced.QueueDeclareAsync(
            _options.Queue,
            c => c.AsDurable(true)
                .WithArgument("x-dead-letter-exchange", "")
                .WithArgument("x-dead-letter-routing-key", _options.Queue + ".dlq"),
            cancellationToken: cancellationToken);

        foreach (string routingKey in _options.RoutingKeys)
        {
            await _bus.Advanced.BindAsync(exchange, queue, routingKey, cancellationToken: cancellationToken);
        }

        _subscription = await _bus.Advanced.ConsumeAsync(
            queue,
            (body, properties, info, ct) => HandleMessageAsync(body, ct));

        _logger.LogInformation("RabbitMQ consumer for {MessageType} started", typeof(T).Name);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumer for {MessageType}", typeof(T).Name);

        if (_subscription is not null)
        {
            await _subscription.DisposeAsync();
            _subscription = null;
        }
    }

    private async Task HandleMessageAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Deserialize<T>(body.Span, JsonOptions);
            if (message is null)
            {
                _logger.LogWarning("Failed to deserialize message, skipping");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer<T>>();
            await consumer.ConsumeAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message for {MessageType}", typeof(T).Name);
            throw;
        }
    }
}
