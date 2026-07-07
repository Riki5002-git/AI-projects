using Final_Project.Data;
using Final_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Inventories.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var inventory = _context.Inventories.Find(id);
            if (inventory == null) return NotFound();
            return Ok(inventory);
        }

        [HttpPost]
        public IActionResult Create(Inventory inventory)
        {
            if (inventory.Quantity < 0)
                return BadRequest("Quantity must be non-negative.");

            _context.Inventories.Add(inventory);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = inventory.Id }, inventory);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Inventory updatedInventory)
        {
            if (id != updatedInventory.Id)
                return BadRequest();

            var inventory = _context.Inventories.Find(id);
            if (inventory == null) return NotFound();

            inventory.ProductId = updatedInventory.ProductId;
            inventory.Quantity = updatedInventory.Quantity;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var inventory = _context.Inventories.Find(id);
            if (inventory == null) return NotFound();

            _context.Inventories.Remove(inventory);
            _context.SaveChanges();
            return NoContent();
        }
    }
}