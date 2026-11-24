using Aiko.EventConsumer;
using Aiko.EventConsumer.Infrastructure.ClickHouse;
using Aiko.EventConsumer.Infrastructure.RabbitMQ;
using Aiko.EventConsumer.Messages;
using Aiko.EventConsumer.Worker;
using Serilog;
using Serilog.Formatting.Json;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("config/appsettings.json", optional: true);
builder.Services.AddSerilog(config => config
    .MinimumLevel.Information()
    .WriteTo.Console(new JsonFormatter()));

builder.Services.AddMessaging();
builder.Services.AddClickHouse(cfg =>
{
    cfg.ConnectionString = builder.Configuration.GetConnectionString("clickhouse")!;
});

builder.Services
    .AddRabbitMq(builder.Configuration.GetConnectionString("rabbitmq")!)
    .AddRabbitMqConsumer<MdtMessage>(cfg =>
    {
        var config = builder.Configuration.GetSection("Consumer").Get<ConsumerConfig>()!;
        cfg.Exchange = config.Exchange;
        cfg.Queue = config.Queue;
        cfg.CreateRoutingKeys(config.SiteIds, config.MessageTypes);
    });

var host = builder.Build();
host.Run();
