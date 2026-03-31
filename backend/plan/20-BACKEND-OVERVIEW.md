# 20. Backend Agent Development Plan — Master Overview

> **Last updated:** 2026-03-31  
> **Status:** Auth module complete; remaining features to be implemented via agent-driven development.

---

## 1. Project Summary

Dev4All backend is a .NET 10 / C# 13 Web API built with **Clean Architecture (Onion)** and **CQRS (MediatR)**. It powers a B2B freelance marketplace where customers post projects, developers bid, contracts are negotiated, and work is tracked via GitHub webhook integration.

## 2. Tech Stack

| Category | Technology |
|----------|-----------|
| Runtime | .NET 10 / C# 13 |
| ORM | Entity Framework Core 10 (PostgreSQL — Npgsql) |
| CQRS | MediatR 14 |
| Validation | FluentValidation 12 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Email | MailKit |
| Scheduling | Quartz.NET (planned) |
| Testing | xUnit, FluentAssertions, NSubstitute, Bogus, coverlet |

## 3. Solution Architecture

```
backend/src/
  Core/
    Dev4All.Domain          → Entities, Enums, Exceptions (ZERO dependencies)
    Dev4All.Application     → CQRS Commands/Queries, Validators, DTOs, Interfaces
  Infrastructure/
    Dev4All.Persistence     → EF Core DbContext, Configurations, Repositories
    Dev4All.Infrastructure  → JWT, Email, GitHub service, Background Jobs
  Presentation/
    Dev4All.WebAPI          → ASP.NET Core controllers (thin — MediatR only)
backend/tests/
  Dev4All.UnitTests         → Handler, Validator, Entity method tests
  Dev4All.IntegrationTests  → API pipeline tests (WebApplicationFactory)
```

### Dependency Flow (strict — never violate)

```
Domain ← Application ← Persistence/Infrastructure ← WebAPI
```

- Domain has ZERO external NuGet packages.
- Application depends only on Domain.
- Persistence and Infrastructure depend on Application.
- WebAPI is the Composition Root and references all layers for DI wiring.

## 4. What's Already Implemented

| Layer | Component | Status |
|-------|-----------|--------|
| **Domain** | `BaseEntity`, `Project`, `Bid`, `Contract`, `ContractRevision`, `GitHubLog` | ✅ Complete |
| **Domain** | `ProjectStatus`, `BidStatus`, `ContractStatus`, `UserRole` enums | ✅ Complete |
| **Domain** | Exception hierarchy (`DomainException`, `ResourceNotFoundException`, `BusinessRuleViolationException`, `UnauthorizedDomainException`) | ✅ Complete |
| **Application** | Auth CQRS — `RegisterUser`, `LoginUser`, `GetCurrentUser` (Command/Query + Handler + Validator + Response) | ✅ Complete |
| **Application** | All repository interfaces (read/write per aggregate) | ✅ Complete |
| **Application** | `IUnitOfWork`, `IEmailService`, `IGitHubService`, `ICurrentUser`, `IJwtService`, `IIdentityService` | ✅ Complete |
| **Application** | `ValidationBehavior`, `PagedResult<T>`, `ApplicationServiceRegistration` | ✅ Complete |
| **Application** | Options: `DatabaseOptions`, `JwtOptions`, `SmtpOptions` | ✅ Complete |
| **Persistence** | `Dev4AllDbContext` (Identity + all DbSets + audit field override) | ✅ Complete |
| **Persistence** | All EF Configurations (Project, Bid, Contract, ContractRevision, GitHubLog) | ✅ Complete |
| **Persistence** | All Repository implementations (read/write per aggregate) | ✅ Complete |
| **Persistence** | `UnitOfWork`, `PersistenceServiceRegistration` | ✅ Complete |
| **Persistence** | `ApplicationUser` (IdentityUser + Name, CreatedDate) | ✅ Complete |
| **Persistence** | Initial migration (`20260330181333_InitialCreate`) | ✅ Complete |
| **Infrastructure** | `JwtService`, `IdentityService`, `CurrentUser`, `EmailService` | ✅ Complete |
| **Infrastructure** | `InfrastructureServiceRegistration` (JWT Bearer + DI) | ✅ Complete |
| **WebAPI** | `AuthController` (register, login, me) | ✅ Complete |
| **WebAPI** | `GlobalExceptionMiddleware` | ✅ Complete |
| **WebAPI** | `Program.cs` (Options, DbContext, Identity, CORS, Swagger, middleware pipeline, health endpoint) | ✅ Complete |
| **WebAPI** | `IdentityRoleSeeder` | ✅ Complete |
| **CI/CD** | `ef-database-update.yml` workflow | ✅ Complete |

