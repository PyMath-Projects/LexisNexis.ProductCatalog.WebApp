using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Products;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<ProductCatalogDbContext>(opts =>
            opts.UseInMemoryDatabase("ProductCatalog"));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<ISearchCache, SearchCacheRepository>();

        return services;
    }

    /// <summary>Creates the InMemory database and optionally seeds demo data. Called from Api/Program.cs.</summary>
    public static async Task InitialiseDatabaseAsync(this IHost host, IConfiguration config)
    {
        var seedData = bool.TryParse(config["Database:SeedData"], out var v) && v;

        await using var scope = host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (seedData)
            await DatabaseSeeder.SeedAsync(db);
    }
}
