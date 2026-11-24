using System.Globalization;
using System.Text.Json;
using Aiko.EventConsumer.MessageGenerator;
using RabbitMQ.Client;

const string Exchange = "mdt.messages";
const string SiteId = "site-a";

var options = Options.Parse(args);
if (options is null)
{
    Console.Error.WriteLine(Options.Usage);
    return 1;
}

var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
var properties = new BasicProperties { ContentType = "application/json", Persistent = true };
var messageFactory = new MessageFactory(SiteId, options.Types, options.Invalid);

var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "dev",
    Password = "dev",
};

await using var connection = await connectionFactory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();
await channel.ExchangeDeclareAsync(Exchange, ExchangeType.Topic, durable: true);

long returned = 0;
channel.BasicReturnAsync += (_, _) =>
{
    if (Interlocked.Increment(ref returned) == 1)
    {
        Console.Error.WriteLine(
            $"AVISO: o broker está devolvendo mensagens porque nenhuma fila está vinculada a '{Exchange}' " +
            "para elas. Elas serão descartadas. Inicie o Worker ao menos uma vez para criar a fila.");
    }

    return Task.CompletedTask;
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

if (options.Interval is { } interval)
{
    string seconds = interval.TotalSeconds.ToString(CultureInfo.InvariantCulture);
    Console.WriteLine(
        $"Publicando {options.Count} mensagens a cada {seconds} s em '{Exchange}'. Ctrl+C para parar.");
}

using var timer = options.Interval is null ? null : new PeriodicTimer(options.Interval.Value);
long total = 0;

try
{
    do
    {
        var messages = Enumerable.Range(0, options.Count).Select(_ => messageFactory.Create()).ToList();

        foreach (var message in messages)
        {
            string routingKey = $"{(int)message.Type}.{message.SiteId}.{message.EquipmentId}.{message.DeviceId}";
            byte[] body = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);

            await channel.BasicPublishAsync(Exchange, routingKey, mandatory: true, properties, body, cts.Token);
        }

        total += messages.Count;

        var summary = messages
            .GroupBy(m => m.Type)
            .OrderBy(g => g.Key)
            .Select(g => $"{g.Key}={g.Count()}");

        Console.WriteLine($"{DateTime.Now:HH:mm:ss} publicadas {messages.Count} ({string.Join(", ", summary)})");
    }
    while (timer is not null && await timer.WaitForNextTickAsync(cts.Token));
}
catch (OperationCanceledException)
{
}

// O fechamento garante que as devoluções pendentes do broker sejam recebidas antes do resumo.
await channel.CloseAsync();

long totalReturned = Interlocked.Read(ref returned);

Console.WriteLine(totalReturned == 0
    ? $"Encerrado. Total publicado: {total}."
    : $"Encerrado. Total publicado: {total}, das quais {totalReturned} foram devolvidas e descartadas.");
return 0;
