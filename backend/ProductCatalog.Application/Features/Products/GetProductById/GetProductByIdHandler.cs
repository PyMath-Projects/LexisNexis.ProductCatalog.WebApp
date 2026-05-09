using MediatR;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Application.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(IProductRepository productRepo)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    /// <summary>Returns the product DTO for the given ID. Throws NotFoundException when the product does not exist.</summary>
    public async Task<ProductDto> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await productRepo.GetByIdAsync(query.ProductId, ct)
            ?? throw new NotFoundException(nameof(Domain.Products.Product), query.ProductId);

        return product.ToDto();
    }
}
