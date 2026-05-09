using FluentAssertions;
using Moq;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Categories.CreateCategory;
using ProductCatalog.Domain.Categories;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Application.Tests.Features.Categories;

public sealed class CreateCategoryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepo = new();
    private readonly Mock<IDomainEventDispatcher> _dispatcher = new();

    private CreateCategoryHandler CreateHandler() =>
        new(_categoryRepo.Object, _dispatcher.Object);

    [Fact]
    public async Task Handle_RootCategory_CreatesCategoryAndDispatches()
    {
        var cmd = new CreateCategoryCommand("Electronics", "All electronics", null);

        var result = await CreateHandler().Handle(cmd, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Electronics");
        result.ParentCategoryId.Should().BeNull();
        _categoryRepo.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _dispatcher.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<AggregateRoot<Guid>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ChildCategoryWithExistingParent_Creates()
    {
        var parentId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.ExistsAsync(parentId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var cmd = new CreateCategoryCommand("Laptops", null, parentId);
        var result = await CreateHandler().Handle(cmd, CancellationToken.None);

        result.ParentCategoryId.Should().Be(parentId);
    }

    [Fact]
    public async Task Handle_ParentNotFound_ThrowsNotFoundException()
    {
        var parentId = Guid.NewGuid();
        _categoryRepo.Setup(r => r.ExistsAsync(parentId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var cmd = new CreateCategoryCommand("Laptops", null, parentId);
        var act = () => CreateHandler().Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"*{parentId}*");
    }
}
