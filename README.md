Here’s a complete ASP.NET Core Web API example using Entity Framework Core + SQL Server.
This example uses:
.NET 10
EF Core
SQL Server
Swagger enabled

✅ Step 1: Create Project
dotnet new webapi -n ProductApiEF
cd ProductApiEF


✅ Step 2: Install EF Core Packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.8
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.8


✅ Step 3: Create Model
📁 Models/Product.cs
namespace ProductApiEF.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}


✅ Step 4: Create DbContext
📁 Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProductApiEF.Models;

namespace ProductApiEF.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}


✅ Step 5: Configure Connection String
📁 appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

If using SQL Authentication:
"Server=localhost;Database=ProductDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"


✅ Step 6: Configure Program.cs
📁 Program.cs
using Microsoft.EntityFrameworkCore;
using ProductApiEF.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();


✅ Step 7: Create Controller
📁 Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApiEF.Data;
using ProductApiEF.Models;

namespace ProductApiEF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}


✅ Step 8: Create Database Using Migrations
Run:
dotnet ef migrations add InitialCreate
dotnet ef database update

This will:
Create ProductDb
Create Products table

✅ Step 9: Run Project
dotnet run

Open:
https://localhost:xxxx/swagger

You will see:
GET
POST
PUT
DELETE endpoints

🔥 Example POST JSON
{
  "name": "Monitor",
  "price": 299.99
}


🎯 Database Table Created
SQL Server table:
Products
-------------------------
Id      int (PK, Identity)
Name    nvarchar(max)
Price   decimal(18,2)