## 5. What Needs to Be Built

| Sprint | Module | Components |
|--------|--------|------------|
| S1 | **Project CRUD** | Commands (Create, Update, Delete), Queries (List, ById, MyProjects), `ProjectsController`, unit tests |
| S2 | **Bid Module** | Commands (Place, Update, Accept), Queries (ProjectBids, MyBids), `BidsController`, unit tests |
| S3 | **Contract Module** | Commands (Revise, Approve, Cancel), Queries (GetContract, Revisions), `ContractsController`, unit tests |
| S4 | **GitHub Integration** | `GitHubService` impl, Commands (LinkRepo), `WebhookController`, Queries (GitHubLogs), unit tests |
| S5 | **Email & Background Jobs** | Email templates, Quartz.NET EmailDispatchJob, notification triggers, unit tests |
| S6 | **Testing & Quality** | Integration tests (WebApplicationFactory), Serilog, code coverage >80% |
| S7 | **Polish & Docs** | Swagger improvements, API versioning review, performance audit, README updates |

## 6. Reference Documents

| Document | Path | Purpose |
|----------|------|---------|
| AGENTS.md | `docs/AGENTS.md` | AI coding standards, architecture rules |
| BRD | `docs/analyse/01-brd.md` | Business requirements |
| FRD | `docs/analyse/02-frd.md` | Functional requirements, use cases, API endpoints, validation rules |
| NFR | `docs/analyse/03-nfr.md` | Non-functional requirements |
| SADM | `docs/analyse/04-sadm.md` | System architecture, entity model, CQRS operations |
| Integration | `docs/analyse/05-integration.md` | GitHub webhook + email integration specs |
| Week 2 Plan | `docs/plan/2-hafta-backend-mimari-plan.md` | Completed foundation setup guide |

## 7. Domain Model Quick Reference

### Entities

| Entity | Key Fields | Rich Behavior |
|--------|-----------|---------------|
| `Project` | CustomerId, AssignedDeveloperId, Title, Description, Budget, Deadline, BidEndDate, Technologies, Status | `SetCustomer()`, `AssignDeveloper()`, `MoveToAwaitingContract()`, `MoveToOngoing()`, `Complete()`, `Expire()`, `Cancel()` |
| `Bid` | ProjectId, DeveloperId, BidAmount, ProposalNote, Status, IsAccepted | `SetOwnership()`, `Accept()`, `Reject()` |
| `Contract` | ProjectId, Content, RevisionNumber, LastRevisedById, Status, IsCustomerApproved, IsDeveloperApproved | `SetProjectId()`, `Revise()`, `Approve()`, `Cancel()`, `SetInitialContent()` |
| `ContractRevision` | ContractId, RevisedById, ContentSnapshot, RevisionNumber, RevisionNote | `CreateSnapshot()` (static factory) |
| `GitHubLog` | ProjectId, RepoUrl, Branch, CommitHash, CommitMessage, AuthorName, PushedAt | `Create()` (static factory) |

### Enums

| Enum | Values |
|------|--------|
| `ProjectStatus` | Open(0), AwaitingContract(1), Ongoing(2), Completed(3), Expired(4), Cancelled(5) |
| `BidStatus` | Pending(0), Accepted(1), Rejected(2) |
| `ContractStatus` | Draft(0), UnderReview(1), BothApproved(2), Cancelled(3) |
| `UserRole` | Customer, Developer, Admin |

### State Transitions

```
Project: Open → AwaitingContract → Ongoing → Completed
                                          ↘ Cancelled
         Open → Expired
         Open → Cancelled
         AwaitingContract → Cancelled

Bid:     Pending → Accepted
         Pending → Rejected

Contract: Draft → UnderReview → BothApproved
                             → Cancelled
          UnderReview → Cancelled
```

## 8. API Endpoint Map (FRD Reference)

### Implemented ✅

| Method | Endpoint | Auth |
|--------|----------|------|
| `POST` | `/api/v1/auth/register` | Public |
| `POST` | `/api/v1/auth/login` | Public |
| `GET` | `/api/v1/auth/me` | `[Authorize]` |

### To Be Implemented

