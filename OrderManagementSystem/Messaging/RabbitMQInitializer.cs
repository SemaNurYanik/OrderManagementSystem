using Microsoft.Extensions.Options;
using OrderManagementSystem.Models;
using RabbitMQ.Client;

namespace OrderManagementSystem.Messaging
{
    public class RabbitMQInitializer : IRabbitMQInitializer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQInitializer(IOptions<RabbitMQOptions> options)
        {
            var rabbitOptions = options.Value;
            var factory = new ConnectionFactory()
            {
                HostName = rabbitOptions.HostName,
                Port = rabbitOptions.Port,
                UserName = rabbitOptions.UserName,
                Password = rabbitOptions.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Queue’yu declare ediyoruz.
            // Bu kod sadece bir kere çalışır ve hem publisher hem consumer,
            // queue’nun var olduğunu kabul eder.
            _channel.QueueDeclare(
                queue: rabbitOptions.QueueName,
                durable:true,  //false olursa harddisk vs. yazdırmak gerekir,maliyet 
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        public IConnection GetConnection() => _connection;
        public IModel GetChannel() => _channel;
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }      
    }
}
