using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.ModelBinding;
using ProductCatalog.Application.Features.Products.AdjustStock;
using ProductCatalog.Application.Features.Products.CreateProduct;
using ProductCatalog.Application.Features.Products.DeleteProduct;
using ProductCatalog.Application.Features.Products.DiscontinueProduct;
using ProductCatalog.Application.Features.Products.GetProductById;
using ProductCatalog.Application.Features.Products.GetProducts;
using ProductCatalog.Application.Features.Products.SearchProducts;
using ProductCatalog.Application.Features.Products.UpdateProduct;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [ModelBinder(typeof(ProductFilterModelBinder))] ProductFilter filter,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var results = await mediator.Send(new SearchProductsQuery(filter.Search), ct);
            return Ok(results);
        }

        var products = await mediator.Send(new GetProductsQuery(filter.CategoryId), ct);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest req, CancellationToken ct)
    {
        var cmd = new UpdateProductCommand(id, req.Name, req.Description, req.Price, req.Currency, req.CategoryId);
        return Ok(await mediator.Send(cmd, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] AdjustStockRequest req, CancellationToken ct)
    {
        await mediator.Send(new AdjustStockCommand(id, req.Delta), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/discontinue")]
    public async Task<IActionResult> Discontinue(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DiscontinueProductCommand(id), ct);
        return NoContent();
    }
}

public record UpdateProductRequest(string Name, string? Description, decimal Price, string Currency, Guid CategoryId);
public record AdjustStockRequest(int Delta);
