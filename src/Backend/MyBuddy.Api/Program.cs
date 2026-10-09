using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBuddy.Application.Tenancy;
using MyBuddy.Infrastructure.Identity;
using MyBuddy.Infrastructure.Persistence;
using MyBuddy.Infrastructure.Tenancy;
using MyBuddy.Api.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MyBuddyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyBuddy")
        ?? throw new InvalidOperationException("ConnectionStrings:MyBuddy must be configured.")));
builder.Services.AddIdentity<MyBuddyUser, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<MyBuddyDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<CurrentTenantContext>();
builder.Services.AddScoped<ITenantContext>(
    services => services.GetRequiredService<CurrentTenantContext>());
builder.Services.AddScoped<IActiveTenantContext>(
    services => services.GetRequiredService<CurrentTenantContext>());
builder.Services.AddScoped<ITenantMembershipReader, EfTenantMembershipReader>();
builder.Services.AddScoped<ActiveTenantResolver>();

var app = builder.Build();

app.UseAuthentication();
app.UseMiddleware<ActiveTenantMiddleware>();
app.UseAuthorization();
app.MapOpenApi();
app.MapHealthChecks("/health");
app.MapGet("/api/v1/tenant/current", (ITenantContext tenant) =>
        Results.Ok(new { tenantId = tenant.TenantId }))
    .RequireAuthorization();

app.Run();
