using FluentAssertions;
using ProductCatalog.Domain.Products;
using ProductCatalog.Domain.Products.Events;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Tests.Products;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidInputs_RaisesProductCreatedEvent()
    {
        Product product = CreateProduct(initialQuantity: 10);

        product.Id.Should().NotBeEmpty();
        product.Status.Should().Be(ProductStatus.Active);
        product.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreated>();
    }

    [Fact]
    public void AdjustStock_WhenDeltaIsLargeNegative_ThrowsDomainException()
    {
        Product product = CreateProduct(initialQuantity: 10);

        Action act = () => product.AdjustStock(-999);

        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient stock*");
    }

    [Fact]
    public void AdjustStock_WhenDeltaExceedsQuantity_ThrowsDomainException()
    {
        Product product = CreateProduct(initialQuantity: 3);

        Action act = () => product.AdjustStock(-5);

        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient stock*");
    }

    [Fact]
    public void AdjustStock_WhenRestockingActiveProduct_RaisesStockLevelChanged()
    {
        Product product = CreateProduct(initialQuantity: 3);
        product.ClearDomainEvents();

        product.AdjustStock(5);

        product.Quantity.Value.Should().Be(8);
        product.Status.Should().Be(ProductStatus.Active);
        StockLevelChanged domainEvent = product.GetDomainEvents()
            .Should().ContainSingle().Which.Should().BeOfType<StockLevelChanged>().Subject;
        domainEvent.Previous.Should().Be(3);
        domainEvent.Current.Should().Be(8);
    }

    [Fact]
    public void AdjustStock_WhenStockBecomesEmpty_SetsStatusToOutOfStock()
    {
        Product product = CreateProduct(initialQuantity: 3);
        product.ClearDomainEvents();

        product.AdjustStock(-3);

        product.Quantity.Value.Should().Be(0);
        product.Status.Should().Be(ProductStatus.OutOfStock);
        product.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<StockLevelChanged>();
    }

    [Fact]
    public void Discontinue_WhenActive_RaisesProductDiscontinued()
    {
        Product product = CreateProduct(initialQuantity: 3);
        product.ClearDomainEvents();

        product.Discontinue();

        product.Status.Should().Be(ProductStatus.Discontinued);
        product.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ProductDiscontinued>();
    }

    [Fact]
    public void Discontinue_WhenAlreadyDiscontinued_ThrowsDomainException()
    {
        Product product = CreateProduct(initialQuantity: 3);
        product.Discontinue();

        Action act = product.Discontinue;

        act.Should().Throw<DomainException>()
            .WithMessage("*already discontinued*");
    }

    [Fact]
    public void Reactivate_WhenProductIsNotDiscontinued_ThrowsDomainException()
    {
        Product product = CreateProduct(initialQuantity: 3);

        Action act = () => product.Reactivate(5);

        act.Should().Throw<DomainException>()
            .WithMessage("Only discontinued products can be reactivated*");
    }

    [Fact]
    public void Reactivate_WhenProductIsDiscontinued_SetsStockAndRaisesProductReactivated()
    {
        Product product = CreateProduct(initialQuantity: 3);
        product.Discontinue();
        product.ClearDomainEvents();

        product.Reactivate(5);

        product.Quantity.Value.Should().Be(5);
        product.Status.Should().Be(ProductStatus.Active);
        product.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ProductReactivated>();
    }

    [Fact]
    public void UpdateDetails_WhenProductIsDiscontinued_ThrowsDomainException()
    {
        Product product = CreateProduct(initialQuantity: 3);
        product.Discontinue();

        Action act = () => product.UpdateDetails(
            ProductName.Create("Updated"),
            "Updated description",
            Money.Of(15m),
            Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Cannot update details on discontinued product*");
    }

    [Fact]
    public void UpdateDetails_WhenProductIsActive_UpdatesDetailsAndRaisesProductUpdated()
    {
        Product product = CreateProduct(initialQuantity: 3);
        Guid categoryId = Guid.NewGuid();
        product.ClearDomainEvents();

        product.UpdateDetails(
            ProductName.Create("Updated"),
            "Updated description",
            Money.Of(15m),
            categoryId);

        product.Name.Value.Should().Be("Updated");
        product.Description.Should().Be("Updated description");
        product.Price.Amount.Should().Be(15m);
        product.CategoryId.Should().Be(categoryId);
        product.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<ProductUpdated>();
    }

    [Fact]
    public void CompareTo_WhenProductsHaveDifferentPrices_SortsByPrice()
    {
        Product low = CreateProduct(name: "Zeta", price: 10m, sku: "EL-000001");
        Product high = CreateProduct(name: "Alpha", price: 20m, sku: "EL-000002");

        Product[] products = [high, low];
        Array.Sort(products);

        products.Should().Equal(low, high);
    }

    [Fact]
    public void CompareTo_WhenProductsHaveSamePrice_SortsByName()
    {
        Product alpha = CreateProduct(name: "Alpha", price: 10m, sku: "EL-000001");
        Product zeta = CreateProduct(name: "Zeta", price: 10m, sku: "EL-000002");

        Product[] products = [zeta, alpha];
        Array.Sort(products);

        products.Should().Equal(alpha, zeta);
    }

    private static Product CreateProduct(
        string name = "Laptop",
        decimal price = 10m,
        string sku = "EL-004521",
        int initialQuantity = 1) =>
        Product.Create(
            ProductName.Create(name),
            "Portable computer",
            Sku.Create(sku),
            Money.Of(price),
            initialQuantity,
            Guid.NewGuid());
}
