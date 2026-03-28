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

## Adım 5 — Persistence Katmanı — Identity ve DbContext

Dosya konumu: `backend/src/Infrastructure/Dev4All.Persistence/`

### 5.1. ApplicationUser

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Identity/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace Dev4All.Persistence.Identity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
```

### 5.2. Dev4AllDbContext

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Context/Dev4AllDbContext.cs`

```csharp
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Context;

public class Dev4AllDbContext(DbContextOptions<Dev4AllDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<GitHubLog> GitHubLogs => Set<GitHubLog>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractRevision> ContractRevisions => Set<ContractRevision>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(Dev4AllDbContext).Assembly);
    }
}
```

### 5.3. Entity Type Configurations

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Configurations/ProjectConfiguration.cs`

```csharp
using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Budget).HasPrecision(18, 2);
        builder.Property(x => x.Technologies).HasMaxLength(500);

        // Soft delete global filter
        builder.HasQueryFilter(x => x.DeletedDate == null);

        // İlişkiler
        builder.HasMany(x => x.Bids)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.GitHubLog)
            .WithOne(x => x.Project)
            .HasForeignKey<GitHubLog>(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Configurations/BidConfiguration.cs`

```csharp
using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BidAmount).HasPrecision(18, 2);
        builder.Property(x => x.ProposalNote).IsRequired().HasMaxLength(1000);
        builder.HasQueryFilter(x => x.DeletedDate == null);
    }
}
```

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Configurations/GitHubLogConfiguration.cs`

```csharp
using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class GitHubLogConfiguration : IEntityTypeConfiguration<GitHubLog>
{
    public void Configure(EntityTypeBuilder<GitHubLog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RepoUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Branch).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CommitHash).IsRequired().HasMaxLength(40);
        builder.Property(x => x.CommitMessage).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.AuthorName).IsRequired().HasMaxLength(200);
    }
}
```

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Configurations/ContractConfiguration.cs`

```csharp
using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).IsRequired().HasMaxLength(10000);
        builder.Property(x => x.LastRevisedById).IsRequired().HasMaxLength(450);

        // 1-1 ilişki: bir Project'in en fazla bir Contract'u olabilir
        builder.HasOne(x => x.Project)
            .WithOne(x => x.Contract)
            .HasForeignKey<Contract>(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Revisions)
            .WithOne(x => x.Contract)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Configurations/ContractRevisionConfiguration.cs`

```csharp
using Dev4All.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev4All.Persistence.Configurations;

public class ContractRevisionConfiguration : IEntityTypeConfiguration<ContractRevision>
{
    public void Configure(EntityTypeBuilder<ContractRevision> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ContentSnapshot).IsRequired().HasMaxLength(10000);
        builder.Property(x => x.RevisedById).IsRequired().HasMaxLength(450);
        builder.Property(x => x.RevisionNote).HasMaxLength(500);
    }
}
```

### 5.4. Repository Implementasyonları

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/Repositories/ProjectRepository.cs`

```csharp
using Dev4All.Application.Abstractions.Repositories;
using Dev4All.Application.Common.Pagination;
using Dev4All.Domain.Entities;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dev4All.Persistence.Repositories;

public class ProjectRepository(Dev4AllDbContext context) : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Projects.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Project?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default)
        => await context.Projects
            .Include(x => x.Bids)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<Project>> GetOpenProjectsAsync(
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Projects
            .Where(x => x.Status == Dev4All.Domain.Enums.ProjectStatus.Open)
            .AsQueryable();

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Project>(items, totalCount, page, pageSize);
    }

    public async Task<List<Project>> GetByCustomerIdAsync(string customerId, CancellationToken ct = default)
        => await context.Projects
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(ct);

    public async Task AddAsync(Project entity, CancellationToken ct = default)
        => await context.Projects.AddAsync(entity, ct);

    public void Update(Project entity) => context.Projects.Update(entity);

    public void Delete(Project entity)
    {
        entity.MarkAsDeleted();
        context.Projects.Update(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);
}
```

### 5.5. UnitOfWork Implementasyonu

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/UnitOfWork.cs`

```csharp
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dev4All.Persistence;

public class UnitOfWork(Dev4AllDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("A transaction is already in progress.");
        _transaction = await context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null) throw new InvalidOperationException("No transaction to commit.");
        try
        {
            await context.SaveChangesAsync(ct);
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
        => await context.SaveChangesAsync(ct);
}
```

### 5.6. PersistenceServiceRegistration

**Dosya:** `backend/src/Infrastructure/Dev4All.Persistence/PersistenceServiceRegistration.cs`

```csharp
using Dev4All.Application.Abstractions.Persistence;
using Dev4All.Application.Abstractions.Repositories;
using Dev4All.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Dev4All.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IGitHubLogRepository, GitHubLogRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IContractRevisionRepository, ContractRevisionRepository>();
        return services;
    }
}
```

---

## Adım 6 — Infrastructure Katmanı — JWT ve Auth

Dosya konumu: `backend/src/Infrastructure/Dev4All.Infrastructure/`

### 6.1. IJwtService Interface (Application katmanında)

**Dosya:** `backend/src/Core/Dev4All.Application/Abstractions/Auth/IJwtService.cs`

```csharp
namespace Dev4All.Application.Abstractions.Auth;

