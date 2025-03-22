using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderManagementData;
using Microsoft.EntityFrameworkCore;
namespace BackgroundServices
{
    internal class OrderProcessingServices : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;        
          public OrderProcessingServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"**Received {message}");
                var orderData = JsonSerializer.Deserialize<Order>(message);
                if (orderData == null)
                {
                    Console.WriteLine("**Error: Order data could not be retrieved.");
                    return;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                    var order = context.Orders.Find(orderData.Id);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Completed;
                        context.SaveChanges();
                        Console.WriteLine($"**Order {orderData.Id} processed!");
                    }
                }    

            };
            channel.BasicConsume(queue: "orderQueue",
                                 autoAck: true,  //acknowledgement :kuyruktan datayı aldıktan sonra okundu bilgisini döner ve kuyruktan siler
                                 consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