| Method | Endpoint | Auth | Sprint |
|--------|----------|------|--------|
| `GET` | `/api/v1/projects` | Auth | S1 |
| `POST` | `/api/v1/projects` | Customer | S1 |
| `GET` | `/api/v1/projects/{id}` | Auth | S1 |
| `PUT` | `/api/v1/projects/{id}` | Customer (Owner) | S1 |
| `DELETE` | `/api/v1/projects/{id}` | Customer (Owner) | S1 |
| `GET` | `/api/v1/projects/my` | Customer | S1 |
| `GET` | `/api/v1/projects/{id}/bids` | Customer (Owner) | S2 |
| `POST` | `/api/v1/projects/{id}/bids` | Developer | S2 |
| `PUT` | `/api/v1/bids/{id}` | Developer (Owner) | S2 |
| `POST` | `/api/v1/bids/{id}/accept` | Customer (Project Owner) | S2 |
| `GET` | `/api/v1/bids/my` | Developer | S2 |
| `GET` | `/api/v1/contracts/{projectId}` | Auth (Project Party) | S3 |
| `PUT` | `/api/v1/contracts/{projectId}` | Customer/Developer (Party) | S3 |
| `POST` | `/api/v1/contracts/{projectId}/approve` | Customer/Developer (Party) | S3 |
| `POST` | `/api/v1/contracts/{projectId}/cancel` | Customer/Developer (Party) | S3 |
| `GET` | `/api/v1/contracts/{projectId}/revisions` | Auth (Project Party) | S3 |
| `PUT` | `/api/v1/projects/{id}/repo` | Developer (Assigned) | S4 |
| `GET` | `/api/v1/projects/{id}/github-logs` | Auth (Project Party) | S4 |
| `POST` | `/api/v1/webhooks/github` | HMAC-SHA256 | S4 |

## 9. Existing Repository Interfaces (Application Layer)

Handlers use these interfaces — implementations already exist in Persistence.

| Interface | Custom Methods |
|-----------|---------------|
| `IProjectReadRepository` | `GetByIdWithBidsAsync`, `GetOpenProjectsAsync`, `GetByCustomerIdAsync` |
| `IProjectWriteRepository` | (base: Add, Update, Remove) |
| `IBidReadRepository` | `GetByDeveloperAndProjectAsync`, `GetByProjectIdAsync`, `GetByDeveloperIdAsync` |
| `IBidWriteRepository` | (base) |
| `IContractReadRepository` | `GetByProjectIdAsync`, `GetByIdWithRevisionsAsync` |
| `IContractWriteRepository` | (base) |
| `IContractRevisionReadRepository` | `GetByContractIdAsync` |
| `IContractRevisionWriteRepository` | (base) |
| `IGitHubLogReadRepository` | `GetByProjectIdAsync` |
| `IGitHubLogWriteRepository` | (base: Add, AddRange) |
| `IUnitOfWork` | `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`, `SaveChangesAsync` |

## 10. Agent Conventions

### Feature Folder Structure

```
Features/
  {Module}/
    Commands/{ActionName}/
      {ActionName}Command.cs          (sealed record : IRequest<TResponse>)
      {ActionName}CommandHandler.cs   (sealed class : IRequestHandler<...>)
      {ActionName}CommandValidator.cs (sealed class : AbstractValidator<...>)
      {ActionName}Response.cs         (sealed record)
    Queries/{QueryName}/
      {QueryName}Query.cs
      {QueryName}QueryHandler.cs
      {QueryName}Response.cs
```

### Coding Rules

- Use `sealed record` for Commands, Queries, Responses.
- Use `sealed class` with primary constructors for Handlers and Validators.
- Every Command **must** have a matching FluentValidation Validator.
- Queries return **DTOs (records)**, never raw entities.
- Handlers call **repository interfaces** and **IUnitOfWork** — never DbContext directly.
- Controllers are thin: inject `ISender`, call `sender.Send()`, return HTTP status.
- Use `CancellationToken` on all async methods.
- Propagate domain exceptions — `GlobalExceptionMiddleware` handles mapping.
- Test naming: `MethodName_Scenario_ExpectedResult`.
- Test pattern: Arrange-Act-Assert (AAA).

### Git & Branch Strategy

- Branch from `develop`.
- Branch naming: `feat/backend-{issue-number}-{short-name}` (e.g., `feat/backend-31-project-crud`).
- Conventional Commits: `feat:`, `fix:`, `refactor:`, `test:`, `docs:`.
- PR to `develop`, squash merge.
