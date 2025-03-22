using Microsoft.Extensions.Options;
using OrderManagementData;
using OrderManagementSystem.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderManagementSystem.Messaging
{
    public class OrderPublisher:IOrderPublisher
    {
        private readonly IModel _channel;
        private readonly string _queueName;

        public OrderPublisher(IRabbitMQInitializer initializer, IOptions<RabbitMQOptions> options)
        {
            _channel = initializer.GetChannel();
            _queueName = options.Value.QueueName;
        }

        public void PublishOrder(Order order)
        {
            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);

            // exchange belirtmeden default exchange kullanılarak direkt ilgili queue’ya gönderim yapılır.
            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: body);

            Console.WriteLine($"order send to queue: {message}");
        }
    }
}
