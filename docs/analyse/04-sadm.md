# 04. Sistem Mimarisi ve Veri Modeli (SADM) - Dev4All

## 1. Giriş ve Amaç

Bu doküman, Dev4All platformunun **backend mimari tasarımını**, **katman yapısını**, **entity ilişki modelini (ER)** ve **CQRS deseninin uygulanış biçimini** tanımlar. Geliştirme sürecinde alınan tüm mimari kararlar bu dokümana dayanır.

> Mimari değişiklikler bu dokümanı güncellemeyi gerektirir. Katman sınırlarını ihlal eden pull request'ler kabul edilmez.

---

## 2. Mimari Genel Bakış

Dev4All backend'i **Onion Architecture (Soğan Mimarisi / Hexagonal Architecture)** prensipleriyle inşa edilir. Bu yaklaşımın temel amacı:

- **İş mantığını** dış bileşenlerden (veritabanı, e-posta, GitHub API) **izole etmek.**
- Her katmanın yalnızca **iç katmana bağımlı** olmasını sağlamak; dış katmanlar iç katmanları **referans alabilir**, tersi geçerli değildir.
- Birim testlerinin altyapı bağımlılığı olmadan yazılabilmesine olanak tanımak.

### Katman Bağımlılık Kuralı

```
WebAPI  ──►  Application  ──►  Domain
Infrastructure  ──►  Application  ──►  Domain
Persistence  ──►  Application  ──►  Domain
```

> Oklar "bağımlıdır" yönünü gösterir. **Domain hiçbir katmana bağımlı değildir.**

---

## 3. Katman Yapısı

### 3.1. Domain Katmanı

**Sorumluluk:** Uygulamanın kalbi. Tüm iş varlıklarını, enum'ları ve domain kurallarını içerir.

| İçerik | Açıklama |
|--------|----------|
| **Entity'ler** | `User`, `Project`, `Bid`, `GitHubLog`, `Contract`, `ContractRevision` — `BaseEntity`'den türeyen POCO sınıfları. |
| **Enum'lar** | `ProjectStatus`, `BidStatus`, `UserRole` |
| **Exception'lar** | `DomainException`, `ResourceNotFoundException`, `BusinessRuleViolationException` vb. |
| **Bağımlılık** | **Sıfır** dış bağımlılık. NuGet paketi, interface veya DataAnnotation içermez. |

### 3.2. Application Katmanı

**Sorumluluk:** İş mantığı ve uygulama akışı. CQRS desenleri ve validation burada yönetilir.

| İçerik | Açıklama |
|--------|----------|
| **Command'lar** | `CreateProjectCommand`, `PlaceBidCommand`, `AcceptBidCommand`, `ReviseContractCommand`, `ApproveContractCommand` |
| **Query'ler** | `GetProjectsQuery`, `GetProjectBidsQuery`, `GetContractQuery`, `GetGitHubLogsByProjectQuery` |
| **Handler'lar** | Her Command/Query için `IRequestHandler<TRequest, TResponse>` implementasyonu. |
| **DTO'lar** | Request ve Response — `sealed record` tipinde veri transfer nesneleri. |
| **Validator'lar** | FluentValidation tabanlı; her Command/Query ile aynı klasörde ayrı validator sınıfı. |
| **Persistence Soyutlamaları** | `IReadRepository<T>`, `IWriteRepository<T>`, `IUnitOfWork` — Persistence'a karşı soyutlama. |
| **Entity Repository Interface'leri** | `IProjectRepository`, `IBidRepository`, `IGitHubLogRepository`, `IContractRepository` — entity bazlı özelleşmiş interface'ler. |
| **Servis Interface'leri** | `IEmailService`, `IGitHubService` — Infrastructure'a karşı soyutlama. |
| **Bağımlılık** | Yalnızca Domain katmanına bağımlıdır. `MediatR`, `FluentValidation` referansları. |

### 3.3. Infrastructure Katmanı

**Sorumluluk:** Dış servis entegrasyonları. Uygulama katmanındaki interface'lerin somut implementasyonlarını barındırır.

