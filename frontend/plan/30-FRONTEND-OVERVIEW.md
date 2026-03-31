# 30. Frontend (ASP.NET Core MVC) — Master Overview

> **Framework:** ASP.NET Core MVC (.NET 10)  
> **Styling:** Bootstrap 5 + custom CSS  
> **Auth:** Cookie-based (backend JWT exchanged for cookie session)  
> **API Communication:** HttpClient → backend Web API  

---

## 1. Why ASP.NET Core MVC?

- Developer (Burhan) is already proficient in .NET/C#
- Server-side rendering simplifies auth flow (no token storage in browser)
- Razor views + Tag Helpers provide productive UI development
- Same toolchain as backend — single `dotnet` CLI, shared models possible
- Bootstrap 5 gives responsive, professional UI out of the box

## 2. Architecture

```
frontend/
├── Dev4All.Web/                         # ASP.NET Core MVC project
│   ├── Controllers/                     # MVC Controllers
│   │   ├── HomeController.cs
│   │   ├── AuthController.cs
│   │   ├── ProjectsController.cs
│   │   ├── BidsController.cs
│   │   ├── ContractsController.cs
│   │   └── DashboardController.cs
│   ├── Models/                          # ViewModels + API DTOs
│   │   ├── Auth/
│   │   ├── Projects/
│   │   ├── Bids/
│   │   ├── Contracts/
│   │   └── Shared/
│   ├── Views/                           # Razor Views
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml           # Master layout
│   │   │   ├── _LoginPartial.cshtml
│   │   │   ├── _ValidationScriptsPartial.cshtml
│   │   │   └── Error.cshtml
│   │   ├── Home/
│   │   ├── Auth/
│   │   ├── Projects/
│   │   ├── Bids/
│   │   ├── Contracts/
│   │   └── Dashboard/
│   ├── Services/                        # API client services
│   │   ├── IApiClient.cs
│   │   ├── ApiClient.cs
│   │   ├── IAuthService.cs
│   │   ├── AuthService.cs
│   │   ├── IProjectService.cs
│   │   ├── ProjectService.cs
│   │   ├── IBidService.cs
│   │   ├── BidService.cs
│   │   ├── IContractService.cs
│   │   └── ContractService.cs
│   ├── Infrastructure/
│   │   ├── ApiTokenHandler.cs           # DelegatingHandler for JWT
│   │   └── CookieAuthenticationEvents.cs
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   └── lib/ (Bootstrap, etc.)
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dev4All.Web.csproj
└── plan/
    ├── 30-FRONTEND-OVERVIEW.md
    ├── 31-FRONTEND-ROADMAP.md
    └── 32-FRONTEND-AGENT-PROMPTS.md
```

## 3. Auth Flow (Cookie + JWT Bridge)

```
Browser → MVC AuthController.Login (POST form)
    → AuthService.LoginAsync(email, password)
        → HttpClient POST backend/api/v1/auth/login
        ← JWT token + expiry
    → Store JWT in encrypted cookie (Cookie Authentication)
    → Redirect to Dashboard

Subsequent requests:
Browser → MVC Controller (cookie sent automatically)
    → ApiTokenHandler reads JWT from cookie
    → Attaches "Authorization: Bearer {jwt}" to backend API calls
```

## 4. Page Map

| Page | Route | Auth | Role | Description |
|------|-------|------|------|-------------|
| Landing | `/` | No | - | Welcome page, features, CTA |
| Login | `/auth/login` | No | - | Email + password form |
| Register | `/auth/register` | No | - | Name, email, password, role select |
| **Customer Dashboard** | `/dashboard` | Yes | Customer | My projects, stats |
| **Developer Dashboard** | `/dashboard` | Yes | Developer | My bids, assigned projects |
| Project List | `/projects` | Yes | Any | Paginated open projects |
| Project Detail | `/projects/{id}` | Yes | Any | Full project info, bids, timeline |
| Create Project | `/projects/create` | Yes | Customer | Project creation form |
| Edit Project | `/projects/{id}/edit` | Yes | Customer (owner) | Edit form |
| My Projects | `/projects/my` | Yes | Customer | Customer's own projects |
| Place Bid | `/projects/{id}/bid` | Yes | Developer | Bid form |
| My Bids | `/bids/my` | Yes | Developer | Developer's bids |
| Contract View | `/contracts/{projectId}` | Yes | Party | Contract details + revisions |
| Contract Edit | `/contracts/{projectId}/edit` | Yes | Party | Revise contract content |
| GitHub Timeline | `/projects/{id}/timeline` | Yes | Party | Commit activity log |

## 5. Backend API Dependency

The frontend calls ALL backend endpoints listed in `backend/plan/20-BACKEND-OVERVIEW.md`. The frontend project depends on the backend being fully implemented (Sprint 1–4 minimum for core flows).

## 6. Reference Documents

| Document | Path |
|----------|------|
| FRD | `docs/analyse/02-frd.md` |
| BRD | `docs/analyse/01-brd.md` |
| Backend API Map | `backend/plan/20-BACKEND-OVERVIEW.md` (Section 8) |
| AGENTS.md | `docs/AGENTS.md` |
