using Dev4All.Application;
using Dev4All.Application.Options;
using Dev4All.Infrastructure;
using Dev4All.Persistence;
using Dev4All.Persistence.Context;
using Dev4All.Persistence.Identity;
using Dev4All.WebAPI.Seed;
using Dev4All.WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Options — validate at startup so misconfiguration fails early
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection(SmtpOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// DbContext — reads connection string from DatabaseOptions
var dbOptions = builder.Configuration
    .GetSection(DatabaseOptions.SectionName)
    .Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database configuration section is missing.");

// Workaround: Npgsql 10 + Supabase pooler kombinasyonunda bazı ortamlarda
// ResetCancellation/ManualResetEventSlim kaynaklı ObjectDisposedException oluşabiliyor.
// Bu yüzden yalnızca "supabase" + "pooler" tespit edilirse ve kullanıcı GSS ayarı vermemişse
// connection string'e "GSS Encryption Mode=Disable" eklenir.
// Not: Bu sadece GSS katmanını kapatır; SSL/TLS şifrelemesi devam eder.
var connectionString = dbOptions.ConnectionString;
var connSegments = connectionString
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var shouldDisableGssForPooler =
    connectionString.Contains("supabase", StringComparison.OrdinalIgnoreCase) &&
    connectionString.Contains("pooler", StringComparison.OrdinalIgnoreCase) &&
    !connSegments.Any(s =>
        s.StartsWith("GssEncMode=", StringComparison.OrdinalIgnoreCase) ||
        s.StartsWith("GssEncryptionMode=", StringComparison.OrdinalIgnoreCase) ||
        s.StartsWith("GSS Encryption Mode=", StringComparison.OrdinalIgnoreCase));

var effectiveConnectionString = shouldDisableGssForPooler
    ? $"{connectionString};GSS Encryption Mode=Disable"
    : connectionString;

builder.Services.AddDbContext<Dev4AllDbContext>(options =>
{
    options.UseNpgsql(effectiveConnectionString, npgsql =>
    {
        if (dbOptions.MaxRetryCount > 0)
        {
            npgsql.EnableRetryOnFailure(dbOptions.MaxRetryCount);
        }
    });
});

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<Dev4AllDbContext>()
.AddDefaultTokenProviders();

// Layer services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices();

// Authorization policies aligned with FRD/NFR role model (Customer, Developer, Admin).
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCustomer", policy => policy.RequireRole("Customer"));
    options.AddPolicy("RequireDeveloper", policy => policy.RequireRole("Developer"));
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

// CORS
builder.Services.AddCors(opt =>
{
    var origins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

    opt.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { statusCode = 429, message = "Çok fazla istek gönderildi. 1 dakika bekleyip tekrar deneyin." }, ct);
    };
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token. Example: Bearer eyJhbGciOi..."
    });

    options.AddSecurityRequirement(_ =>
    {
        var schemeRef = new OpenApiSecuritySchemeReference("Bearer");
        return new OpenApiSecurityRequirement { [schemeRef] = [] };
    });
});

var app = builder.Build();

// Ensure new schema changes (e.g., EmailQueue) are applied before handling requests.
// This avoids runtime "relation does not exist" errors after deployments.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Dev4AllDbContext>();
    await dbContext.Database.MigrateAsync();
}

var seedRolesOnStartup = builder.Configuration.GetValue<bool>("Seed:SeedRolesOnStartup");
if (seedRolesOnStartup)
{
    await IdentityRoleSeeder.SeedAsync(app.Services);
}

// Middleware pipeline (order is mandatory)
app.UseMiddleware<GlobalExceptionMiddleware>();   // 1. Global error handling

app.Use(async (context, next) =>                 // 2. Security headers
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();                                // 3. HSTS (production only)
}

app.UseHttpsRedirection();                        // 4. HTTPS

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                             // 5. OpenAPI — development only
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseCors("AllowFrontend");                     // 6. CORS
app.UseRateLimiter();                             // 7. Rate limiting

app.UseAuthentication();                          // 8. JWT validation
app.UseAuthorization();                           // 9. Role/Policy
app.MapControllers();                             // 10. Controller routing

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();

/// <summary>
/// Partial marker so integration tests (WebApplicationFactory&lt;Program&gt;) can reference the host entry point.
/// </summary>
public partial class Program;
