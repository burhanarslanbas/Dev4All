# 22. Backend Agent Prompts — All Issues

> Copy-paste ready agent prompts for every backend issue.  
> Execution order follows dependency graph: S1 → S2 → S3 → S4 → S5 → S6 → S7.  
> Within each sprint, issues are ordered by dependency.

---

## Recommended Execution Order

```
1.  #B01 — CreateProjectCommand (S1 — no dependency)
2.  #B02 — GetProjectsQuery (S1 — no dependency)
3.  #B03 — GetProjectByIdQuery (S1 — no dependency)
4.  #B04 — GetMyProjectsQuery (S1 — no dependency)
5.  #B05 — UpdateProjectCommand (S1 — depends on GetById pattern)
6.  #B06 — DeleteProjectCommand (S1 — depends on GetById pattern)
7.  #B07 — ProjectsController (S1 — depends on all Project CQRS)
8.  #B08 — Unit tests — Project (S1 — depends on all Project CQRS)
9.  #B09 — PlaceBidCommand (S2 — depends on Project module)
10. #B10 — UpdateBidCommand (S2)
11. #B11 — AcceptBidCommand (S2 — complex, depends on Bid + Contract entities)
12. #B12 — GetProjectBidsQuery (S2)
13. #B13 — GetMyBidsQuery (S2)
14. #B14 — BidsController (S2 — depends on all Bid CQRS)
15. #B15 — Unit tests — Bid (S2)
16. #B16 — ReviseContractCommand (S3 — depends on AcceptBid creating contracts)
17. #B17 — ApproveContractCommand (S3)
18. #B18 — CancelContractCommand (S3)
19. #B19 — GetContractQuery (S3)
20. #B20 — GetContractRevisionsQuery (S3)
21. #B21 — ContractsController (S3 — depends on all Contract CQRS)
22. #B22 — Unit tests — Contract (S3)
23. #B23 — GitHubService implementation (S4 — no handler dependency)
24. #B24 — LinkGitHubRepoCommand (S4)
25. #B25 — WebhookController (S4 — depends on GitHubService)
26. #B26 — GetGitHubLogsByProjectQuery (S4)
27. #B27 — Unit tests — GitHub (S4)
28. #B28 — Email HTML templates (S5)
29. #B29 — Quartz.NET + EmailDispatchJob (S5)
30. #B30 — Email notification triggers (S5)
31. #B31 — Unit tests — Email (S5)
32. #B32 — Integration tests — Auth (S6)
33. #B33 — Integration tests — Project (S6)
34. #B34 — Integration tests — Bid + Contract (S6)
35. #B35 — Serilog structured logging (S6)
36. #B36 — Code coverage report (S6)
37. #B37 — Swagger documentation (S7)
38. #B38 — Performance review (S7)
39. #B39 — API versioning audit (S7)
40. #B40 — Backend README update (S7)
```

---

## Sprint 1 — Project Module

---

### #B01 — feat: implement CreateProjectCommand

```
You are a senior .NET backend developer working on the Dev4All project — a B2B freelance marketplace.

## Context

The project uses .NET 10 / C# 13 with Clean Architecture (Onion), CQRS via MediatR 14, and FluentValidation 12.

**READ THESE FILES FIRST** (mandatory before writing any code):
- `docs/AGENTS.md` — coding standards, architecture rules, feature creation pattern
- `backend/plan/20-BACKEND-OVERVIEW.md` — current state, what's done, what's missing
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs` — entity with rich behavior
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectWriteRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/IUnitOfWork.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Auth/ICurrentUser.cs`
- `backend/src/Core/Dev4All.Application/Features/Auth/Commands/RegisterUser/` — reference for command pattern
- `docs/analyse/02-frd.md` — validation rules (Section 8.2)

## Task

Create the `CreateProjectCommand` feature in `backend/src/Core/Dev4All.Application/Features/Projects/Commands/CreateProject/`.

### Files to create:

1. **`CreateProjectCommand.cs`** — `sealed record : IRequest<CreateProjectResponse>`
   - Fields: `string Title`, `string Description`, `decimal Budget`, `DateTime Deadline`, `DateTime BidEndDate`, `string? Technologies`

2. **`CreateProjectResponse.cs`** — `sealed record`
   - Fields: `Guid Id`, `string Title`, `DateTime CreatedDate`

3. **`CreateProjectCommandValidator.cs`** — `sealed class : AbstractValidator<CreateProjectCommand>`
   - Title: NotEmpty, Length(3, 100)
   - Description: NotEmpty, Length(10, 2000)
   - Budget: GreaterThan(0)
   - Deadline: GreaterThan(DateTime.UtcNow)
   - BidEndDate: GreaterThan(DateTime.UtcNow) AND LessThan(Deadline)

4. **`CreateProjectCommandHandler.cs`** — `sealed class : IRequestHandler<CreateProjectCommand, CreateProjectResponse>`
   - Inject: `ICurrentUser`, `IProjectWriteRepository`, `IUnitOfWork`
   - Logic:
     a. Verify `currentUser.Role == "Customer"` → throw `UnauthorizedDomainException` if not
     b. Create new `Project` entity
     c. Call `project.SetCustomer(currentUser.UserId)`
     d. Set Title, Description, Budget, Deadline, BidEndDate, Technologies
     e. `await projectWriteRepository.AddAsync(project, ct)`
     f. `await unitOfWork.SaveChangesAsync(ct)`
     g. Return `new CreateProjectResponse(project.Id, project.Title, project.CreatedDate)`

### Rules:
- Use file-scoped namespaces
- Use primary constructors for DI
- Use `sealed` on all types
- Propagate `CancellationToken` through the entire call chain
- Do NOT reference DbContext or any infrastructure type
- Do NOT add comments explaining what the code does
- Verify the solution builds: `dotnet build backend/Dev4All.slnx`
```

---

### #B02 — feat: implement GetProjectsQuery (paginated open projects)

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Common/Pagination/PagedResult.cs`
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs`

## Task

Create the `GetProjectsQuery` feature in `backend/src/Core/Dev4All.Application/Features/Projects/Queries/GetProjects/`.

### Files to create:

1. **`GetProjectsQuery.cs`** — `sealed record(int Page = 1, int PageSize = 10) : IRequest<GetProjectsResponse>`

2. **`GetProjectsResponse.cs`** — `sealed record`
   ```csharp
   public sealed record GetProjectsResponse(
       IReadOnlyList<ProjectListItemDto> Items,
       int TotalCount,
       int Page,
       int PageSize);

   public sealed record ProjectListItemDto(
       Guid Id,
       string Title,
       string Description,
       decimal Budget,
       DateTime Deadline,
       DateTime BidEndDate,
       string? Technologies,
       string Status,
       int BidCount,
       DateTime CreatedDate);
   ```

3. **`GetProjectsQueryHandler.cs`** — `sealed class : IRequestHandler<...>`
   - Inject: `IProjectReadRepository`
   - Call `GetOpenProjectsAsync(page, pageSize)`
   - Map `PagedResult<Project>` → `GetProjectsResponse` with `ProjectListItemDto`
   - Map `project.Bids.Count` to `BidCount` (or 0 if Bids not loaded)
   - Map `project.Status.ToString()` to Status string

