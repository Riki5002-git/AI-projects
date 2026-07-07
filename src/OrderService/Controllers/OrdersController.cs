using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        // ...
        private readonly OrderPublisher _publisher;

        // בתוך Create:
        _publisher.PublishOrderPlaced(order);
        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Orders.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (order.Quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";
            _context.Orders.Add(order);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Order updatedOrder)
        {
            if (id != updatedOrder.Id)
                return BadRequest();

            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Quantity = updatedOrder.Quantity;
            order.Status = updatedOrder.Status;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }

        public OrdersController(OrderDbContext context, OrderPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }
    }
}