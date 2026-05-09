using ProductCatalog.Domain.Products.Events;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Products;

public sealed class Product : AggregateRoot<Guid>, IComparable<Product>
{
    public ProductName Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public Sku Sku { get; private set; } = default!;

    public Money Price { get; private set; } = default!;

    public StockQuantity Quantity { get; private set; } = default!;

    public Guid CategoryId { get; private set; }

    public ProductStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    private Product()
    {
    }

    /// <summary>
    /// Creates a product while enforcing category identity and initial stock invariants.
    /// </summary>
    public static Product Create(
        ProductName name,
        string? description,
        Sku sku,
        Money price,
        int initialQuantity,
        Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new DomainException("CategoryId cannot be empty.");
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;
        Product product = new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Sku = sku,
            Price = price,
            Quantity = StockQuantity.Of(initialQuantity),
            CategoryId = categoryId,
            Status = initialQuantity > 0 ? ProductStatus.Active : ProductStatus.OutOfStock,
            CreatedAt = now,
            UpdatedAt = now
        };

        product.RaiseDomainEvent(new ProductCreated(product.Id, sku.Value, now));
        return product;
    }

    /// <summary>
    /// Updates mutable catalog details while preserving the discontinued product invariant.
    /// </summary>
    public void UpdateDetails(ProductName name, string? description, Money price, Guid categoryId)
    {
        EnsureNotDiscontinued("update details on");
        if (categoryId == Guid.Empty)
        {
            throw new DomainException("CategoryId cannot be empty.");
        }

        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ProductUpdated(Id, UpdatedAt));
    }

    /// <summary>
    /// Adjusts stock while preventing negative inventory and keeping status aligned to stock level.
    /// </summary>
    public void AdjustStock(int delta)
    {
        EnsureNotDiscontinued("adjust stock on");

        int previous = Quantity.Value;
        Quantity = Quantity.Adjust(delta);
        Status = Quantity.IsEmpty ? ProductStatus.OutOfStock : ProductStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new StockLevelChanged(Id, previous, Quantity.Value, UpdatedAt));
    }

    /// <summary>
    /// Discontinues an active or out-of-stock product and prevents duplicate discontinuation.
    /// </summary>
    public void Discontinue()
    {
        if (Status == ProductStatus.Discontinued)
        {
            throw new DomainException($"Product '{Name}' is already discontinued.");
        }

        Status = ProductStatus.Discontinued;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ProductDiscontinued(Id, UpdatedAt));
    }

    /// <summary>
    /// Reactivates only discontinued products and re-establishes stock-derived status.
    /// </summary>
    public void Reactivate(int restockQuantity)
    {
        if (Status != ProductStatus.Discontinued)
        {
            throw new DomainException($"Only discontinued products can be reactivated. Current status: {Status}.");
        }

        Guard.AgainstNegative(restockQuantity, nameof(restockQuantity));

        Quantity = StockQuantity.Of(restockQuantity);
        Status = restockQuantity > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new ProductReactivated(Id, UpdatedAt));
    }

    /// <summary>Raises ProductDeleted so the dispatcher can publish the event after the repo removes the record.</summary>
    public void MarkAsDeleted()
    {
        RaiseDomainEvent(new ProductDeleted(Id, DateTimeOffset.UtcNow));
    }

    public int CompareTo(Product? other)
    {
        if (other is null)
        {
            return 1;
        }

        int priceComparison = Price.Amount.CompareTo(other.Price.Amount);
        return priceComparison != 0
            ? priceComparison
            : string.Compare(Name.Value, other.Name.Value, StringComparison.OrdinalIgnoreCase);
    }

    private void EnsureNotDiscontinued(string action)
    {
        if (Status == ProductStatus.Discontinued)
        {
            throw new DomainException($"Cannot {action} discontinued product '{Name}'.");
        }
    }
}
