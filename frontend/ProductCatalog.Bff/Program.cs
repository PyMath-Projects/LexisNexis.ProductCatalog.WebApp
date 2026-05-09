using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// ── 1. YARP Reverse Proxy ─────────────────────────────────────────────────
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ── 2. CSRF Protection ────────────────────────────────────────────────────
builder.Services.AddAntiforgery(options =>
    options.HeaderName = "X-XSRF-TOKEN");

// ── 3. Auth Extension Point (inactive — uncomment when auth is needed) ────
// builder.Services.AddAuthentication(options => { ... })
//     .AddCookie("session")
//     .AddOpenIdConnect("oidc", options => {
//         options.Authority = "https://your-identity-server";
//         options.ClientId  = "product-catalog-bff";
//         options.ResponseType = "code";
//         options.SaveTokens = true;
//     });
// builder.Services.AddAuthorization();

// ── 4. CORS — dev only ────────────────────────────────────────────────────
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
        options.AddPolicy("DevAngular", p => p
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));
}

// ── 5. Health checks ──────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
    app.UseCors("DevAngular");

app.UseAntiforgery();

// Auth middleware slots (inactive — uncomment when auth is added):
// app.UseAuthentication();
// app.UseAuthorization();

// ── CSRF token endpoint ───────────────────────────────────────────────────
app.MapGet("/bff/antiforgery", (IAntiforgery af, HttpContext ctx) =>
{
    var tokens = af.GetAndStoreTokens(ctx);
    ctx.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
        new CookieOptions { HttpOnly = false });
    return Results.Ok();
}).AllowAnonymous();

// Auth management endpoints (inactive — add when auth is wired):
// app.MapGet("/bff/login",  ...) → redirect to OIDC provider
// app.MapGet("/bff/logout", ...) → clear session + redirect
// app.MapGet("/bff/user",   ...) → return claims as JSON for Angular

// ── Health ────────────────────────────────────────────────────────────────
app.MapHealthChecks("/health");

// ── YARP — proxies /api/* to Api ──────────────────────────────────────────
// When auth is added: .RequireAuthorization() on routes that need it.
app.MapReverseProxy();

// ── Angular SPA static files (production) ────────────────────────────────
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();
