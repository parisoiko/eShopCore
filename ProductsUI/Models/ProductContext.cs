using Microsoft.EntityFrameworkCore;

namespace ProductsUI.Models
{
    public class ProductContext : DbContext
    {
        private readonly IConfiguration _context;
        public ProductContext(IConfiguration connectionString) {
            _context = connectionString;
        }
        public DbSet<Product> Product { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_context.GetConnectionString("ProductDbContext"));
        }
    }
}
