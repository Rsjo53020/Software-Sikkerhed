using WebGoatCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebGoatCore.Data
{
    public class NorthwindContext : IdentityDbContext<IdentityUser>
    {
        public static string ConnString { get; private set; } = string.Empty;

        public static void Initialize(IConfiguration configuration, IHostEnvironment env)
        {
            var execDirectory = configuration.GetValue(Constants.WEBGOAT_ROOT, env.ContentRootPath);
            
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(execDirectory, "NORTHWND.sqlite")
            };

            ConnString = builder.ConnectionString;
            if (string.IsNullOrEmpty(ConnString))
            {
                throw new WebGoatCore.Exceptions.WebGoatStartupException(
                    "Cannot compute connection string to connect database!");
            }
        }

        private static readonly ILoggerFactory MyLoggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.AddDebug();

            });

        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options)
        {
        }

        public DbSet<BlogEntry> BlogEntries { get; set; } = null!;
        public DbSet<BlogResponse> BlogResponses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<OrderPayment> OrderPayments { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Shipper> Shippers { get; set; } = null!;
        public DbSet<Shipment> Shipments { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderDetail>().HasKey(a => new { a.ProductId, a.OrderId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}