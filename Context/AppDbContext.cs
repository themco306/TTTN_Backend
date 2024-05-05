using Microsoft.EntityFrameworkCore;
using backend.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace backend.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Slider> Sliders{ get; set; }
        public DbSet<Order> Orders{ get; set; }
        public DbSet<OrderInfo> OrderInfos{ get; set; }
        public DbSet<OrderDetail> OrderDetails{ get; set; }

        // public DbSet<Cart> Carts { get; set; }
        // public DbSet<CartItem> CartItems{ get; set; }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is DateTimeInfo && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (DateTimeInfo)entry.Entity;
                var now = DateTime.UtcNow;
                now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                }

                entity.UpdatedAt = now;
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {

                e.HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(fk => fk.CreatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Slider>(e =>
            {
                e.HasOne(fk => fk.CreatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Product>(e =>
            {
            //     e.HasOne(c => c.Category)
            //    .WithMany()
            //    .HasForeignKey(c => c.CategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);


                e.HasOne(fk => fk.CreatedBy)
                  .WithMany()
                  .HasForeignKey(fk => fk.CreatedById)
                  .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });
 modelBuilder.Entity<OrderInfo>(e =>
            {
                e.HasOne(fk => fk.User)
                .WithMany()
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<Order>(e =>
            {
                e.HasOne(fk => fk.User)
                .WithMany()
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(fk => fk.OrderInfo)
                .WithMany()
                .HasForeignKey(fk => fk.OrderInfoId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });
                       
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.HasOne(fk => fk.Order)
                .WithMany(fk=>fk.OrderDetails)
                .HasForeignKey(fk => fk.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(fk => fk.Product)
                .WithMany()
                .HasForeignKey(fk => fk.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            

            // modelBuilder.Entity<Cart>(e =>
            // {
            //     e.HasOne(c => c.User)
            //    .WithMany()
            //    .HasForeignKey(c => c.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);
            // });
            // modelBuilder.Entity<CartItem>(e =>
            // {
            //     e.HasOne(c => c.Cart)
            //    .WithMany()
            //    .HasForeignKey(c => c.CartId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //    e.HasOne(c => c.Product)
            //    .WithMany()
            //    .HasForeignKey(c => c.ProductId)
            //    .OnDelete(DeleteBehavior.Cascade);
            // });

            base.OnModelCreating(modelBuilder);

        }
    }
}
