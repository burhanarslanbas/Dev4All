# 21. Backend Development Roadmap

> Sprint-based development plan from current state to MVP completion.  
> Each sprint ≈ 1 week. Issues are ordered by dependency.
> **Testler her modülden hemen sonra yazılır.** Integration testleri Sprint 6'ya ertelenmez.

---

## Sprint 0 — Auth Tamamlama

**Milestone:** `Backend S0 — Full Auth`  
**Depends on:** Mevcut auth altyapısı (register/login/me implemented)

| # | Issue # | Issue Title | Type | Est. |
|---|---------|-------------|------|------|
| 1 | #108 | Auth altyapı genişletme (IIdentityService, IJwtService, RefreshToken entity) | feat | 4h |
| 2 | #109 | EmailQueue + Quartz.NET setup + auth email templates | feat | 5h |
| 3 | #110 | RefreshTokenCommand + Handler + Validator | feat | 2h |
| 4 | #111 | LogoutCommand + Handler | feat | 1.5h |
| 5 | #112 | ConfirmEmailCommand + Handler + Validator | feat | 2h |
| 6 | #113 | ForgotPasswordCommand + Handler + Validator | feat | 2h |
| 7 | #114 | ResetPasswordCommand + Handler + Validator | feat | 2h |
| 8 | #115 | ChangePasswordCommand + Handler + Validator | feat | 2h |
| 9 | #116 | ResendConfirmationCommand + Handler + Validator | feat | 1.5h |
| 10 | #117 | AuthController güncelleme (7 yeni endpoint) | feat | 2h |
| 11 | #118 | Auth Unit Tests (tüm handlers + validators) | test | 4h |
| 12 | #119 | Auth Integration Tests (replaces #62) | test | 3h |

### E-posta Mimarisi Notu

Handler içinde `IEmailService.SendAsync()` **DOĞRUDAN ÇAĞIRILMAZ**.  
Doğru akış: Handler → `IEmailNotificationService.QueueXxxEmailAsync()` → INSERT EmailQueue (Pending) → Quartz `EmailDispatchJob` (1 dk aralık) → TemplateRenderer → MailKit.

### Dependency Flow

```
S0 (Full Auth) ──► S1 (Project) ──► S2 (Bid) ──► S3 (Contract) ──► S4 (GitHub) ──► S5 (Email triggers) ──► S6/S7
```

---

## Sprint 1 — Project Module (CRUD + Listing)

