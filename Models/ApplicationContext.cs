using EFENGSI_RAHMANTO_ZALUKHU.Helper;
using EFENGSI_RAHMANTO_ZALUKHU.Models.DB;
using EFENGSI_RAHMANTO_ZALUKHU.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EFENGSI_RAHMANTO_ZALUKHU.Models
{
    public class ApplicationContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSaldo> UserSaldos { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductSize> ProductSizes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserSaldo>()
               .HasOne(p => p.User)
               .WithMany(p => p.UserSaldos)
               .HasForeignKey(p => p.UserId);

            // Product-Category one-to-many relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<ProductSize>()
              .HasOne(p => p.Product)
              .WithMany(p => p.ProductSizes)
              .HasForeignKey(p => p.ProductId);

            // Order-User many-to-one relationship
            modelBuilder.Entity<Order>()
              .HasOne(o => o.User)
              .WithMany(o => o.Orders)
              .HasForeignKey(o => o.UserId);

            // OrderDetail-Order many-to-one relationship
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(od => od.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            // OrderDetail-Product many-to-one relationship
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId);

            // Payment memiliki relasi one-to-one dengan tabel Order
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);

            // Seeding admin default with SHA512 hashing

             System.Diagnostics.Debug.WriteLine("cek321");
            var pepper = _configuration["Security:Papper"];
            var iteration = Convert.ToInt32(_configuration["Security:Iteration"]);
            var salt = Hasher.GenerateSalt();

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    Id = 1,
                    Name = "Administrator",
                    Username = "admin",
                    Email = "admin@example.com",
                    PhoneNumber = "081267874199",
                    Address = "Bandung",
                    Image = "default-product.png.jpeg",
                    Salt = salt,
                    PasswordHash = Hasher.ComputeHash("admin123", salt, pepper, iteration),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UserStatus = GeneralStatus.GeneralStatusData.Published
                }
            );


            base.OnModelCreating(modelBuilder);

        }

    }
}
