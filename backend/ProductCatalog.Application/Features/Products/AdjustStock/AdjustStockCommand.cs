using MediatR;

namespace ProductCatalog.Application.Features.Products.AdjustStock;

public record AdjustStockCommand(Guid ProductId, int Delta) : IRequest;
