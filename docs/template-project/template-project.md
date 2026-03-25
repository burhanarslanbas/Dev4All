# .NET Onion Architecture — Proje Template Belgesi

> Bu belge, yapay zeka asistanlarını ve yeni geliştiricileri doğru yönlendirmek için hazırlanmıştır.
> Referans alınan gerçek proje: `ApidevaEC` (B2B e-ticaret platformu).
> **Tüm yeni .NET projelerinde bu belgede tanımlanan desenler ve kurallar birebir uygulanır.**

---

## 1. Teknoloji Yığını

| Alan | Teknoloji |
|------|-----------|
| **Framework** | .NET 10, ASP.NET Core Web API |
| **Dil** | C# 12+ (file-scoped namespaces, primary constructors, record types) |
| **Mimari** | Onion Architecture (Hexagonal) |
| **Desen** | CQRS + MediatR, Repository Pattern, Unit of Work |
| **ORM** | Entity Framework Core + Npgsql (PostgreSQL) |
| **Kimlik** | ASP.NET Core Identity + JWT Bearer |
| **Validation** | FluentValidation (pipeline behavior ile otomatik) |
| **Loglama** | Microsoft.Extensions.Logging (`ILogger<T>`) |
| **Frontend** | React + Vite + TypeScript + Tailwind CSS |
| **Veritabanı** | PostgreSQL 15+ |
| **Hosting** | Azure App Service |

---

## 2. Solution Klasör Yapısı

```
{ProjectName}.sln
├── Core/
│   ├── {ProjectName}.Domain/          ← Entity, Enum, Exception — SIFIR dış bağımlılık
│   └── {ProjectName}.Application/    ← CQRS, DTO, Validator, Interface'ler
├── Infrastructure/
│   ├── {ProjectName}.Infrastructure/ ← Email, JWT, Identity implementasyonları
│   └── {ProjectName}.Persistence/    ← DbContext, Repository, UnitOfWork, Migration
└── Presentation/
    └── {ProjectName}.API/            ← Controller, Middleware, Program.cs
```

### Katman Bağımlılık Kuralı (KESİNLİKLE İHLAL EDİLEMEZ)

```
API → Application → Domain
Infrastructure → Application → Domain
Persistence → Application → Domain

❌ Domain → Hiçbir şeye bağımlı OLAMAZ
❌ API → Infrastructure/Persistence'a direkt referans VERILEMEZ
❌ Application → Infrastructure/Persistence'a direkt referans VERILEMEZ
```

---

## 3. Domain Katmanı — `{ProjectName}.Domain`

**Kural:** Hiçbir NuGet paketi, DataAnnotation veya dış referans içermez.

### 3.1. Klasör Yapısı

```
{ProjectName}.Domain/
├── Common/
│   └── BaseEntity.cs
├── Entities/
│   └── {EntityName}.cs
├── Enums/
│   └── {EnumName}.cs
└── Exceptions/
    ├── Base/
    │   └── DomainException.cs
    └── {Context}/
        └── {SpecificException}.cs
```

### 3.2. BaseEntity — Her Entity Bu Sınıftan Türer

```csharp
namespace {ProjectName}.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; protected set; }
    public DateTime? DeletedDate { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public void MarkAsUpdated() => UpdatedDate = DateTime.UtcNow;
    public void MarkAsDeleted() => DeletedDate = DateTime.UtcNow;
}
```

**Kurallar:**
- Tüm entity'ler `BaseEntity`'den türer.
- `Id` → `Guid`, constructor'da `Guid.NewGuid()` ile set edilir.
- Soft delete: `DeletedDate != null` → silinmiş. `MarkAsDeleted()` çağrılır, fiziksel silme yapılmaz.
- `UpdatedDate` → `MarkAsUpdated()` ile set edilir; EF Core interceptor veya manuel çağrılabilir.

### 3.3. Entity Yapısı