### Rules:
- Queries do NOT have validators (no side effects)
- Return DTOs (records), never raw entities
- Use file-scoped namespaces, sealed types
- Verify build: `dotnet build backend/Dev4All.slnx`
```

---

### #B03 — feat: implement GetProjectByIdQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`
- `backend/src/Core/Dev4All.Domain/Exceptions/ResourceNotFoundException.cs`

## Task

Create `GetProjectByIdQuery` in `backend/src/Core/Dev4All.Application/Features/Projects/Queries/GetProjectById/`.

### Files to create:

1. **`GetProjectByIdQuery.cs`** — `sealed record(Guid Id) : IRequest<GetProjectByIdResponse>`

2. **`GetProjectByIdResponse.cs`** — `sealed record`
   - Fields: Guid Id, string CustomerId, string? AssignedDeveloperId, string Title, string Description, decimal Budget, DateTime Deadline, DateTime BidEndDate, string? Technologies, string Status, DateTime CreatedDate, DateTime UpdatedDate

3. **`GetProjectByIdQueryHandler.cs`**
   - Inject: `IProjectReadRepository`
   - `GetByIdAsync(id)` → throw `ResourceNotFoundException("Project", id)` if null
   - Map entity to response DTO

### Rules:
- Throw `ResourceNotFoundException` for missing entities — middleware maps to 404
- Use file-scoped namespaces, sealed types, CancellationToken
- Verify build
```

---

### #B04 — feat: implement GetMyProjectsQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Auth/ICurrentUser.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`

## Task

Create `GetMyProjectsQuery` in `backend/src/Core/Dev4All.Application/Features/Projects/Queries/GetMyProjects/`.

### Files to create:

1. **`GetMyProjectsQuery.cs`** — `sealed record : IRequest<GetMyProjectsResponse>` (no parameters — uses ICurrentUser)

2. **`GetMyProjectsResponse.cs`** — `sealed record(IReadOnlyList<MyProjectDto> Projects)`
   - `MyProjectDto`: Guid Id, string Title, decimal Budget, DateTime Deadline, string Status, int BidCount, DateTime CreatedDate

3. **`GetMyProjectsQueryHandler.cs`**
   - Inject: `ICurrentUser`, `IProjectReadRepository`
   - Verify `currentUser.Role == "Customer"` → throw `UnauthorizedDomainException`
   - Call `GetByCustomerIdAsync(currentUser.UserId)`
   - Map to DTOs

### Rules:
- Role checks use string comparison with `UserRole` enum name
- Use file-scoped namespaces, sealed types, CancellationToken
- Verify build
```

---

### #B05 — feat: implement UpdateProjectCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectWriteRepository.cs`
- `docs/analyse/02-frd.md` — Section 8.2 validation rules

## Task

Create `UpdateProjectCommand` in `backend/src/Core/Dev4All.Application/Features/Projects/Commands/UpdateProject/`.

### Files to create:

1. **`UpdateProjectCommand.cs`** — `sealed record : IRequest<Unit>`
   - Fields: Guid Id, string Title, string Description, decimal Budget, DateTime Deadline, DateTime BidEndDate, string? Technologies

2. **`UpdateProjectCommandValidator.cs`**
   - Id: NotEmpty
   - Same validation rules as CreateProject for other fields

3. **`UpdateProjectCommandHandler.cs`**
   - Inject: `ICurrentUser`, `IProjectReadRepository`, `IUnitOfWork`
   - Logic:
     a. `GetByIdAsync(id)` → throw `ResourceNotFoundException` if null
     b. Verify `project.CustomerId == currentUser.UserId` → throw `UnauthorizedDomainException`
     c. Verify `project.Status == ProjectStatus.Open` → throw `BusinessRuleViolationException("Only open projects can be updated.")`
     d. Update: project.Title, project.Description, project.Budget, project.Deadline, project.BidEndDate, project.Technologies
     e. `project.MarkAsUpdated()`
     f. `await unitOfWork.SaveChangesAsync(ct)`
     g. Return `Unit.Value`

### Rules:
- Use `Unit` return type for commands with no response body
- Ownership check before mutation
- Status guard before mutation
- Verify build
```

---

### #B06 — feat: implement DeleteProjectCommand (soft delete)

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Domain/Common/BaseEntity.cs` — MarkAsDeleted()
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs`

## Task

Create `DeleteProjectCommand` in `backend/src/Core/Dev4All.Application/Features/Projects/Commands/DeleteProject/`.

### Files to create:

1. **`DeleteProjectCommand.cs`** — `sealed record(Guid Id) : IRequest<Unit>`

2. **`DeleteProjectCommandValidator.cs`** — Id: NotEmpty

3. **`DeleteProjectCommandHandler.cs`**
   - Inject: `ICurrentUser`, `IProjectReadRepository`, `IUnitOfWork`
   - Logic:
     a. Get project → 404 if null
     b. Verify ownership (CustomerId == currentUser.UserId)
     c. Verify `project.Status == ProjectStatus.Open` → "Only open projects can be deleted."
     d. `project.MarkAsDeleted()` — sets `DeletedDate`, EF soft-delete filter handles the rest
     e. `await unitOfWork.SaveChangesAsync(ct)`
     f. Return `Unit.Value`

### Rules:
- Soft delete only — never physically remove data
- EF global query filter on `DeletedDate == null` already exists in `ProjectConfiguration`
- Verify build
```

---

### #B07 — feat: implement ProjectsController

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs` — reference for controller pattern
- All files under `backend/src/Core/Dev4All.Application/Features/Projects/` (commands + queries you created)

## Task

Create `ProjectsController` at `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/ProjectsController.cs`.

### Implementation:

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public sealed class ProjectsController(ISender sender) : ControllerBase
```

### Endpoints:

| Method | Route | Command/Query | HTTP Status |
|--------|-------|---------------|-------------|
| `GET` | `/` | `GetProjectsQuery(page, pageSize)` | 200 |
| `POST` | `/` | `CreateProjectCommand` (from body) | 201 + Location header |
| `GET` | `/{id:guid}` | `GetProjectByIdQuery(id)` | 200 |
| `PUT` | `/{id:guid}` | `UpdateProjectCommand` (from body, verify id matches) | 200 |
| `DELETE` | `/{id:guid}` | `DeleteProjectCommand(id)` | 204 NoContent |
| `GET` | `/my` | `GetMyProjectsQuery` | 200 |

### Rules:
- Controller is THIN — only delegates to MediatR via `ISender`
- No business logic in controller
- Use `[FromQuery]` for page/pageSize on GET list
- Use `CancellationToken ct` on every action
- `POST` returns `CreatedAtAction` with location pointing to `GetById`
- Add XML summary comments for Swagger
- Verify build: `dotnet build backend/Dev4All.slnx`
```

---

### #B08 — test: unit tests for Project module

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md` — testing standards, naming convention
- `backend/plan/20-BACKEND-OVERVIEW.md`
- All files under `backend/src/Core/Dev4All.Application/Features/Projects/`
- `backend/tests/Dev4All.UnitTests/Dev4All.UnitTests.csproj`

