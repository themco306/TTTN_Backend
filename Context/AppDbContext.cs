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
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderInfo> OrderInfos { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<WebInfo> WebInfos { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<PaidOrder> PaidOrders { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Contact> Contacts { get; set; }


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
            modelBuilder.Entity<Brand>(e =>
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
                    e.HasOne(c => c.Category)
                   .WithMany(c=>c.Products)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
                    e.HasOne(c => c.Brand)
                   .WithMany(c=>c.Products)
                   .HasForeignKey(c => c.BrandId)
                   .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.CreatedBy)
                  .WithMany()
                  .HasForeignKey(fk => fk.CreatedById)
                  .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<ProductTag>(e =>
            {
                // Khai báo khóa chính kết hợp của bảng ProductTag
                e.HasKey(pt => new { pt.ProductId, pt.TagId });

                // Định nghĩa mối quan hệ với bảng Product
                e.HasOne(pt => pt.Product)
                      .WithMany(p => p.ProductTags) // Một Product có thể có nhiều ProductTag
                      .HasForeignKey(pt => pt.ProductId) // Khóa ngoại sẽ là ProductId
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pt => pt.Tag)
                      .WithMany(t => t.ProductTags) // Một Tag có thể có nhiều ProductTag
                      .HasForeignKey(pt => pt.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<OrderInfo>(e =>
                       {
                           e.HasOne(fk => fk.User)
                           .WithMany()
                           .HasForeignKey(fk => fk.UserId)
                           .OnDelete(DeleteBehavior.Cascade);
                       });
            modelBuilder.Entity<Order>(e =>
            {
                e.HasOne(o => o.PaidOrder)
                .WithOne(p => p.Order)
                .HasForeignKey<PaidOrder>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .WithMany(fk => fk.OrderDetails)
                .HasForeignKey(fk => fk.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(fk => fk.Product)
                .WithMany()
                .HasForeignKey(fk => fk.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Cart>(e =>
            {
                e.HasOne(c => c.User)
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CartItem>(e =>
            {
                e.HasOne(c => c.Cart)
               .WithMany(c=>c.CartItems)
               .HasForeignKey(c => c.CartId)
               .OnDelete(DeleteBehavior.Cascade);

               e.HasOne(c => c.Product)
               .WithMany()
               .HasForeignKey(c => c.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
            });

                        modelBuilder.Entity<Coupon>(e =>
            {
                 e.HasIndex(c => c.Code)
                .IsUnique();


                e.HasOne(fk => fk.CreatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.CreatedById)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.Cascade);
            });
                         modelBuilder.Entity<CouponUsage>(e =>
            {
                e.HasOne(fk=>fk.User)
                .WithMany()
                .HasForeignKey(fk=>fk.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(fk=>fk.Coupon)
                .WithMany(c=>c.CouponUsages)
                .HasForeignKey(fk=>fk.CouponId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(fk=>fk.Order)
                .WithMany()
                .HasForeignKey(fk=>fk.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            });

              modelBuilder.Entity<PaidOrder>(e =>
            {

                // e.HasOne(fk => fk.Order)
                // .WithMany()
                // .HasForeignKey(fk => fk.OrderId)
                // .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Menu>(e =>
            {
                e.HasOne(fk=>fk.Parent)
                .WithMany()
                .HasForeignKey(fk=>fk.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(fk => fk.CreatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<Topic>(e =>
            {
                e.HasOne(fk=>fk.Parent)
                .WithMany()
                .HasForeignKey(fk=>fk.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(fk => fk.CreatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });
                        modelBuilder.Entity<Post>(e =>
            {
                e.HasOne(fk=>fk.Topic)
                .WithMany()
                .HasForeignKey(fk=>fk.TopicId)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.CreatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(fk => fk.UpdatedBy)
                .WithMany()
                .HasForeignKey(fk => fk.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            });
            base.OnModelCreating(modelBuilder);

        }
    }
}
