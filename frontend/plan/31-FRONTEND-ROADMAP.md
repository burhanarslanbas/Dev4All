# 31. Frontend (MVC) Development Roadmap

> 7 sprint, dependency-ordered. Each sprint ≈ 1 week.

---

## Sprint 1 — Project Scaffold & Auth UI

**Milestone:** `Frontend S1 — Scaffold & Auth`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 1 | MVC project scaffold + Program.cs + Bootstrap layout | feat | 3h |
| 2 | API client infrastructure (HttpClient + JWT handler) | feat | 3h |
| 3 | Login page + AuthService + cookie auth | feat | 3h |
| 4 | Register page + role selection | feat | 2h |
| 5 | Shared layout: navbar, login partial, footer | feat | 2h |
| 6 | Landing page (home) | feat | 2h |

---

## Sprint 2 — Project Pages

**Milestone:** `Frontend S2 — Projects`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 7 | Project list page (paginated, search/filter) | feat | 3h |
| 8 | Project detail page | feat | 2.5h |
| 9 | Create project form + validation | feat | 3h |
| 10 | Edit project form | feat | 2h |
| 11 | My projects page (Customer dashboard) | feat | 2h |
| 12 | Delete project (confirmation modal) | feat | 1h |

---

## Sprint 3 — Bid Pages

**Milestone:** `Frontend S3 — Bids`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 13 | Place bid form (on project detail page) | feat | 2.5h |
| 14 | Project bids list (Customer view, on detail page) | feat | 2h |
| 15 | Accept bid button + confirmation | feat | 2h |
| 16 | My bids page (Developer dashboard) | feat | 2h |
| 17 | Update bid form | feat | 1.5h |

---

## Sprint 4 — Contract Pages

**Milestone:** `Frontend S4 — Contracts`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 18 | Contract view page | feat | 2.5h |
| 19 | Contract edit/revise page | feat | 3h |
| 20 | Approve contract button | feat | 1.5h |
| 21 | Cancel contract button + confirmation | feat | 1.5h |
| 22 | Contract revision history view | feat | 2h |

---

## Sprint 5 — GitHub & Dashboard

**Milestone:** `Frontend S5 — GitHub & Dashboard`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 23 | Link GitHub repo form (Developer) | feat | 2h |
| 24 | GitHub activity timeline component | feat | 3h |
| 25 | Customer dashboard (stats, projects, contracts) | feat | 3h |
| 26 | Developer dashboard (stats, bids, projects) | feat | 3h |

---

## Sprint 6 — UX Polish & Validation

**Milestone:** `Frontend S6 — UX Polish`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 27 | Client-side validation (jQuery Validation) | feat | 2h |
| 28 | Toast notifications (success/error feedback) | feat | 2h |
| 29 | Loading states and empty states | feat | 2h |
| 30 | Responsive design audit (mobile/tablet) | feat | 2h |
| 31 | Error pages (404, 403, 500) | feat | 1.5h |

---

## Sprint 7 — Testing & Docs

**Milestone:** `Frontend S7 — Testing & Docs`

| # | Issue | Type | Est. |
|---|-------|------|------|
| 32 | Integration tests (WebApplicationFactory) | test | 4h |
| 33 | Accessibility audit (ARIA, contrast) | feat | 2h |
| 34 | Frontend README update | docs | 1h |

---

## Dependency Graph

```
S1 (Scaffold + Auth) → S2 (Projects) → S3 (Bids) → S4 (Contracts)
                                                  → S5 (GitHub + Dashboard)
S1–S5 → S6 (UX Polish)
S1–S6 → S7 (Testing & Docs)
```

## Backend API Dependency

| Frontend Sprint | Required Backend Sprints |
|----------------|------------------------|
| S1 (Auth) | Backend Auth (✅ done) |
| S2 (Projects) | Backend S1 (Project CRUD) |
| S3 (Bids) | Backend S2 (Bid module) |
| S4 (Contracts) | Backend S3 (Contract module) |
| S5 (GitHub) | Backend S4 (GitHub integration) |
