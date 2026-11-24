using Aiko.EventConsumer.Messages;

namespace Aiko.EventConsumer.Infrastructure.RabbitMQ;

public class RabbitMqConsumerOptions<T>
{
    public string Exchange { get; set; } = default!;
    public string Queue { get; set; } = default!;
    public string[] RoutingKeys { get; set; } = Array.Empty<string>();

    public void CreateRoutingKeys(string[] sites, MdtMessageType[] messageTypes)
    {
        List<string> routingKeys = new();

        foreach (string siteId in sites)
        {
            foreach (MdtMessageType messageType in messageTypes)
            {
                routingKeys.Add($"{(int)messageType}.{siteId}.#");
            }
        }

        RoutingKeys = routingKeys.ToArray();
    }
}
