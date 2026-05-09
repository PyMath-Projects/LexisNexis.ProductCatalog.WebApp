using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Infrastructure.Persistence;

internal static class DatabaseSeeder
{
    internal static async Task SeedAsync(ProductCatalogDbContext db)
    {
        if (await db.Categories.AnyAsync()) return;

        var electronics = Category.Create("Electronics", "Electronic devices", null);
        var laptops     = Category.Create("Laptops", "Portable computers", electronics.Id);
        var phones      = Category.Create("Phones", "Mobile devices", electronics.Id);

        await db.Categories.AddRangeAsync([electronics, laptops, phones]);
        await db.SaveChangesAsync();

        var macbook = Product.Create(
            ProductName.Create("MacBook Pro 14"),
            "Apple M3 Pro chip, 18GB RAM",
            Sku.Create("LT-000001"),
            Money.Of(1999.99m),
            15, laptops.Id);

        var thinkpad = Product.Create(
            ProductName.Create("ThinkPad X1 Carbon"),
            "Intel Core Ultra 7, 32GB RAM",
            Sku.Create("LT-000002"),
            Money.Of(1599.99m),
            8, laptops.Id);

        var iphone = Product.Create(
            ProductName.Create("iPhone 15 Pro"),
            "A17 Pro chip, 256GB",
            Sku.Create("PH-000001"),
            Money.Of(999.99m),
            25, phones.Id);

        await db.Products.AddRangeAsync([macbook, thinkpad, iphone]);
        await db.SaveChangesAsync();
    }
}
