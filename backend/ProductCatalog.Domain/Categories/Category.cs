using ProductCatalog.Domain.Categories.Events;
using ProductCatalog.Domain.Shared;

namespace ProductCatalog.Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Category() { }

    /// <summary>Creates a new Category and raises CategoryCreated. ParentCategoryId null means root.</summary>
    public static Category Create(string name, string? description, Guid? parentCategoryId)
    {
        var now = DateTimeOffset.UtcNow;
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name)).Trim(),
            Description = description?.Trim(),
            ParentCategoryId = parentCategoryId,
            CreatedAt = now
        };

        category.RaiseDomainEvent(new CategoryCreated(category.Id, parentCategoryId, now));
        return category;
    }

    /// <summary>Renames the category. No-op when the new name matches the current name.</summary>
    public void Rename(string newName)
    {
        var cleaned = Guard.AgainstNullOrWhiteSpace(newName, nameof(newName)).Trim();
        if (Name == cleaned) return;
        Name = cleaned;
    }

    /// <summary>Replaces the optional description.</summary>
    public void UpdateDescription(string? description) =>
        Description = description?.Trim();

    /// <summary>Moves the category under a new parent. Circular-reference prevention is the caller's responsibility.</summary>
    public void MoveTo(Guid? newParentId)
    {
        if (newParentId == Id)
            throw new DomainException("A category cannot be its own parent.");
        ParentCategoryId = newParentId;
    }

    /// <summary>Returns true when the category has no parent (is a root category).</summary>
    public bool IsRoot() => ParentCategoryId is null;
}
