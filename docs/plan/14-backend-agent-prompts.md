# Dev4All Backend — All Issue Agent Prompts

Bu dosya, GitHub'daki açık backend issue'lar için kolayca kopyalanıp agent'a verilebilecek hazır prompt'ları içerir.

## Kullanım

1. GitHub issue numarasını seç.
2. Aşağıdaki ilgili prompt'u kopyala.
3. Agent ekranında:
   - Base branch: `develop`
   - Model: `GPT-5.3-Codex` (önerilen)
4. Prompt'u yapıştırıp çalıştır.

## Önerilen Uygulama Sırası (Dependency-Aware)

Bu sıra, issue numarasına değil bağımlılıklara göre optimize edilmiştir.

1. `#31` — CreateProjectCommand (S1)
2. `#32` — GetProjectsQuery (S1)
3. `#33` — GetProjectByIdQuery (S1)
4. `#34` — GetMyProjectsQuery (S1)
5. `#35` — UpdateProjectCommand (S1)
6. `#36` — DeleteProjectCommand (S1)
7. `#37` — ProjectsController (S1)
8. `#38` — Unit tests — Project (S1)
9. `#39` — PlaceBidCommand (S2)
10. `#40` — UpdateBidCommand (S2)
11. `#41` — AcceptBidCommand (S2)
12. `#42` — GetProjectBidsQuery (S2)
13. `#43` — GetMyBidsQuery (S2)
14. `#44` — BidsController (S2)
15. `#45` — Unit tests — Bid (S2)
16. `#46` — ReviseContractCommand (S3)
17. `#47` — ApproveContractCommand (S3)
18. `#48` — CancelContractCommand (S3)
19. `#49` — GetContractQuery (S3)
20. `#50` — GetContractRevisionsQuery (S3)
21. `#51` — ContractsController (S3)
22. `#52` — Unit tests — Contract (S3)
23. `#53` — GitHubService HMAC + push parsing (S4)
24. `#54` — LinkGitHubRepoCommand (S4)
25. `#55` — WebhookController (S4)
26. `#56` — GetGitHubLogsByProjectQuery (S4)
27. `#57` — Unit tests — GitHub (S4)
28. `#58` — Email HTML templates (S5)
29. `#61` — Quartz.NET + EmailDispatchJob (S5)
30. `#60` — Email notification triggers (S5)
31. `#59` — Unit tests — Email (S5)
32. `#62` — Integration tests — Auth (S6)
33. `#63` — Integration tests — Project (S6)
34. `#64` — Integration tests — Bid + Contract (S6)
35. `#65` — Serilog structured logging (S6)
36. `#66` — Code coverage report (S6)
37. `#67` — Swagger documentation (S7)
38. `#68` — Performance review (S7)
39. `#69` — API versioning audit (S7)
40. `#70` — Backend README update (S7)

---

## Global Prompt Header (Her issue için üstte kalsın)

