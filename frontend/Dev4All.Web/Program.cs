using Microsoft.AspNetCore.Authentication.Cookies;
using Dev4All.Web.Infrastructure;
using Dev4All.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.CustomerOrAdmin, policy =>
        policy.RequireRole(AppRoles.Customer, AppRoles.Admin));
    options.AddPolicy(AppPolicies.DeveloperOrAdmin, policy =>
        policy.RequireRole(AppRoles.Developer, AppRoles.Admin));
});
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = ".Dev4All.Auth";
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient(nameof(AuthService), (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["BackendApi:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("BackendApi:BaseUrl is not configured.");
    }

    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["BackendApi:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("BackendApi:BaseUrl is not configured.");
    }

    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<ApiTokenHandler>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
