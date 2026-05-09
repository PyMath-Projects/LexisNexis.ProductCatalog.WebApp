using Microsoft.AspNetCore.Diagnostics;
using ProductCatalog.Api.Middleware;
using ProductCatalog.Api.Serialization;
using ProductCatalog.Application.Common.Behaviours;
using ProductCatalog.Application.Common.Events;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Domain.Shared;
using ProductCatalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Health checks ──────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

// ── Controllers + JSON ─────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(new SkuJsonConverter()));

// ── MediatR: scan Application assembly, add pipeline behaviours ────────────
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(DomainEventNotification<>).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
});

// ── Infrastructure (EF InMemory, repos, dispatcher, search cache) ──────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Middleware ─────────────────────────────────────────────────────────────
builder.Services.AddTransient<RequestTimingMiddleware>();

var app = builder.Build();

// Exception handler — maps domain/application exceptions to HTTP status codes
app.UseExceptionHandler(errorApp =>
    errorApp.Run(async ctx =>
    {
        var feature = ctx.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = ex switch
        {
            NotFoundException   => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            DomainException     => StatusCodes.Status400BadRequest,
            _                   => StatusCodes.Status500InternalServerError
        };

        await ctx.Response.WriteAsJsonAsync(new { error = ex?.Message });
    }));

app.UseMiddleware<RequestTimingMiddleware>();
app.MapHealthChecks("/health");
app.MapControllers();

await app.InitialiseDatabaseAsync(app.Configuration);
app.Run();
