using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Features.Categories.CreateCategory;
using ProductCatalog.Application.Features.Products.AdjustStock;
using ProductCatalog.Application.Features.Products.CreateProduct;
using ProductCatalog.Application.Features.Products.UpdateProduct;

namespace ProductCatalog.Application.Common.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse>(ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var error = request switch
        {
            CreateProductCommand { Name: null or "" }     => "Name is required",
            CreateProductCommand { Price: <= 0 }          => "Price must be positive",
            CreateProductCommand { Sku: null or "" }      => "SKU is required",
            CreateProductCommand { InitialQuantity: < 0 } => "Quantity cannot be negative",
            CreateProductCommand { CategoryId: var id }
                when id == Guid.Empty                     => "CategoryId is required",
            UpdateProductCommand { Name: null or "" }     => "Name is required",
            AdjustStockCommand { ProductId: var id }
                when id == Guid.Empty                     => "ProductId is required",
            CreateCategoryCommand { Name: null or "" }    => "Category name is required",
            _                                             => null
        };

        if (error is not null)
        {
            logger.LogWarning("Validation failed for {Request}: {Error}", typeof(TRequest).Name, error);
            throw new ValidationException(error);
        }

        return await next();
    }
}
