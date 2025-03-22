using Xunit;
using Moq;
using RabbitMQ.Client;
using OrderManagementSystem.Messaging;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrderManagementSystem.Models;
using OrderManagementData;

namespace OrderManagementSystem.Tests
{
    public class RabbitMQServiceTests
    {
        private readonly Mock<IModel> _mockChannel;
        private readonly OrderPublisher _orderPublisher;
        private readonly Mock<IConnection> _mockConnection;
        private readonly Mock<IConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IOptions<RabbitMQOptions>> _mockOptions;

        public RabbitMQServiceTests()
        {
            _mockChannel = new Mock<IModel>();
            _mockConnection = new Mock<IConnection>();
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockOptions = new Mock<IOptions<RabbitMQOptions>>();
          
            _mockConnectionFactory.Setup(x => x.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(x => x.CreateModel()).Returns(_mockChannel.Object);
            _mockOptions.Setup(o => o.Value).Returns(new RabbitMQOptions { QueueName = "testQueue" });
          
            var mockInitializer = new Mock<IRabbitMQInitializer>();
            mockInitializer.Setup(i => i.GetChannel()).Returns(_mockChannel.Object);

            // OrderPublisher sınıfını oluşturdu (gerçek test edilecek sınıf)
            _orderPublisher = new OrderPublisher(mockInitializer.Object, _mockOptions.Object);
        }

        // <summary>
        /// RabbitMQ servisinin gerçekten mesaj gönderdiğini test eder.
        /// </summary>
        [Fact]
        public void PublishOrder_SendsMessageToQueue()
        {
            // Arrange - Sahte bir sipariş oluşturuyoruz
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = "Test Product",
                Price = 1500
            };
            var expectedMessage = JsonSerializer.Serialize(order);
            var expectedBody = Encoding.UTF8.GetBytes(expectedMessage);

            // Act - RabbitMQ'ya mesaj gönder
            _orderPublisher.PublishOrder(order);

            _mockChannel.Verify(x => x.BasicPublish(
     It.IsAny<string>(),
     It.IsAny<string>(),
     It.IsAny<bool>(),
     It.IsAny<IBasicProperties>(),
     It.Is<ReadOnlyMemory<byte>>(body => body.ToArray().SequenceEqual(expectedBody))  // Burada dönüşümü yaptık
 ), Times.Once);




        }
    }
}
