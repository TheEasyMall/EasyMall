using EasyMall.DALs.Entities;
using MayNghien.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EasyMall.DALs.Data
{
    public class ApplicationDbContext : BaseContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var appSetting = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText("appsettings.json"));
                optionsBuilder.UseMySql(appSetting!.ConnectionString, 
                    new MySqlServerVersion(new Version(8, 0, 40)));
            }
        }
    }
}
