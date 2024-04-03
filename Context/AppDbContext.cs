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
        public DbSet<Gallery> Galleries {get;set;}
        // public DbSet<ProductCategory> ProductsCategoies {get;set;}
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
                var now = DateTime.UtcNow; // Lấy giờ UTC

                if (entry.State == EntityState.Added)
                {
                    // Đổi giờ từ UTC sang giờ của khu vực của bạn
                    now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                    entity.CreatedAt = now;
                }

                entity.UpdatedAt = now;
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Category>(e =>
            // {
            //     e.HasOne(c => c.Parent)
            //     .WithMany()
            //     .HasForeignKey(c => c.ParentId)
            //     .OnDelete(DeleteBehavior.Restrict);

            //     e.HasOne(fk => fk.CreatedBy)
            //     .WithMany()
            //     .HasForeignKey(fk => fk.CreatedById)
            //     .OnDelete(DeleteBehavior.Restrict);

            //     e.HasOne(fk => fk.UpdatedBy)
            //     .WithMany()
            //     .HasForeignKey(fk => fk.UpdatedById)
            //     .OnDelete(DeleteBehavior.Restrict);
            // });



            // modelBuilder.Entity<ProductCategory>(e=>{
            //     e.HasKey(pk=>new{pk.ProductId,pk.CategoryId});

            //     e.HasOne(fk=>fk.Product)
            //     .WithMany(fk=>fk.ProductCategories)
            //     .HasForeignKey(fk=>fk.ProductId);

            //     e.HasOne(fk=>fk.Category)
            //     .WithMany(fk=>fk.ProductCategories)
            //     .HasForeignKey(fk=>fk.CategoryId);
            // });

            base.OnModelCreating(modelBuilder);

        }
    }
}
