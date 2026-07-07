using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using InventoryService.Data;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

public class OrderConsumer : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _config;
    if     if (inventory == null || inventory.Quantity<order.Quantity)
    {
        Log.Information("Compensation: Order {OrderId} rejected due to insufficient inventory (CorrelationId={CorrelationId})", order.Id, correlationIdStr);
        status = "InventoryRejected";
    }
(inventory != null && inventory.Quantity >= order.Quantity)
    {
        // בדוק אם כבר טופל בעבר (למשל, לפי מזהה הזמנה)
        if (inventory.Quantity<order.Quantity) return; // לא לעדכן שוב
        inventory.Quantity -= order.Quantity;
        await db.SaveChangesAsync();
    status = "InventoryReserved";
    }
    public OrderConsumer(IServiceProvider provider, IConfiguration config)
    {
        _provider = provider;
        _config = config;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = _config["RabbitMQ:Host"] ?? "rabbitmq" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "order-placed", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var order = JsonSerializer.Deserialize<OrderMessage>(Encoding.UTF8.GetString(body));

            // קריאת מזהה קורלציה מההודעה
            var correlationId = ea.BasicProperties?.Headers?["X-Correlation-ID"] as byte[];
            var correlationIdStr = correlationId != null ? Encoding.UTF8.GetString(correlationId) : Guid.NewGuid().ToString();

            Log.Information("Handling order {OrderId} (CorrelationId={CorrelationId})", order.Id, correlationIdStr);

            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            var inventory = await db.Inventories.FirstOrDefaultAsync(i => i.ProductId == order.ProductId);
            string status;
            if (inventory != null && inventory.Quantity >= order.Quantity)
            {
                inventory.Quantity -= order.Quantity;
                await db.SaveChangesAsync();
                status = "InventoryReserved";
            }
            else
            {
                status = "InventoryRejected";
            }

            // שליחת הודעה חזרה ל-RabbitMQ עם מזהה קורלציה
            var replyFactory = new ConnectionFactory() { HostName = _config["RabbitMQ:Host"] ?? "rabbitmq" };
            using var replyConn = replyFactory.CreateConnection();
            using var replyChannel = replyConn.CreateModel();
            replyChannel.QueueDeclare(queue: status, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var replyBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));
            var props = replyChannel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object> { ["X-Correlation-ID"] = correlationIdStr };
            replyChannel.BasicPublish(exchange: "", routingKey: status, basicProperties: props, body: replyBody);
        };
        channel.BasicConsume(queue: "order-placed", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
}

public class OrderMessage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}seq:
image: datalust / seq:latest
environment:
    -ACCEPT_EULA = Y
  ports:
-"5341:5341"

orderservice:
# ...
healthcheck:
test: ["CMD", "curl", "-f", "http://localhost:80/health"]
    interval: 30s
    timeout: 10s
    retries: 3
var correlationId = ea.BasicProperties?.Headers?["X-Correlation-ID"] as byte[];
var correlationIdStr = correlationId != null ? Encoding.UTF8.GetString(correlationId) : Guid.NewGuid().ToString();
Log.Information("Handling order {OrderId} (CorrelationId={CorrelationId})", order.Id, correlationIdStr);
