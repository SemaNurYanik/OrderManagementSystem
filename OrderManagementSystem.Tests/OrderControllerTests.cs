using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Controllers;
using OrderManagementSystem.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OrderManagementData;

namespace OrderManagementSystem.Tests
{
    public class OrderControllerTests:IDisposable
    {

        private readonly OrdersController _controller;
        private readonly OrderDbContext _context;
        private readonly Mock<IOrderPublisher> _mockRabbitMQService;
        private readonly string _databasePath;

        public OrderControllerTests()
        {
            // Her test için farklı bir veritabanı oluşturulacak.
            _databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlite($"Data Source={_databasePath}") // SQLite bağlantısı
                .Options;

            _context = new OrderDbContext(options);
            _context.Database.EnsureCreated(); // Veritabanını oluştur

            _mockRabbitMQService = new Mock<IOrderPublisher>();
            _controller = new OrdersController(_context, _mockRabbitMQService.Object);
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Testten sonra veritabanını sil
            _context.Dispose();
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }

        [Fact]
        public void CreateOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = "Test Product",
                Price = 100
            };

            // Act
            var result = _controller.CreateOrder(order);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
            var createdOrder = Assert.IsType<Order>(createdResult.Value);
            Assert.NotNull(createdOrder.Id);
            Assert.Equal(OrderStatus.Pending, createdOrder.Status);
            Assert.Single(_context.Orders.ToList()); // DB'ye eklendi mi?

            _mockRabbitMQService.Verify(x => x.PublishOrder(It.IsAny<Order>()), Times.Once); // RabbitMQ'ya mesaj gitti mi?
        }

        [Fact]
        public void GetOrder_ReturnsOrder_WhenOrderExist()
        {
            var order = new Order { Id = Guid.NewGuid(), ProductName = "Test Product", Price = 100 };
            _context.Orders.Add(order);
            _context.SaveChanges();

            var result = _controller.GetOrder(order.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);

            Assert.Equal(order.Id, returnedOrder.Id);
        }

        [Fact]
        public void GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            var result = _controller.GetOrder(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetOrders_ReturnsListOfOrders()
        {
            _context.Orders.Add(new Order { Id = Guid.NewGuid(), ProductName = "Test Product 1", Price = 100 });
            _context.Orders.Add(new Order { Id = Guid.NewGuid(), ProductName = "Test Product 2", Price = 200 });
            _context.SaveChanges();

            var result = _controller.GetOrders();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orders = Assert.IsType<List<Order>>(okResult.Value);

            Assert.Equal(2, orders.Count);
        }
       
    }
}