## Task

Create unit tests for all Project handlers and validators in `backend/tests/Dev4All.UnitTests/Features/Projects/`.

### Setup:

1. Add NuGet packages to `Dev4All.UnitTests.csproj` (if not already present):
   - `NSubstitute` (latest)
   - `FluentAssertions` (latest)
   - `Bogus` (latest)

2. Add project reference to `Dev4All.Application` (if not already present).

### Test files to create:

1. **`CreateProjectCommandHandlerTests.cs`**
   - `Handle_ValidCommand_CreatesProjectAndReturnsId`
   - `Handle_NonCustomerRole_ThrowsUnauthorizedException`

2. **`CreateProjectCommandValidatorTests.cs`**
   - `Validate_ValidCommand_PassesValidation`
   - `Validate_EmptyTitle_FailsValidation`
   - `Validate_TitleTooLong_FailsValidation`
   - `Validate_NegativeBudget_FailsValidation`
   - `Validate_PastDeadline_FailsValidation`
   - `Validate_BidEndDateAfterDeadline_FailsValidation`

3. **`UpdateProjectCommandHandlerTests.cs`**
   - `Handle_ValidCommand_UpdatesProject`
   - `Handle_NonExistentProject_ThrowsNotFoundException`
   - `Handle_NotOwner_ThrowsUnauthorizedException`
   - `Handle_NonOpenProject_ThrowsBusinessRuleViolation`

4. **`DeleteProjectCommandHandlerTests.cs`**
   - `Handle_ValidCommand_SoftDeletesProject`
   - `Handle_NotOwner_ThrowsUnauthorizedException`

5. **`GetProjectByIdQueryHandlerTests.cs`**
   - `Handle_ExistingProject_ReturnsDto`
   - `Handle_NonExistentProject_ThrowsNotFoundException`

6. **`GetProjectsQueryHandlerTests.cs`**
   - `Handle_ReturnsPagedProjectList`

### Pattern:
- Arrange-Act-Assert (AAA)
- Mock all dependencies with NSubstitute
- Use FluentAssertions for assertions
- One test class per handler/validator
- Test naming: `MethodName_Scenario_ExpectedResult`

### Rules:
- Run tests: `dotnet test backend/tests/Dev4All.UnitTests/`
- All tests must pass
```

---

## Sprint 2 — Bid Module

---

### #B09 — feat: implement PlaceBidCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Domain/Entities/Bid.cs` — entity with SetOwnership()
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs` — need to check status
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Bids/IBidReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Bids/IBidWriteRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`
- `docs/analyse/02-frd.md` — Section 8.3 Bid validation rules

## Task

Create `PlaceBidCommand` in `backend/src/Core/Dev4All.Application/Features/Bids/Commands/PlaceBid/`.

### Files to create:

1. **`PlaceBidCommand.cs`** — `sealed record(Guid ProjectId, decimal BidAmount, string ProposalNote) : IRequest<PlaceBidResponse>`

2. **`PlaceBidResponse.cs`** — `sealed record(Guid Id, Guid ProjectId, decimal BidAmount, DateTime CreatedDate)`

3. **`PlaceBidCommandValidator.cs`**
   - ProjectId: NotEmpty
   - BidAmount: GreaterThan(0)
   - ProposalNote: NotEmpty, Length(10, 1000)

4. **`PlaceBidCommandHandler.cs`**
   - Inject: `ICurrentUser`, `IProjectReadRepository`, `IBidReadRepository`, `IBidWriteRepository`, `IUnitOfWork`
   - Logic:
     a. Verify `currentUser.Role == "Developer"` → `UnauthorizedDomainException`
     b. Get project → `ResourceNotFoundException` if null
     c. Verify `project.Status == ProjectStatus.Open` → `BusinessRuleViolationException("Project is not accepting bids.")`
     d. Verify `project.BidEndDate > DateTime.UtcNow` → `BusinessRuleViolationException("Bid period has ended.")`
     e. Check duplicate: `IBidReadRepository.GetByDeveloperAndProjectAsync(currentUser.UserId, projectId)` → `BusinessRuleViolationException("You have already placed a bid on this project.")` if exists
     f. Create `Bid`, call `bid.SetOwnership(projectId, currentUser.UserId)`, set BidAmount + ProposalNote
     g. `await bidWriteRepository.AddAsync(bid, ct)`
     h. `await unitOfWork.SaveChangesAsync(ct)`
     i. Return response

### Rules:
- Multiple business rule checks before mutation
- Verify build
```

---

### #B10 — feat: implement UpdateBidCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Domain/Entities/Bid.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Bids/IBidReadRepository.cs`

## Task

Create `UpdateBidCommand` in `backend/src/Core/Dev4All.Application/Features/Bids/Commands/UpdateBid/`.

### Files to create:

1. **`UpdateBidCommand.cs`** — `sealed record(Guid Id, decimal BidAmount, string ProposalNote) : IRequest<Unit>`

2. **`UpdateBidCommandValidator.cs`**
   - Id: NotEmpty
   - BidAmount: GreaterThan(0)
   - ProposalNote: NotEmpty, Length(10, 1000)

3. **`UpdateBidCommandHandler.cs`**
   - Inject: `ICurrentUser`, `IBidReadRepository`, `IUnitOfWork`
   - Logic:
     a. Get bid by Id → `ResourceNotFoundException` if null
     b. Verify `bid.DeveloperId == currentUser.UserId` → `UnauthorizedDomainException`
     c. Verify `bid.Status == BidStatus.Pending` → `BusinessRuleViolationException("Only pending bids can be updated.")`
     d. Update bid.BidAmount, bid.ProposalNote
     e. `bid.MarkAsUpdated()` (from BaseEntity)
     f. `await unitOfWork.SaveChangesAsync(ct)`

### Rules:
- Only the bid owner (Developer) can update
- Only Pending bids can be updated
- Verify build
```

---

### #B11 — feat: implement AcceptBidCommand (complex transaction)

```
You are a senior .NET backend developer working on the Dev4All project.

## Context — CRITICAL: This is the most complex handler in the system.

READ FIRST (ALL of these):
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/plan/21-BACKEND-ROADMAP.md` — Sprint 2, Issue 11 details
- `backend/src/Core/Dev4All.Domain/Entities/Bid.cs` — Accept(), Reject() methods
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs` — AssignDeveloper(), MoveToAwaitingContract()
- `backend/src/Core/Dev4All.Domain/Entities/Contract.cs` — SetProjectId(), SetInitialContent()
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/IUnitOfWork.cs` — transaction methods
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Bids/IBidReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Contracts/IContractWriteRepository.cs`
- `docs/analyse/02-frd.md` — UC-03 Accept Bid flow
- `docs/analyse/04-sadm.md` — CQRS operations list

## Task

Create `AcceptBidCommand` in `backend/src/Core/Dev4All.Application/Features/Bids/Commands/AcceptBid/`.

### Files to create:

1. **`AcceptBidCommand.cs`** — `sealed record(Guid BidId) : IRequest<AcceptBidResponse>`

2. **`AcceptBidResponse.cs`** — `sealed record(Guid ProjectId, Guid AcceptedBidId, Guid ContractId)`

