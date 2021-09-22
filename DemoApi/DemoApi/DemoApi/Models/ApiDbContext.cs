using Microsoft.EntityFrameworkCore;

namespace DemoApi.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1001, Name = "Book", Quantity = 100, UnitCost = 200 },
                new Product { ProductId = 1005, Name = "Pen", Quantity = 40, UnitCost = 15 },
                new Product { ProductId = 1010, Name = "Samsung Galaxy M31", Quantity = 20, UnitCost = 22999 },
                new Product { ProductId = 1015, Name = "Apple Ear Buds", Quantity = 12, UnitCost = 7989 }
            );
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "User1", Password = "Pass@1" },
                new User { Id = 2, Username = "ApiUser", Password = "User@123" },
                new User { Id = 3, Username = "AuthUser", Password = "User@111" }
            );
        }
    }
}
