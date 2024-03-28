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
        public DbSet<Product> Products {get;set;}
        public DbSet<ProductCategory> ProductsCategoies {get;set;}
        public override int SaveChanges()
{
    var entities = ChangeTracker.Entries()
        .Where(x => x.Entity is DateTimeInfo && (x.State == EntityState.Added || x.State == EntityState.Modified));

    foreach (var entity in entities)
    {
        if (entity.State == EntityState.Added)
        {
            ((DateTimeInfo)entity.Entity).CreatedAt = DateTimeOffset.UtcNow.AddHours(7);
        }

        ((DateTimeInfo)entity.Entity).UpdatedAt = DateTimeOffset.UtcNow.AddHours(7);
    }

    return base.SaveChanges();
}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e=>{
                e.HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductCategory>(e=>{
                e.HasKey(pk=>new{pk.ProductId,pk.CategoryId});

                e.HasOne(fk=>fk.Product)
                .WithMany(fk=>fk.ProductCategories)
                .HasForeignKey(fk=>fk.ProductId);

                e.HasOne(fk=>fk.Category)
                .WithMany(fk=>fk.ProductCategories)
                .HasForeignKey(fk=>fk.CategoryId);
            });

            base.OnModelCreating(modelBuilder);
                
        }
    }
}
