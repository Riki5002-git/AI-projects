using Microsoft.EntityFrameworkCore;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();// ...
builder.Services.AddSingleton<OrderPublisher>();
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ... שאר ההגדרות שלך

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<OrderDbContext>(); // החלף ל-DbContext של כל שירות

app.MapHealthChecks("/health");
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
orderservice:
# ...
healthcheck:
test: ["CMD", "curl", "-f", "http://localhost:80/health"]
    interval: 30s
    timeout: 10s
    retries: 3
builder.Services.AddDefaultCorrelationId();
app.UseCorrelationId();