```csharp
using {ProjectName}.Domain.Common;
using {ProjectName}.Domain.Enums;

namespace {ProjectName}.Domain.Entities;

public class {EntityName} : BaseEntity
{
    // Zorunlu alanlar — constructor ile set edilir
    public string Title { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    
    // FK'lar
    public Guid {RelatedEntityId} { get; set; }
    
    // Nullable FK'lar (ilişki kurulmadan null olabilir)
    public Guid? {OptionalRelatedEntityId} { get; set; }
    
    // Enum alanlar
    public {StatusEnum} Status { get; set; } = {StatusEnum}.Default;
    
    // Navigation properties
    public {RelatedEntity} {RelatedEntity} { get; set; } = null!;
    public {OptionalRelated}? {OptionalRelated} { get; set; }
    public ICollection<{ChildEntity}> {ChildEntities} { get; set; } = [];
}
```

### 3.4. Exception Hiyerarşisi

```csharp
// Tüm domain exception'larının tabanı
namespace {ProjectName}.Domain.Exceptions.Base;
public class DomainException(string message) : Exception(message);

// Context'e özel exception'lar
namespace {ProjectName}.Domain.Exceptions.{Context};
public class ResourceNotFoundException(string resource, Guid id)
    : DomainException($"{resource} with id '{id}' was not found.");

public class BusinessRuleViolationException(string message) : DomainException(message);
```

**Exception isimlendirme standardı:**
- `ResourceNotFoundException` → 404 Not Found
- `BusinessRuleViolationException` → 400 Bad Request
- `InvalidCredentialsException` → 401 Unauthorized
- `{EntityName}{SorunAdı}Exception` → context'e özel

---

## 4. Application Katmanı — `{ProjectName}.Application`

### 4.1. Klasör Yapısı

```
{ProjectName}.Application/
├── Abstractions/
│   ├── Auth/
│   │   ├── ICurrentUser.cs
│   │   └── IUserService.cs (varsa)
│   ├── Persistence/
│   │   ├── IUnitOfWork.cs
│   │   └── Repositories/
│   │       └── Base/
│   │           ├── IReadRepository.cs
│   │           └── IWriteRepository.cs
│   ├── Repositories/
│   │   └── I{EntityName}Repository.cs  ← Her entity için özel
│   └── Services/
│       ├── IEmailService.cs
│       └── I{ServiceName}Service.cs
├── Behaviors/
│   └── ValidationBehavior.cs
├── Common/
│   ├── Models/
│   │   ├── ApiResponse.cs
│   │   └── ErrorResponse.cs
│   └── Pagination/
│       ├── PagedRequest.cs
│       └── PagedResult.cs
├── Features/
│   └── {BoundedContext}/
│       ├── Commands/
│       │   └── {UseCaseName}/
│       │       ├── {UseCaseName}Command.cs
│       │       ├── {UseCaseName}CommandHandler.cs
│       │       ├── {UseCaseName}CommandValidator.cs
│       │       └── {UseCaseName}Response.cs
│       └── Queries/
│           └── {UseCaseName}/
│               ├── {UseCaseName}Query.cs
│               ├── {UseCaseName}QueryHandler.cs
│               └── {UseCaseName}Response.cs
└── ApplicationServiceRegistration.cs
```

### 4.2. Repository Interface'leri — Application Katmanında Tanımlanır

#### Base Read Repository
```csharp
using {ProjectName}.Application.Common.Pagination;
using {ProjectName}.Domain.Common;

namespace {ProjectName}.Application.Abstractions.Persistence.Repositories.Base;

public interface IReadRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
```

#### Base Write Repository
```csharp
using {ProjectName}.Domain.Common;

namespace {ProjectName}.Application.Abstractions.Persistence.Repositories.Base;

public interface IWriteRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity, CancellationToken cancellationToken = default);
    void UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Remove(TEntity entity, CancellationToken cancellationToken = default);
    void RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
```

