# Dev4All Frontend (MVC) — All Issue Agent Prompts

Bu dosya, GitHub'daki açık frontend issue'lar için kolayca kopyalanıp agent'a verilebilecek hazır prompt'ları içerir.

> **Cross-reference:** Detaylı prompt içerikleri: `frontend/plan/32-FRONTEND-AGENT-PROMPTS.md`  
> Issue eşleşme: #72 = #F01, #73 = #F02, ..., #105 = #F34.  
> **Backend dependency:** Auth akışı backend Sprint 0 (#108-#119) tamamlanmış olmalı.

## Kullanım

1. GitHub issue numarasını seç.
2. Aşağıdaki ilgili prompt'u kopyala.
3. Agent ekranında:
   - Base branch: `develop`
   - Model: `GPT-5.3-Codex` (önerilen)
4. Prompt'u yapıştırıp çalıştır.

## Baştan Sona Uygulama Sırası (Önce Yapılacaklara Göre)

Bu sıra, issue numarasına değil bağımlılıklara göre optimize edilmiştir.
Issue'ları baştan sona bu sırayla uygula; prompt içerikleri değişmeden kalır.

1. `#72` — MVC project scaffold (S1)
2. `#73` — API client infrastructure (S1)
3. `#74` — Login page + cookie auth (S1)
4. `#75` — Register page (S1)
5. `#76` — Shared layout (S1)
6. `#77` — Landing page (S1)
7. `#78` — Project list page (S2)
8. `#79` — Project detail page (S2)
9. `#80` — Create project form (S2)
10. `#81` — Edit project form (S2)
11. `#82` — My projects page (S2)
12. `#83` — Delete project modal (S2)
13. `#84` — Place bid form (S3)
14. `#85` — Project bids list (S3)
15. `#86` — Accept bid button (S3)
16. `#87` — My bids page (S3)
17. `#88` — Update bid form (S3)
18. `#89` — Contract view page (S4)
19. `#90` — Contract edit/revise page (S4)
20. `#91` — Approve contract button (S4)
21. `#92` — Cancel contract button (S4)
22. `#93` — Contract revision history (S4)
23. `#94` — Link GitHub repo form (S5)
24. `#95` — GitHub activity timeline (S5)
25. `#96` — Customer dashboard (S5)
26. `#97` — Developer dashboard (S5)
27. `#98` — Client-side validation (S6)
28. `#99` — Toast notifications (S6)
29. `#100` — Loading & empty states (S6)
30. `#101` — Responsive design audit (S6)
31. `#102` — Error pages (S6)
32. `#103` — Integration tests (S7)
33. `#104` — Accessibility audit (S7)
34. `#105` — Frontend README (S7)

---

## Auth Odaklı Hızlı Sıra (Sadece bugün auth hedefi için)

Genel geliştirme sırası yukarıdaki dependency sırasıdır.  
Eğer sadece auth hedefli bir gün planı yapıyorsan frontend tarafında şu kısa rotayı kullan:

1. `#72` — MVC project scaffold + Program.cs + Bootstrap layout
2. `#73` — API client infrastructure (HttpClient + JWT handler)
3. `#74` — Login page + AuthService + cookie authentication
4. `#75` — Register page + role selection

Not:
- `#74` ve `#75` için `#72` ve `#73` tamamlanmış olmalı.
- Auth akışı backend Auth endpointlerine bağımlıdır.

---

## Global Prompt Header (Her issue için üstte kalsın)

```md
You are a senior ASP.NET Core MVC developer working in repository `Dev4All`.

General constraints:
- Follow `docs/AGENTS.md` and `frontend/plan/*.md`.
- Keep changes scoped to this issue only.
- Use Conventional Commits.
- Open PR to `develop`.
- Include `Closes #<ISSUE_NUMBER>` in PR body.
- Run and report:
  - `dotnet build frontend/Dev4All.Web/`
  - `dotnet test frontend/Dev4All.Web.Tests/` (if test project exists)

Output format required:
1) Summary
2) Files changed
3) Validation steps + results
4) Known blockers (if any)
```

---

## Sprint 1 — Scaffold & Auth

---

## #72 — feat: MVC project scaffold + Program.cs + Bootstrap layout

**Sıra (Order):** 1

**Uygulama Sırası / Order:** 1

```md
<GLOBAL HEADER>

Issue: #72
Sıra: 1
Goal:
- Create ASP.NET Core MVC project, configure cookie auth, HttpClient, Bootstrap 5 layout.

Tasks:
1) Create project: `dotnet new mvc -n Dev4All.Web -o frontend/Dev4All.Web --use-program-main false`
2) Configure Program.cs:
   - Cookie Authentication (LoginPath=/auth/login, ExpireTimeSpan=2h)
   - HttpClient with backend base URL
   - Authorization services
   - Middleware order: Exception → HTTPS → Static Files → Routing → Auth → Authorization → MapControllers
3) Configure appsettings.json with BackendApi.BaseUrl.
4) Add Bootstrap 5 to _Layout.cshtml (CDN or LibMan).
5) Create base _Layout.cshtml: navbar (Dev4All brand), footer, @RenderBody, @RenderSection.

Key files:
- `frontend/plan/30-FRONTEND-OVERVIEW.md`
- `backend/src/Presentation/Dev4All.WebAPI/appsettings.json`

Constraints:
- .NET 10 / C# 13, file-scoped namespaces.
- Verify: `dotnet build frontend/Dev4All.Web/`
```

---

## #73 — feat: API client infrastructure (HttpClient + JWT handler)

**Sıra (Order):** 2

**Uygulama Sırası / Order:** 2

```md
<GLOBAL HEADER>

Issue: #73
Sıra: 2
Goal:
- Create generic API client + JWT DelegatingHandler for backend communication.

Tasks:
1) Create `Infrastructure/ApiTokenHandler.cs` (DelegatingHandler):
   - Read JWT from cookie claims → attach Authorization: Bearer header.
2) Create `Services/IApiClient.cs` + `Services/ApiClient.cs`:
   - GetAsync<T>, PostAsync<T>, PutAsync<T>, DeleteAsync
   - System.Text.Json with camelCase, handle 400/401/403/404/500.
3) Register in Program.cs: AddTransient<ApiTokenHandler> + AddHttpClient<IApiClient, ApiClient> + AddHttpMessageHandler.

Key files:
- `frontend/plan/30-FRONTEND-OVERVIEW.md` (auth flow)

Constraints:
- Handle all HTTP error codes gracefully.
```

---

## #74 — feat: Login page + AuthService + cookie authentication

**Sıra (Order):** 3

**Uygulama Sırası / Order:** 3

```md
<GLOBAL HEADER>

Issue: #74
Sıra: 3
Goal:
- Implement login page, AuthService, and cookie-based JWT bridge authentication.

Tasks:
1) Create Models/Auth/LoginViewModel.cs (Email, Password, ReturnUrl).
2) Create Models/Auth/LoginApiResponse.cs (Token, ExpiresAt, Email, Role).
3) Create Services/IAuthService.cs + Services/AuthService.cs (LoginAsync, RegisterAsync).
4) Create Controllers/AuthController.cs:
   - GET /auth/login → view
   - POST /auth/login → call AuthService → parse JWT → create ClaimsIdentity → SignInAsync cookie → redirect
   - POST /auth/logout → SignOutAsync → redirect home
5) Create Views/Auth/Login.cshtml: Bootstrap form, validation, link to register.

Key files:
- `backend/src/Core/Dev4All.Application/Features/Auth/Commands/LoginUser/LoginUserResponse.cs`

Constraints:
- Store JWT token as custom claim for ApiTokenHandler.
- [ValidateAntiForgeryToken] on POST actions.
```

---

## #75 — feat: Register page + role selection

**Sıra (Order):** 4

**Uygulama Sırası / Order:** 4

```md
<GLOBAL HEADER>

Issue: #75
Sıra: 4
Goal:
- Implement register page with Customer/Developer role selection.

Tasks:
1) Create Models/Auth/RegisterViewModel.cs (Name, Email, Password, ConfirmPassword, Role dropdown).
2) Create Models/Auth/RegisterApiResponse.cs.
3) Create Views/Auth/Register.cshtml: Bootstrap form with role dropdown.
4) Add GET/POST Register actions to AuthController.
5) POST → AuthService.RegisterAsync() → success: redirect to login, failure: show errors.
```

---

## #76 — feat: Shared layout - navbar, login partial, footer

**Sıra (Order):** 5

**Uygulama Sırası / Order:** 5

```md
<GLOBAL HEADER>

Issue: #76
Sıra: 5
Goal:
- Update _Layout.cshtml and create _LoginPartial.cshtml.

Tasks:
1) _LoginPartial.cshtml:
   - Authenticated: user name/email, role badge, dropdown (Dashboard + Logout).
   - Not authenticated: Login/Register buttons.
2) _Layout.cshtml navbar:
   - Brand "Dev4All", links (Home, Projects), _LoginPartial.
   - Mobile hamburger menu (Bootstrap collapse).
3) Styling: primary #2563EB, clean professional look, responsive.
```

---

## #77 — feat: Landing page (home)

**Sıra (Order):** 6

**Uygulama Sırası / Order:** 6

```md
<GLOBAL HEADER>

Issue: #77
Sıra: 6
Goal:
- Create attractive landing page.

Tasks:
1) Hero section: tagline + CTA buttons (Post Project / Browse Projects).
2) How it works: 3-step process (Post → Bid → Build) with icons.
3) Features section: project management, bid system, GitHub tracking, contracts.
4) CTA: Register now.
5) Design: Bootstrap 5 grid, blue/white scheme, responsive.
```

---

## Sprint 2 — Project Pages

---

## #78 — feat: Project list page (paginated, filterable)

**Sıra (Order):** 7

**Uygulama Sırası / Order:** 7

```md
<GLOBAL HEADER>

Issue: #78
Sıra: 7
Goal:
- Build paginated project listing with card grid layout.

Tasks:
1) Create Services/IProjectService.cs + ProjectService.cs:
   - GetProjectsAsync, GetProjectByIdAsync, GetMyProjectsAsync, CreateProjectAsync, UpdateProjectAsync, DeleteProjectAsync
2) Create Models/Projects/ProjectListViewModel.cs + ProjectListItemDto.cs.
3) Create Controllers/ProjectsController.cs: GET /projects with page/pageSize.
4) Create Views/Projects/Index.cshtml:
   - Card grid (3 columns desktop, 1 mobile).
   - Each card: title, budget, deadline, bid count, technologies, status badge.
   - Pagination controls, "Create Project" button for Customer.
5) Status badge colors: Open=green, AwaitingContract=yellow, Ongoing=blue, Completed=gray.
6) Register IProjectService in DI.
```

---

## #79 — feat: Project detail page

**Sıra (Order):** 8

**Uygulama Sırası / Order:** 8

```md
<GLOBAL HEADER>

Issue: #79
Sıra: 8
Goal:
- Full project detail view with role-conditional sections.

Tasks:
1) Create Models/Projects/ProjectDetailViewModel.cs.
2) Create Views/Projects/Detail.cshtml:
   - Project info: title, description, budget, deadline, bid end date, technologies, status.
   - Sidebar: owner info, assigned developer.
   - Customer (owner): Edit/Delete buttons, bid list.
   - Developer: Place Bid button (if Open + active bid period).
   - Ongoing: GitHub timeline link, contract link.
3) GET /projects/{id} action in ProjectsController.
```

---

## #80 — feat: Create project form + validation

**Sıra (Order):** 9

**Uygulama Sırası / Order:** 9

```md
<GLOBAL HEADER>

Issue: #80
Sıra: 9
Goal:
- Create project creation form with server-side validation.

Tasks:
1) Create Models/Projects/CreateProjectViewModel.cs (DataAnnotations).
2) Create Views/Projects/Create.cshtml: Bootstrap form with all fields.
3) ProjectsController GET/POST /projects/create actions.
4) Authorization: [Authorize(Roles = "Customer")].
5) Handle backend validation errors.
```

---

## #81 — feat: Edit project form

**Sıra (Order):** 10

**Uygulama Sırası / Order:** 10

```md
<GLOBAL HEADER>

Issue: #81
Sıra: 10
Goal:
- Edit form pre-filled with existing project data.

Tasks:
1) GET/POST /projects/{id}/edit.
2) Pre-fill form, only project owner can edit, only Open projects.
3) Same validation as Create.
4) Redirect to detail on success.
```

---

## #82 — feat: My Projects page (Customer)

**Sıra (Order):** 11

**Uygulama Sırası / Order:** 11

```md
<GLOBAL HEADER>

Issue: #82
Sıra: 11
Goal:
- Customer's own projects listing.

Tasks:
1) GET /projects/my → ProjectService.GetMyProjectsAsync().
2) Table/card view: status badges, action links (View, Edit, Delete).
3) Empty state if no projects.
4) Authorization: Customer role only.
```

---

## #83 — feat: Delete project (confirmation modal)

**Sıra (Order):** 12

**Uygulama Sırası / Order:** 12

```md
<GLOBAL HEADER>

Issue: #83
Sıra: 12
Goal:
- Delete with Bootstrap modal confirmation.

Tasks:
1) Bootstrap modal confirmation dialog.
2) AJAX POST to delete endpoint.
3) Only for Open projects owned by current user.
4) On success: remove card/row + toast. On failure: error toast.
```

---

## Sprint 3 — Bid Pages

---

## #84 — feat: Place bid form (on project detail page)

**Sıra (Order):** 13

**Uygulama Sırası / Order:** 13

```md
<GLOBAL HEADER>

Issue: #84
Sıra: 13
Goal:
- Bid placement form for Developers, inline on project detail.

Tasks:
1) Create Services/IBidService.cs + BidService.cs (PlaceBid, GetProjectBids, GetMyBids, UpdateBid, AcceptBid).
2) Create Models/Bids/PlaceBidViewModel.cs.
3) Create Views/Projects/_PlaceBidPartial.cshtml.
4) Create Controllers/BidsController.cs: POST action.
5) UI rules: only Developer role, project Open, bid period active.
6) Register IBidService in DI.
```

---

## #85 — feat: Project bids list (Customer view)

**Sıra (Order):** 14

**Uygulama Sırası / Order:** 14

```md
<GLOBAL HEADER>

Issue: #85
Sıra: 14
Goal:
- Show all bids on a project for the project owner.

Tasks:
1) Partial view _BidListPartial.cshtml on project detail page.
2) Table: Developer name, bid amount, proposal note, status, date.
3) "Accept" button on each Pending bid row.
4) Only visible to project owner (Customer).
```

---

## #86 — feat: Accept bid button + confirmation

**Sıra (Order):** 15

**Uygulama Sırası / Order:** 15

```md
<GLOBAL HEADER>

Issue: #86
Sıra: 15
Goal:
- Confirmation modal for accepting a bid.

Tasks:
1) Modal: "Are you sure? This will reject all other bids."
2) POST to backend AcceptBid endpoint.
3) On success: page refresh showing AwaitingContract status.
4) On failure: error toast.
```

---

## #87 — feat: My Bids page (Developer)

**Sıra (Order):** 16

**Uygulama Sırası / Order:** 16

```md
<GLOBAL HEADER>

Issue: #87
Sıra: 16
Goal:
- Developer's bids listing.

Tasks:
1) GET /bids/my → BidService.GetMyBidsAsync().
2) Table: project title, bid amount, proposal note, status badge, date.
3) Link to project detail.
4) Status badges: Pending=yellow, Accepted=green, Rejected=red.
5) Authorization: Developer role only.
```

---

## #88 — feat: Update bid form

**Sıra (Order):** 17

**Uygulama Sırası / Order:** 17

```md
<GLOBAL HEADER>

Issue: #88
Sıra: 17
Goal:
- Edit bid amount and proposal for Pending bids.

Tasks:
1) Modal or inline edit form on My Bids page.
2) Pre-fill current amount and proposal.
3) Only for Pending bids.
4) POST to UpdateBid endpoint.
5) Refresh row on success.
```

---

## Sprint 4 — Contract Pages

---

## #89 — feat: Contract view page

**Sıra (Order):** 18

**Uygulama Sırası / Order:** 18

```md
<GLOBAL HEADER>

Issue: #89
Sıra: 18
Goal:
- Contract detail view with status and approval indicators.

Tasks:
1) Create Services/IContractService.cs + ContractService.cs (GetContract, Revise, Approve, Cancel, GetRevisions).
2) Create Models/Contracts/ContractViewModel.cs.
3) Create Views/Contracts/Detail.cshtml:
   - Contract content, status badge (Draft/UnderReview/BothApproved/Cancelled).
   - Approval indicators: Customer ✅/❌, Developer ✅/❌.
   - Action buttons: Edit, Approve, Cancel (conditional).
   - Revision count + link to history.
4) Register IContractService in DI.
```

---

## #90 — feat: Contract edit/revise page

**Sıra (Order):** 19

**Uygulama Sırası / Order:** 19

```md
<GLOBAL HEADER>

Issue: #90
Sıra: 19
Goal:
- Contract text revision form.

Tasks:
1) GET/POST /contracts/{projectId}/edit.
2) Textarea with current content, optional revision note.
3) Warning: "Saving will reset the other party's approval".
4) POST → ContractService.ReviseContractAsync().
5) Only for Draft/UnderReview contracts, only project parties.
```

---

## #91 — feat: Approve contract button

**Sıra (Order):** 20

**Uygulama Sırası / Order:** 20

```md
<GLOBAL HEADER>

Issue: #91
Sıra: 20
Goal:
- Approval button with confirmation modal.

Tasks:
1) Confirmation modal: "This will register your approval".
2) POST → ContractService.ApproveContractAsync().
3) If both approved: show "Project is now Ongoing!" message.
4) Refresh page on success.
```

---

## #92 — feat: Cancel contract button + confirmation

**Sıra (Order):** 21

**Uygulama Sırası / Order:** 21

```md
<GLOBAL HEADER>

Issue: #92
Sıra: 21
Goal:
- Cancel button with serious confirmation.

Tasks:
1) Red "Cancel Contract" button.
2) Modal: "This will cancel the contract AND the project".
3) POST → ContractService.CancelContractAsync().
4) Redirect to project detail on success.
```

---

## #93 — feat: Contract revision history view

**Sıra (Order):** 22

**Uygulama Sırası / Order:** 22

```md
<GLOBAL HEADER>

Issue: #93
Sıra: 22
Goal:
- Timeline/table showing contract revision history.

Tasks:
1) GET /contracts/{projectId}/revisions.
2) Call ContractService.GetRevisionsAsync().
3) Each row: revision number, revised by, date, note, expandable content snapshot.
4) Ordered by revision number descending.
```

---

## Sprint 5 — GitHub & Dashboard

---

## #94 — feat: Link GitHub repo form (Developer)

**Sıra (Order):** 23

**Uygulama Sırası / Order:** 23

```md
<GLOBAL HEADER>

Issue: #94
Sıra: 23
Goal:
- Form for Developer to link GitHub repository.

Tasks:
1) Inputs: Repository URL, Branch (optional, default "main").
2) URL validation (must contain github.com).
3) Only visible to assigned Developer on Ongoing projects.
4) POST → backend API endpoint.
5) Success: confirmation + refresh timeline section.
```

---

## #95 — feat: GitHub activity timeline component

**Sıra (Order):** 24

**Uygulama Sırası / Order:** 24

```md
<GLOBAL HEADER>

Issue: #95
Sıra: 24
Goal:
- Vertical timeline showing commit history.

Tasks:
1) Create _GitHubTimelinePartial.cshtml.
2) Call backend GET /projects/{id}/github-logs.
3) Vertical timeline: commit message, author, branch, timestamp, abbreviated hash linked to GitHub.
4) Empty state: "No activity yet."
5) Manual refresh button or AJAX polling.
6) Max 50 entries, "Load more" if needed.
```

---

## #96 — feat: Customer dashboard (stats, projects, contracts)

**Sıra (Order):** 25

**Uygulama Sırası / Order:** 25

```md
<GLOBAL HEADER>

Issue: #96
Sıra: 25
Goal:
- Customer dashboard with stats and recent activity.

Tasks:
1) GET /dashboard (Customer role).
2) Stats cards: total projects, active projects, pending bids count.
3) Recent projects table (top 5).
4) Quick action buttons: Create Project, View All Projects.
5) Bootstrap grid layout, colored stat cards.
```

---

## #97 — feat: Developer dashboard (stats, bids, projects)

**Sıra (Order):** 26

**Uygulama Sırası / Order:** 26

```md
<GLOBAL HEADER>

Issue: #97
Sıra: 26
Goal:
- Developer dashboard with bids and projects.

Tasks:
1) GET /dashboard (Developer role).
2) Stats cards: active bids, accepted bids, assigned projects.
3) My active bids list (top 5), assigned projects list.
4) Use DashboardController with role-based view selection.
```

---

## Sprint 6 — UX Polish

---

## #98 — feat: Client-side validation (jQuery Validation)

**Sıra (Order):** 27

**Uygulama Sırası / Order:** 27

```md
<GLOBAL HEADER>

Issue: #98
Sıra: 27
Goal:
- jQuery Validation + Unobtrusive for all forms.

Tasks:
1) Add jQuery Validation + Unobtrusive Validation libraries.
2) Ensure all forms match backend validation rules.
3) Date pickers with min date constraints.
4) Numeric validation for budget/amount fields.
5) Password strength indicator on register.
```

---

## #99 — feat: Toast notifications (success/error feedback)

**Sıra (Order):** 28

**Uygulama Sırası / Order:** 28

```md
<GLOBAL HEADER>

Issue: #99
Sıra: 28
Goal:
- Bootstrap toast notification system.

Tasks:
1) TempData-based flash messages (success, error, warning, info).
2) JavaScript toast helper for AJAX operations.
3) Auto-dismiss after 5 seconds, position: top-right.
```

---

## #100 — feat: Loading states and empty states

**Sıra (Order):** 29

**Uygulama Sırası / Order:** 29

```md
<GLOBAL HEADER>

Issue: #100
Sıra: 29
Goal:
- Loading spinners and empty state components.

Tasks:
1) Loading spinners for AJAX operations.
2) Empty states: "No projects yet" + CTA, "No bids received", "No activity yet".
3) Skeleton loading for page transitions (optional).
```

---

## #101 — feat: Responsive design audit (mobile/tablet)

**Sıra (Order):** 30

**Uygulama Sırası / Order:** 30

```md
<GLOBAL HEADER>

Issue: #101
Sıra: 30
Goal:
- Test and fix all pages for mobile/tablet/desktop.

Tasks:
1) Test at 375px, 768px, 1200px+.
2) Fix Bootstrap grid issues, scrollable tables on mobile.
3) Test navigation menu collapse, fix overflow issues.
```

---

## #102 — feat: Error pages (404, 403, 500)

**Sıra (Order):** 31

**Uygulama Sırası / Order:** 31

```md
<GLOBAL HEADER>

Issue: #102
Sıra: 31
Goal:
- Custom error pages with friendly messages.

Tasks:
1) Views/Shared/Error.cshtml (500), NotFound.cshtml (404), AccessDenied.cshtml (403).
2) Program.cs: UseStatusCodePagesWithReExecute + UseExceptionHandler.
3) "Go back" and "Home" buttons, consistent with site design.
```

---

## Sprint 7 — Testing & Docs

---

## #103 — test: Frontend integration tests (WebApplicationFactory)

**Sıra (Order):** 32

**Uygulama Sırası / Order:** 32

```md
<GLOBAL HEADER>

Issue: #103
Sıra: 32
Goal:
- Integration tests with mocked backend.

Tasks:
1) Create test project: frontend/Dev4All.Web.Tests/.
2) Add Mvc.Testing, FluentAssertions.
3) Use WebApplicationFactory with mocked backend API (WireMock or similar).
4) Tests: HomePage success, LoginPage form, LoginPost redirect, Projects auth required, CreateProject role check.
```

---

## #104 — feat: Accessibility audit (ARIA, contrast)

**Sıra (Order):** 33

**Uygulama Sırası / Order:** 33

```md
<GLOBAL HEADER>

Issue: #104
Sıra: 33
Goal:
- WCAG AA compliance check.

Tasks:
1) ARIA labels on all interactive elements.
2) Proper heading hierarchy (h1 → h2 → h3).
3) Color contrast ratios check (WCAG AA).
4) Alt text on all images, keyboard navigation for forms.
```

---

## #105 — docs: Frontend README update

**Sıra (Order):** 34

**Uygulama Sırası / Order:** 34

```md
<GLOBAL HEADER>

Issue: #105
Sıra: 34
Goal:
- Comprehensive frontend README.

Tasks:
1) Project overview (ASP.NET Core MVC).
2) Prerequisites (.NET 10).
3) Getting started (clone, configure, run).
4) Page structure and routing table.
5) Architecture overview (Services → HttpClient → Backend API).
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
- [x] dotnet build frontend/Dev4All.Web/
- [x] dotnet test frontend/Dev4All.Web.Tests/ (if exists)

## Notes
- <risk/blocker notes if any>
```