3. **`AcceptBidCommandHandler.cs`** (TRANSACTIONAL)
   - Inject: `ICurrentUser`, `IBidReadRepository`, `IBidWriteRepository`, `IProjectReadRepository`, `IContractWriteRepository`, `IUnitOfWork`
   - Logic (inside transaction):
     a. `await unitOfWork.BeginTransactionAsync(ct)`
     b. Get bid → `ResourceNotFoundException` if null
     c. Get project (by bid.ProjectId) WITH bids loaded (`GetByIdWithBidsAsync`) → 404
     d. Verify `project.CustomerId == currentUser.UserId` → `UnauthorizedDomainException("Only the project owner can accept bids.")`
     e. Verify `project.Status == ProjectStatus.Open` → `BusinessRuleViolationException("Only open projects can accept bids.")`
     f. `bid.Accept()` — domain method (changes status, sets IsAccepted)
     g. `project.AssignDeveloper(bid.DeveloperId)` — domain method
     h. `project.MoveToAwaitingContract()` — domain method (Open → AwaitingContract)
     i. Reject all other pending bids on this project:
        ```
        foreach (var otherBid in project.Bids.Where(b => b.Id != bid.Id && b.Status == BidStatus.Pending))
            otherBid.Reject();
        ```
     j. Create Contract:
        ```
        var contract = new Contract();
        contract.SetProjectId(project.Id);
        contract.SetInitialContent("", currentUser.UserId);
        await contractWriteRepository.AddAsync(contract, ct);
        ```
     k. `await unitOfWork.SaveChangesAsync(ct)`
     l. `await unitOfWork.CommitTransactionAsync(ct)`
     m. Return `new AcceptBidResponse(project.Id, bid.Id, contract.Id)`
   - Wrap in try/catch: `unitOfWork.RollbackTransactionAsync()` on failure

### Rules:
- This MUST be transactional — all changes succeed or all roll back
- Multiple domain entity mutations in one handler — this is acceptable for a complex use case
- Use domain methods (Accept, Reject, AssignDeveloper, MoveToAwaitingContract) — they encapsulate rules
- Verify build
```

---

### #B12 — feat: implement GetProjectBidsQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Bids/IBidReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`

## Task

Create `GetProjectBidsQuery` in `backend/src/Core/Dev4All.Application/Features/Bids/Queries/GetProjectBids/`.

### Files:

1. **`GetProjectBidsQuery.cs`** — `sealed record(Guid ProjectId) : IRequest<GetProjectBidsResponse>`

2. **`GetProjectBidsResponse.cs`**
   ```csharp
   public sealed record GetProjectBidsResponse(IReadOnlyList<BidDto> Bids);
   public sealed record BidDto(Guid Id, string DeveloperId, decimal BidAmount, string ProposalNote, string Status, DateTime CreatedDate);
   ```

3. **`GetProjectBidsQueryHandler.cs`**
   - Inject: `ICurrentUser`, `IProjectReadRepository`, `IBidReadRepository`
   - Verify project exists + current user is project's Customer (owner can see all bids)
   - Get bids by ProjectId → map to DTOs

### Rules:
- Only the project Customer can see all bids (authorization check)
- Verify build
```

---

### #B13 — feat: implement GetMyBidsQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Bids/IBidReadRepository.cs`

## Task

Create `GetMyBidsQuery` in `backend/src/Core/Dev4All.Application/Features/Bids/Queries/GetMyBids/`.

### Files:

1. **`GetMyBidsQuery.cs`** — `sealed record : IRequest<GetMyBidsResponse>`

2. **`GetMyBidsResponse.cs`**
   ```csharp
   public sealed record GetMyBidsResponse(IReadOnlyList<MyBidDto> Bids);
   public sealed record MyBidDto(Guid Id, Guid ProjectId, decimal BidAmount, string ProposalNote, string Status, bool IsAccepted, DateTime CreatedDate);
   ```

3. **`GetMyBidsQueryHandler.cs`**
   - Inject: `ICurrentUser`, `IBidReadRepository`
   - Verify Developer role
   - `GetByDeveloperIdAsync(currentUser.UserId)` → map to DTOs
```

---

### #B14 — feat: implement BidsController

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs` — reference pattern
- All Bid commands/queries under `backend/src/Core/Dev4All.Application/Features/Bids/`

## Task

Create `BidsController` at `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/BidsController.cs`.

### Endpoints:

| Method | Route | Command/Query | HTTP Status |
|--------|-------|---------------|-------------|
| `POST` | `/api/v1/projects/{projectId:guid}/bids` | `PlaceBidCommand` | 201 |
| `GET` | `/api/v1/projects/{projectId:guid}/bids` | `GetProjectBidsQuery` | 200 |
| `PUT` | `/api/v1/bids/{id:guid}` | `UpdateBidCommand` | 200 |
| `POST` | `/api/v1/bids/{id:guid}/accept` | `AcceptBidCommand` | 200 |
| `GET` | `/api/v1/bids/my` | `GetMyBidsQuery` | 200 |

### Notes:
- This controller uses TWO route prefixes (nested under projects + standalone)
- For `PlaceBidCommand`, inject `projectId` from route into the command
- Thin controller — delegate everything to MediatR
- Verify build
```

---

### #B15 — test: unit tests for Bid module

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md` — testing standards
- All Bid handlers under `backend/src/Core/Dev4All.Application/Features/Bids/`
- Existing Project test files under `backend/tests/Dev4All.UnitTests/Features/Projects/` (reference)

## Task

Create unit tests in `backend/tests/Dev4All.UnitTests/Features/Bids/`.

### Test files:

1. **`PlaceBidCommandHandlerTests.cs`**
   - `Handle_ValidCommand_CreatesBid`
   - `Handle_NonDeveloper_ThrowsUnauthorized`
   - `Handle_ClosedProject_ThrowsBusinessRule`
   - `Handle_ExpiredBidPeriod_ThrowsBusinessRule`
   - `Handle_DuplicateBid_ThrowsBusinessRule`

2. **`PlaceBidCommandValidatorTests.cs`**
   - Valid/invalid scenarios for all fields

3. **`AcceptBidCommandHandlerTests.cs`** (most important)
   - `Handle_ValidAccept_TransitionsProjectAndCreateContract`
   - `Handle_NotProjectOwner_ThrowsUnauthorized`
   - `Handle_NonOpenProject_ThrowsBusinessRule`
   - `Handle_RejectsOtherPendingBids`

4. **`UpdateBidCommandHandlerTests.cs`**
   - `Handle_ValidUpdate_UpdatesBid`
   - `Handle_NotBidOwner_ThrowsUnauthorized`
   - `Handle_NonPendingBid_ThrowsBusinessRule`