| İçerik | Açıklama |
|--------|----------|
| **EmailService** | `IEmailService` implementasyonu — MailKit ile SMTP gönderimi. |
| **GitHubService** | `IGitHubService` implementasyonu — GitHub Webhook payload işleme. |
| **Background Jobs** | Quartz.NET tabanlı zamanlayıcı görevler (`ExpiredBidJob`, `EmailDispatchJob`). |
| **Bağımlılık** | **Yalnızca Application** katmanına bağımlıdır. Domain'e transitif erişim Application üzerinden sağlanır. `MailKit`, `Quartz` referansları. |

### 3.4. Persistence Katmanı

**Sorumluluk:** Veritabanı erişimi. Entity Framework Core altyapısı ve repository implementasyonları.

| İçerik | Açıklama |
|--------|----------|
| **DbContext** | `Dev4AllDbContext` — EF Core DbSet tanımları; her entity için ayrı `IEntityTypeConfiguration<T>`. |
| **Repository'ler** | `IProjectRepository`, `IBidRepository` vb. entity-specific interface implementasyonları. Soft delete için `MarkAsDeleted()` çağrılır. |
| **Unit of Work** | `IUnitOfWork` implementasyonu — `BeginTransactionAsync / CommitTransactionAsync / RollbackTransactionAsync`. |
| **Migration'lar** | EF Core migration dosyaları (`dotnet ef migrations add`). |
| **Seed Verileri** | Geliştirme ortamı için başlangıç verileri (Admin kullanıcısı vb.). |
| **Bağımlılık** | **Yalnızca Application** katmanına bağımlıdır. Domain'e transitif erişim Application üzerinden sağlanır. `EF Core`, `Npgsql` referansları. |

### 3.5. Web API Katmanı (Presentation)

**Sorumluluk:** HTTP iletişimi. Dış dünyanın platformla etkileşim kapısı.

| İçerik | Açıklama |
|--------|----------|
| **Controller'lar** | `AuthController`, `ProjectsController`, `BidsController`, `ContractsController`, `WebhookController` |
| **Middleware'ler** | Global Exception Handler, JWT Authentication Middleware. |
| **Program.cs** | Dependency Injection kayıtları, Swagger konfigürasyonu, middleware pipeline. |
| **Bağımlılık** | Yalnızca Application katmanına bağımlıdır. MediatR üzerinden Command/Query gönderir. |

---

## 4. Proje Klasör Yapısı

```
Dev4All/   (monorepo kökü)
├── backend/
│   ├── Dev4All.slnx
│   ├── Directory.Build.props
│   ├── src/
│   │   ├── Core/
│   │   │   ├── Dev4All.Domain/       # Entity, Enum, Exception (sıfır bağımlılık)
│   │   │   └── Dev4All.Application/  # CQRS, DTO, Validator, Abstractions
│   │   ├── Infrastructure/
│   │   │   ├── Dev4All.Infrastructure/  # Email, GitHub, Background Jobs
│   │   │   └── Dev4All.Persistence/     # DbContext, Repository, Migration
│   │   └── Presentation/
│   │       └── Dev4All.WebAPI/       # Controller, Middleware, Program.cs
│   └── tests/
│       ├── Dev4All.UnitTests/
│       └── Dev4All.IntegrationTests/
├── frontend/   (planlı web istemcisi)
├── mobile/     (planlı mobil istemci)
└── docs/
```

---

## 5. Veri Modeli (Entity İlişki Diyagramı)

