using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;

namespace eShopCore.Models
{

    public class Product
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }
        public string Category { get; set; }
        public float Price { get; set; }
        public string ImageSource { get; set; }
        public string Manufacturer { get; set; }
       
        

        [Timestamp]
        public byte[] Concurrency { get; set; }
        public Product Update(Product product) {
            Title = product.Title;
            DescriptionShort = product.DescriptionShort;
            DescriptionLong = product.DescriptionLong;
            Category = product.Category;
            Price = product.Price;
            ImageSource = product.ImageSource;
            Manufacturer = product.Manufacturer;
            return this;
        }
    }


    public class SearchParameters
    {
        public SearchParameters() {
            PageSize = 4;
        }

        public string Query { get; set; }
        public int? Page { get; set; }
        public int PageSize { get; set; }

    }
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) {
        }
        public MyDbContext() {

        }

        public DbSet<Product> Products { get; set; }
    }

    public class ProductDbContext : MyDbContext
    {
        public ProductDbContext(DbContextOptions options) : base(options) {

        }
        public ProductDbContext() {

        }
    }

    public class SqliteDbContext : MyDbContext
    {
        public SqliteDbContext() {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Filename=eShopDatabase.db", options => {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Map table names
            modelBuilder.Entity<Product>().ToTable("Products", "test");
            modelBuilder.Entity<Product>(entity => {
                entity.HasKey(e => e.ID);

                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.DescriptionLong).IsRequired();
                entity.Property(e => e.DescriptionShort).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.ImageSource).IsRequired();
                entity.Property(e => e.Manufacturer).IsRequired();
                entity.Property(e => e.Concurrency).IsRequired();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}

