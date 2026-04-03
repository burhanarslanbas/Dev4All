using Dev4All.Application;
using Dev4All.Application.Options;
using Dev4All.Infrastructure;
using Dev4All.Persistence;
using Dev4All.Persistence.Context;
using Dev4All.Persistence.Identity;
using Dev4All.WebAPI.Seed;
using Dev4All.WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

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

builder.Services.AddOptions<FrontendOptions>()
    .Bind(builder.Configuration.GetSection(FrontendOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// DbContext — reads connection string from DatabaseOptions
var dbOptions = builder.Configuration
    .GetSection(DatabaseOptions.SectionName)
    .Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database configuration section is missing.");

builder.Services.AddDbContext<Dev4AllDbContext>(options =>
{
    options.UseNpgsql(dbOptions.ConnectionString, npgsql =>
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
})
.AddEntityFrameworkStores<Dev4AllDbContext>()
.AddDefaultTokenProviders();

// Layer services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices();

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

var seedRolesOnStartup = builder.Configuration.GetValue<bool>("Seed:SeedRolesOnStartup");
if (seedRolesOnStartup)
{
    await IdentityRoleSeeder.SeedAsync(app.Services);
}

// Middleware pipeline (order is mandatory)
app.UseMiddleware<GlobalExceptionMiddleware>();   // 1. Global error handling

app.UseHttpsRedirection();                        // 2. HTTPS

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                             // 3. OpenAPI — development only
    app.UseSwagger();                             // 3.1 Swagger JSON
    app.UseSwaggerUI();                           // 3.2 Swagger UI
}

app.UseCors("AllowFrontend");                     // 4. CORS

app.UseAuthentication();                          // 5. JWT validation
app.UseAuthorization();                           // 6. Role/Policy
app.MapControllers();                             // 7. Controller routing

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
