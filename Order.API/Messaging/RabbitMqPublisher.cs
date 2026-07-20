using Order.Application.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Order.API.Messaging;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        var exchange = _configuration["RabbitMQ:Exchange"];
        var queue = _configuration["RabbitMQ:Queue"];
        var routingKey = _configuration["RabbitMQ:RoutingKey"];

        await channel.ExchangeDeclareAsync(
            exchange: exchange,
            type: ExchangeType.Direct,
            durable: true);

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: queue,
            exchange: exchange,
            routingKey: routingKey);

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            body: body);

        Console.WriteLine("OrderCreatedEvent Published Successfully");
    }
}