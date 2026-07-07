using Microsoft.AspNetCore.Mvc;
using InventoryService.Data;
using InventoryService.Models;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public InventoryController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Inventories.ToList());

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
            _context.Inventories.Add(inventory);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = inventory.Id }, inventory);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Inventory inventory)
        {
            if (id != inventory.Id) return BadRequest();
            var existing = _context.Inventories.Find(id);
            if (existing == null) return NotFound();

            existing.ProductId = inventory.ProductId;
            existing.Quantity = inventory.Quantity;
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