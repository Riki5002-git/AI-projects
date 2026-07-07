using MongoDB.Driver;
using ProductCatalogService.Models;

namespace ProductCatalogService.Data
{
    public class ProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IDatabase _cache;
        builder.Services.AddDefaultCorrelationId();
        app.UseCorrelationId();
        public ProductRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["Mongo:ConnectionString"]);
            var database = client.GetDatabase("ProductCatalogDb");
            _products = database.GetCollection<Product>("Products");

            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"] ?? "redis:6379");
            _cache = redis.GetDatabase();
        }

        public Product? Get(string id)
        {
            var cached = _cache.StringGet($"product:{id}");
            if (cached.HasValue)
            {
                Console.WriteLine("Cache HIT");
                return JsonSerializer.Deserialize<Product>(cached!);
            }

            var product = _products.Find(p => p.Id == id).FirstOrDefault();
            if (product != null)
            {
                Console.WriteLine("Cache MISS");
                _cache.StringSet($"product:{id}", JsonSerializer.Serialize(product));
            }
            return product;
        }
    }
}