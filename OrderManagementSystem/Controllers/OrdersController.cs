using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Models;
using OrderManagementSystem.Messaging;
using OrderManagementData;

namespace OrderManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IOrderPublisher _orderPublisher;

        public OrdersController(OrderDbContext context,IOrderPublisher orderPublisher)
        {
            _context = context;
            _orderPublisher = orderPublisher;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            order.Id = Guid.NewGuid();
            order.Status = OrderStatus.Pending;
            _context.Orders.Add(order);
            _context.SaveChanges();
            _orderPublisher.PublishOrder(order);
            //_rabbitMQService.PublisOrder(order.Id);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                //HttpContext.RequestServices.GetRequiredService<IOrderPublisher>() *anlık DI
                return NotFound();
            }
            return Ok(order);
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            return Ok(_context.Orders.ToList());
        }

    }
}