```mermaid
erDiagram
    User ||--o{ Project : "oluşturur"
    User ||--o{ Bid : "verir"

    User {
        uuid    Id            PK
        string  Email
        string  PasswordHash
        string  Name
        string  Role          "Customer | Developer | Admin"
        bool    IsEmailConfirmed
        datetime CreatedAt
    }

    Project ||--o{ Bid : "alır"
    Project ||--o| GitHubLog : "üretir"
    Project ||--o| Contract : "sahiptir"

    Project {
        uuid    Id                  PK
        uuid    CustomerId          FK
        uuid    AssignedDeveloperId FK "NULL: atanmamış"
        string  Title
        string  Description
        decimal Budget
        datetime Deadline
        datetime BidEndDate
        int     Status              "0:Open 1:AwaitingContract 2:Ongoing 3:Completed 4:Expired 5:Cancelled"
        string  Technologies        "Opsiyonel"
        datetime CreatedAt
        datetime UpdatedAt
        bool    IsDeleted           "Soft Delete"
    }

    Bid {
        uuid    Id          PK
        uuid    ProjectId   FK
        uuid    DeveloperId FK
        decimal BidAmount
        string  ProposalNote
        int     Status      "0:Pending 1:Accepted 2:Rejected"
        bool    IsAccepted
        datetime CreatedAt
        datetime UpdatedAt
    }

    Contract ||--o{ ContractRevision : "revize edilir"

    Contract {
        uuid    Id                  PK
        uuid    ProjectId           FK
        string  Content             "Sözleşme metni"
        int     RevisionNumber      "Her düzenlemede artar"
        uuid    LastRevisedById     "Kimin düzenlediği"
        int     Status              "0:Draft 1:UnderReview 2:BothApproved 3:Cancelled"
        bool    IsCustomerApproved
        bool    IsDeveloperApproved
        datetime CustomerApprovedAt
        datetime DeveloperApprovedAt
    }

    ContractRevision {
        uuid    Id              PK
        uuid    ContractId      FK
        uuid    RevisedById     "Hangi kullanıcı"
        string  ContentSnapshot "O anki sözleşme metni"
        int     RevisionNumber
        string  RevisionNote    "Opsiyonel açıklama"
        datetime CreatedAt
    }

    GitHubLog {
        uuid    Id          PK
        uuid    ProjectId   FK
        string  RepoUrl
        string  Branch
        string  CommitHash
        string  CommitMessage
        string  AuthorName
        datetime PushedAt
    }
```

---

## 6. Entity Detayları

### 6.1. User
| Alan | Tip | Kural |
|------|-----|-------|
| `Id` | `Guid` | PK, otomatik üretilir. |
| `Email` | `string` | Benzersiz, boş olamaz. |
| `PasswordHash` | `string` | bcrypt ile hashlenmiş. |
| `Name` | `string` | 2–100 karakter. |
| `Role` | `enum UserRole` | `Customer`, `Developer`, `Admin` |
| `IsEmailConfirmed` | `bool` | Varsayılan: `false`. |
| `CreatedAt` | `DateTime` | UTC, otomatik. |

### 6.2. Project
| Alan | Tip | Kural |
|------|-----|-------|
| `Id` | `Guid` | PK, otomatik üretilir. |
| `CustomerId` | `Guid` | FK → User. |
| `AssignedDeveloperId` | `Guid?` | FK → User; NULL iken `Open`. |
| `Title` | `string` | 3–100 karakter. |
| `Description` | `string` | 10–2000 karakter. |
| `Budget` | `decimal` | > 0. |
| `Deadline` | `DateTime` | UTC, gelecek tarih. |
| `BidEndDate` | `DateTime` | UTC, Deadline'dan önce. |
| `Technologies` | `string?` | Opsiyonel etiket listesi. |
| `Status` | `enum ProjectStatus` | `Open → AwaitingContract → Ongoing → Completed / Expired / Cancelled` |
| `IsDeleted` | `bool` | Soft Delete; varsayılan `false`. |

### 6.3. Bid
| Alan | Tip | Kural |
|------|-----|-------|
| `Id` | `Guid` | PK, otomatik üretilir. |
| `ProjectId` | `Guid` | FK → Project. |
| `DeveloperId` | `Guid` | FK → User. |
| `BidAmount` | `decimal` | > 0. |
| `ProposalNote` | `string` | 10–1000 karakter. |
| `Status` | `enum BidStatus` | `Pending → Accepted / Rejected` |
| `IsAccepted` | `bool` | Yalnızca bir Bid için `true` olabilir. |

### 6.4. GitHubLog
| Alan | Tip | Kural |
|------|-----|-------|
| `Id` | `Guid` | PK, otomatik üretilir. |
| `ProjectId` | `Guid` | FK → Project. |
| `RepoUrl` | `string` | Geçerli GitHub URL formatı. |
| `Branch` | `string` | Varsayılan: `"main"`. |
| `CommitHash` | `string` | 40 karakter SHA-1 hash. |
| `CommitMessage` | `string` | Boş olamaz. |
| `AuthorName` | `string` | GitHub commit author. |
| `PushedAt` | `DateTime` | UTC, GitHub'dan gelen zaman. |

