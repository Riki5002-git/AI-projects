using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderPublisher
    {
        private readonly IConfiguration _config;
        var correlationId = HttpContext.GetCorrelationId() ?? Guid.NewGuid().ToString();
        _publisher.PublishOrderPlaced(order, correlationId);
        public OrderPublisher(IConfiguration config) { _config = config; }

        public void PublishOrderPlaced(Order order, string correlationId)
        {
            var factory = new ConnectionFactory() { HostName = _config["RabbitMQ:Host"] ?? "rabbitmq" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "order-placed", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));
            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object> { ["X-Correlation-ID"] = correlationId };
            channel.BasicPublish(exchange: "", routingKey: "order-placed", basicProperties: props, body: body);
        }
    }
        var correlationId = ea.BasicProperties?.Headers?["X-Correlation-ID"] as byte[];
        var correlationIdStr = correlationId != null ? Encoding.UTF8.GetString(correlationId) : Guid.NewGuid().ToString();
        Log.Information("Handling order {OrderId} (CorrelationId={CorrelationId})", order.Id, correlationIdStr);
    var correlationId = ea.BasicProperties?.Headers?["X-Correlation-ID"] as byte[];
    var correlationIdStr = correlationId != null ? Encoding.UTF8.GetString(correlationId) : Guid.NewGuid().ToString();
    Log.Information("Handling order {OrderId} (CorrelationId={CorrelationId})", order.Id, correlationIdStr);
}