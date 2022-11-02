using Microsoft.EntityFrameworkCore;

namespace eShopCore.Models
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable("Products");
        }
    }
}
