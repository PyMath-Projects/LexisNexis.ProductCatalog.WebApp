using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .HasConversion(v => v.Value, v => ProductName.Create(v));

            entity.Property(p => p.Sku)
                .HasConversion(v => v.Value, v => Sku.Create(v));

            entity.Property(p => p.Quantity)
                .HasConversion(v => v.Value, v => StockQuantity.Of(v));

            // Money is an owned value object — column names are irrelevant for InMemory
            entity.OwnsOne(p => p.Price, money =>
            {
                money.Property(nameof(Money.Amount));
                money.Property(nameof(Money.Currency));
            });

            entity.Property(p => p.Status);
            entity.Property(p => p.CategoryId);
            entity.Property(p => p.Description);
            entity.Property(p => p.CreatedAt);
            entity.Property(p => p.UpdatedAt);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name);
            entity.Property(c => c.Description);
            entity.Property(c => c.ParentCategoryId);
            entity.Property(c => c.CreatedAt);
        });
    }
}
