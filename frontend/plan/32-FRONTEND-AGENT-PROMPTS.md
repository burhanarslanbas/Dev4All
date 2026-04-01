# 32. Frontend (MVC) Agent Prompts — All Issues

> Copy-paste ready agent prompts. Execution order follows dependency graph.  
> **Cross-reference:** Operasyonel kısa sürüm: `docs/plan/15-frontend-agent-prompts.md`  
> Issue eşleşme: #F01 = GitHub #72, #F02 = #73, ..., #F34 = #105.

---

## Full Delivery Sırası (Baştan Sona Dependency-Aware)

Bu sıra dependency bazlıdır; issue'ları baştan sona bu akışta uygula.

```
── Sprint 1: Scaffold & Auth ─────────────────────────────────
1.  #F01 — MVC project scaffold (S1)
2.  #F02 — API client infrastructure (S1)
3.  #F03 — Login page + cookie auth (S1)
4.  #F04 — Register page (S1)
5.  #F05 — Shared layout (S1)
6.  #F06 — Landing page (S1)

── Sprint 2: Project Pages ───────────────────────────────────
7.  #F07 — Project list page (S2)
8.  #F08 — Project detail page (S2)
9.  #F09 — Create project form (S2)
10. #F10 — Edit project form (S2)
11. #F11 — My projects page (S2)
12. #F12 — Delete project modal (S2)

── Sprint 3: Bid Pages ───────────────────────────────────────
13. #F13 — Place bid form (S3)
14. #F14 — Project bids list (S3)
15. #F15 — Accept bid button (S3)
16. #F16 — My bids page (S3)
17. #F17 — Update bid form (S3)

── Sprint 4: Contract Pages ──────────────────────────────────
18. #F18 — Contract view page (S4)
19. #F19 — Contract edit/revise page (S4)
20. #F20 — Approve contract button (S4)
21. #F21 — Cancel contract button (S4)
22. #F22 — Contract revision history (S4)

── Sprint 5: GitHub & Dashboard ──────────────────────────────
23. #F23 — Link GitHub repo form (S5)
24. #F24 — GitHub activity timeline (S5)
25. #F25 — Customer dashboard (S5)
26. #F26 — Developer dashboard (S5)

── Sprint 6: UX Polish ───────────────────────────────────────
27. #F27 — Client-side validation (S6)
28. #F28 — Toast notifications (S6)
29. #F29 — Loading & empty states (S6)
30. #F30 — Responsive design audit (S6)
31. #F31 — Error pages (S6)

── Sprint 7: Testing & Docs ──────────────────────────────────
32. #F32 — Integration tests (S7)
33. #F33 — Accessibility audit (S7)
34. #F34 — Frontend README (S7)
```

---

## Auth Odaklı Hızlı Sıra (sadece auth hedefi için)

Eğer sadece auth hedefli bir gün planı yapıyorsan frontend tarafında şu kısa rotayı kullan:

```
1. #F01 — MVC project scaffold + Program.cs + Bootstrap layout
2. #F02 — API client infrastructure (HttpClient + JWT handler)
3. #F03 — Login page + AuthService + cookie authentication
4. #F04 — Register page + role selection
```

