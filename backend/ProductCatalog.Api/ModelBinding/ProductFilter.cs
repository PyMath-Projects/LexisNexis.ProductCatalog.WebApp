namespace ProductCatalog.Api.ModelBinding;

public record ProductFilter(string? Search, Guid? CategoryId);
