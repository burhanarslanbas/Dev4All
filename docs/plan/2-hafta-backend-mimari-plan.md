# 2. Hafta — Sistem Mimarisi ve Proje Altyapısının Oluşturulması

> **Süre:** 23 Mart – 29 Mart  
> **Hedef:** `Dev4All` backend'inin Onion Architecture temelli solution yapısını eksiksiz kurmak; tüm entity'leri, base class'ları, exception hiyerarşisini ve CQRS altyapısını oluşturmak; JWT + Identity entegrasyonunu yapılandırmak; Global Exception Middleware'i eklemek; build'in sıfır hatayla geçtiğini doğrulamak.

---

## Referans Dokümanlar

| Doküman | Kullanım Amacı |
|---------|----------------|
| [docs/analyse/04-sadm.md](file:///c:/Users/burha/Desktop/Dev4All/docs/analyse/04-sadm.md) | Katman yapısı, entity modeli, CQRS operation listesi |
| [docs/analyse/02-frd.md](file:///c:/Users/burha/Desktop/Dev4All/docs/analyse/02-frd.md) | İş kuralları, validation rules, yetki matrisi |
| [docs/template-project/template-project.md](file:///c:/Users/burha/Desktop/Dev4All/docs/template-project/template-project.md) | Tüm kod şablonları, naming conventions |

---

## Adım 1 — Solution ve Proje Yapısının Oluşturulması

### 1.1. Klasör Yapısı Oluşturma

```
Dev4All/   (monorepo kökü)
├── backend/
│   ├── Dev4All.slnx
│   ├── Directory.Build.props
│   ├── src/
│   │   ├── Core/
│   │   │   ├── Dev4All.Domain/
│   │   │   └── Dev4All.Application/
│   │   ├── Infrastructure/
│   │   │   ├── Dev4All.Infrastructure/
│   │   │   └── Dev4All.Persistence/
│   │   └── Presentation/
│   │       └── Dev4All.WebAPI/
│   └── tests/
│       ├── Dev4All.UnitTests/
│       └── Dev4All.IntegrationTests/
├── frontend/          (ileride web istemcisi)
├── mobile/            (ileride mobil istemci)
└── docs/
```

### 1.2. Solution ve Projeleri Oluşturma

> **Monorepo:** Komutları **depo kökünden** çalıştırın. Çözüm dosyası: `backend/Dev4All.slnx` (.NET 10 XML formatı).

```powershell
# Solution (backend klasöründe; sıfırdan kurulumda .sln veya .slnx tercihinize göre)
dotnet new sln -n Dev4All -o backend

# Core katmanı
dotnet new classlib -n Dev4All.Domain -o backend/src/Core/Dev4All.Domain
dotnet new classlib -n Dev4All.Application -o backend/src/Core/Dev4All.Application

# Infrastructure katmanı
dotnet new classlib -n Dev4All.Infrastructure -o backend/src/Infrastructure/Dev4All.Infrastructure
dotnet new classlib -n Dev4All.Persistence -o backend/src/Infrastructure/Dev4All.Persistence

# Presentation katmanı
dotnet new webapi -n Dev4All.WebAPI -o backend/src/Presentation/Dev4All.WebAPI --use-controllers

# Test projeleri
dotnet new xunit -n Dev4All.UnitTests -o backend/tests/Dev4All.UnitTests
dotnet new xunit -n Dev4All.IntegrationTests -o backend/tests/Dev4All.IntegrationTests

# Solution'a ekle
dotnet sln backend/Dev4All.slnx add backend/src/Core/Dev4All.Domain/Dev4All.Domain.csproj
dotnet sln backend/Dev4All.slnx add backend/src/Core/Dev4All.Application/Dev4All.Application.csproj
dotnet sln backend/Dev4All.slnx add backend/src/Infrastructure/Dev4All.Infrastructure/Dev4All.Infrastructure.csproj
dotnet sln backend/Dev4All.slnx add backend/src/Infrastructure/Dev4All.Persistence/Dev4All.Persistence.csproj
dotnet sln backend/Dev4All.slnx add backend/src/Presentation/Dev4All.WebAPI/Dev4All.WebAPI.csproj
dotnet sln backend/Dev4All.slnx add backend/tests/Dev4All.UnitTests/Dev4All.UnitTests.csproj
dotnet sln backend/Dev4All.slnx add backend/tests/Dev4All.IntegrationTests/Dev4All.IntegrationTests.csproj
```

### 1.3. Proje Referanslarını Bağlama

```powershell
# Application → Domain
dotnet add backend/src/Core/Dev4All.Application reference backend/src/Core/Dev4All.Domain

# Infrastructure → Application (Domain'e transitif erişim)
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure reference backend/src/Core/Dev4All.Application

# Persistence → Application
dotnet add backend/src/Infrastructure/Dev4All.Persistence reference backend/src/Core/Dev4All.Application

# WebAPI → Application + Infrastructure + Persistence
dotnet add backend/src/Presentation/Dev4All.WebAPI reference backend/src/Core/Dev4All.Application
dotnet add backend/src/Presentation/Dev4All.WebAPI reference backend/src/Infrastructure/Dev4All.Infrastructure
dotnet add backend/src/Presentation/Dev4All.WebAPI reference backend/src/Infrastructure/Dev4All.Persistence

# UnitTests → Application + Domain
dotnet add backend/tests/Dev4All.UnitTests reference backend/src/Core/Dev4All.Application
dotnet add backend/tests/Dev4All.UnitTests reference backend/src/Core/Dev4All.Domain
```

> ℹ️ **Not:** WebAPI, Composition Root rolünü üstlenerek Application, Infrastructure ve Persistence'a proje referansı içerir. Bağımlılık yönü her zaman dışarıdan içeriye doğrudur; Domain ve Application hiçbir zaman üst katmanlara referans vermez.

---

## Adım 2 — NuGet Paket Kurulumları

### 2.1. Dev4All.Application

```powershell
dotnet add backend/src/Core/Dev4All.Application package MediatR
dotnet add backend/src/Core/Dev4All.Application package FluentValidation
dotnet add backend/src/Core/Dev4All.Application package FluentValidation.DependencyInjectionExtensions
dotnet add backend/src/Core/Dev4All.Application package Microsoft.Extensions.DependencyInjection.Abstractions
```

### 2.2. Dev4All.Infrastructure

```powershell
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure package System.IdentityModel.Tokens.Jwt
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure package MailKit
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure package Microsoft.Extensions.Configuration.Abstractions
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure package Quartz
dotnet add backend/src/Infrastructure/Dev4All.Infrastructure package Quartz.Extensions.Hosting
```

### 2.3. Dev4All.Persistence

```powershell
dotnet add backend/src/Infrastructure/Dev4All.Persistence package Microsoft.EntityFrameworkCore
dotnet add backend/src/Infrastructure/Dev4All.Persistence package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add backend/src/Infrastructure/Dev4All.Persistence package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add backend/src/Infrastructure/Dev4All.Persistence package Microsoft.EntityFrameworkCore.Design
```

### 2.4. Dev4All.WebAPI

```powershell
dotnet add backend/src/Presentation/Dev4All.WebAPI package Swashbuckle.AspNetCore
dotnet add backend/src/Presentation/Dev4All.WebAPI package Microsoft.AspNetCore.Authentication.JwtBearer
```

---

## Adım 3 — Domain Katmanı

Dosya konumu: `backend/src/Core/Dev4All.Domain/`

### 3.1. BaseEntity

**Dosya:** `backend/src/Core/Dev4All.Domain/Common/BaseEntity.cs`

```csharp
namespace Dev4All.Domain.Common;

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

### 3.2. Enum'lar

**Dosya:** `backend/src/Core/Dev4All.Domain/Enums/ProjectStatus.cs`

```csharp
namespace Dev4All.Domain.Enums;

public enum ProjectStatus
{
    Open = 0,
    AwaitingContract = 1,   // Teklif kabul edildi — sözleşme onayı bekleniyor
    Ongoing = 2,
    Completed = 3,
    Expired = 4,
    Cancelled = 5           // Sözleşme sürecinde bir taraf iptal etti
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Enums/ContractStatus.cs`

```csharp
namespace Dev4All.Domain.Enums;

public enum ContractStatus
{
    Draft = 0,          // Sistem taslağı oluşturdu
    UnderReview = 1,    // En az bir taraf revize etti
    BothApproved = 2,   // İki taraf da onayladı → Ongoing'e geçiş
    Cancelled = 3       // Bir taraf iptal etti → proje Cancelled
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Enums/BidStatus.cs`

```csharp
namespace Dev4All.Domain.Enums;

public enum BidStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Enums/UserRole.cs`

```csharp
namespace Dev4All.Domain.Enums;

public enum UserRole
{
    Customer,
    Developer,
    Admin
}
```

### 3.3. Entity'ler

**Dosya:** `backend/src/Core/Dev4All.Domain/Entities/ProjectRequest.cs`

```csharp
using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;

namespace Dev4All.Domain.Entities;

public class Project : BaseEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public string? AssignedDeveloperId { get; set; }
    public string Title { get; set; } = string.Empty;             // 3–100 karakter
    public string Description { get; set; } = string.Empty;       // 10–2000 karakter
    public decimal Budget { get; set; }                           // > 0
    public DateTime Deadline { get; set; }                        // UTC, gelecek tarih
    public DateTime BidEndDate { get; set; }                      // UTC, Deadline'dan önce
    public string? Technologies { get; set; }                     // Opsiyonel etiket listesi
    public ProjectStatus Status { get; set; } = ProjectStatus.Open;

    // Navigation properties
    public ICollection<Bid> Bids { get; set; } = [];
    public GitHubLog? GitHubLog { get; set; }
    public Contract? Contract { get; set; }                       // 1-1: teklif kabulünde oluşur
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Entities/Bid.cs`

```csharp
using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;

namespace Dev4All.Domain.Entities;

public class Bid : BaseEntity
{
    public Guid ProjectId { get; set; }                            // FK → Project
    public string DeveloperId { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }                        // > 0
    public string ProposalNote { get; set; } = string.Empty;      // 10–1000 karakter
    public BidStatus Status { get; set; } = BidStatus.Pending;
    public bool IsAccepted { get; set; } = false;

    // Navigation properties
    public Project Project { get; set; } = null!;
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Entities/GitHubLog.cs`

```csharp
using Dev4All.Domain.Common;

namespace Dev4All.Domain.Entities;

public class GitHubLog : BaseEntity
{
    public Guid ProjectId { get; set; }                            // FK → Project
    public string RepoUrl { get; set; } = string.Empty;
    public string Branch { get; set; } = "main";
    public string CommitHash { get; set; } = string.Empty;        // 40 karakter SHA-1
    public string CommitMessage { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime PushedAt { get; set; }                        // UTC

    // Navigation properties
    public Project Project { get; set; } = null!;
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Entities/Contract.cs`

```csharp
using Dev4All.Domain.Common;
using Dev4All.Domain.Enums;

namespace Dev4All.Domain.Entities;

public class Contract : BaseEntity
{
    public Guid ProjectId { get; set; }                          // FK → Project (1-1)
    public string Content { get; set; } = string.Empty;
    public int RevisionNumber { get; set; } = 1;
    public string LastRevisedById { get; set; } = string.Empty;
    public ContractStatus Status { get; set; } = ContractStatus.Draft;
    public bool IsCustomerApproved { get; set; } = false;
    public bool IsDeveloperApproved { get; set; } = false;
    public DateTime? CustomerApprovedAt { get; set; }
    public DateTime? DeveloperApprovedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public ICollection<ContractRevision> Revisions { get; set; } = [];
}
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Entities/ContractRevision.cs`

```csharp
using Dev4All.Domain.Common;

namespace Dev4All.Domain.Entities;

public class ContractRevision : BaseEntity
{
    public Guid ContractId { get; set; }                         // FK → Contract
    public string RevisedById { get; set; } = string.Empty;      // Revize eden kullanıcı Id
    public string ContentSnapshot { get; set; } = string.Empty;  // O anki sözleşme metni
    public int RevisionNumber { get; set; }
    public string? RevisionNote { get; set; }                     // Opsiyonel açıklama

    // Navigation properties
    public Contract Contract { get; set; } = null!;
}
```

### 3.4. Exception Hiyerarşisi

**Dosya:** `backend/src/Core/Dev4All.Domain/Exceptions/Base/DomainException.cs`

```csharp
namespace Dev4All.Domain.Exceptions.Base;
public class DomainException(string message) : Exception(message);
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Exceptions/ResourceNotFoundException.cs`

```csharp
using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

public class ResourceNotFoundException(string resource, Guid id)
    : DomainException($"{resource} with id '{id}' was not found.");
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Exceptions/BusinessRuleViolationException.cs`

```csharp
using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

public class BusinessRuleViolationException(string message) : DomainException(message);
```

**Dosya:** `backend/src/Core/Dev4All.Domain/Exceptions/UnauthorizedDomainException.cs`

```csharp
using Dev4All.Domain.Exceptions.Base;

namespace Dev4All.Domain.Exceptions;

public class UnauthorizedDomainException(string message) : DomainException(message);
```

---

## Adım 4 — Application Katmanı — Soyutlamalar

Dosya konumu: `backend/src/Core/Dev4All.Application/`

### 4.1. Pagination

**Dosya:** `backend/src/Core/Dev4All.Application/Common/Pagination/PagedResult.cs`

```csharp
namespace Dev4All.Application.Common.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
```

### 4.2. Base Repository Interface'leri

> **Klasör Kuralı:** Tüm repository interface'leri (generic base dahil) `Abstractions/Repositories/` altında toplanır.  
> `Abstractions/Persistence/` klasörü **yalnızca** `IUnitOfWork.cs` içerir; buraya başka dosya eklenmez.

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/Base/IReadRepository.cs`

```csharp
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Common;

namespace Dev4All.Application.Abstractions.Repositories.Base;

public interface IReadRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/Base/IWriteRepository.cs`

```csharp
using Dev4All.Domain.Common;

namespace Dev4All.Application.Abstractions.Repositories.Base;

public interface IWriteRepository<TEntity> where TEntity : BaseEntity, new()
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
```

### 4.3. Entity-Specific Repository Interface'leri

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/IProjectRepository.cs`

```csharp
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>Project repository interface</summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Project?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Project>> GetOpenProjectsAsync(int page, int pageSize, CancellationToken ct = default);
    Task<List<Project>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default);
    Task AddAsync(Project entity, CancellationToken ct = default);
    void Update(Project entity);
    void Delete(Project entity);   // Soft delete
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/IBidRepository.cs`

```csharp
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>Bid repository interface</summary>
public interface IBidRepository
{
    Task<Bid?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Bid?> GetByDeveloperAndProjectAsync(string developerId, Guid projectId, CancellationToken ct = default);
    Task<List<Bid>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<List<Bid>> GetByDeveloperIdAsync(string developerId, CancellationToken ct = default);
    Task AddAsync(Bid entity, CancellationToken ct = default);
    void Update(Bid entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/IGitHubLogRepository.cs`

```csharp
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>GitHubLog repository interface</summary>
public interface IGitHubLogRepository
{
    Task<List<GitHubLog>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task AddAsync(GitHubLog entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<GitHubLog> entities, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/IContractRepository.cs`

```csharp
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>Contract repository interface</summary>
public interface IContractRepository
{
    Task<Contract?> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<Contract?> GetByIdWithRevisionsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Contract entity, CancellationToken ct = default);
    void Update(Contract entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Repositories/IContractRevisionRepository.cs`

```csharp
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Repositories;

/// <summary>ContractRevision repository interface</summary>
public interface IContractRevisionRepository
{
    Task<List<ContractRevision>> GetByContractIdAsync(Guid contractId, CancellationToken ct = default);
    Task AddAsync(ContractRevision entity, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

### 4.4. IUnitOfWork

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Persistence/IUnitOfWork.cs`

```csharp
namespace Dev4All.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 4.5. Servis Interface'leri

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Services/IEmailService.cs`

```csharp
namespace Dev4All.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Services/IGitHubService.cs`

```csharp
using Dev4All.Domain.Entities;

namespace Dev4All.Application.Abstractions.Services;

public interface IGitHubService
{
    /// <summary>GitHub Webhook HMAC-SHA256 imzasını doğrular</summary>
    bool ValidateWebhookSignature(string payload, string signature, string secret);

    /// <summary>Push event payload'ından GitHubLog listesi oluşturur</summary>
    List<GitHubLog> ParsePushEvent(string jsonPayload, Guid projectId);
}
```

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Auth/ICurrentUser.cs`

```csharp
namespace Dev4All.Application.Abstractions.Auth;

public interface ICurrentUser
{
    string UserId { get; }
    string Email { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}
```

### 4.6. ValidationBehavior

**Dosya:** `backend/src/Core/Dev4All.Application/Behaviors/ValidationBehavior.cs`

```csharp
using FluentValidation;
using MediatR;

namespace Dev4All.Application.Behaviors;

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

### 4.7. ApplicationServiceRegistration

**Dosya:** `backend/src/Core/Dev4All.Application/ApplicationServiceRegistration.cs`

```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Dev4All.Application.Behaviors;
using System.Reflection;

namespace Dev4All.Application;

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

## Adım 5 — Persistence Katmanı — Identity, DbContext, Repository implementasyonları

**Konum:** `backend/src/Infrastructure/Dev4All.Persistence/`

**Application ile hizalama:** Okuma/yazma arayüzleri `Dev4All.Application.Abstractions.Persistence.Repositories.*` altında tanımlıdır (Adım 4 güncel yapı). Persistence tarafında **aggregate başına iki somut sınıf:** `{Aggregate}ReadRepository` → `I{Aggregate}ReadRepository`, `{Aggregate}WriteRepository` → `I{Aggregate}WriteRepository`. **`SaveChangesAsync` yalnızca `IUnitOfWork`** üzerinden çağrılır (repository’lerde yok).

### 5.1. Klasör yapısı

```
Dev4All.Persistence/
├── Context/
│   └── Dev4AllDbContext.cs
├── Configurations/
│   ├── ProjectConfiguration.cs
│   ├── BidConfiguration.cs
│   ├── GitHubLogConfiguration.cs
│   ├── ContractConfiguration.cs
│   └── ContractRevisionConfiguration.cs
├── Identity/
│   └── ApplicationUser.cs
├── Repositories/
│   ├── Projects/ProjectReadRepository.cs, ProjectWriteRepository.cs
│   ├── Bids/BidReadRepository.cs, BidWriteRepository.cs
│   ├── GitHubLogs/GitHubLogReadRepository.cs, GitHubLogWriteRepository.cs
│   ├── Contracts/ContractReadRepository.cs, ContractWriteRepository.cs
│   └── ContractRevisions/ContractRevisionReadRepository.cs, ContractRevisionWriteRepository.cs
├── UnitOfWork.cs
└── PersistenceServiceRegistration.cs
```

### 5.2. ApplicationUser

**Dosya:** `Identity/ApplicationUser.cs` — `IdentityUser` türevi; `Name`, `CreatedDate`.

### 5.3. Dev4AllDbContext

**Dosya:** `Context/Dev4AllDbContext.cs`

- `IdentityDbContext<ApplicationUser>` tabanı.
- `DbSet`: `Project`, `Bid`, `GitHubLog`, `Contract`, `ContractRevision`.
- `OnModelCreating`: `ApplyConfigurationsFromAssembly(typeof(Dev4AllDbContext).Assembly)`.
- `SaveChangesAsync` override: `BaseEntity` için `Added` → `CreatedDate`, `Modified` → `MarkAsUpdated()`.

### 5.4. Entity konfigürasyonları (Fluent API)

- **Project:** `CustomerId` / `AssignedDeveloperId` uzunlukları, precision, soft-delete `HasQueryFilter`, `HasMany` Bids, **`HasMany` GitHubLogs** (1-N; domain’de `Project.GitHubLogs`), `HasOne` Contract (1-1).
- **Bid:** precision, `ProposalNote`, soft-delete filter.
- **GitHubLog:** string uzunlukları (commit hash, mesaj, vb.).
- **Contract:** içerik uzunluğu, Project ile 1-1, `Revisions` ile 1-N cascade.
- **ContractRevision:** snapshot ve not alanları.

### 5.5. Repository sınıfları

Read ve write ayrı dosyalarda: `Repositories/{Aggregate}/{Entity}ReadRepository.cs` ve `{Entity}WriteRepository.cs`.

- Application’daki **`IReadRepository<T>`** / **`IWriteRepository<T>`** taban metotlarının tamamı + aggregate’a özel read/write metotları.
- **`IProjectWriteRepository.SoftDelete`:** `MarkAsDeleted()` + `Update`.
- Read repository’lerde mümkün olduğunca **`AsNoTracking()`**; write repository’ler aynı scope’taki `DbContext` ile tracking kullanır.

### 5.6. UnitOfWork

**Dosya:** `UnitOfWork.cs` — `IUnitOfWork`: transaction başlatma/commit/rollback + `SaveChangesAsync`.

### 5.7. DI kaydı

**Dosya:** `PersistenceServiceRegistration.cs`

- `AddScoped<IProjectReadRepository, ProjectReadRepository>()` ve `AddScoped<IProjectWriteRepository, ProjectWriteRepository>()` (aynı kalıp diğer aggregate’ler için). Her ikisi de aynı scope’tan tek `Dev4AllDbContext` alır.
- **`AddScoped<IUnitOfWork, UnitOfWork>()`**

### 5.8. WebAPI

`Program.cs` içinde `AddDbContext<Dev4AllDbContext>(…)` ve `AddPersistenceServices()` çağrıları (connection string ile) bu adımın parçasıdır; tam örnek Adım 7’de toplanabilir.

---

## Adım 6 — Infrastructure Katmanı — JWT ve Auth

Dosya konumu: `backend/src/Infrastructure/Dev4All.Infrastructure/`

### 6.1. Application Katmanındaki Soyutlamalar

> **Mimari Kural:** Application katmanı yalnızca kendi tanımladığı interface'lere bağımlıdır. `UserManager<ApplicationUser>`, `IdentityUser`, `ApplicationUser` gibi Infrastructure/Persistence'a ait tipler **asla Application katmanına girmez**. Tüm imzalar primitive türler (string, bool, Guid) kullanır.

#### 6.1.a IJwtService

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Auth/IJwtService.cs`

```csharp
namespace Dev4All.Application.Abstractions.Auth;

public interface IJwtService
{
    /// <summary>Generates a signed JWT token for the given user identity.</summary>
    string GenerateToken(string userId, string email, string role);
}
```

#### 6.1.b IIdentityService

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Auth/IIdentityService.cs`

```csharp
namespace Dev4All.Application.Abstractions.Auth;

/// <summary>Abstracts ASP.NET Core Identity operations. Implemented in Infrastructure.</summary>
public interface IIdentityService
{
    Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string name, string email, string password, string role, CancellationToken ct = default);

    Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email, string password, CancellationToken ct = default);

    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default);

    Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default);
}
```

### 6.2. JwtService Implementasyonu

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/Auth/JwtService.cs`

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dev4All.Infrastructure.Auth;

public sealed class JwtService(IOptions<JwtOptions> options) : IJwtService
{
    private readonly JwtOptions _jwt = options.Value;

    public string GenerateToken(string userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 6.2.b IdentityService Implementasyonu

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/Auth/IdentityService.cs`

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Dev4All.Infrastructure.Auth;

/// <summary>Implements IIdentityService using ASP.NET Core Identity UserManager/RoleManager.</summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IIdentityService
{
    public async Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string name, string email, string password, string role, CancellationToken ct = default)
    {
        var user = new ApplicationUser { UserName = email, Email = email, Name = name };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, string.Empty, result.Errors.Select(e => e.Description));

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        await userManager.AddToRoleAsync(user, role);
        return (true, user.Id, []);
    }

    public async Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return (false, string.Empty, string.Empty, string.Empty);

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid) return (false, string.Empty, string.Empty, string.Empty);

        var roles = await userManager.GetRolesAsync(user);
        return (true, user.Id, user.Email!, roles.FirstOrDefault() ?? string.Empty);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user?.UserName;
    }
}
```

> **Not:** `IdentityService`, `Dev4All.Persistence` projesine proje referansı gerektirir (`ApplicationUser` orada tanımlı). `Dev4All.Infrastructure.csproj`'a `<ProjectReference>` eklenmelidir.

---

### 6.3. CurrentUser Implementasyonu

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/Auth/CurrentUser.cs`

```csharp
using Dev4All.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dev4All.Infrastructure.Auth;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    public string Email => User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    public string Role => User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
```

### 6.3.b EmailService Implementasyonu

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/Email/EmailService.cs`

```csharp
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dev4All.Infrastructure.Email;

public sealed class EmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _smtp = options.Value;

    public async Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_smtp.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();

        var secureSocket = _smtp.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocket, ct);

        if (!string.IsNullOrWhiteSpace(_smtp.Username))
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password, ct);

        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
```

> **Not:** `MailKit` NuGet paketi `Dev4All.Infrastructure.csproj`'a eklenmelidir (`dotnet add package MailKit`).

---

### 6.4. InfrastructureServiceRegistration

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/InfrastructureServiceRegistration.cs`

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
using Dev4All.Application.Options;
using Dev4All.Infrastructure.Auth;
using Dev4All.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Dev4All.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

        services.AddHttpContextAccessor();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
```

---

## Adım 7 — WebAPI Katmanı — Program.cs ve Middleware

Dosya konumu: `backend/src/Presentation/Dev4All.WebAPI/`

### 7.1. appsettings.json

**Dosya:** `backend/src/Presentation/Dev4All.WebAPI/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Database": {
    "ConnectionString": ""
  },
  "Jwt": {
    "Issuer": "Dev4All",
    "Audience": "Dev4AllUsers",
    "SecretKey": "",
    "ExpiryInMinutes": 120
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "noreply@dev4all.com",
    "Username": "",
    "Password": "",
    "UseSsl": true
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:5173" ]
  }
}
```

### 7.2. GlobalExceptionMiddleware

**Dosya:** `backend/src/Presentation/Dev4All.WebAPI/Middlewares/GlobalExceptionMiddleware.cs`

```csharp
using Dev4All.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Dev4All.WebAPI.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex) { await HandleExceptionAsync(context, ex); }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException or BusinessRuleViolationException:
                logger.LogWarning(exception, "Business/Validation error: {Message}", exception.Message);
                break;
            case ResourceNotFoundException:
                logger.LogInformation(exception, "Not found: {Message}", exception.Message);
                break;
            default:
                logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        context.Response.ContentType = "application/json";

        var (statusCode, body) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, (object)new
            {
                statusCode = 400,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = "Validation failed",
                errors = ve.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            }),
            ResourceNotFoundException => (HttpStatusCode.NotFound, new
            {
                statusCode = 404,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = exception.Message
            }),
            BusinessRuleViolationException => (HttpStatusCode.BadRequest, new
            {
                statusCode = 400,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = exception.Message
            }),
            UnauthorizedDomainException => (HttpStatusCode.Forbidden, new
            {
                statusCode = 403,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = exception.Message
            }),
            _ => (HttpStatusCode.InternalServerError, new
            {
                statusCode = 500,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.ToString(),
                message = "An unexpected error occurred."
            })
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(body, new JsonSerializerOptions
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
```

### 7.3. Program.cs

**Dosya:** `backend/src/Presentation/Dev4All.WebAPI/Program.cs`

```csharp
using Dev4All.Application;
using Dev4All.Application.Options;
using Dev4All.Infrastructure;
using Dev4All.Persistence;
using Dev4All.Persistence.Context;
using Dev4All.Persistence.Identity;
using Dev4All.WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var dbOptions = builder.Configuration
    .GetSection(DatabaseOptions.SectionName)
    .Get<DatabaseOptions>()
    ?? throw new InvalidOperationException("Database configuration section is missing.");

builder.Services.AddDbContext<Dev4AllDbContext>(options =>
    options.UseNpgsql(dbOptions.ConnectionString,
        npgsql => npgsql.EnableRetryOnFailure(dbOptions.MaxRetryCount)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<Dev4AllDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices();

builder.Services.AddCors(opt =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    opt.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
```

> **Not:** `AddOpenApi()` / `MapOpenApi()`, .NET 9+'da gelen yerleşik OpenAPI desteğidir; `Swashbuckle.AspNetCore` paketi ayrıca eklenmez. `MapOpenApi()` varsayılan olarak `/openapi/v1.json` adresinde şema sunar. Swagger UI istenirse `scalar` veya `redoc` paketi entegre edilebilir.

---

## Adım 8 — CQRS Feature: Auth Modülü

Bu adımda Auth feature'ı yalnızca iskelet değil, **çalışır command/query handler implementasyonları** ile tamamlanır. Handler'lar `UserManager` gibi altyapı sınıflarına değil, Application katmanındaki `IIdentityService`, `IJwtService`, `ICurrentUser` soyutlamalarına bağımlıdır.

### Klasör yapısı

```
Dev4All.Application/
└── Features/
    └── Auth/
        ├── Commands/
        │   ├── RegisterUser/
        │   │   ├── RegisterUserCommand.cs
        │   │   ├── RegisterUserCommandHandler.cs
        │   │   ├── RegisterUserCommandValidator.cs
        │   │   └── RegisterUserResponse.cs
│   └── LoginUser/
│       ├── LoginUserCommand.cs
│       ├── LoginUserCommandHandler.cs
│       ├── LoginUserCommandValidator.cs
│       └── LoginUserResponse.cs
        └── Queries/
            └── GetCurrentUser/
                ├── GetCurrentUserQuery.cs
                ├── GetCurrentUserQueryHandler.cs
                └── GetCurrentUserResponse.cs
```

**RegisterUserCommand.cs:**

```csharp
using Dev4All.Domain.Enums;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    UserRole Role   // Enum — type-safe, string tabanlı kontrol yok
) : IRequest<RegisterUserResponse>;
```

**RegisterUserCommandValidator.cs:**

```csharp
using FluentValidation;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .Length(2, 100).WithMessage("Ad 2-100 karakter arası olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az 1 büyük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az 1 rakam içermelidir.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Geçersiz kullanıcı rolü.");  // Enum doğrulama
    }
}
```

**RegisterUserCommandHandler.cs** — `IIdentityService` kullanır:

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Domain.Exceptions;
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IIdentityService identityService) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(
        RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, errors) = await identityService.CreateUserAsync(
            request.Name, request.Email, request.Password,
            request.Role.ToString(), cancellationToken);

        if (!succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", errors));

        return new RegisterUserResponse(Guid.Parse(userId), request.Email, request.Name);
    }
}
```

**LoginUserCommandHandler.cs** — `IIdentityService` + `IJwtService` + `IOptions<JwtOptions>` kullanır:

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Options;
using Dev4All.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Dev4All.Application.Features.Auth.Commands.LoginUser;

public sealed class LoginUserCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(
        LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, email, role) = await identityService.AuthenticateAsync(
            request.Email, request.Password, cancellationToken);

        if (!succeeded)
            throw new UnauthorizedDomainException("Geçersiz e-posta veya şifre.");

        var token = jwtService.GenerateToken(userId, email, role);
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiryInMinutes);
        return new LoginUserResponse(token, expiresAt, email, role);
    }
}
```

**GetCurrentUserQueryHandler.cs** — yalnızca `ICurrentUser` kullanır (DB çağrısı yok):

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Domain.Exceptions;
using MediatR;

namespace Dev4All.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser) : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    public Task<GetCurrentUserResponse> Handle(
        GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedDomainException("Kullanıcı doğrulaması gereklidir.");

        return Task.FromResult(
            new GetCurrentUserResponse(currentUser.UserId, currentUser.Email, currentUser.Role));
    }
}
```

> **Mimari Özet:** Handler'lar `IIdentityService`, `IJwtService`, `ICurrentUser` interface'lerini inject eder. `UserManager`, `RoleManager`, `ApplicationUser`, `IdentityUser` gibi sınıflar Application katmanında **asla görünmez**. Bu sınıflar yalnızca `IdentityService` içinde (`Dev4All.Infrastructure`) kullanılır.

---

## Adım 9 — AuthController İskeleti

**Dosya:** `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dev4All.Application.Features.Auth.Commands.RegisterUser;
using Dev4All.Application.Features.Auth.Commands.LoginUser;
using Dev4All.Application.Features.Auth.Queries.GetCurrentUser;
using Microsoft.AspNetCore.Authorization;

namespace Dev4All.WebAPI.Controllers.v1;

/// <summary>Authentication ve kullanıcı işlemleri</summary>
[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Yeni kullanıcı kaydı</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Created($"/api/v1/auth/me", response);
    }

    /// <summary>Kullanıcı girişi — JWT döner</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        var response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>Mevcut kullanıcı profili</summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var response = await sender.Send(new GetCurrentUserQuery(), ct);
        return Ok(response);
    }
}
```

---

## Adım 10 — Build Doğrulama

```powershell
# Tüm solution'ı derle
dotnet build backend/Dev4All.slnx

# Beklenen çıktı:
# Build succeeded.
# 0 Warning(s)
# 0 Error(s)

# Sağlık kontrolü (WebAPI çalışıyor ise)
# GET http://localhost:5000/health
```

---

## Hafta Sonu Kontrol Listesi

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Solution 7 proje içeriyor (Domain, Application, Infrastructure, Persistence, WebAPI, UnitTests, IntegrationTests) | ☑ |
| 2 | Katman referans kuralları ihlal edilmedi (Domain → hiçbir şey) | ☑ |
| 3 | `BaseEntity`, `Project`, `Bid`, `GitHubLog`, **`Contract`**, **`ContractRevision`** entity'leri oluşturuldu | ☑ |
| 4 | `ProjectStatus`, `BidStatus`, `UserRole`, **`ContractStatus`** enum'ları tanımlı | ☑ |
| 5 | `DomainException`, `ResourceNotFoundException`, `BusinessRuleViolationException` hazır | ☑ |
| 6 | Application'da aggregate read/write arayüzleri (`IProjectReadRepository` / `IProjectWriteRepository`, …) tanımlı | ☑ |
| 7 | `IUnitOfWork` interface'i Application'da tanımlı | ☑ |
| 8 | `ValidationBehavior<,>` MediatR pipeline'a eklenmiş | ☑ |
| 9 | `Dev4AllDbContext` Identity + entity DbSet'leriyle hazır (`Contracts`, `ContractRevisions` dahil) | ☑ |
| 10 | Entity konfigürasyonları (soft delete filtre, precision, ilişkiler, Project–GitHubLog 1-N, Contract 1-1) tamamlandı | ☑ |
| 11 | `JwtService`, `CurrentUser` ve `IdentityService` implementasyonları hazır | ☑ |
| 12 | `GlobalExceptionMiddleware` middleware pipeline'ının başına eklendi | ☑ |
| 13 | `Program.cs` temiz ve middleware sırası doğru | ☑ |
| 14 | `Auth` feature command/query, validator, handler ve response dosyaları oluşturuldu | ☑ |
| 15 | `dotnet build` sıfır hatayla tamamlandı | ☑ |

---

## Sonraki Hafta (3. Hafta) Önizleme

- `Dev4AllDbContext` migration'larını çalıştırma (`dotnet ef migrations add InitialCreate`)
- PostgreSQL veritabanı oluşturma ve seed verileri (Admin kullanıcısı, roller)
- Auth akışı için integration testleri (`register/login/me`) ve hata senaryoları