> **Not:**  
> - #F03 ve #F04 için #F01 ve #F02 tamamlanmış olmalı.  
> - Auth akışı **Backend Sprint 0** (#108-#119) tamamlanmış olmalı — özellikle `POST /auth/login` ve `POST /auth/register` endpoint'leri.  
> - Backend'de 10 auth endpoint bulunur: register, login, me, refresh-token, logout, confirm-email, forgot-password, reset-password, change-password, resend-confirmation.

---

## Sprint 1 — Scaffold & Auth

---

### #F01 — feat: MVC project scaffold + Program.cs + Bootstrap layout

**Sıra (Order):** 1

**Uygulama Sırası / Order:** 1

```
You are a senior ASP.NET Core developer building the Dev4All web frontend.

## Context

Dev4All is a B2B freelance marketplace. The frontend is an ASP.NET Core MVC (.NET 10) application that communicates with a backend Web API via HttpClient.

READ FIRST:
- `docs/AGENTS.md` — coding standards
- `frontend/plan/30-FRONTEND-OVERVIEW.md` — architecture, page map, auth flow
- `backend/src/Presentation/Dev4All.WebAPI/appsettings.json` — backend API config

## Task

Create the MVC project scaffold at `frontend/Dev4All.Web/`.

### Steps:

1. **Create project:**
   ```bash
   dotnet new mvc -n Dev4All.Web -o frontend/Dev4All.Web --use-program-main false
   ```

2. **Configure `Program.cs`:**
   - Add Cookie Authentication:
     ```csharp
     builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
         .AddCookie(options =>
         {
             options.LoginPath = "/auth/login";
             options.AccessDeniedPath = "/auth/access-denied";
             options.ExpireTimeSpan = TimeSpan.FromHours(2);
         });
     ```
   - Add HttpClient with base address pointing to backend API
   - Add authorization services
   - Configure CORS if needed
   - Middleware order: Exception → HTTPS → Static Files → Routing → Auth → Authorization → MapControllers

3. **Configure `appsettings.json`:**
   ```json
   {
     "BackendApi": {
       "BaseUrl": "https://localhost:7001/api/v1/"
     }
   }
   ```

4. **Add Bootstrap 5 via LibMan or CDN** in `_Layout.cshtml`

5. **Create base `_Layout.cshtml`:**
   - Responsive navbar with brand "Dev4All"
   - Navigation links (Home, Projects)
   - Login/Register or User dropdown (partial)
   - Footer with copyright
   - Bootstrap 5 CSS + JS references
   - `@RenderBody()` and `@RenderSection("Scripts", required: false)`

6. **Verify build:** `dotnet build frontend/Dev4All.Web/`

### Rules:
- Use file-scoped namespaces
- Use modern .NET 10 / C# 13 patterns
- Keep Program.cs clean and organized
- No business logic in controllers
```

---

### #F02 — feat: API client infrastructure (HttpClient + JWT handler)

**Sıra (Order):** 2

**Uygulama Sırası / Order:** 2

```
You are a senior ASP.NET Core developer building the Dev4All web frontend.

## Context

READ FIRST:
- `frontend/plan/30-FRONTEND-OVERVIEW.md` — auth flow, architecture
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs` — API endpoints

## Task

Create the API client infrastructure in `frontend/Dev4All.Web/Services/` and `frontend/Dev4All.Web/Infrastructure/`.

### Files to create:

1. **`Infrastructure/ApiTokenHandler.cs`** — `DelegatingHandler`
   - Reads JWT token from the user's authentication cookie claims
   - Attaches `Authorization: Bearer {token}` header to outgoing HttpClient requests
   - If no token found, request proceeds without auth header

2. **`Services/IApiClient.cs`** — generic API client interface
   ```csharp
   public interface IApiClient
   {
       Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
       Task<T?> PostAsync<T>(string endpoint, object body, CancellationToken ct = default);
       Task<T?> PutAsync<T>(string endpoint, object body, CancellationToken ct = default);
       Task DeleteAsync(string endpoint, CancellationToken ct = default);
   }
   ```

3. **`Services/ApiClient.cs`** — implementation using `HttpClient`
   - Inject `HttpClient` (typed client)
   - Use `System.Text.Json` for serialization
   - Handle HTTP error responses (400 → validation errors, 401 → redirect to login, 403 → access denied, 404 → not found, 500 → generic error)
   - Return deserialized response or throw appropriate exception

4. **Register in `Program.cs`:**
   ```csharp
   builder.Services.AddTransient<ApiTokenHandler>();
   builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
   {
       client.BaseAddress = new Uri(builder.Configuration["BackendApi:BaseUrl"]!);
   })
   .AddHttpMessageHandler<ApiTokenHandler>();
   ```

### Rules:
- Use `System.Text.Json` with camelCase naming policy
- Handle all HTTP error codes gracefully
- Verify build
```

---

### #F03 — feat: Login page + AuthService + cookie authentication

**Sıra (Order):** 3

**Uygulama Sırası / Order:** 3

```
You are a senior ASP.NET Core developer building the Dev4All web frontend.

## Context

READ FIRST:
- `frontend/plan/30-FRONTEND-OVERVIEW.md` — auth flow diagram
- `backend/src/Core/Dev4All.Application/Features/Auth/Commands/LoginUser/LoginUserCommand.cs`
- `backend/src/Core/Dev4All.Application/Features/Auth/Commands/LoginUser/LoginUserResponse.cs`

## Task

### Files to create:

1. **`Models/Auth/LoginViewModel.cs`**
   ```csharp
   public sealed class LoginViewModel
   {
       [Required, EmailAddress]
       public string Email { get; set; } = string.Empty;
       [Required, MinLength(8)]
       public string Password { get; set; } = string.Empty;
       public string? ReturnUrl { get; set; }
   }
   ```

2. **`Models/Auth/LoginApiResponse.cs`** — matches backend response
   ```csharp
   public sealed record LoginApiResponse(string Token, DateTime ExpiresAt, string Email, string Role);
   ```

3. **`Services/IAuthService.cs`**
   ```csharp
   public interface IAuthService
   {
       Task<LoginApiResponse?> LoginAsync(string email, string password, CancellationToken ct = default);
       Task<RegisterApiResponse?> RegisterAsync(string name, string email, string password, string role, CancellationToken ct = default);
   }
   ```

4. **`Services/AuthService.cs`** — calls backend `/auth/login` and `/auth/register`

5. **`Controllers/AuthController.cs`**
   - `GET /auth/login` → returns Login view
   - `POST /auth/login`:
     a. Validate model
     b. Call `AuthService.LoginAsync()`
     c. On success: create `ClaimsIdentity` with claims (UserId from JWT sub, Email, Role, and store raw JWT token as a claim)
     d. `HttpContext.SignInAsync()` with cookie
     e. Redirect to ReturnUrl or Dashboard
     f. On failure: add ModelError, return view
   - `POST /auth/logout` → `HttpContext.SignOutAsync()` → redirect to home

6. **`Views/Auth/Login.cshtml`**
   - Clean Bootstrap 5 form: email, password, remember me checkbox
   - Validation error display
   - Link to register page
   - Card layout, centered on page

### Rules:
- Parse JWT claims to extract UserId (sub claim)
- Store JWT token as a custom claim for API calls
- Use `[ValidateAntiForgeryToken]` on POST actions
- Verify build
```

---

### #F04 — feat: Register page + role selection

**Sıra (Order):** 4

**Uygulama Sırası / Order:** 4

```
You are a senior ASP.NET Core developer building the Dev4All web frontend.

## Task

Create register functionality.

### Files:

1. **`Models/Auth/RegisterViewModel.cs`** — Name, Email, Password, ConfirmPassword, Role (dropdown: Customer/Developer)
2. **`Models/Auth/RegisterApiResponse.cs`** — matches backend
3. **`Views/Auth/Register.cshtml`** — Bootstrap form with role selection dropdown
4. **`AuthController` additions** — GET/POST Register actions

### Logic:
- POST → call `AuthService.RegisterAsync()`
- On success → redirect to login with "Registration successful" message
- On failure → show validation errors from backend

Verify build.
```

---

### #F05 — feat: shared layout with navbar and login partial

**Sıra (Order):** 5

**Uygulama Sırası / Order:** 5

```
Update `Views/Shared/_Layout.cshtml` and create `_LoginPartial.cshtml`.

### _LoginPartial.cshtml:
- If user is authenticated: show user name/email, role badge, dropdown with Dashboard + Logout
- If not authenticated: show Login and Register buttons

### _Layout.cshtml navbar:
- Brand: "Dev4All" with logo placeholder
- Links: Home, Projects (always visible)
- Right side: _LoginPartial
- Mobile hamburger menu (Bootstrap collapse)

### Styling:
- Primary color: #2563EB (Tech Blue)
- Clean, professional look
- Responsive

Verify build.
```

---

### #F06 — feat: landing page (home)

**Sıra (Order):** 6

**Uygulama Sırası / Order:** 6

```
Create an attractive landing page at `Views/Home/Index.cshtml`.

### Sections:
1. Hero section: tagline "Find the perfect developer for your project", CTA buttons (Post Project / Browse Projects)
2. How it works: 3-step process (Post → Bid → Build) with icons
3. Features: project management, bid system, GitHub tracking, contract management
4. CTA: Register now

### Design:
- Bootstrap 5 grid
- Clean typography, whitespace
- Professional blue/white color scheme
- Responsive

Update `HomeController` if needed. Verify build.
```

---

## Sprint 2 — Project Pages

---

### #F07 — feat: project list page (paginated, filterable)

**Sıra (Order):** 7

**Uygulama Sırası / Order:** 7

```
You are a senior ASP.NET Core developer building the Dev4All web frontend.

## Context

READ FIRST:
- `frontend/plan/30-FRONTEND-OVERVIEW.md`
- Backend endpoint: GET /api/v1/projects?page=1&pageSize=10

## Task

### Files:

1. **`Services/IProjectService.cs` + `ProjectService.cs`**
   - `GetProjectsAsync(int page, int pageSize)` → calls backend GET /projects
   - `GetProjectByIdAsync(Guid id)` → calls GET /projects/{id}
   - `GetMyProjectsAsync()` → calls GET /projects/my
   - `CreateProjectAsync(...)`, `UpdateProjectAsync(...)`, `DeleteProjectAsync(...)`

2. **`Models/Projects/ProjectListViewModel.cs`** — items, pagination info
3. **`Models/Projects/ProjectListItemDto.cs`** — Id, Title, Description, Budget, Deadline, BidCount, Status, Technologies

4. **`Controllers/ProjectsController.cs`**
   - `GET /projects` → Index action with page/pageSize query params
   - Injects `IProjectService`

5. **`Views/Projects/Index.cshtml`**
   - Card grid layout showing project cards
   - Each card: title, budget, deadline, bid count, technologies tags, status badge
   - Pagination controls (Previous/Next/Page numbers)
   - Search bar (client-side filter or future server-side)
   - "Create Project" button for Customer role

### Design:
- Bootstrap card grid (3 columns desktop, 1 mobile)
- Budget displayed as currency
- Deadline with countdown or date format
- Status badges with colors (Open=green, AwaitingContract=yellow, Ongoing=blue, Completed=gray)

Register `IProjectService` in DI. Verify build.
```

---

### #F08 — feat: project detail page

**Sıra (Order):** 8

**Uygulama Sırası / Order:** 8

```
Create project detail page at `Views/Projects/Detail.cshtml`.

### Route: GET /projects/{id}

### Content:
- Project info: title, description, budget, deadline, bid end date, technologies, status
- Status badge with color
- Sidebar: project owner info, assigned developer (if any)
- Conditional sections by role:
  - Customer (owner): Edit/Delete buttons, bid list
  - Developer: Place Bid button (if Open + bid period active)
  - Any party (if Ongoing): GitHub timeline link, contract link
- Bid count indicator

### ViewModel: `ProjectDetailViewModel` with full project data + user role context

Verify build.
```

---

### #F09 — feat: create project form

**Sıra (Order):** 9

**Uygulama Sırası / Order:** 9

```
Create project creation form.

### Route: GET/POST /projects/create

### Files:
1. **`Models/Projects/CreateProjectViewModel.cs`** — Title, Description, Budget, Deadline, BidEndDate, Technologies
   - DataAnnotations: Required, StringLength, Range
2. **`Views/Projects/Create.cshtml`** — Bootstrap form
   - Title input, Description textarea, Budget number input, Deadline date picker, BidEndDate date picker, Technologies input
   - Client-side validation
3. **`ProjectsController.Create` actions**
   - GET: return form view (Customer only)
   - POST: validate → call `ProjectService.CreateProjectAsync()` → redirect to detail page
   - Handle backend validation errors

### Authorization: `[Authorize(Roles = "Customer")]`

Verify build.
```

---

### #F10 — feat: edit project form

**Sıra (Order):** 10

**Uygulama Sırası / Order:** 10

```
### Route: GET/POST /projects/{id}/edit
- Pre-fill form with existing project data
- Only project owner (Customer) can edit
- Only Open projects can be edited
- Same validation as Create
- On success: redirect to detail page

Verify build.
```

---

### #F11 — feat: My Projects page (Customer)

**Sıra (Order):** 11

**Uygulama Sırası / Order:** 11

```
### Route: GET /projects/my
- Call ProjectService.GetMyProjectsAsync()
- Table/card view of customer's own projects
- Status badges, action links (View, Edit, Delete)
- Empty state if no projects

Authorization: Customer role only.
Verify build.
```

---

### #F12 — feat: delete project (confirmation modal)

**Sıra (Order):** 12

**Uygulama Sırası / Order:** 12

```
- Bootstrap modal confirmation dialog
- AJAX POST to delete endpoint
- Only for Open projects owned by current user
- On success: remove card/row from page, show toast
- On failure: show error toast

Verify build.
```

---

## Sprint 3 — Bid Pages

---

### #F13 — feat: place bid form

**Sıra (Order):** 13

**Uygulama Sırası / Order:** 13

```
Create bid placement form on the project detail page.

### Route: POST /projects/{projectId}/bids/create

### Implementation:
1. **`Services/IBidService.cs` + `BidService.cs`**
   - `PlaceBidAsync(Guid projectId, decimal amount, string proposalNote)`
   - `GetProjectBidsAsync(Guid projectId)`
   - `GetMyBidsAsync()`
   - `UpdateBidAsync(Guid id, decimal amount, string proposalNote)`
   - `AcceptBidAsync(Guid bidId)`

2. **`Models/Bids/PlaceBidViewModel.cs`** — ProjectId, BidAmount, ProposalNote
3. **`Views/Projects/_PlaceBidPartial.cshtml`** — inline form on project detail
4. **`Controllers/BidsController.cs`** — POST action

### Business rules shown in UI:
- Only Developer role can see the form
- Project must be Open
- Bid period must not have ended
- Show error if developer already bid on this project

Register IBidService in DI. Verify build.
```

---

### #F14 — feat: project bids list (Customer view)

**Sıra (Order):** 14

**Uygulama Sırası / Order:** 14

```
Show all bids on a project for the Customer (project owner).

### Implementation:
- Partial view `_BidListPartial.cshtml` rendered on project detail page
- Table: Developer name, bid amount, proposal note, status, date
- "Accept" button on each Pending bid row
- Only visible to project owner (Customer)

Verify build.
```

---

### #F15 — feat: accept bid button + confirmation

**Sıra (Order):** 15

**Uygulama Sırası / Order:** 15

```
- Confirmation modal: "Are you sure? This will reject all other bids."
- POST to backend AcceptBid endpoint
- On success: page refresh showing new AwaitingContract status
- On failure: error toast

Verify build.
```

---

### #F16 — feat: My Bids page (Developer)

**Sıra (Order):** 16

**Uygulama Sırası / Order:** 16

```
### Route: GET /bids/my
- Call BidService.GetMyBidsAsync()
- Table: project title, bid amount, proposal note, status badge, date
- Link to project detail
- Status badges: Pending=yellow, Accepted=green, Rejected=red

Authorization: Developer role only.
Verify build.
```

---

### #F17 — feat: update bid form

**Sıra (Order):** 17

**Uygulama Sırası / Order:** 17

```
- Modal or inline edit form on My Bids page
- Pre-fill current amount and proposal
- Only for Pending bids
- POST to UpdateBid endpoint
- Refresh row on success

Verify build.
```

---

## Sprint 4 — Contract Pages

---

### #F18 — feat: contract view page

**Sıra (Order):** 18

**Uygulama Sırası / Order:** 18

```
Create contract view page.

### Route: GET /contracts/{projectId}

### Files:
1. **`Services/IContractService.cs` + `ContractService.cs`**
   - `GetContractAsync(Guid projectId)`
   - `ReviseContractAsync(Guid projectId, string content, string? note)`
   - `ApproveContractAsync(Guid projectId)`
   - `CancelContractAsync(Guid projectId)`
   - `GetRevisionsAsync(Guid projectId)`

2. **`Models/Contracts/ContractViewModel.cs`**
3. **`Views/Contracts/Detail.cshtml`**

### Content:
- Contract content (rendered text)
- Status badge (Draft, UnderReview, BothApproved, Cancelled)
- Approval status indicators: Customer ✅/❌, Developer ✅/❌
- Action buttons: Edit, Approve, Cancel (conditional)
- Revision count + link to history
- Project info sidebar

Register IContractService in DI. Verify build.
```

---

### #F19 — feat: contract edit/revise page

**Sıra (Order):** 19

**Uygulama Sırası / Order:** 19

```
### Route: GET/POST /contracts/{projectId}/edit
- Textarea with current contract content
- Optional revision note field
- "Save Revision" button
- Warning: "Saving will reset the other party's approval"
- POST → ContractService.ReviseContractAsync()
- Redirect to contract view on success

Only for Draft/UnderReview contracts, only project parties.
Verify build.
```

---

### #F20 — feat: approve contract button

**Sıra (Order):** 20

**Uygulama Sırası / Order:** 20

```
- Button on contract detail page
- Confirmation modal: "This will register your approval"
- POST → ContractService.ApproveContractAsync()
- If both approved: show "Contract fully approved! Project is now Ongoing"
- Refresh page on success

Verify build.
```

---

### #F21 — feat: cancel contract button + confirmation

**Sıra (Order):** 21

**Uygulama Sırası / Order:** 21

```
- Red "Cancel Contract" button
- Serious confirmation modal: "This will cancel the contract AND the project"
- POST → ContractService.CancelContractAsync()
- Redirect to project detail on success

Verify build.
```

---

### #F22 — feat: contract revision history

**Sıra (Order):** 22

**Uygulama Sırası / Order:** 22

```
### Route: GET /contracts/{projectId}/revisions
- Call ContractService.GetRevisionsAsync()
- Timeline/table showing each revision
- Fields: revision number, revised by, date, note, content snapshot (expandable)
- Ordered by revision number descending

Verify build.
```

---

## Sprint 5 — GitHub & Dashboard

---

### #F23 — feat: link GitHub repo form (Developer)

**Sıra (Order):** 23

**Uygulama Sırası / Order:** 23

```
Create form for Developer to link GitHub repo to project.

### Route: POST /projects/{id}/repo
- Input fields: Repository URL, Branch (optional, default "main")
- URL validation (must contain github.com)
- Only visible to assigned Developer on Ongoing projects
- POST → backend PUT /projects/{id}/repo
- Success: show repo linked confirmation, refresh timeline section

Verify build.
```

---

### #F24 — feat: GitHub activity timeline component

**Sıra (Order):** 24

**Uygulama Sırası / Order:** 24

```
Create a timeline component showing GitHub commit activity.

### Displayed on: Project detail page (for Ongoing projects)

### Implementation:
- Partial view `_GitHubTimelinePartial.cshtml`
- Call backend GET /projects/{id}/github-logs
- Display as vertical timeline:
  - Each entry: commit message, author, branch, timestamp
  - Commit hash (abbreviated, linked to GitHub)
  - Color-coded by branch
- Empty state: "No activity yet. Developer needs to link a GitHub repo."
- Auto-refresh option (AJAX polling or manual refresh button)

### Design:
- Clean vertical timeline with left border
- Responsive
- Max 50 entries, "Load more" if needed

Verify build.
```

---

### #F25 — feat: Customer dashboard

**Sıra (Order):** 25

**Uygulama Sırası / Order:** 25

```
### Route: GET /dashboard (Customer role)

### Content:
- Stats cards: total projects, active projects, pending bids count
- Recent projects table/cards (top 5)
- Recent activity feed (new bids, contract updates)
- Quick action buttons: Create Project, View All Projects

### Design:
- Bootstrap grid layout
- Stats at top in colored cards
- Tables below

Verify build.
```

---

### #F26 — feat: Developer dashboard

**Sıra (Order):** 26

**Uygulama Sırası / Order:** 26

```
### Route: GET /dashboard (Developer role)

### Content:
- Stats cards: active bids, accepted bids, assigned projects
- My active bids list (top 5)
- Assigned projects list
- Recent GitHub activity on assigned projects

### Design:
- Same layout pattern as Customer dashboard
- Different stat cards and content

Use `DashboardController` with role-based view selection.
Verify build.
```

---

## Sprint 6 — UX Polish

---

### #F27 — feat: client-side validation

**Sıra (Order):** 27

**Uygulama Sırası / Order:** 27

```
- Add jQuery Validation + Unobtrusive Validation
- Ensure all forms have proper client-side validation matching backend rules
- Date pickers with min date constraints
- Budget/amount fields with numeric validation
- Password strength indicator on register

Verify build.
```

---

### #F28 — feat: toast notifications

**Sıra (Order):** 28

**Uygulama Sırası / Order:** 28

```
- Implement Bootstrap toast notification system
- TempData-based flash messages from server (success, error, warning, info)
- JavaScript toast helper for AJAX operations
- Auto-dismiss after 5 seconds
- Position: top-right

Verify build.
```

---

### #F29 — feat: loading & empty states

**Sıra (Order):** 29

**Uygulama Sırası / Order:** 29

```
- Add loading spinners for AJAX operations
- Empty state components for lists with no data:
  - "No projects yet" with CTA
  - "No bids received" 
  - "No activity yet"
- Skeleton loading for page transitions (optional)

Verify build.
```

---

### #F30 — feat: responsive design audit

**Sıra (Order):** 30

**Uygulama Sırası / Order:** 30

```
- Test all pages on mobile (375px), tablet (768px), desktop (1200px+)
- Fix any Bootstrap grid issues
- Ensure tables are scrollable on mobile
- Test navigation menu collapse
- Fix any overflow issues
- Screenshot verification for key pages

Verify build.
```

---

### #F31 — feat: error pages (404, 403, 500)

**Sıra (Order):** 31

**Uygulama Sırası / Order:** 31

```
Create custom error pages.

### Files:
1. `Views/Shared/Error.cshtml` — generic error (500)
2. `Views/Shared/NotFound.cshtml` — 404
3. `Views/Shared/AccessDenied.cshtml` — 403

### Program.cs configuration:
```csharp
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseExceptionHandler("/error");
```

### Design:
- Friendly error messages
- "Go back" and "Home" buttons
- Consistent with site design

Verify build.
```

---

## Sprint 7 — Testing & Docs

---

### #F32 — test: integration tests

**Sıra (Order):** 32

**Uygulama Sırası / Order:** 32

```
Create integration tests for the MVC frontend.

### Setup:
1. Create test project: `frontend/Dev4All.Web.Tests/`
2. Add `Microsoft.AspNetCore.Mvc.Testing`, `FluentAssertions`
3. Use `WebApplicationFactory<Program>` with mocked backend API (WireMock or similar)

### Tests:
- `HomePage_ReturnsSuccess`
- `LoginPage_ReturnsForm`
- `LoginPost_ValidCredentials_RedirectsToDashboard`
- `Projects_RequiresAuthentication`
- `CreateProject_AsCustomer_ReturnsForm`
- `CreateProject_AsDeveloper_ReturnsForbidden`

Verify all tests pass.
```

---

### #F33 — feat: accessibility audit

**Sıra (Order):** 33

**Uygulama Sırası / Order:** 33

```
- Add ARIA labels to all interactive elements
- Ensure proper heading hierarchy (h1 → h2 → h3)
- Check color contrast ratios (WCAG AA)
- Add alt text to all images
- Ensure keyboard navigation works for all forms
- Test with browser accessibility tools

Verify build.
```

---

### #F34 — docs: Frontend README update

**Sıra (Order):** 34

**Uygulama Sırası / Order:** 34

```
Update `frontend/README.md` with:
1. Project overview (ASP.NET Core MVC)
2. Prerequisites (.NET 10)
3. Getting started (clone, configure appsettings, run)
4. Page structure and routing table
5. Architecture overview (Services → HttpClient → Backend API)
6. Configuration reference
7. Development workflow

Verify build.
```

---

## CI Workflow

The frontend needs a `.github/workflows/mvc-develop.yml` that:
- Triggers on push/PR to develop for `frontend/**` changes
- Runs `dotnet restore`, `dotnet build`, `dotnet test`
- Job name: `Build & Test Frontend (develop)`
