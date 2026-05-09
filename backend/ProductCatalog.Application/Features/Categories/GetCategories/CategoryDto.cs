namespace ProductCatalog.Application.Features.Categories.GetCategories;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId,
    DateTimeOffset CreatedAt);