#### Entity-Specific Repository (Her entity için ayrı interface)
```csharp
using {ProjectName}.Application.Common.Pagination;
using {ProjectName}.Domain.Entities;

namespace {ProjectName}.Application.Abstractions.Repositories;

/// <summary>{EntityName} repository interface</summary>
public interface I{EntityName}Repository
{
    Task<{EntityName}?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<{EntityName}?> GetByIdWith{Related}Async(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<{EntityName}>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync({EntityName} entity, CancellationToken cancellationToken = default);
    void Update({EntityName} entity);
    void Delete({EntityName} entity);  // Soft delete — MarkAsDeleted() çağırır
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**ÖNEMLİ:** Generic `IRepository<T>` kullanılmaz. Her entity için **özelleştirilmiş** interface yazılır.

#### UnitOfWork Interface
```csharp
namespace {ProjectName}.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4.3. Pagination

```csharp
// PagedResult — sealed record, immutable
namespace {ProjectName}.Application.Common.Pagination;
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
```

### 4.4. CQRS Kuralları

#### Command (sealed record)
```csharp
using MediatR;

namespace {ProjectName}.Application.Features.{Context}.Commands.{UseCaseName};

public sealed record {UseCaseName}Command(
    string Field1,
    decimal Field2,
    Guid RelatedId
) : IRequest<{UseCaseName}Response>;
```

#### Command Handler (sealed class, primary constructor)
```csharp
using MediatR;

namespace {ProjectName}.Application.Features.{Context}.Commands.{UseCaseName};

public sealed class {UseCaseName}CommandHandler(
    I{EntityName}Repository repository,
    IUnitOfWork unitOfWork
) : IRequestHandler<{UseCaseName}Command, {UseCaseName}Response>
{
    public async Task<{UseCaseName}Response> Handle(
        {UseCaseName}Command request, CancellationToken cancellationToken)
    {
        // ValidationBehavior zaten çalıştı — try-catch GEREK YOK
        // GlobalExceptionMiddleware hataları yakalar

        var entity = new {EntityName}
        {
            Field1 = request.Field1,
            // ...
        };

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new {UseCaseName}Response(entity.Id, "İşlem başarılı.");
    }
}
```

#### Validator
```csharp
using FluentValidation;

namespace {ProjectName}.Application.Features.{Context}.Commands.{UseCaseName};

public sealed class {UseCaseName}CommandValidator : AbstractValidator<{UseCaseName}Command>
{
    public {UseCaseName}CommandValidator()
    {
        RuleFor(x => x.Field1)
            .NotEmpty().WithMessage("Alan boş olamaz.")
            .MaximumLength(100).WithMessage("En fazla 100 karakter.");

        RuleFor(x => x.Field2)
            .GreaterThan(0).WithMessage("Sıfırdan büyük olmalıdır.");
    }
}
```

#### Response (sealed record)
```csharp
namespace {ProjectName}.Application.Features.{Context}.Commands.{UseCaseName};

public sealed record {UseCaseName}Response(Guid Id, string Message);
```

### 4.5. ValidationBehavior — Paralel Çalıştırma

```csharp
using FluentValidation;
using MediatR;

namespace {ProjectName}.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);

        // Tüm validator'lar PARALEL çalıştırılır
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

### 4.6. ApplicationServiceRegistration

```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using {ProjectName}.Application.Behaviors;
using System.Reflection;

