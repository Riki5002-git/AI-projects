using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.Data;
using ProductCatalogService.Models;

namespace ProductCatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repo;

        public ProductsController(ProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var product = _repo.Get(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _repo.Create(product);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, Product product)
        {
            var existing = _repo.Get(id);
            if (existing == null) return NotFound();
            _repo.Update(id, product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var existing = _repo.Get(id);
            if (existing == null) return NotFound();
            _repo.Delete(id);
            return NoContent();
        }
    }
}