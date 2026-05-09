using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Features.Categories.CreateCategory;
using ProductCatalog.Application.Features.Categories.GetCategories;
using ProductCatalog.Application.Features.Categories.GetCategoryTree;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCategoriesQuery(), ct));

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCategoryTreeQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetCategories), new { id = result.Id }, result);
    }
}