namespace {ProjectName}.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
```

---

## 5. Infrastructure Katmanı — `{ProjectName}.Infrastructure`

```
{ProjectName}.Infrastructure/
├── Auth/
│   ├── JwtService.cs           ← IJwtService implementasyonu
│   └── CurrentUser.cs          ← ICurrentUser implementasyonu (HttpContext'ten)
├── Email/
│   └── EmailService.cs         ← IEmailService implementasyonu (MailKit)
├── Common/
│   └── Seeders/
│       └── RolePermissionSeeder.cs
└── DependencyInjection.cs
```

#### InfrastructureServiceRegistration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace {ProjectName}.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Identity + JWT
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
```

---

## 6. Persistence Katmanı — `{ProjectName}.Persistence`

```
{ProjectName}.Persistence/
├── Context/
│   └── {ProjectName}DbContext.cs
├── Configurations/
│   └── {EntityName}Configuration.cs   ← Her entity için ayrı IEntityTypeConfiguration<T>
├── Identity/
│   ├── ApplicationUser.cs
│   └── ApplicationRole.cs
├── Migrations/                          ← dotnet ef migrations add ile oluşturulur
├── Repositories/
│   └── {EntityName}Repository.cs       ← I{EntityName}Repository implementasyonu
├── UnitOfWork.cs
└── PersistenceServiceRegistration.cs
```

#### Repository Implementasyonu

```csharp
using {ProjectName}.Application.Abstractions.Repositories;
using {ProjectName}.Application.Common.Pagination;
using {ProjectName}.Domain.Entities;
using {ProjectName}.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace {ProjectName}.Persistence.Repositories;

public class {EntityName}Repository(ApplicationDbContext context) : I{EntityName}Repository
{
    public async Task<{EntityName}?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.{EntityName}s
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedDate == null, cancellationToken);

    public async Task<PagedResult<{EntityName}>> GetAllAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.{EntityName}s
            .Where(x => x.DeletedDate == null)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<{EntityName}>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync({EntityName} entity, CancellationToken cancellationToken = default)
        => await context.{EntityName}s.AddAsync(entity, cancellationToken);

    public void Update({EntityName} entity) => context.{EntityName}s.Update(entity);

    public void Delete({EntityName} entity)
    {
        entity.MarkAsDeleted(); // Soft delete
        context.{EntityName}s.Update(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);
}
```

#### UnitOfWork Implementasyonu

```csharp
using {ProjectName}.Application.Abstractions.Persistence;
using {ProjectName}.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace {ProjectName}.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No transaction to commit.");
        try
        {
            await _context.SaveChangesAsync(ct);
            await _transaction.CommitAsync(ct);
        }
        catch { await RollbackTransactionAsync(ct); throw; }
        finally { await _transaction.DisposeAsync(); _transaction = null; }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;
        try { await _transaction.RollbackAsync(ct); }
        finally { await _transaction.DisposeAsync(); _transaction = null; }
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
```

#### PersistenceServiceRegistration

```csharp
using Microsoft.Extensions.DependencyInjection;
using {ProjectName}.Application.Abstractions.Persistence;
using {ProjectName}.Application.Abstractions.Repositories;
using {ProjectName}.Persistence.Repositories;

namespace {ProjectName}.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<I{EntityName}Repository, {EntityName}Repository>();
        // Diğer repository'ler buraya eklenir
        return services;
    }
}
```

---

## 7. WebAPI Katmanı — `{ProjectName}.API`

```
{ProjectName}.API/
├── Configuration/
│   ├── ApiServiceConfiguration.cs      ← AddApiServices() extension
│   ├── MiddlewareConfiguration.cs      ← ConfigureMiddlewarePipeline() extension
│   └── Constants/
│       └── CorsOrigins.cs
├── Controllers/
│   └── v1/
│       └── {FeatureName}Controller.cs
├── Extensions/
│   └── SwaggerRegistration.cs
├── Middlewares/
│   └── GlobalExceptionMiddleware.cs
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

### 7.1. Program.cs — Temiz ve Kısa

```csharp
using {ProjectName}.API.Configuration;
using {ProjectName}.Application;
using {ProjectName}.Infrastructure;
using {ProjectName}.Persistence;
using {ProjectName}.Persistence.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices();

var app = builder.Build();

app.ConfigureMiddlewarePipeline();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
```

### 7.2. Middleware Pipeline Sırası (KESİNLİKLE BU SIRADA)

```csharp
// MiddlewareConfiguration.cs
public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
{
    app.UseMiddleware<GlobalExceptionMiddleware>(); // 1. Global hata yakalama
    app.UseHttpsRedirection();                      // 2. HTTPS yönlendirme
    app.UseSwaggerWithUI();                         // 3. Swagger
    app.UseCors("AllowFrontend");                   // 4. CORS
    app.UseAuthentication();                        // 5. JWT doğrulama
    app.UseAuthorization();                         // 6. Rol/Policy yetkilendirme
    app.MapControllers();                           // 7. Controller routing
    return app;
}
```

### 7.3. GlobalExceptionMiddleware — Detaylı Yapı

```csharp
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex) { LogException(ex); await HandleExceptionAsync(context, ex); }
    }

    private void LogException(Exception exception)
    {
        switch (exception)
        {
            case ValidationException:
            case BusinessRuleViolationException:
                logger.LogWarning(exception, "Business/Validation error: {Message}", exception.Message);
                break;
            case ResourceNotFoundException:
                logger.LogInformation(exception, "Not found: {Message}", exception.Message);
                break;
            default:
                logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorDetails) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, (object)new
            {
                message = "Validation failed",
                errors = ve.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            }),
            ResourceNotFoundException => (HttpStatusCode.NotFound, new { message = exception.Message }),
            BusinessRuleViolationException => (HttpStatusCode.BadRequest, new { message = exception.Message }),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, new { message = "Unauthorized" }),
            _ => (HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." })
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.ToString(),
            // errorDetails alanları buraya merge edilir
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
```

**HTTP Response Yapısı:**
```json
{
  "statusCode": 400,
  "timestamp": "2026-03-25T20:00:00Z",
  "path": "/api/v1/bids",
  "message": "Validation failed",
  "errors": [
    { "field": "BidAmount", "message": "Sıfırdan büyük olmalıdır." }
  ]
}
```

### 7.4. Controller Yapısı

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using {ProjectName}.Application.Features.{Context}.Commands.{UseCaseName};
using {ProjectName}.Application.Features.{Context}.Queries.{UseCaseName};

namespace {ProjectName}.API.Controllers.v1;

/// <summary>{Context} feature controller</summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class {FeatureName}Controller(IMediator mediator) : ControllerBase
{
    /// <summary>Yeni {entity} oluşturur</summary>
    [Authorize(Roles = "{RoleName}")]
    [HttpPost]
    [ProducesResponseType(typeof({UseCaseName}Response), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] {UseCaseName}Command command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        return Created($"/api/v1/{controller}/{response.Id}", response);
    }

    /// <summary>{entity} listesini getirir</summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof({UseCaseName}Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] {UseCaseName}Query query, CancellationToken ct)
    {
        var response = await mediator.Send(query, ct);
        return Ok(response);
    }
}
```

**Controller kuralları:**
- `IMediator mediator` primary constructor ile inject edilir.
- Her action `CancellationToken ct` parametresi alır.
- Handler'da hiç try-catch yoktur.
- Her action `[ProducesResponseType]` attribute'larına sahiptir.
- Route: `api/v1/[controller]` (versiyonlama zorunlu).

---

## 8. Veritabanı Konfigürasyonu

### Entity Type Configuration (Her entity için ayrı dosya)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using {ProjectName}.Domain.Entities;

namespace {ProjectName}.Persistence.Configurations;

public class {EntityName}Configuration : IEntityTypeConfiguration<{EntityName}>
{
    public void Configure(EntityTypeBuilder<{EntityName}> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Budget)
            .HasPrecision(18, 2);

        // Soft delete global filter
        builder.HasQueryFilter(x => x.DeletedDate == null);

        // İlişkiler
        builder.HasMany(x => x.ChildEntities)
            .WithOne(x => x.ParentEntity)
            .HasForeignKey(x => x.ParentEntityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

**Kurallar:**
- Her entity için `Configurations/` klasöründe ayrı dosya.
- `HasQueryFilter(x => x.DeletedDate == null)` → soft delete otomatik filtre.
- `HasPrecision(18, 2)` → tüm `decimal` alanlarda zorunlu.
- `OnDelete(DeleteBehavior.Restrict)` → cascade delete yasak.

---

## 9. Coding Standards

### Naming Convention

| Öğe | Kural | Örnek |
|-----|-------|-------|
| Class | PascalCase | `ProductRepository` |
| Interface | IPascalCase | `IProductRepository` |
| Method | PascalCase | `GetByIdAsync` |
| Private field | `_camelCase` | `_context` |
| Local variable | camelCase | `cancellationToken` |
| Record/DTO | sealed record | `sealed record LoginResponse(...)` |
| Command/Query | sealed record | `sealed record {Name}Command(...) : IRequest<{Response}>` |
| Handler/Validator | sealed class | `sealed class {Name}Handler(...)` |

### C# Dil Kuralları (C# 12+)

```csharp
// ✅ File-scoped namespaces
namespace {ProjectName}.Application.Features.Auth;

// ✅ Primary constructors
public sealed class MyService(IRepository repo, ILogger<MyService> logger) { }

// ✅ Collection initializer
public ICollection<Item> Items { get; set; } = [];

// ✅ Record types for Commands/Responses
public sealed record MyCommand(string Field1, Guid Id) : IRequest<MyResponse>;

// ✅ Target-typed new
var entity = new ProductRequest { Title = request.Title };

// ✅ Pattern matching
var result = exception switch
{
    ValidationException => (400, "Validation error"),
    NotFoundException => (404, "Not found"),
    _ => (500, "Internal error")
};
```

### Handler'da Ne Yapılmaz

```csharp
// ❌ YANLIŞ — Handler'da try-catch KULLANILMAZ
public async Task<Response> Handle(Command request, CancellationToken ct)
{
    try { ... }
    catch (Exception ex) { return new Response(false, ex.Message); } // ← YASAK
}

// ✅ DOĞRU — Exception fırlat, Middleware yakalar
public async Task<Response> Handle(Command request, CancellationToken ct)
{
    var entity = await _repo.GetByIdAsync(request.Id, ct)
        ?? throw new ResourceNotFoundException(nameof(Entity), request.Id);
    // ...
    return new Response(entity.Id);
}
```

---

## 10. appsettings.json Yapısı

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database={db_name};Username=postgres;Password={password}"
  },
  "Jwt": {
    "Issuer": "{ProjectName}",
    "Audience": "{ProjectName}",
    "Key": "CHANGE_ME_IN_PRODUCTION_MIN_32_CHARS",
    "ExpiryMinutes": 60
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "noreply@{domain}.com",
    "UseSsl": true
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  },
  "IdentitySettings": {
    "SeedRoles": true
  }
}
```

**Güvenlik kuralları:**
- `SenderPassword` → **asla** appsettings'e yazılmaz. Environment variable veya Azure Key Vault.
- `Jwt.Key` → Production'da environment variable.
- `appsettings.Development.json` → `.gitignore`'a eklenir.

---

## 11. Sık Sorulan Mimari Sorular

### `IRepository<T>` Domain'de mi, Application'da mı?
**Application'da.** Domain yalnızca entity, enum ve domain exception içerir. Repository ihtiyacı use-case seviyesindedir.

### Generic repository mi, entity-specific mi?
**Entity-specific.** Her entity için `I{EntityName}Repository` ayrı interface olarak tanımlanır. Generic base (`IReadRepository<T>`, `IWriteRepository<T>`) yalnızca temel imzaları taşır; implementasyonlar entity'nin ihtiyacına göre özelleşir.

### Handler'da UnitOfWork mu, Repository.SaveChanges mı?
- **Basit işlemler** → `await _repo.SaveChangesAsync(ct)`
- **Transaction gerektiren işlemler** → `await _uow.BeginTransactionAsync()` → işlemler → `await _uow.CommitTransactionAsync()`

### Validation nerede?
FluentValidation Validators → `{UseCaseName}CommandValidator : AbstractValidator<{UseCaseName}Command>`. `ValidationBehavior` MediatR pipeline'a eklendiği için validator'lar **otomatik** çalışır. Handler'da manual validation yapılmaz.

### Soft delete nasıl çalışır?
`entity.MarkAsDeleted()` → `DeletedDate = DateTime.UtcNow`. EF Core `HasQueryFilter(x => x.DeletedDate == null)` ile silinmiş kayıtlar otomatik filtre edilir.
