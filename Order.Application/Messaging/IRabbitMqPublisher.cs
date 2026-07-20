namespace Order.Application.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(T message);
}