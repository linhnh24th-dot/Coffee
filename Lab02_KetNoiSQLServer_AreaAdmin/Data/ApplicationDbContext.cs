using Microsoft.EntityFrameworkCore;
using HyliCoffeeWeb.Models;

namespace HyliCoffeeWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Article> Articles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Staff> Staffs { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== User =====
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Username).IsUnique();
                e.Property(u => u.Role).HasConversion<int>();
            });

            // ===== Order =====
            modelBuilder.Entity<Order>(e =>
            {
                e.Property(o => o.Status).HasConversion<int>();

                e.HasOne(o => o.User)
                 .WithMany(u => u.Orders)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== OrderDetail =====
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.HasOne(od => od.Order)
                 .WithMany(o => o.OrderDetails)
                 .HasForeignKey(od => od.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(od => od.Product)
                 .WithMany(p => p.OrderDetails)
                 .HasForeignKey(od => od.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Lưu ý: KHÔNG seed User bằng HasData ở đây vì cần hash mật khẩu bằng
            // PasswordHasher (chạy lúc runtime). Việc seed Admin + dữ liệu mẫu được
            // thực hiện trong Data/DbInitializer.cs, gọi từ Program.cs sau khi migrate.
        }
    }
}