**Milestone:** `Backend Sprint 1 — Project Module`  
**Depends on:** Sprint 0 (Full Auth) tamamlanmış olmalı

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 1 | `CreateProjectCommand` + Handler + Validator | feat | 3h | `Features/Projects/Commands/CreateProject/` (4 files) |
| 2 | `GetProjectsQuery` (paginated open projects) | feat | 2h | `Features/Projects/Queries/GetProjects/` (3 files) |
| 3 | `GetProjectByIdQuery` | feat | 1.5h | `Features/Projects/Queries/GetProjectById/` (3 files) |
| 4 | `GetMyProjectsQuery` (customer's own) | feat | 1.5h | `Features/Projects/Queries/GetMyProjects/` (3 files) |
| 5 | `UpdateProjectCommand` + Handler + Validator | feat | 2.5h | `Features/Projects/Commands/UpdateProject/` (4 files) |
| 6 | `DeleteProjectCommand` (soft delete) | feat | 1.5h | `Features/Projects/Commands/DeleteProject/` (3 files) |
| 7 | `ProjectsController` | feat | 2h | `Controllers/v1/ProjectsController.cs` |
| 8 | Unit tests — Project handlers & validators | test | 4h | `Dev4All.UnitTests/Features/Projects/` |

### Sprint 1 — Detailed Task Breakdown

#### Issue 1: CreateProjectCommand

**Command:** `CreateProjectCommand(string Title, string Description, decimal Budget, DateTime Deadline, DateTime BidEndDate, string? Technologies)`  
**Response:** `CreateProjectResponse(Guid Id, string Title, DateTime CreatedDate)`  
**Validator rules:**
- Title: NotEmpty, 3–100 chars
- Description: NotEmpty, 10–2000 chars
- Budget: > 0
- Deadline: > UtcNow
- BidEndDate: > UtcNow AND < Deadline

**Handler logic:**
1. Get current user via `ICurrentUser` → verify role is Customer
2. Create `Project` entity, call `SetCustomer(currentUser.UserId)`
3. Set all properties (Title, Description, Budget, Deadline, BidEndDate, Technologies)
4. `IProjectWriteRepository.AddAsync(project)`
5. `IUnitOfWork.SaveChangesAsync()`
6. Return `CreateProjectResponse`

#### Issue 2: GetProjectsQuery (paginated)

**Query:** `GetProjectsQuery(int Page, int PageSize)`  
**Response:** `GetProjectsResponse` wrapping `PagedResult<ProjectListItemDto>`  
**ProjectListItemDto:** `(Guid Id, string Title, string Description, decimal Budget, DateTime Deadline, DateTime BidEndDate, string? Technologies, int BidCount)`  
**Handler:** `IProjectReadRepository.GetOpenProjectsAsync(page, pageSize)` → map to DTO

#### Issue 3: GetProjectByIdQuery

**Query:** `GetProjectByIdQuery(Guid Id)`  
**Response:** `GetProjectByIdResponse(Guid Id, string CustomerId, string? AssignedDeveloperId, string Title, string Description, decimal Budget, DateTime Deadline, DateTime BidEndDate, string? Technologies, string Status, DateTime CreatedDate)`  
**Handler:** `IProjectReadRepository.GetByIdAsync(id)` → throw `ResourceNotFoundException` if null → map to response

#### Issue 4: GetMyProjectsQuery

**Query:** `GetMyProjectsQuery` (no params — uses `ICurrentUser`)  
**Response:** `GetMyProjectsResponse(IReadOnlyList<MyProjectDto> Projects)`  
**Handler:** verify Customer role → `IProjectReadRepository.GetByCustomerIdAsync(currentUser.UserId)`

#### Issue 5: UpdateProjectCommand

**Command:** `UpdateProjectCommand(Guid Id, string Title, string Description, decimal Budget, DateTime Deadline, DateTime BidEndDate, string? Technologies)`  
**Validator:** same rules as Create + Id must not be empty  
**Handler:**
1. Get project by Id → 404 if not found
2. Verify `project.CustomerId == currentUser.UserId` → throw `UnauthorizedDomainException` if not
3. Verify `project.Status == ProjectStatus.Open` → throw `BusinessRuleViolationException` if not
4. Update fields, `IUnitOfWork.SaveChangesAsync()`

#### Issue 6: DeleteProjectCommand

**Command:** `DeleteProjectCommand(Guid Id)`  
**Handler:**
1. Get project by Id → 404 if not found
2. Verify ownership (Customer)
3. Verify status is Open → cannot delete projects with active bids/contracts
4. `project.MarkAsDeleted()`, `IUnitOfWork.SaveChangesAsync()`

#### Issue 7: ProjectsController

```
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    GET  /             → GetProjectsQuery(page, pageSize)   → 200
    POST /             → CreateProjectCommand               → 201 + Location
    GET  /{id}         → GetProjectByIdQuery(id)            → 200
    PUT  /{id}         → UpdateProjectCommand               → 200
    DELETE /{id}       → DeleteProjectCommand(id)           → 204
    GET  /my           → GetMyProjectsQuery                 → 200
}
```

---

## Sprint 2 — Bid Module

**Milestone:** `Backend Sprint 2 — Bid Module`  
**Depends on:** Sprint 1 (Project CRUD)

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 9 | `PlaceBidCommand` + Handler + Validator | feat | 3h | `Features/Bids/Commands/PlaceBid/` (4 files) |
| 10 | `UpdateBidCommand` + Handler + Validator | feat | 2h | `Features/Bids/Commands/UpdateBid/` (4 files) |
| 11 | `AcceptBidCommand` + Handler | feat | 4h | `Features/Bids/Commands/AcceptBid/` (3 files) |
| 12 | `GetProjectBidsQuery` | feat | 1.5h | `Features/Bids/Queries/GetProjectBids/` (3 files) |
| 13 | `GetMyBidsQuery` | feat | 1.5h | `Features/Bids/Queries/GetMyBids/` (3 files) |
| 14 | `BidsController` | feat | 2h | `Controllers/v1/BidsController.cs` |
| 15 | Unit tests — Bid handlers & validators | test | 4h | `Dev4All.UnitTests/Features/Bids/` |

### Sprint 2 — Detailed Task Breakdown

#### Issue 9: PlaceBidCommand

**Command:** `PlaceBidCommand(Guid ProjectId, decimal BidAmount, string ProposalNote)`  
**Response:** `PlaceBidResponse(Guid Id, Guid ProjectId, decimal BidAmount)`  
**Validator:**
- ProjectId: NotEmpty
- BidAmount: > 0
- ProposalNote: NotEmpty, 10–1000 chars

**Handler logic:**
1. Verify current user is Developer
2. Get project → 404 if not found
3. Verify `project.Status == Open` AND `project.BidEndDate > UtcNow`
4. Verify developer hasn't already bid: `IBidReadRepository.GetByDeveloperAndProjectAsync()`
5. Create Bid entity, call `SetOwnership(projectId, currentUser.UserId)`
6. `IBidWriteRepository.AddAsync(bid)`, `IUnitOfWork.SaveChangesAsync()`

#### Issue 10: UpdateBidCommand

**Command:** `UpdateBidCommand(Guid Id, decimal BidAmount, string ProposalNote)`  
**Handler:**
1. Get bid → 404
2. Verify ownership (`bid.DeveloperId == currentUser.UserId`)
3. Verify `bid.Status == Pending`
4. Update fields, save

#### Issue 11: AcceptBidCommand (complex — key business flow)

**Command:** `AcceptBidCommand(Guid BidId)`  
**Handler (transactional):**
1. `IUnitOfWork.BeginTransactionAsync()`
2. Get bid with project → 404
3. Verify current user is project's Customer
4. Verify project status is Open
5. `bid.Accept()` — domain method
6. `project.AssignDeveloper(bid.DeveloperId)`
7. `project.MoveToAwaitingContract()`
8. Reject all other pending bids: get by projectId, loop `bid.Reject()`
9. Create new `Contract` entity: `SetProjectId()`, `SetInitialContent("", currentUser.UserId)`
10. `IContractWriteRepository.AddAsync(contract)`
11. `IUnitOfWork.SaveChangesAsync()`, `CommitTransactionAsync()`
12. (Later in Sprint 5: trigger email notifications)

#### Issue 14: BidsController

```
[ApiController]
[Route("api/v1")]
[Authorize]
public sealed class BidsController(ISender sender) : ControllerBase
{
    POST /projects/{projectId}/bids  → PlaceBidCommand      → 201
    GET  /projects/{projectId}/bids  → GetProjectBidsQuery   → 200
    PUT  /bids/{id}                  → UpdateBidCommand       → 200
    POST /bids/{id}/accept           → AcceptBidCommand       → 200
    GET  /bids/my                    → GetMyBidsQuery         → 200
}
```

---

## Sprint 3 — Contract Module

**Milestone:** `Backend Sprint 3 — Contract Module`  
**Depends on:** Sprint 2 (AcceptBid creates Contract)

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 16 | `ReviseContractCommand` + Handler + Validator | feat | 3h | `Features/Contracts/Commands/ReviseContract/` (4 files) |
| 17 | `ApproveContractCommand` + Handler | feat | 3h | `Features/Contracts/Commands/ApproveContract/` (3 files) |
| 18 | `CancelContractCommand` + Handler | feat | 2h | `Features/Contracts/Commands/CancelContract/` (3 files) |
| 19 | `GetContractQuery` | feat | 1.5h | `Features/Contracts/Queries/GetContract/` (3 files) |
| 20 | `GetContractRevisionsQuery` | feat | 1.5h | `Features/Contracts/Queries/GetContractRevisions/` (3 files) |
| 21 | `ContractsController` | feat | 2h | `Controllers/v1/ContractsController.cs` |
| 22 | Unit tests — Contract handlers & validators | test | 4h | `Dev4All.UnitTests/Features/Contracts/` |

### Sprint 3 — Detailed Task Breakdown

#### Issue 16: ReviseContractCommand

**Command:** `ReviseContractCommand(Guid ProjectId, string Content, string? RevisionNote)`  
**Validator:** Content: NotEmpty, min 50 chars; RevisionNote: max 500 chars  
**Handler:**
1. Get contract by ProjectId → 404
2. Get project → verify current user is Customer or assigned Developer
3. Determine `isCustomer = (currentUser.UserId == project.CustomerId)`
4. Create `ContractRevision.CreateSnapshot(contract.Id, currentUser.UserId, contract.Content, contract.RevisionNumber, revisionNote)`
5. `IContractRevisionWriteRepository.AddAsync(revision)`
6. `contract.Revise(content, currentUser.UserId, isCustomer)` — domain method resets other party's approval
7. `IUnitOfWork.SaveChangesAsync()`

#### Issue 17: ApproveContractCommand

**Command:** `ApproveContractCommand(Guid ProjectId)`  
**Handler:**
1. Get contract by ProjectId → 404
2. Verify party membership
3. `contract.Approve(isCustomer)` — domain method; if both approved, status → BothApproved
4. If `contract.Status == BothApproved` → `project.MoveToOngoing()`
5. `IUnitOfWork.SaveChangesAsync()`

#### Issue 18: CancelContractCommand

**Command:** `CancelContractCommand(Guid ProjectId)`  
**Handler:**
1. Get contract → 404
2. Verify party membership
3. `contract.Cancel()` — domain method
4. `project.Cancel()` — domain method
5. `IUnitOfWork.SaveChangesAsync()`

#### Issue 21: ContractsController

```
[ApiController]
[Route("api/v1/contracts")]
[Authorize]
public sealed class ContractsController(ISender sender) : ControllerBase
{
    GET  /{projectId}            → GetContractQuery              → 200
    PUT  /{projectId}            → ReviseContractCommand         → 200
    POST /{projectId}/approve    → ApproveContractCommand        → 200
    POST /{projectId}/cancel     → CancelContractCommand         → 200
    GET  /{projectId}/revisions  → GetContractRevisionsQuery     → 200
}
```

---

## Sprint 4 — GitHub Integration

**Milestone:** `Backend Sprint 4 — GitHub Integration`  
**Depends on:** Sprint 2 (project assignment)

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 23 | `GitHubService` implementation (HMAC + push parsing) | feat | 3h | `Infrastructure/GitHub/GitHubService.cs`, `InfrastructureServiceRegistration.cs` |
| 24 | `LinkGitHubRepoCommand` + Handler + Validator | feat | 2.5h | `Features/GitHub/Commands/LinkGitHubRepo/` (4 files) |
| 25 | `WebhookController` — GitHub push event receiver | feat | 3h | `Controllers/v1/WebhookController.cs`, `Options/GitHubWebhookOptions.cs` |
| 26 | `GetGitHubLogsByProjectQuery` | feat | 1.5h | `Features/GitHub/Queries/GetGitHubLogsByProject/` (3 files) |
| 27 | Unit tests — GitHub handlers & service | test | 3h | `Dev4All.UnitTests/Features/GitHub/`, `Dev4All.UnitTests/Infrastructure/` |

### Sprint 4 — Detailed Task Breakdown

#### Issue 23: GitHubService implementation

**File:** `backend/src/Infrastructure/Dev4All.Infrastructure/GitHub/GitHubService.cs`  
**Implements:** `IGitHubService` (already defined in Application)

**`ValidateWebhookSignature`:**
1. Compute HMAC-SHA256 of payload using secret
2. Compare with signature header using `CryptographicOperations.FixedTimeEquals` (timing-safe)

**`ParsePushEvent`:**
1. Deserialize JSON payload (use `System.Text.Json`)
2. Extract `ref` → branch name (strip `refs/heads/`)
3. Extract `repository.html_url` → repoUrl
4. Loop `commits[]` → create `GitHubLog.Create(projectId, repoUrl, branch, commitHash, commitMessage, authorName, pushedAt)`

#### Issue 24: LinkGitHubRepoCommand

**Command:** `LinkGitHubRepoCommand(Guid ProjectId, string RepoUrl, string? Branch)`  
**Validator:** RepoUrl: NotEmpty, must contain `github.com`; Branch defaults to "main"  
**Handler:**
1. Get project → 404
2. Verify current user is assigned Developer
3. Verify project status is Ongoing
4. Create initial `GitHubLog` entry (repo link record)
5. Save

#### Issue 25: WebhookController

**NOT a standard `[Authorize]` endpoint** — uses HMAC-SHA256 validation.

**Endpoint:** `POST /api/v1/webhooks/github`  
**Logic:**
1. Read raw request body
2. Get `X-Hub-Signature-256` header
3. Call `IGitHubService.ValidateWebhookSignature()` → 401 if invalid
4. Call `IGitHubService.ParsePushEvent()` → list of `GitHubLog`
5. `IGitHubLogWriteRepository.AddRangeAsync(logs)`
6. `IUnitOfWork.SaveChangesAsync()`
7. Return 200

---

## Sprint 5 — Email Notifications & Background Jobs

**Milestone:** `Backend Sprint 5 — Email & Jobs`  
**Depends on:** Sprint 2 & 3 (business events)

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 28 | Email HTML templates | feat | 2h | `Infrastructure/Templates/*.html` |
| 29 | Quartz.NET integration + EmailDispatchJob | feat | 4h | `Infrastructure/Jobs/`, NuGet packages, `InfrastructureServiceRegistration.cs`, `Program.cs` |
| 30 | Email notification triggers in handlers | feat | 3h | Modify existing handlers (AcceptBid, PlaceBid, LinkRepo) |
| 31 | Unit tests — email dispatch & templates | test | 2h | `Dev4All.UnitTests/Infrastructure/Email/` |

### Sprint 5 — Detailed Task Breakdown

#### Issue 28: Email Templates

Create HTML templates in `Infrastructure/Templates/`:
- `welcome.html` (MAIL-01: registration welcome)
- `new-bid.html` (MAIL-02: new bid notification to Customer)
- `bid-accepted.html` (MAIL-03: bid accepted notification to Developer)
- `bid-rejected.html` (MAIL-04: bid rejected notification to Developer)
- `repo-linked.html` (MAIL-05: repo linked notification to Customer)

Use `{{PlaceholderName}}` syntax for variable substitution.

#### Issue 29: Quartz.NET + EmailDispatchJob

1. Add `Quartz` and `Quartz.Extensions.Hosting` NuGet packages
2. Create `EmailQueue` entity (or use a simple in-memory/DB queue)
3. Create `EmailDispatchJob : IJob` that:
   - Queries pending emails
   - Sends via `IEmailService`
   - Updates status (Sent/Failed with retry count)
4. Register Quartz in `InfrastructureServiceRegistration` with 1-minute trigger

#### Issue 30: Email triggers

Add email-sending calls (or queue entries) to:
- `RegisterUserCommandHandler` → welcome email
- `PlaceBidCommandHandler` → notify project Customer
- `AcceptBidCommandHandler` → notify accepted Developer + rejected Developers
- `LinkGitHubRepoCommandHandler` → notify Customer

---

## Sprint 6 — Testing & Quality

**Milestone:** `Backend Sprint 6 — Testing & Quality`  
**Depends on:** All feature sprints

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 32 | Integration tests — Auth endpoints | test | 3h | `Dev4All.IntegrationTests/Auth/` |
| 33 | Integration tests — Project endpoints | test | 3h | `Dev4All.IntegrationTests/Projects/` |
| 34 | Integration tests — Bid + Contract endpoints | test | 4h | `Dev4All.IntegrationTests/Bids/`, `Contracts/` |
| 35 | Serilog structured logging | feat | 2h | NuGet packages, `Program.cs`, `appsettings.json` |
| 36 | Code coverage report & gap analysis | test | 2h | CI workflow update, coverlet config |

### Sprint 6 — Details

#### Issues 32–34: Integration Tests

Use `WebApplicationFactory<Program>` with in-memory PostgreSQL (or test containers):
1. Set up test database with seeded roles
2. Register a user, get JWT token
3. Call each endpoint with valid/invalid data
4. Verify HTTP status codes and response bodies
5. Test authorization (wrong role, no token, expired token)

Test packages to add to `Dev4All.IntegrationTests.csproj`:
- `Microsoft.AspNetCore.Mvc.Testing`
- `Testcontainers.PostgreSql` (or `Microsoft.EntityFrameworkCore.InMemory` for quick tests)

#### Issue 35: Serilog

1. Add NuGet: `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`
2. Configure in `Program.cs` via `UseSerilog()`
3. Add structured logging to key handlers (request/response logging)
4. Configure `appsettings.json` for log levels and file paths

---

## Sprint 7 — Polish & Documentation

**Milestone:** `Backend Sprint 7 — Polish & Docs`  
**Depends on:** All previous sprints

| # | Issue Title | Type | Est. | Files to Create/Modify |
|---|-------------|------|------|----------------------|
| 37 | Swagger/OpenAPI documentation improvements | docs | 2h | Controller XML comments, `Program.cs` |
| 38 | Performance review — N+1 queries, projections | refactor | 3h | Repository queries |
| 39 | API versioning consistency audit | refactor | 1.5h | Controllers, route templates |
| 40 | Backend README update | docs | 1h | `backend/README.md` |

---

## Dependency Graph

```
S0 (Auth ✅) ──► S1 (Project) ──► S2 (Bid) ──► S3 (Contract)
                                      │
                                      └──► S4 (GitHub)
                                      
S2 + S3 ──► S5 (Email/Jobs)

S1–S5 ──► S6 (Testing)
S1–S6 ──► S7 (Polish)
```

## Risk & Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| AcceptBid transaction complexity | High | Use explicit `IUnitOfWork` transaction; write thorough unit + integration tests |
| GitHub webhook security | High | Timing-safe HMAC comparison; validate all payloads before DB writes |
| Email queue reliability | Medium | Quartz retry logic; max 3 retries with exponential backoff; `Failed` status logging |
| N+1 query performance | Medium | Use `.Include()` judiciously; prefer projection (`Select`) for list queries |
| Test coverage gap | Medium | Mandate tests per issue; CI enforces coverage threshold |