```md
You are a senior .NET backend developer working in repository `Dev4All`.

General constraints:
- Follow `docs/AGENTS.md` and `backend/plan/*.md`.
- Keep changes scoped to this issue only.
- Use Conventional Commits.
- Open PR to `develop`.
- Include `Closes #<ISSUE_NUMBER>` in PR body.
- Run and report:
  - `dotnet build backend/Dev4All.slnx`
  - `dotnet test backend/tests/Dev4All.UnitTests/` (if test project exists)

Output format required:
1) Summary
2) Files changed
3) Validation steps + results
4) Known blockers (if any)
```

---

## Sprint 1 — Project Module

---

## #31 — feat: implement CreateProjectCommand + Handler + Validator

```md
<GLOBAL HEADER>

Issue: #31
Goal:
- Implement the CreateProjectCommand CQRS feature following existing Auth pattern.

Tasks:
1) Create files under `backend/src/Core/Dev4All.Application/Features/Projects/Commands/CreateProject/`:
   - CreateProjectCommand.cs (IRequest<CreateProjectResponse>)
   - CreateProjectResponse.cs (Guid Id, string Title, DateTime CreatedDate)
   - CreateProjectCommandValidator.cs (Title 3-100, Description 10-2000, Budget>0, Deadline>UtcNow, BidEndDate>UtcNow AND <Deadline)
   - CreateProjectCommandHandler.cs
2) Handler logic: verify Customer role → create Project entity → SetCustomer → save → return response.
3) Inject: ICurrentUser, IProjectWriteRepository, IUnitOfWork.

Key files:
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectWriteRepository.cs`
- `backend/src/Core/Dev4All.Application/Features/Auth/Commands/RegisterUser/` (reference pattern)
- `docs/analyse/02-frd.md` (Section 8.2)

Constraints:
- sealed types, file-scoped namespaces, CancellationToken propagation.
- No DbContext or infrastructure references in Application layer.
```

---

## #32 — feat: implement GetProjectsQuery (paginated open projects)

```md
<GLOBAL HEADER>

Issue: #32
Goal:
- Implement paginated open project listing query.

Tasks:
1) Create files under `backend/src/Core/Dev4All.Application/Features/Projects/Queries/GetProjects/`:
   - GetProjectsQuery.cs (int Page=1, int PageSize=10)
   - GetProjectsResponse.cs (Items list, TotalCount, Page, PageSize) + ProjectListItemDto
   - GetProjectsQueryHandler.cs
2) Handler: call IProjectReadRepository.GetOpenProjectsAsync(page, pageSize) → map to DTOs.
3) Map project.Status.ToString(), project.Bids.Count → BidCount.

Key files:
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`

Constraints:
- Queries have no validator (no side effects).
- Return DTOs (records), never raw entities.
```

---

## #33 — feat: implement GetProjectByIdQuery

```md
<GLOBAL HEADER>

Issue: #33
Goal:
- Implement single project detail query.

Tasks:
1) Create files under `.../Features/Projects/Queries/GetProjectById/`:
   - GetProjectByIdQuery.cs (Guid Id)
   - GetProjectByIdResponse.cs (full project detail DTO)
   - GetProjectByIdQueryHandler.cs
2) Handler: GetByIdAsync → throw ResourceNotFoundException if null → map to DTO.

Key files:
- `backend/src/Core/Dev4All.Domain/Exceptions/ResourceNotFoundException.cs`

Constraints:
- ResourceNotFoundException → middleware maps to 404.
```

---

## #34 — feat: implement GetMyProjectsQuery (customer's own projects)

```md
<GLOBAL HEADER>

Issue: #34
Goal:
- Implement query returning current Customer's projects.

Tasks:
1) Create files under `.../Features/Projects/Queries/GetMyProjects/`:
   - GetMyProjectsQuery.cs (no parameters, uses ICurrentUser)
   - GetMyProjectsResponse.cs (IReadOnlyList<MyProjectDto>)
   - GetMyProjectsQueryHandler.cs
2) Handler: verify Customer role → GetByCustomerIdAsync(currentUser.UserId) → map to DTOs.

Key files:
- `backend/src/Core/Dev4All.Application/Abstractions/Auth/ICurrentUser.cs`
```

---

## #35 — feat: implement UpdateProjectCommand + Handler + Validator

```md
<GLOBAL HEADER>

Issue: #35
Goal:
- Implement project update with ownership and status guards.

Tasks:
1) Create files under `.../Features/Projects/Commands/UpdateProject/`:
   - UpdateProjectCommand.cs (Guid Id, Title, Description, Budget, Deadline, BidEndDate, Technologies)
   - UpdateProjectCommandValidator.cs (same rules as Create + Id NotEmpty)
   - UpdateProjectCommandHandler.cs
2) Handler logic: get project → verify ownership (CustomerId == currentUser) → verify Open status → update fields → MarkAsUpdated → save.

Constraints:
- Ownership check before mutation.
- Status guard: only Open projects can be updated.
```

---

## #36 — feat: implement DeleteProjectCommand (soft delete)

```md
<GLOBAL HEADER>

Issue: #36
Goal:
- Implement soft delete for projects.

Tasks:
1) Create files under `.../Features/Projects/Commands/DeleteProject/`:
   - DeleteProjectCommand.cs (Guid Id)
   - DeleteProjectCommandValidator.cs (Id NotEmpty)
   - DeleteProjectCommandHandler.cs
2) Handler: get → verify ownership → verify Open status → MarkAsDeleted() → save.

Key files:
- `backend/src/Core/Dev4All.Domain/Common/BaseEntity.cs` (MarkAsDeleted)

Constraints:
- Soft delete only — EF global query filter on DeletedDate handles the rest.
```

---

## #37 — feat: implement ProjectsController

```md
<GLOBAL HEADER>

Issue: #37
Goal:
- Create thin MVC controller delegating to MediatR for all Project endpoints.

Tasks:
1) Create `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/ProjectsController.cs`.
2) Endpoints:
   - GET / → GetProjectsQuery (page, pageSize from query) → 200
   - POST / → CreateProjectCommand → 201 + CreatedAtAction
   - GET /{id:guid} → GetProjectByIdQuery → 200
   - PUT /{id:guid} → UpdateProjectCommand → 200
   - DELETE /{id:guid} → DeleteProjectCommand → 204
   - GET /my → GetMyProjectsQuery → 200

Key files:
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs` (reference pattern)

Constraints:
- [Authorize] on class level.
- Thin controller: no business logic.
- XML summary comments for Swagger.
```

---

## #38 — test: unit tests for Project module handlers and validators

```md
<GLOBAL HEADER>

Issue: #38
Goal:
- Full unit test coverage for Project CQRS handlers and validators.

Tasks:
1) Add NSubstitute, FluentAssertions, Bogus to Dev4All.UnitTests.csproj (if missing).
2) Create test files under `backend/tests/Dev4All.UnitTests/Features/Projects/`:
   - CreateProjectCommandHandlerTests.cs (valid creates, non-customer throws)
   - CreateProjectCommandValidatorTests.cs (all field validations)
   - UpdateProjectCommandHandlerTests.cs (valid, not found, not owner, non-open)
   - DeleteProjectCommandHandlerTests.cs (valid soft delete, not owner)
   - GetProjectByIdQueryHandlerTests.cs (exists → DTO, missing → 404)
   - GetProjectsQueryHandlerTests.cs (returns paged list)
3) Pattern: AAA, NSubstitute mocks, FluentAssertions.

Constraints:
- All tests must pass: `dotnet test backend/tests/Dev4All.UnitTests/`
```

---

## Sprint 2 — Bid Module

---

## #39 — feat: implement PlaceBidCommand + Handler + Validator

```md
<GLOBAL HEADER>

Issue: #39
Goal:
- Implement bid placement with multiple business rule checks.

Tasks:
1) Create files under `.../Features/Bids/Commands/PlaceBid/`:
   - PlaceBidCommand.cs (Guid ProjectId, decimal BidAmount, string ProposalNote)
   - PlaceBidResponse.cs
   - PlaceBidCommandValidator.cs (BidAmount>0, ProposalNote 10-1000)
   - PlaceBidCommandHandler.cs
2) Handler logic:
   a. Verify Developer role
   b. Get project → verify Open status
   c. Verify BidEndDate > UtcNow
   d. Check duplicate bid (GetByDeveloperAndProjectAsync)
   e. Create Bid → SetOwnership → save

Key files:
- `backend/src/Core/Dev4All.Domain/Entities/Bid.cs`
- `docs/analyse/02-frd.md` (Section 8.3)

Constraints:
- Multiple business rules checked before mutation.
```

---

## #40 — feat: implement UpdateBidCommand + Handler + Validator

```md
<GLOBAL HEADER>

Issue: #40
Goal:
- Implement bid update (amount + proposal) for Pending bids only.

Tasks:
1) Create files under `.../Features/Bids/Commands/UpdateBid/`:
   - UpdateBidCommand.cs (Guid Id, decimal BidAmount, string ProposalNote)
   - UpdateBidCommandValidator.cs
   - UpdateBidCommandHandler.cs
2) Handler: get bid → verify ownership (DeveloperId) → verify Pending status → update → MarkAsUpdated → save.
```

---

## #41 — feat: implement AcceptBidCommand (transactional)

```md
<GLOBAL HEADER>

Issue: #41 — CRITICAL: Most complex handler in the system.
Goal:
- Implement transactional bid acceptance: accept bid, reject others, create contract.

Tasks:
1) Create files under `.../Features/Bids/Commands/AcceptBid/`:
   - AcceptBidCommand.cs (Guid BidId)
   - AcceptBidResponse.cs (ProjectId, AcceptedBidId, ContractId)
   - AcceptBidCommandHandler.cs
2) Handler logic (inside transaction):
   a. BeginTransaction
   b. Get bid → get project with bids loaded
   c. Verify customer ownership + Open status
   d. bid.Accept() → project.AssignDeveloper() → project.MoveToAwaitingContract()
   e. Reject all other pending bids
   f. Create Contract (SetProjectId, SetInitialContent)
   g. SaveChanges → CommitTransaction
   h. Rollback on failure

Key files:
- `backend/src/Core/Dev4All.Domain/Entities/Bid.cs` (Accept, Reject)
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs` (AssignDeveloper, MoveToAwaitingContract)
- `backend/src/Core/Dev4All.Domain/Entities/Contract.cs` (SetProjectId, SetInitialContent)
- `docs/analyse/02-frd.md` (UC-03)

Constraints:
- MUST be transactional — all or nothing.
- Use domain methods for state transitions.
```

---

## #42 — feat: implement GetProjectBidsQuery

```md
<GLOBAL HEADER>

Issue: #42
Goal:
- Query returning all bids for a specific project (Customer owner only).

Tasks:
1) Create files under `.../Features/Bids/Queries/GetProjectBids/`:
   - GetProjectBidsQuery.cs (Guid ProjectId)
   - GetProjectBidsResponse.cs (IReadOnlyList<BidDto>)
   - GetProjectBidsQueryHandler.cs
2) Handler: verify project exists + current user is project Customer → get bids → map.

Constraints:
- Only the project Customer can see all bids.
```

---

## #43 — feat: implement GetMyBidsQuery (developer's bids)

```md
<GLOBAL HEADER>

Issue: #43
Goal:
- Query returning current Developer's bids.

Tasks:
1) Create files under `.../Features/Bids/Queries/GetMyBids/`:
   - GetMyBidsQuery.cs
   - GetMyBidsResponse.cs (IReadOnlyList<MyBidDto>)
   - GetMyBidsQueryHandler.cs
2) Handler: verify Developer role → GetByDeveloperIdAsync → map.
```

---

## #44 — feat: implement BidsController

```md
<GLOBAL HEADER>

Issue: #44
Goal:
- Create thin controller for all Bid endpoints.

Tasks:
1) Create `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/BidsController.cs`.
2) Endpoints:
   - POST /api/v1/projects/{projectId}/bids → PlaceBidCommand → 201
   - GET /api/v1/projects/{projectId}/bids → GetProjectBidsQuery → 200
   - PUT /api/v1/bids/{id} → UpdateBidCommand → 200
   - POST /api/v1/bids/{id}/accept → AcceptBidCommand → 200
   - GET /api/v1/bids/my → GetMyBidsQuery → 200

Constraints:
- Two route prefixes (nested under projects + standalone).
- Inject projectId from route into commands.
```

---

## #45 — test: unit tests for Bid module handlers and validators

```md
<GLOBAL HEADER>

Issue: #45
Goal:
- Full unit test coverage for Bid handlers and validators.

Tasks:
1) Create test files under `backend/tests/Dev4All.UnitTests/Features/Bids/`:
   - PlaceBidCommandHandlerTests.cs (valid, non-dev, closed project, expired bid, duplicate)
   - PlaceBidCommandValidatorTests.cs
   - AcceptBidCommandHandlerTests.cs (valid, not owner, non-open, rejects others)
   - UpdateBidCommandHandlerTests.cs (valid, not owner, non-pending)
2) For AcceptBid: verify transaction methods called, all domain transitions.

Constraints:
- All tests must pass.
```

---

## Sprint 3 — Contract Module

---

## #46 — feat: implement ReviseContractCommand + Handler + Validator

```md
<GLOBAL HEADER>

Issue: #46
Goal:
- Implement contract revision with snapshot creation and approval reset.

Tasks:
1) Create files under `.../Features/Contracts/Commands/ReviseContract/`:
   - ReviseContractCommand.cs (Guid ProjectId, string Content, string? RevisionNote)
   - ReviseContractCommandValidator.cs (Content min 50, RevisionNote max 500)
   - ReviseContractCommandHandler.cs
2) Handler:
   a. Get contract by ProjectId → verify party membership
   b. Snapshot OLD content → ContractRevision.CreateSnapshot → save revision
   c. contract.Revise(content, userId, isCustomer) → resets other party's approval
   d. Save

Key files:
- `backend/src/Core/Dev4All.Domain/Entities/Contract.cs` (Revise)
- `backend/src/Core/Dev4All.Domain/Entities/ContractRevision.cs` (CreateSnapshot)

Constraints:
- Save snapshot BEFORE applying new content.
```

---

## #47 — feat: implement ApproveContractCommand + Handler

```md
<GLOBAL HEADER>

Issue: #47
Goal:
- Implement contract approval with dual-party logic.

Tasks:
1) Create files under `.../Features/Contracts/Commands/ApproveContract/`:
   - ApproveContractCommand.cs (Guid ProjectId)
   - ApproveContractResponse.cs (bool IsFullyApproved, string ContractStatus)
   - ApproveContractCommandHandler.cs
2) Handler:
   a. Get contract → verify party → contract.Approve(isCustomer)
   b. If BothApproved → project.MoveToOngoing()
   c. Return IsFullyApproved flag

Constraints:
- First party → flag only. Second party → BothApproved + project Ongoing.
```

---

## #48 — feat: implement CancelContractCommand + Handler

```md
<GLOBAL HEADER>

Issue: #48
Goal:
- Implement contract cancellation (also cancels the project).

Tasks:
1) Create files under `.../Features/Contracts/Commands/CancelContract/`:
   - CancelContractCommand.cs (Guid ProjectId)
   - CancelContractCommandHandler.cs
2) Handler: get contract → verify party → contract.Cancel() → project.Cancel() → save.

Constraints:
- Domain methods enforce state guards.
```

---

## #49 — feat: implement GetContractQuery

```md
<GLOBAL HEADER>

Issue: #49
Goal:
- Query returning contract details for a project.

Tasks:
1) Create files under `.../Features/Contracts/Queries/GetContract/`:
   - GetContractQuery.cs (Guid ProjectId)
   - GetContractResponse.cs (full contract DTO with approval flags)
   - GetContractQueryHandler.cs
2) Handler: get by ProjectId → verify party → map to DTO.
```

---

## #50 — feat: implement GetContractRevisionsQuery

```md
<GLOBAL HEADER>

Issue: #50
Goal:
- Query returning contract revision history.

Tasks:
1) Create files under `.../Features/Contracts/Queries/GetContractRevisions/`:
   - GetContractRevisionsQuery.cs (Guid ProjectId)
   - GetContractRevisionsResponse.cs (IReadOnlyList<ContractRevisionDto>)
   - GetContractRevisionsQueryHandler.cs
2) Handler: get contract → verify party → GetByContractIdAsync → map, order by RevisionNumber desc.
```

---

## #51 — feat: implement ContractsController

```md
<GLOBAL HEADER>

Issue: #51
Goal:
- Create thin controller for Contract endpoints.

Tasks:
1) Create `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/ContractsController.cs`.
2) Endpoints:
   - GET /api/v1/contracts/{projectId} → GetContractQuery → 200
   - PUT /api/v1/contracts/{projectId} → ReviseContractCommand → 200
   - POST /api/v1/contracts/{projectId}/approve → ApproveContractCommand → 200
   - POST /api/v1/contracts/{projectId}/cancel → CancelContractCommand → 200
   - GET /api/v1/contracts/{projectId}/revisions → GetContractRevisionsQuery → 200
```

---

## #52 — test: unit tests for Contract module handlers and validators

```md
<GLOBAL HEADER>

Issue: #52
Goal:
- Full unit test coverage for Contract handlers.

Tasks:
1) Create test files under `backend/tests/Dev4All.UnitTests/Features/Contracts/`:
   - ReviseContractCommandHandlerTests.cs (valid, non-party, approved contract)
   - ApproveContractCommandHandlerTests.cs (first approval, both approved)
   - CancelContractCommandHandlerTests.cs (valid cancel)
   - ReviseContractCommandValidatorTests.cs
2) Verify all transitions and approval reset logic.
```

---

## Sprint 4 — GitHub Integration

---

## #53 — feat: implement GitHubService (HMAC validation + push event parsing)

```md
<GLOBAL HEADER>

Issue: #53
Goal:
- Implement Infrastructure-level GitHub webhook service.

Tasks:
1) Create `backend/src/Infrastructure/Dev4All.Infrastructure/GitHub/GitHubService.cs`.
2) Implement:
   - ValidateWebhookSignature(payload, signature, secret) → HMAC-SHA256 + FixedTimeEquals
   - ParsePushEvent(jsonPayload, projectId) → List<GitHubLog>
3) Register in InfrastructureServiceRegistration.

Key files:
- `backend/src/Core/Dev4All.Application/Abstractions/Services/IGitHubService.cs`
- `backend/src/Core/Dev4All.Domain/Entities/GitHubLog.cs`
- `docs/analyse/05-integration.md`

Constraints:
- Use CryptographicOperations.FixedTimeEquals (timing-safe comparison).
- Use System.Text.Json for payload parsing.
```

---

## #54 — feat: implement LinkGitHubRepoCommand + Handler + Validator

```md
<GLOBAL HEADER>

Issue: #54
Goal:
- Implement command for Developer to link GitHub repository to a project.

Tasks:
1) Create files under `.../Features/GitHub/Commands/LinkGitHubRepo/`:
   - LinkGitHubRepoCommand.cs (Guid ProjectId, string RepoUrl, string? Branch)
   - LinkGitHubRepoCommandValidator.cs (RepoUrl must contain github.com)
   - LinkGitHubRepoCommandHandler.cs
2) Handler: verify assigned Developer + Ongoing status → create initial GitHubLog → save.

Constraints:
- Only the assigned Developer can link a repo.
- Project must be Ongoing.
```

---

## #55 — feat: implement WebhookController (GitHub push event receiver)

```md
<GLOBAL HEADER>

Issue: #55
Goal:
- Create webhook endpoint that receives GitHub push events.

Tasks:
1) Create GitHubWebhookOptions in Application/Options/.
2) Create `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/WebhookController.cs`.
3) POST /api/v1/webhooks/github (NO [Authorize], uses HMAC):
   a. Read raw body → get X-Hub-Signature-256 header
   b. Validate signature → 401 if invalid
   c. Parse push event → save GitHubLogs → 200
4) Register GitHubWebhookOptions in Program.cs + appsettings.json.

Key files:
- `docs/analyse/05-integration.md`

Constraints:
- Public endpoint protected by HMAC, not JWT.
```

---

## #56 — feat: implement GetGitHubLogsByProjectQuery

```md
<GLOBAL HEADER>

Issue: #56
Goal:
- Query returning GitHub commit history for a project.

Tasks:
1) Create files under `.../Features/GitHub/Queries/GetGitHubLogsByProject/`.
2) Handler: verify project + party membership → GetByProjectIdAsync → map, order by PushedAt desc.
3) Add endpoint: GET /api/v1/projects/{id}/github-logs in ProjectsController (or a new controller).
```

---

## #57 — test: unit tests for GitHub module handlers and service

```md
<GLOBAL HEADER>

Issue: #57
Goal:
- Unit tests for GitHubService and GitHub handlers.

Tasks:
1) Create `backend/tests/Dev4All.UnitTests/Infrastructure/GitHubServiceTests.cs`:
   - ValidateSignature valid/invalid, ParsePushEvent single/multiple commits.
2) Create `backend/tests/Dev4All.UnitTests/Features/GitHub/LinkGitHubRepoCommandHandlerTests.cs`:
   - Valid link, not assigned dev, non-ongoing project.
3) Add project reference to Dev4All.Infrastructure in test project.
```

---

## Sprint 5 — Email & Background Jobs

---

## #58 — feat: email HTML templates

```md
<GLOBAL HEADER>

Issue: #58
Goal:
- Create responsive HTML email templates and a template renderer.

Tasks:
1) Create templates in `backend/src/Infrastructure/Dev4All.Infrastructure/Templates/`:
   - welcome.html, new-bid.html, bid-accepted.html, bid-rejected.html, repo-linked.html
2) Create TemplateRenderer.cs in Email/ — reads template, replaces {{placeholders}}.
3) Use Dev4All blue branding (#2563EB), responsive layout, no external images.

Key files:
- `docs/analyse/05-integration.md` (email matrix)
```

---

## #61 — feat: Quartz.NET integration + EmailDispatchJob

```md
<GLOBAL HEADER>

Issue: #61
Goal:
- Add Quartz.NET for async email dispatch.

Tasks:
1) Add Quartz, Quartz.Extensions.Hosting NuGet packages.
2) Create EmailQueue entity (Domain) + DbSet/Config/Repo/Migration (Persistence).
3) Create EmailDispatchJob.cs (IJob) in Infrastructure/Jobs/.
4) Register Quartz in DI with 1-minute interval trigger.

Constraints:
- Email sending must NOT block API responses.
- Retry up to 3 times, then mark as Failed.
```

---

## #60 — feat: email notification triggers in handlers

```md
<GLOBAL HEADER>

Issue: #60
Goal:
- Wire email notifications into existing command handlers.

Tasks:
1) Create IEmailNotificationService abstraction in Application/Abstractions/Services/.
2) Implement EmailNotificationService in Infrastructure (uses TemplateRenderer + queue).
3) Modify handlers:
   - RegisterUser → welcome email
   - PlaceBid → new-bid to Customer
   - AcceptBid → accepted to winner, rejected to others
   - LinkGitHubRepo → repo-linked to Customer

Constraints:
- Email calls must not fail the main transaction (wrap in try/catch, log errors).
```

---

## #59 — test: unit tests for email dispatch and templates

```md
<GLOBAL HEADER>

Issue: #59
Goal:
- Unit tests for TemplateRenderer and EmailNotificationService.

Tasks:
1) TemplateRendererTests: valid replacement, missing placeholder leaves as-is.
2) EmailNotificationServiceTests: verify correct template + variables for each type.
```

---

## Sprint 6 — Testing & Quality

---

## #62 — test: integration tests for Auth endpoints

```md
<GLOBAL HEADER>

Issue: #62
Goal:
- Set up integration test infra + Auth endpoint tests.

Tasks:
1) Add NuGet: Mvc.Testing, FluentAssertions, Testcontainers.PostgreSql (or InMemory).
2) Create CustomWebApplicationFactory (replace DB, seed roles, helper methods).
3) AuthEndpointTests: register 201, duplicate 400, login valid/invalid, me with/without token.

Constraints:
- `dotnet test backend/tests/Dev4All.IntegrationTests/`
```

---

## #63 — test: integration tests for Project endpoints

```md
<GLOBAL HEADER>

Issue: #63
Goal:
- Integration tests for all Project endpoints.

Tasks:
1) ProjectEndpointTests:
   - CreateProject as Customer/Developer (201/403)
   - GetProjects returns paged list
   - GetById existing/non-existent (200/404)
   - Update as owner/non-owner (200/403)
   - Delete as owner (204)
   - GetMyProjects returns own projects
```

---

## #64 — test: integration tests for Bid + Contract endpoints

```md
<GLOBAL HEADER>

Issue: #64
Goal:
- Integration tests for Bid and Contract workflows.

Tasks:
1) BidEndpointTests: PlaceBid as Dev/Customer, duplicate bid, AcceptBid creates contract.
2) ContractEndpointTests: Revise as party, both approve → Ongoing, cancel → project cancelled.
```

---

## #65 — feat: Serilog structured logging

```md
<GLOBAL HEADER>

Issue: #65
Goal:
- Add Serilog with console + file sinks.

Tasks:
1) Add Serilog.AspNetCore, Sinks.Console, Sinks.File NuGet.
2) Configure in Program.cs: UseSerilog + ReadFrom.Configuration.
3) Add Serilog section to appsettings.json (Info default, Warning for Microsoft).
4) Add app.UseSerilogRequestLogging().
5) Update GlobalExceptionMiddleware to use structured logging.
```

---

## #66 — test: code coverage report and gap analysis

```md
<GLOBAL HEADER>

Issue: #66
Goal:
- Configure code coverage collection and enforce 80% threshold.

Tasks:
1) Ensure coverlet.collector in both test projects.
2) Run: dotnet test --collect:"XPlat Code Coverage".
3) Add coverage report generation to CI workflow.
4) Document gaps, create follow-up issues if needed.
```

---

## Sprint 7 — Polish & Docs

---

## #67 — docs: Swagger/OpenAPI documentation improvements

```md
<GLOBAL HEADER>

Issue: #67
Goal:
- Improve Swagger documentation for all endpoints.

Tasks:
1) Enable XML doc generation (<GenerateDocumentationFile>true).
2) Configure Swagger to include XML comments.
3) Add [ProducesResponseType] to all controller actions.
4) Group endpoints by tag, add example values.
```

---

## #68 — refactor: performance review (N+1, projections, indexes)

```md
<GLOBAL HEADER>

Issue: #68
Goal:
- Audit and optimize query performance.

Tasks:
1) Check all repos for N+1 problems.
2) Ensure .AsNoTracking() on read queries.
3) Review Include() usage — only load what's needed.
4) Verify server-side pagination (not in-memory).
5) Add indexes if query patterns suggest them + migration.
```

---

## #69 — refactor: API versioning consistency audit

```md
<GLOBAL HEADER>

Issue: #69
Goal:
- Ensure consistent versioning and RESTful naming.

Tasks:
1) Verify all controllers use api/v1/ prefix.
2) Check route consistency (plural nouns, RESTful).
3) Verify WebhookController follows same convention.
4) Document versioning strategy in README.
```

---

## #70 — docs: Backend README update

```md
<GLOBAL HEADER>

Issue: #70
Goal:
- Comprehensive backend README.

Tasks:
1) Project overview + architecture diagram.
2) Prerequisites (.NET 10, PostgreSQL, user-secrets).
3) Getting started (clone, restore, migrate, run).
4) API endpoint summary table.
5) Testing instructions.
6) Configuration reference.
```

---

## PR Body Template (Agent için)

```md
## Summary
- <short bullet 1>
- <short bullet 2>

## Issue
- Closes #<ISSUE_NUMBER>

## Test plan
- [x] dotnet build backend/Dev4All.slnx
- [x] dotnet test backend/tests/Dev4All.UnitTests/

## Notes
- <risk/blocker notes if any>
```
