using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace BffService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderDetailsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> Get(int orderId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://apigateway");

            var order = await client.GetFromJsonAsync<OrderDto>($"/orders/{orderId}");
            if (order == null) return NotFound();

            var product = await client.GetFromJsonAsync<ProductDto>($"/products/{order.ProductId}");
            return Ok(new { order, product });
        }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}