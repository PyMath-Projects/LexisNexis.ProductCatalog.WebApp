using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductCatalog.Api.ModelBinding;

/// <summary>Reads product filter parameters directly from the query string. No [ApiController] validation applied.</summary>
public sealed class ProductFilterModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var query = bindingContext.HttpContext.Request.Query;

        var search = query.TryGetValue("search", out var s) ? s.ToString() : null;

        Guid? categoryId = null;
        if (query.TryGetValue("categoryId", out var catStr)
            && Guid.TryParse(catStr, out var parsed))
            categoryId = parsed;

        bindingContext.Result = ModelBindingResult.Success(new ProductFilter(search, categoryId));
        return Task.CompletedTask;
    }
}