### Pattern:
- Mock with NSubstitute, assert with FluentAssertions
- For AcceptBid: mock transaction methods, verify all domain calls
- `dotnet test backend/tests/Dev4All.UnitTests/`
```

---

## Sprint 3 — Contract Module

---

### #B16 — feat: implement ReviseContractCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Domain/Entities/Contract.cs` — Revise() method
- `backend/src/Core/Dev4All.Domain/Entities/ContractRevision.cs` — CreateSnapshot() factory
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Contracts/IContractReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/ContractRevisions/IContractRevisionWriteRepository.cs`
- `docs/analyse/02-frd.md` — Section 8.5 Contract validation
- `docs/analyse/04-sadm.md` — ReviseContractCommand specification

## Task

Create `ReviseContractCommand` in `backend/src/Core/Dev4All.Application/Features/Contracts/Commands/ReviseContract/`.

### Files:

1. **`ReviseContractCommand.cs`** — `sealed record(Guid ProjectId, string Content, string? RevisionNote) : IRequest<Unit>`

2. **`ReviseContractCommandValidator.cs`**
   - Content: NotEmpty, MinimumLength(50)
   - RevisionNote: MaximumLength(500) (when not null)

3. **`ReviseContractCommandHandler.cs`**
   - Inject: `ICurrentUser`, `IContractReadRepository`, `IProjectReadRepository`, `IContractRevisionWriteRepository`, `IUnitOfWork`
   - Logic:
     a. Get contract by ProjectId → 404
     b. Get project → verify current user is Customer OR assigned Developer
     c. `isCustomer = (currentUser.UserId == project.CustomerId)`
     d. Create revision snapshot BEFORE modifying contract:
        `ContractRevision.CreateSnapshot(contract.Id, currentUser.UserId, contract.Content, contract.RevisionNumber, revisionNote)`
     e. `await contractRevisionWriteRepository.AddAsync(revision, ct)`
     f. `contract.Revise(content, currentUser.UserId, isCustomer)` — resets other party's approval
     g. `await unitOfWork.SaveChangesAsync(ct)`

### Rules:
- Save snapshot of OLD content before applying new content
- Domain method handles status transition and approval reset
- Verify build
```

---

### #B17 — feat: implement ApproveContractCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Domain/Entities/Contract.cs` — Approve() method
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs` — MoveToOngoing() method
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Contracts/IContractReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Projects/IProjectReadRepository.cs`

## Task

Create `ApproveContractCommand` in `backend/src/Core/Dev4All.Application/Features/Contracts/Commands/ApproveContract/`.

### Files:

1. **`ApproveContractCommand.cs`** — `sealed record(Guid ProjectId) : IRequest<ApproveContractResponse>`

2. **`ApproveContractResponse.cs`** — `sealed record(bool IsFullyApproved, string ContractStatus)`

3. **`ApproveContractCommandHandler.cs`**
   - Logic:
     a. Get contract by ProjectId → 404
     b. Get project → verify party membership
     c. `isCustomer = (currentUser.UserId == project.CustomerId)`
     d. `contract.Approve(isCustomer)` — domain method; sets approval flags, checks if both approved
     e. If `contract.Status == ContractStatus.BothApproved`:
        `project.MoveToOngoing()` — transitions project from AwaitingContract to Ongoing
     f. `await unitOfWork.SaveChangesAsync(ct)`
     g. Return response with `IsFullyApproved = contract.Status == ContractStatus.BothApproved`

### Key behavior:
- First party approves → status stays UnderReview, only their flag is set
- Second party approves → status becomes BothApproved, project moves to Ongoing
- Verify build
```

---

### #B18 — feat: implement CancelContractCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Domain/Entities/Contract.cs` — Cancel()
- `backend/src/Core/Dev4All.Domain/Entities/Project.cs` — Cancel()

## Task

Create `CancelContractCommand` in `backend/src/Core/Dev4All.Application/Features/Contracts/Commands/CancelContract/`.

### Files:

1. **`CancelContractCommand.cs`** — `sealed record(Guid ProjectId) : IRequest<Unit>`

2. **`CancelContractCommandHandler.cs`**
   - Logic:
     a. Get contract by ProjectId → 404
     b. Get project → verify party membership
     c. `contract.Cancel()` — domain method
     d. `project.Cancel()` — domain method
     e. `await unitOfWork.SaveChangesAsync(ct)`

