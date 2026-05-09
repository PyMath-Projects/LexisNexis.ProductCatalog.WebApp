namespace ProductCatalog.Application.Features.Categories.GetCategoryTree;

public record CategoryTreeDto(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyList<CategoryTreeDto> Children);
