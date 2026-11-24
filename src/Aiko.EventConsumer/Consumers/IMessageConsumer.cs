namespace Aiko.EventConsumer.Consumers;

public interface IMessageConsumer<in T>
{
    Task ConsumeAsync(T message, CancellationToken cancellationToken = default);
}
