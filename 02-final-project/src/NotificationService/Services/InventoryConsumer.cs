using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using NotificationService.Data;
using NotificationService.Models;

public class InventoryConsumer : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _config;

    public InventoryConsumer(IServiceProvider provider, IConfiguration config)
    {
        _provider = provider;
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = _config["RabbitMQ:Host"] ?? "rabbitmq" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        string[] queues = { "InventoryReserved", "InventoryRejected" };
        foreach (var queue in queues)
            channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var order = JsonSerializer.Deserialize<OrderMessage>(Encoding.UTF8.GetString(body));
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

            var notification = new Notification
            {
                Message = $"Order {order.Id} status: {ea.RoutingKey}",
                SentAt = DateTime.UtcNow
            };
            db.Notifications.Add(notification);
            await db.SaveChangesAsync();
        };

        foreach (var queue in queues)
            channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}

public class OrderMessage { public int Id { get; set; } }