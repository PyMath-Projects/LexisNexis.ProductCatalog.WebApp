using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Features.Categories.GetCategories;
using ProductCatalog.Domain.Categories;

namespace ProductCatalog.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryHandler(
    ICategoryRepository categoryRepo,
    IDomainEventDispatcher dispatcher)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    /// <summary>Creates a category, verifying that the parent exists when a ParentCategoryId is supplied.</summary>
    public async Task<CategoryDto> Handle(CreateCategoryCommand cmd, CancellationToken ct)
    {
        if (cmd.ParentCategoryId.HasValue
            && !await categoryRepo.ExistsAsync(cmd.ParentCategoryId.Value, ct))
            throw new NotFoundException(nameof(Category), cmd.ParentCategoryId.Value);

        var category = Category.Create(cmd.Name, cmd.Description, cmd.ParentCategoryId);

        await categoryRepo.AddAsync(category, ct);
        await dispatcher.DispatchAsync([category], ct);

        return new CategoryDto(category.Id, category.Name, category.Description,
            category.ParentCategoryId, category.CreatedAt);
    }
}
