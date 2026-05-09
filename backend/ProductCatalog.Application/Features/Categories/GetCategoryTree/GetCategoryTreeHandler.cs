using MediatR;
using ProductCatalog.Domain.Categories;

namespace ProductCatalog.Application.Features.Categories.GetCategoryTree;

public sealed class GetCategoryTreeHandler(ICategoryRepository categoryRepo)
    : IRequestHandler<GetCategoryTreeQuery, IReadOnlyList<CategoryTreeDto>>
{
    /// <summary>Builds a recursive category tree ordered alphabetically at each level.</summary>
    public async Task<IReadOnlyList<CategoryTreeDto>> Handle(GetCategoryTreeQuery query, CancellationToken ct)
    {
        var all = await categoryRepo.GetAllAsync(ct);
        var lookup = all.ToLookup(c => c.ParentCategoryId);
        return BuildTree(lookup, parentId: null);
    }

    private static IReadOnlyList<CategoryTreeDto> BuildTree(
        ILookup<Guid?, Category> lookup,
        Guid? parentId) =>
        lookup[parentId]
            .OrderBy(c => c.Name)
            .Select(c => new CategoryTreeDto(c.Id, c.Name, c.Description, BuildTree(lookup, c.Id)))
            .ToList()
            .AsReadOnly();
}
