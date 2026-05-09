using FluentAssertions;
using Moq;
using ProductCatalog.Application.Features.Categories.GetCategoryTree;
using ProductCatalog.Domain.Categories;

namespace ProductCatalog.Application.Tests.Features.Categories;

public sealed class GetCategoryTreeHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepo = new();

    private GetCategoryTreeHandler CreateHandler() => new(_categoryRepo.Object);

    [Fact]
    public async Task Handle_FlatCategories_ReturnsRootsWithNoChildren()
    {
        var cats = new[] { Category.Create("A", null, null), Category.Create("B", null, null) };
        _categoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(cats);

        var result = await CreateHandler().Handle(new GetCategoryTreeQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(c => c.Children.Count == 0).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NestedCategories_BuildsTreeCorrectly()
    {
        var root = Category.Create("Root", null, null);
        var child = Category.Create("Child", null, root.Id);
        _categoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([root, child]);

        var result = await CreateHandler().Handle(new GetCategoryTreeQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Root");
        result[0].Children.Should().HaveCount(1);
        result[0].Children[0].Name.Should().Be("Child");
    }

    [Fact]
    public async Task Handle_EmptyRepo_ReturnsEmptyList()
    {
        _categoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Category>());

        var result = await CreateHandler().Handle(new GetCategoryTreeQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
