using Final_Project.Data;
using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
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

            var product = _context.Products.Find(order.ProductId);
            if (product == null) return BadRequest("Product not found.");

            var inventory = _context.Inventories.FirstOrDefault(i => i.ProductId == order.ProductId);
            if (inventory == null || inventory.Quantity < order.Quantity)
            {
                order.Status = "Rejected";
            }
            else
            {
                inventory.Quantity -= order.Quantity;
                order.Status = "Confirmed";
            }

            order.OrderDate = DateTime.UtcNow;
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
            // לא משנים ProductId או OrderDate בהזמנה קיימת
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
    }
}