### Key behavior:
- Cancelling a contract also cancels the project
- Domain methods enforce state guards (can't cancel already approved/cancelled)
- Verify build
```

---

### #B19 — feat: implement GetContractQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Contracts/IContractReadRepository.cs`

## Task

Create `GetContractQuery` in `backend/src/Core/Dev4All.Application/Features/Contracts/Queries/GetContract/`.

### Files:

1. **`GetContractQuery.cs`** — `sealed record(Guid ProjectId) : IRequest<GetContractResponse>`

2. **`GetContractResponse.cs`**
   ```csharp
   public sealed record GetContractResponse(
       Guid Id, Guid ProjectId, string Content, int RevisionNumber,
       string Status, bool IsCustomerApproved, bool IsDeveloperApproved,
       DateTime? CustomerApprovedAt, DateTime? DeveloperApprovedAt,
       DateTime CreatedDate, DateTime UpdatedDate);
   ```

3. **`GetContractQueryHandler.cs`**
   - Get contract by ProjectId → 404
   - Verify current user is project party (Customer or assigned Developer)
   - Map to response DTO
```

---

### #B20 — feat: implement GetContractRevisionsQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/ContractRevisions/IContractRevisionReadRepository.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/Contracts/IContractReadRepository.cs`

## Task

Create `GetContractRevisionsQuery` in `backend/src/Core/Dev4All.Application/Features/Contracts/Queries/GetContractRevisions/`.

### Files:

1. **`GetContractRevisionsQuery.cs`** — `sealed record(Guid ProjectId) : IRequest<GetContractRevisionsResponse>`

2. **`GetContractRevisionsResponse.cs`**
   ```csharp
   public sealed record GetContractRevisionsResponse(IReadOnlyList<ContractRevisionDto> Revisions);
   public sealed record ContractRevisionDto(
       Guid Id, string RevisedById, int RevisionNumber,
       string ContentSnapshot, string? RevisionNote, DateTime CreatedDate);
   ```

3. **`GetContractRevisionsQueryHandler.cs`**
   - Get contract by ProjectId → 404 (need contract.Id)
   - Verify party membership
   - `GetByContractIdAsync(contract.Id)` → map to DTOs
   - Order by RevisionNumber descending
```

---

### #B21 — feat: implement ContractsController

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs`
- All Contract CQRS under `backend/src/Core/Dev4All.Application/Features/Contracts/`

## Task

Create `ContractsController` at `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/ContractsController.cs`.

### Endpoints:

| Method | Route | Command/Query | HTTP Status |
|--------|-------|---------------|-------------|
| `GET` | `/api/v1/contracts/{projectId:guid}` | `GetContractQuery(projectId)` | 200 |
| `PUT` | `/api/v1/contracts/{projectId:guid}` | `ReviseContractCommand` | 200 |
| `POST` | `/api/v1/contracts/{projectId:guid}/approve` | `ApproveContractCommand(projectId)` | 200 |
| `POST` | `/api/v1/contracts/{projectId:guid}/cancel` | `CancelContractCommand(projectId)` | 200 |
| `GET` | `/api/v1/contracts/{projectId:guid}/revisions` | `GetContractRevisionsQuery(projectId)` | 200 |

Thin controller, MediatR delegation, CancellationToken, XML summaries.
Verify build.
```

---

### #B22 — test: unit tests for Contract module

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md` — testing standards
- All Contract handlers under `backend/src/Core/Dev4All.Application/Features/Contracts/`

## Task

Create unit tests in `backend/tests/Dev4All.UnitTests/Features/Contracts/`.

### Test files:

1. **`ReviseContractCommandHandlerTests.cs`**
   - `Handle_ValidRevision_SavesSnapshotAndUpdatesContract`
   - `Handle_NonPartyUser_ThrowsUnauthorized`
   - `Handle_ApprovedContract_ThrowsBusinessRule`

2. **`ApproveContractCommandHandlerTests.cs`**
   - `Handle_FirstApproval_SetsFlag`
   - `Handle_BothApproved_MovesProjectToOngoing`

3. **`CancelContractCommandHandlerTests.cs`**
   - `Handle_ValidCancel_CancelsContractAndProject`

4. **`ReviseContractCommandValidatorTests.cs`**
   - Content too short, RevisionNote too long

All tests: NSubstitute mocks, FluentAssertions, AAA pattern.
`dotnet test backend/tests/Dev4All.UnitTests/`
```

---

## Sprint 4 — GitHub Integration

---

### #B23 — feat: implement GitHubService (HMAC + push parsing)

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Services/IGitHubService.cs` — interface to implement
- `backend/src/Core/Dev4All.Domain/Entities/GitHubLog.cs` — Create() factory
- `docs/analyse/05-integration.md` — full GitHub webhook spec (HMAC, payload format, field mapping)

## Task

Create `GitHubService` at `backend/src/Infrastructure/Dev4All.Infrastructure/GitHub/GitHubService.cs`.

### Implementation:

1. **`ValidateWebhookSignature(string payload, string signature, string secret)`**
   - Compute `HMAC-SHA256` of payload bytes using secret bytes
   - Format as `sha256=<hex>`
   - Compare with signature using `CryptographicOperations.FixedTimeEquals` (timing-safe)
   - Return `true` if match

2. **`ParsePushEvent(string jsonPayload, Guid projectId)`**
   - Deserialize using `System.Text.Json`
   - Extract `ref` → strip `refs/heads/` prefix → branch name
   - Extract `repository.html_url` → repoUrl
   - Loop `commits[]`:
     - `GitHubLog.Create(projectId, repoUrl, branch, commit.id, commit.message, commit.author.name, commit.timestamp)`
   - Return `List<GitHubLog>`

3. **Register in `InfrastructureServiceRegistration.cs`:**
   - `services.AddScoped<IGitHubService, GitHubService>()`

### JSON model classes (private nested or separate file):

```csharp
private sealed record GitHubPushPayload(string Ref, GitHubRepository Repository, List<GitHubCommit> Commits);
private sealed record GitHubRepository(string Html_Url);
private sealed record GitHubCommit(string Id, string Message, GitHubAuthor Author, DateTime Timestamp);
private sealed record GitHubAuthor(string Name);
```

### Security notes:
- Use `System.Security.Cryptography.HMACSHA256`
- Use `CryptographicOperations.FixedTimeEquals` — NOT `==` or `string.Equals`
- Verify build
```

---

### #B24 — feat: implement LinkGitHubRepoCommand

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Domain/Entities/GitHubLog.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/GitHubLogs/IGitHubLogWriteRepository.cs`
- `docs/analyse/02-frd.md` — Section 8.4 GitHub Repo validation
- `docs/analyse/05-integration.md` — repo linking flow

## Task

Create `LinkGitHubRepoCommand` in `backend/src/Core/Dev4All.Application/Features/GitHub/Commands/LinkGitHubRepo/`.

### Files:

1. **`LinkGitHubRepoCommand.cs`** — `sealed record(Guid ProjectId, string RepoUrl, string? Branch) : IRequest<Unit>`

2. **`LinkGitHubRepoCommandValidator.cs`**
   - ProjectId: NotEmpty
   - RepoUrl: NotEmpty, Must contain "github.com"
   - Branch: when provided, NotEmpty

3. **`LinkGitHubRepoCommandHandler.cs`**
   - Inject: `ICurrentUser`, `IProjectReadRepository`, `IGitHubLogWriteRepository`, `IUnitOfWork`
   - Logic:
     a. Get project → 404
     b. Verify `project.AssignedDeveloperId == currentUser.UserId` → unauthorized
     c. Verify `project.Status == ProjectStatus.Ongoing` → business rule
     d. Create initial GitHubLog: `GitHubLog.Create(projectId, repoUrl, branch ?? "main", "initial", "Repository linked", currentUser.UserId, DateTime.UtcNow)`
     e. Save

### Rules:
- Only the assigned Developer can link a repo
- Project must be in Ongoing status
- Verify build
```

---

### #B25 — feat: implement WebhookController

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/plan/20-BACKEND-OVERVIEW.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Services/IGitHubService.cs`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/GitHubLogs/IGitHubLogWriteRepository.cs`
- `docs/analyse/05-integration.md` — full webhook endpoint spec

## Task

Create `WebhookController` at `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/WebhookController.cs`.

### Also create (in Application):
**`GitHubWebhookOptions.cs`** in `backend/src/Core/Dev4All.Application/Options/`:
```csharp
public sealed class GitHubWebhookOptions
{
    public const string SectionName = "GitHub";
    public string WebhookSecret { get; set; } = string.Empty;
}
```

### Controller Implementation:

```
[ApiController]
[Route("api/v1/webhooks")]
public sealed class WebhookController(IGitHubService gitHubService, ...) : ControllerBase
```

**`POST /api/v1/webhooks/github`** — NO `[Authorize]` (uses HMAC instead):
1. Read raw request body as string
2. Get `X-Hub-Signature-256` header
3. Get webhook secret from `IOptions<GitHubWebhookOptions>`
4. Call `gitHubService.ValidateWebhookSignature(payload, signature, secret)` → 401 if false
5. Parse project identifier from payload (convention: use a custom header or query param for project mapping, OR look up by repo URL)
6. Call `gitHubService.ParsePushEvent(payload, projectId)` → list of GitHubLog
7. `await gitHubLogWriteRepository.AddRangeAsync(logs, ct)`
8. `await unitOfWork.SaveChangesAsync(ct)`
9. Return 200 OK

### Notes:
- Register `GitHubWebhookOptions` in `Program.cs` (bind to "GitHub" section)
- Add `"GitHub": { "WebhookSecret": "" }` to `appsettings.json`
- This endpoint is PUBLIC but protected by HMAC signature verification
- Verify build
```

---

### #B26 — feat: implement GetGitHubLogsByProjectQuery

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Core/Dev4All.Application/Abstractions/Persistence/Repositories/GitHubLogs/IGitHubLogReadRepository.cs`

## Task

Create `GetGitHubLogsByProjectQuery` in `backend/src/Core/Dev4All.Application/Features/GitHub/Queries/GetGitHubLogsByProject/`.

### Files:

1. **`GetGitHubLogsByProjectQuery.cs`** — `sealed record(Guid ProjectId) : IRequest<GetGitHubLogsResponse>`

2. **`GetGitHubLogsResponse.cs`**
   ```csharp
   public sealed record GetGitHubLogsResponse(IReadOnlyList<GitHubLogDto> Logs);
   public sealed record GitHubLogDto(
       Guid Id, string RepoUrl, string Branch, string CommitHash,
       string CommitMessage, string AuthorName, DateTime PushedAt);
   ```

3. **`GetGitHubLogsByProjectQueryHandler.cs`**
   - Verify project exists + party membership
   - `GetByProjectIdAsync(projectId)` → map to DTOs
   - Order by PushedAt descending

Also add to `ProjectsController` (or a separate controller):
- `GET /api/v1/projects/{id}/github-logs` → delegates to `GetGitHubLogsByProjectQuery`
```

---

### #B27 — test: unit tests for GitHub module

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `backend/src/Infrastructure/Dev4All.Infrastructure/GitHub/GitHubService.cs`
- All GitHub handlers under `backend/src/Core/Dev4All.Application/Features/GitHub/`

## Task

Create unit tests in `backend/tests/Dev4All.UnitTests/Features/GitHub/` and `backend/tests/Dev4All.UnitTests/Infrastructure/`.

### Test files:

1. **`GitHubServiceTests.cs`** (in `Infrastructure/`)
   - `ValidateWebhookSignature_ValidSignature_ReturnsTrue`
   - `ValidateWebhookSignature_InvalidSignature_ReturnsFalse`
   - `ParsePushEvent_ValidPayload_ReturnsGitHubLogs`
   - `ParsePushEvent_MultipleCommits_ReturnsAllLogs`

2. **`LinkGitHubRepoCommandHandlerTests.cs`** (in `Features/GitHub/`)
   - `Handle_ValidLink_CreatesInitialLog`
   - `Handle_NotAssignedDeveloper_ThrowsUnauthorized`
   - `Handle_NonOngoingProject_ThrowsBusinessRule`

### Notes:
- For GitHubService tests, add project reference to `Dev4All.Infrastructure`
- Use real HMAC computation to verify signatures in tests
- `dotnet test backend/tests/Dev4All.UnitTests/`
```

---

## Sprint 5 — Email & Background Jobs

---

### #B28 — feat: email HTML templates

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/analyse/05-integration.md` — email notification matrix (MAIL-01 through MAIL-05)
- `backend/src/Infrastructure/Dev4All.Infrastructure/Email/EmailService.cs`

## Task

Create email HTML templates in `backend/src/Infrastructure/Dev4All.Infrastructure/Templates/`.

### Files to create:

1. **`welcome.html`** — MAIL-01: Welcome + registration confirmation
   - Placeholders: `{{UserName}}`, `{{Email}}`

2. **`new-bid.html`** — MAIL-02: New bid notification to Customer
   - Placeholders: `{{CustomerName}}`, `{{ProjectTitle}}`, `{{DeveloperName}}`, `{{BidAmount}}`, `{{ProjectUrl}}`

3. **`bid-accepted.html`** — MAIL-03: Bid accepted notification to Developer
   - Placeholders: `{{DeveloperName}}`, `{{ProjectTitle}}`, `{{BidAmount}}`, `{{ProjectUrl}}`

4. **`bid-rejected.html`** — MAIL-04: Bid rejected notification to Developer
   - Placeholders: `{{DeveloperName}}`, `{{ProjectTitle}}`

5. **`repo-linked.html`** — MAIL-05: Repo linked notification to Customer
   - Placeholders: `{{CustomerName}}`, `{{ProjectTitle}}`, `{{RepoUrl}}`

### Also create:
**`TemplateRenderer.cs`** in `backend/src/Infrastructure/Dev4All.Infrastructure/Email/`:
- Reads template files from embedded resources or file path
- Replaces `{{PlaceholderName}}` with actual values
- Method: `string Render(string templateName, Dictionary<string, string> variables)`

### Template design:
- Clean, professional HTML email layout
- Responsive (mobile-friendly)
- Dev4All branding (blue accent color #2563EB)
- No external image dependencies
```

---

### #B29 — feat: Quartz.NET integration + EmailDispatchJob

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `docs/analyse/05-integration.md` — async email dispatch architecture
- `backend/src/Presentation/Dev4All.WebAPI/Program.cs`
- `backend/src/Infrastructure/Dev4All.Infrastructure/InfrastructureServiceRegistration.cs`

## Task

### 1. Add NuGet packages:
- `Quartz` to `Dev4All.Infrastructure`
- `Quartz.Extensions.Hosting` to `Dev4All.Infrastructure`

### 2. Create email queue entity (optional — can use simple DB table or in-memory):

**Option A (recommended for MVP):** Add `EmailQueue` entity to Domain and Persistence:
- `Id`, `Recipient`, `Subject`, `HtmlBody`, `Status` (Pending/Sent/Failed), `RetryCount`, `CreatedDate`, `SentDate`
- Add DbSet, Configuration, Repository, migration

**Option B (simpler):** Use in-memory `ConcurrentQueue<EmailMessage>` in Infrastructure.

### 3. Create `EmailDispatchJob`:
**File:** `backend/src/Infrastructure/Dev4All.Infrastructure/Jobs/EmailDispatchJob.cs`
- Implements `IJob` (Quartz)
- Queries pending emails (or dequeues from in-memory queue)
- Sends via `IEmailService`
- On success: mark as Sent
- On failure: increment RetryCount; if RetryCount >= 3, mark as Failed + log error

### 4. Register Quartz in DI:
In `InfrastructureServiceRegistration.cs`:
```csharp
services.AddQuartz(q =>
{
    var jobKey = new JobKey("EmailDispatchJob");
    q.AddJob<EmailDispatchJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
});
services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
```

### Rules:
- Email sending must NOT block API responses
- Use structured logging for send/fail events
- Verify build
```

---

### #B30 — feat: email notification triggers in handlers

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md`
- `docs/analyse/05-integration.md` — notification matrix
- `backend/src/Infrastructure/Dev4All.Infrastructure/Email/TemplateRenderer.cs`
- Existing handlers that need email triggers

## Task

Add email notification calls to existing command handlers. Use the email queue (from B29) or call `IEmailService` directly (for MVP simplicity).

### Create an abstraction:
**`IEmailNotificationService`** in `backend/src/Core/Dev4All.Application/Abstractions/Services/`:
```csharp
public interface IEmailNotificationService
{
    Task QueueWelcomeEmailAsync(string email, string name, CancellationToken ct = default);
    Task QueueNewBidEmailAsync(string customerEmail, string customerName, string projectTitle, string developerName, decimal bidAmount, CancellationToken ct = default);
    Task QueueBidAcceptedEmailAsync(string developerEmail, string developerName, string projectTitle, decimal bidAmount, CancellationToken ct = default);
    Task QueueBidRejectedEmailAsync(string developerEmail, string developerName, string projectTitle, CancellationToken ct = default);
    Task QueueRepoLinkedEmailAsync(string customerEmail, string customerName, string projectTitle, string repoUrl, CancellationToken ct = default);
}
```

### Implement in Infrastructure:
**`EmailNotificationService`** — uses `TemplateRenderer` + email queue/direct send

### Modify handlers:
1. `RegisterUserCommandHandler` → after successful registration, queue welcome email
2. `PlaceBidCommandHandler` → after saving bid, queue new-bid notification to Customer
3. `AcceptBidCommandHandler` → queue bid-accepted to winner, bid-rejected to others
4. `LinkGitHubRepoCommandHandler` → queue repo-linked notification to Customer

### Notes:
- Email operations must not fail the main transaction
- Wrap email calls in try/catch — log errors but don't throw
- Verify build
```

---

### #B31 — test: unit tests for email

```
You are a senior .NET backend developer working on the Dev4All project.

## Task

Create email-related tests in `backend/tests/Dev4All.UnitTests/Infrastructure/Email/`.

1. **`TemplateRendererTests.cs`**
   - `Render_ValidTemplate_ReplacesPlaceholders`
   - `Render_MissingPlaceholder_LeavesAsIs`

2. **`EmailNotificationServiceTests.cs`**
   - Verify correct template + variables are passed for each notification type
   - Mock `IEmailService`, verify `SendAsync` called with expected params

`dotnet test backend/tests/Dev4All.UnitTests/`
```

---

## Sprint 6 — Testing & Quality

---

### #B32 — test: integration tests for Auth endpoints

```
You are a senior .NET backend developer working on the Dev4All project.

## Context

READ FIRST:
- `docs/AGENTS.md` — testing standards
- `backend/src/Presentation/Dev4All.WebAPI/Program.cs`
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs`

## Task

Set up integration test infrastructure and create Auth endpoint tests.

### Setup:
1. Add NuGet to `Dev4All.IntegrationTests`:
   - `Microsoft.AspNetCore.Mvc.Testing`
   - `FluentAssertions`
   - `Testcontainers.PostgreSql` (or use EF InMemory for simplicity)
2. Add project references: `Dev4All.WebAPI`, `Dev4All.Persistence`

3. Create `CustomWebApplicationFactory<TProgram>`:
   - Override `ConfigureWebHost` to replace DB with test PostgreSQL or InMemory
   - Seed test roles
   - Expose helper methods: `CreateClientWithToken(email, password, role)`

### Test files:

**`AuthEndpointTests.cs`**
- `Register_ValidRequest_Returns201`
- `Register_DuplicateEmail_Returns400`
- `Register_InvalidPassword_Returns400`
- `Login_ValidCredentials_ReturnsJwt`
- `Login_InvalidCredentials_Returns403`
- `Me_WithToken_Returns200`
- `Me_WithoutToken_Returns401`

`dotnet test backend/tests/Dev4All.IntegrationTests/`
```

---

### #B33 — test: integration tests for Project endpoints

```
Create integration tests for all Project endpoints in `backend/tests/Dev4All.IntegrationTests/Projects/`.

Use the `CustomWebApplicationFactory` from B32.

**`ProjectEndpointTests.cs`**
- `CreateProject_AsCustomer_Returns201`
- `CreateProject_AsDeveloper_Returns403`
- `GetProjects_ReturnsPagedList`
- `GetProjectById_ExistingProject_Returns200`
- `GetProjectById_NonExistent_Returns404`
- `UpdateProject_AsOwner_Returns200`
- `UpdateProject_AsNonOwner_Returns403`
- `DeleteProject_AsOwner_Returns204`
- `GetMyProjects_AsCustomer_ReturnsOwnProjects`
```

---

### #B34 — test: integration tests for Bid + Contract endpoints

```
Create integration tests for Bid and Contract endpoints.

**`BidEndpointTests.cs`**
- `PlaceBid_AsDeveloper_Returns201`
- `PlaceBid_AsCustomer_Returns403`
- `PlaceBid_DuplicateBid_Returns400`
- `AcceptBid_AsProjectOwner_Returns200AndCreatesContract`

**`ContractEndpointTests.cs`**
- `ReviseContract_AsParty_Returns200`
- `ApproveContract_BothParties_MovesProjectToOngoing`
- `CancelContract_AsParty_CancelsProject`
```

---

### #B35 — feat: Serilog structured logging

```
You are a senior .NET backend developer working on the Dev4All project.

## Task

Add Serilog structured logging to the backend.

### Steps:
1. Add NuGet to `Dev4All.WebAPI`:
   - `Serilog.AspNetCore`
   - `Serilog.Sinks.Console`
   - `Serilog.Sinks.File`

2. Configure in `Program.cs`:
   ```csharp
   builder.Host.UseSerilog((context, config) =>
       config.ReadFrom.Configuration(context.Configuration));
   ```

3. Add Serilog config to `appsettings.json`:
   ```json
   "Serilog": {
     "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Warning" } },
     "WriteTo": [
       { "Name": "Console" },
       { "Name": "File", "Args": { "path": "logs/dev4all-.log", "rollingInterval": "Day" } }
     ]
   }
   ```

4. Add request logging middleware: `app.UseSerilogRequestLogging()`
5. Update `GlobalExceptionMiddleware` to use Serilog's structured format
6. Verify build
```

---

### #B36 — test: code coverage report

```
Configure code coverage collection and reporting.

1. Ensure `coverlet.collector` is in both test projects
2. Add `coverlet.msbuild` for threshold enforcement
3. Run: `dotnet test --collect:"XPlat Code Coverage"`
4. Add coverage report generation to CI workflow
5. Set minimum coverage threshold: 80%
6. Document gaps and create follow-up issues if needed
```

---

## Sprint 7 — Polish & Docs

---

### #B37 — docs: Swagger documentation improvements

```
Improve Swagger/OpenAPI documentation.

1. Add XML comment generation to all API projects (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`)
2. Configure Swagger to include XML comments
3. Add `[ProducesResponseType]` attributes to all controller actions
4. Group endpoints by tag/controller
5. Add example request/response values where helpful
6. Verify Swagger UI renders correctly
```

---

### #B38 — refactor: performance review

```
Review and optimize backend performance.

1. Audit all repository queries for N+1 problems
2. Ensure list queries use `.AsNoTracking()` and projection (`Select`)
3. Review `Include()` usage — only include what's needed
4. Check pagination queries use server-side paging (not loading all then slicing)
5. Add composite indexes if query patterns suggest them
6. Document findings and create migration if indexes added
```

---

### #B39 — refactor: API versioning audit

```
Ensure consistent API versioning across all controllers.

1. Verify all controllers use `[Route("api/v1/[controller]")]`
2. Review route consistency (RESTful naming, plural nouns)
3. Ensure WebhookController follows the same versioning
4. Document the versioning strategy in README
```

---

### #B40 — docs: Backend README update

```
Update `backend/README.md` with:

1. Project overview and architecture diagram
2. Prerequisites (.NET 10, PostgreSQL, user-secrets)
3. Getting started guide (clone, restore, migrate, run)
4. API endpoint summary table
5. Testing instructions
6. Configuration reference (appsettings sections)
7. Deployment notes
```

---

## CI Workflow Requirement

Before starting development, ensure a `.github/workflows/dotnet-develop.yml` exists that:
- Triggers on push/PR to `develop` for `backend/**` changes
- Runs `dotnet restore`, `dotnet build`, `dotnet test`
- Job name: `Build & Test Backend (develop)` — used as required status check