public interface IJwtService
{
    /// <summary>Kullanıcı bilgilerinden JWT Token üretir</summary>
    string GenerateToken(string userId, string email, string role);
}
```

> **Mimari Not:** Interface Application katmanında primitive parametreler alır. `ApplicationUser`'a (Persistence) referans vermek Application → Persistence bağımlılığı yaratır; bu Clean Architecture'ı ihlal eder.

### 6.2. JwtService Implementasyonu

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/Auth/JwtService.cs`

```csharp
using Dev4All.Application.Abstractions.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dev4All.Infrastructure.Auth;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateToken(string userId, string email, string role)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

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

### 6.4. InfrastructureServiceRegistration

**Dosya:** `backend/src/Infrastructure/Dev4All.Infrastructure/InfrastructureServiceRegistration.cs`

```csharp
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Application.Abstractions.Services;
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
        // JWT Authentication
        var jwtSettings = configuration.GetSection("Jwt");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
                };
            });

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddHttpContextAccessor();

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
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dev4all_db;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Issuer": "Dev4All",
    "Audience": "Dev4All",
    "Key": "CHANGE_ME_IN_PRODUCTION_MIN_32_CHARS_SECURE",
    "ExpiryMinutes": 60
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "noreply@dev4all.com",
    "UseSsl": true
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
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
using Dev4All.Infrastructure;
using Dev4All.Persistence;
using Dev4All.Persistence.Context;
using Dev4All.Persistence.Identity;
using Dev4All.WebAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<Dev4AllDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Katman servisleri
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices();

// CORS
builder.Services.AddCors(opt =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    opt.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware pipeline (sıra zorunludur)
app.UseMiddleware<GlobalExceptionMiddleware>();   // 1. Global hata yakalama
app.UseHttpsRedirection();                        // 2. HTTPS
app.UseSwagger();                                 // 3. Swagger
app.UseSwaggerUI();
app.UseCors("AllowFrontend");                     // 4. CORS
app.UseAuthentication();                          // 5. JWT doğrulama
app.UseAuthorization();                           // 6. Rol/Policy
app.MapControllers();                             // 7. Controller routing

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
```

---

## Adım 8 — CQRS Feature Scaffold: Auth Modülü (Örnek)

Bu haftanın kapsamında Features klasörü kurulur; **Auth feature'ının komut ve query iskeletleri** oluşturulur. Tam handler implementasyonu 6. haftada yapılacaktır.

### Klasör yapısı

```
Dev4All.Application/
└── Features/
    └── Auth/
        ├── Commands/
        │   ├── RegisterUser/
        │   │   ├── RegisterUserCommand.cs
        │   │   ├── RegisterUserCommandHandler.cs       ← İskelet (NotImplementedException)
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

**Örnek — RegisterUserCommand.cs:**

```csharp
using MediatR;

namespace Dev4All.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    string Role   // "Customer" veya "Developer"
) : IRequest<RegisterUserResponse>;
```

**Örnek — RegisterUserCommandValidator.cs:**

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
            .NotEmpty()
            .Must(r => r is "Customer" or "Developer")
            .WithMessage("Rol 'Customer' veya 'Developer' olmalıdır.");
    }
}
```

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
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>Yeni kullanıcı kaydı</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        return Created($"/api/v1/auth/me", response);
    }

    /// <summary>Kullanıcı girişi — JWT döner</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        return Ok(response);
    }

    /// <summary>Mevcut kullanıcı profili</summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var response = await mediator.Send(new GetCurrentUserQuery(), ct);
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
| 1 | Solution 5 proje içeriyor (Domain, Application, Infrastructure, Persistence, WebAPI) | ☐ |
| 2 | Katman referans kuralları ihlal edilmedi (Domain → hiçbir şey) | ☐ |
| 3 | `BaseEntity`, `ProjectRequest`, `Bid`, `GitHubLog`, **`Contract`**, **`ContractRevision`** entity'leri oluşturuldu | ☐ |
| 4 | `ProjectStatus`, `BidStatus`, `UserRole`, **`ContractStatus`** enum'ları tanımlı | ☐ |
| 5 | `DomainException`, `ResourceNotFoundException`, `BusinessRuleViolationException` hazır | ☐ |
| 6 | `IProjectRepository`, `IBidRepository`, `IGitHubLogRepository`, **`IContractRepository`**, **`IContractRevisionRepository`** interface'leri Application'da | ☐ |
| 7 | `IUnitOfWork` interface'i Application'da tanımlı | ☐ |
| 8 | `ValidationBehavior<,>` MediatR pipeline'a eklenmiş | ☐ |
| 9 | `Dev4AllDbContext` Identity + entity DbSet'leriyle hazır (`Contracts`, `ContractRevisions` dahil) | ☐ |
| 10 | Entity konfigürasyonları (soft delete filtre, precision, ilişkiler, Contract 1-1) tamamlandı | ☐ |
| 11 | `JwtService` ve `CurrentUser` implementasyonları hazır | ☐ |
| 12 | `GlobalExceptionMiddleware` middleware pipeline'ının başına eklendi | ☐ |
| 13 | `Program.cs` temiz ve middleware sırası doğru | ☐ |
| 14 | `Auth` feature iskeletleri (Command, Validator, Response) oluşturuldu | ☐ |
| 15 | `dotnet build` sıfır hatayla tamamlandı | ☐ |

---

## Sonraki Hafta (3. Hafta) Önizleme

- `Dev4AllDbContext` migration'larını çalıştırma (`dotnet ef migrations add InitialCreate`)
- PostgreSQL veritabanı oluşturma ve seed verileri (Admin kullanıcısı, roller)
- Contract ve ContractRevision repository implementasyonlarını oluşturma
