using RabbitMQ.Client;

namespace OrderManagementSystem.Messaging
{
    public interface IRabbitMQInitializer
    {
        IConnection GetConnection();
        public IModel GetChannel();
    }
}