### 6.5. Contract
| Alan | Tip | Kural |
|------|-----|-------|
| `Id` | `Guid` | PK, otomatik üretilir. |
| `ProjectId` | `Guid` | FK → Project (1-1). |
| `Content` | `string` | Sözleşme metni; boş olamaz, min 50 karakter. |
| `RevisionNumber` | `int` | Başlangıç: 1; her düzenlemede artar. |
| `LastRevisedById` | `string` | Son düzenlemeyi yapan kullanıcı Id'si. |
| `Status` | `enum ContractStatus` | `Draft → UnderReview → BothApproved / Cancelled` |
| `IsCustomerApproved` | `bool` | Customer onay durumu; varsayılan `false`. |
| `IsDeveloperApproved` | `bool` | Developer onay durumu; varsayılan `false`. |
| `CustomerApprovedAt` | `DateTime?` | UTC, Customer onayladığında. |
| `DeveloperApprovedAt` | `DateTime?` | UTC, Developer onayladığında. |

### 6.6. ContractRevision
| Alan | Tip | Kural |
|------|-----|-------|
| `Id` | `Guid` | PK, otomatik üretilir. |
| `ContractId` | `Guid` | FK → Contract. |
| `RevisedById` | `string` | Revize eden kullanıcı Id'si. |
| `ContentSnapshot` | `string` | Düzenleme öncesi sözleşme metni. |
| `RevisionNumber` | `int` | Bu revizyon numarası. |
| `RevisionNote` | `string?` | Opsiyonel açıklama; max 500 karakter. |

---

## 7. CQRS Mimari Özeti

Sistemde yazma (Command) ve okuma (Query) operasyonları **MediatR** kütüphanesi aracılığıyla birbirinden ayrılır.

### 7.1. Command'lar (Yazma İşlemleri)

| Command | Tetikleyici | Yetki |
|---------|------------|-------|
| `RegisterUserCommand` | Kullanıcı kaydı | Public |
| `LoginUserCommand` | Kullanıcı girişi, JWT döner | Public |
| `CreateProjectCommand` | Yeni proje oluşturma | Customer |
| `UpdateProjectCommand` | Proje güncelleme | Customer (Sahip) |
| `DeleteProjectCommand` | Proje silme (soft) | Customer (Sahip) |
| `PlaceBidCommand` | Teklif verme | Developer |
| `UpdateBidCommand` | Teklif güncelleme | Developer (Sahip) |
| `AcceptBidCommand` | Teklif kabul — `AwaitingContract`'a geçiş + `Contract` oluşturma | Customer (Proje Sahibi) |
| `ReviseContractCommand` | Sözleşme revize etme | Customer/Developer (Proje Tarafı) |
| `ApproveContractCommand` | Sözleşme onaylama — her ikisi onayladıysa `Ongoing`'e geçiş | Customer/Developer (Proje Tarafı) |
| `CancelContractCommand` | Sözleşme iptali — projeyi `Cancelled` yapar | Customer/Developer (Proje Tarafı) |
| `LinkGitHubRepoCommand` | GitHub repo bağlama | Developer (Atanmış) |

### 7.2. Query'ler (Okuma İşlemleri)

| Query | Döndürdüğü Veri | Yetki |
|-------|----------------|-------|
| `GetProjectsQuery` | Sayfalı açık proje listesi | Auth |
| `GetProjectByIdQuery` | Tek proje detayı | Auth |
| `GetMyProjectsQuery` | Giriş yapan Customer'ın projeleri | Customer |
| `GetProjectBidsQuery` | Bir projeye ait tüm teklifler | Customer (Sahip) |
| `GetMyBidsQuery` | Developer'ın verdiği teklifler | Developer |
| `GetContractQuery` | Projeye ait sözleşme detayı | Auth (Proje Tarafı) |
| `GetContractRevisionsQuery` | Sözleşme revizyon geçmişi | Auth (Proje Tarafı) |
| `GetProjectDetailQuery` | Aktif proje detayı | Auth (Proje Tarafı) |
| `GetGitHubLogsByProjectQuery` | Proje aktivite timeline'ı | Auth (Proje Tarafı) |

---

## 8. Bağlantılı Dokümanlar

| Doküman | Açıklama |
|---------|----------|
| `01-brd.md` | Business Requirements — Proje kapsamı ve iş hedefleri. |
| `02-frd.md` | Functional Requirements — Kullanıcı hikayeleri ve iş kuralları. |
| `03-nfr.md` | Non-Functional Requirements — Performans, güvenlik, loglama. |
| `05-integration.md` | Entegrasyon Spesifikasyonları — GitHub Webhook ve MailKit detayları. |
