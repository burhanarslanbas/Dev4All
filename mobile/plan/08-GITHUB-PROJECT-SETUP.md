# 08. GitHub Project Setup Guide

> Bu doküman, Dev4All Mobile için GitHub tarafında issue/PR/board düzenini hızlıca kurman için hazırlanmıştır.

---

## 1) Branch Strategy

- `main`: production-stable
- `develop`: integration branch
- feature branch format: `feat/mobile-<scope>`

Örnekler:
- `feat/mobile-foundation-modules`
- `feat/mobile-auth-login-register`
- `fix/mobile-token-expiry-redirect`

---

## 2) Branch Protection (Manual in GitHub UI)

GitHub > Settings > Branches:

### `main` için:
- Require a pull request before merging: ON
- Require approvals: 1
- Dismiss stale pull request approvals when new commits are pushed: ON
- Require status checks to pass before merging: ON
- Required checks:
  - `Android CI / build`
  - `Android CI / test`
- Require conversation resolution before merging: ON
- Restrict pushes that create files larger than 100MB: ON

### `develop` için:
- Require a pull request before merging: ON
- Require approvals: 1
- Require status checks to pass before merging: ON
- Required checks:
  - `Android CI / build`
  - `Android CI / test`

---

## 3) Labels (GH CLI)

Önce auth:

```bash
gh auth login
```

Ardından repo kökünde şu komutları çalıştır:

```bash
gh label create "type:epic" --color "5319e7" --description "Epic level issue"
gh label create "type:feature" --color "1d76db" --description "Feature implementation task"
gh label create "type:bug" --color "d73a4a" --description "Bug report"
gh label create "type:chore" --color "cfd3d7" --description "Maintenance/chore task"
gh label create "type:docs" --color "0e8a16" --description "Documentation task"

gh label create "area:mobile" --color "0052cc" --description "Mobile app related"
gh label create "area:auth" --color "006b75" --description "Authentication"
gh label create "area:project" --color "0366d6" --description "Project management"
gh label create "area:bid" --color "5319e7" --description "Bid management"
gh label create "area:contract" --color "b60205" --description "Contract management"
gh label create "area:github" --color "24292f" --description "GitHub integration"
gh label create "area:ui" --color "fbca04" --description "UI / design system"
gh label create "area:data" --color "0e8a16" --description "Data / network / storage"
gh label create "area:test" --color "bfdadc" --description "Testing"

gh label create "priority:P1" --color "b60205" --description "High priority"
gh label create "priority:P2" --color "fbca04" --description "Medium priority"
gh label create "priority:P3" --color "0e8a16" --description "Low priority"

gh label create "status:blocked" --color "d93f0b" --description "Blocked by dependency"
```

---

## 4) Milestones (GH CLI)

```bash
gh api repos/:owner/:repo/milestones -f title="M1 Foundation" -f description="Sprint 1 - infrastructure and core setup"
gh api repos/:owner/:repo/milestones -f title="M2 Auth" -f description="Sprint 2 - login/register"
gh api repos/:owner/:repo/milestones -f title="M3 Navigation" -f description="Sprint 3 - onboarding and role-based nav"
gh api repos/:owner/:repo/milestones -f title="M4 Projects" -f description="Sprint 4 - project management"
gh api repos/:owner/:repo/milestones -f title="M5 Bids" -f description="Sprint 5 - bid system"
gh api repos/:owner/:repo/milestones -f title="M6 Contracts" -f description="Sprint 6 - contract management"
gh api repos/:owner/:repo/milestones -f title="M7 GitHub + Polish" -f description="Sprint 7 - repo connect + timeline + UX polish"
gh api repos/:owner/:repo/milestones -f title="M8 MVP Ready" -f description="Sprint 8 - final QA and release prep"
```

Not:
- `:owner` ve `:repo` yerini GH CLI otomatik doldurabilir.
- Dolduramazsa açıkça `repos/BurhanArslanbas/Dev4All/milestones` formatı kullan.

---

## 5) Project Board (GitHub Projects v2)

Board adı önerisi: `Dev4All Mobile MVP`

Kolonlar:
- Backlog
- Ready
- In Progress
- In Review
- QA
- Done

Kural:
- WIP limiti: `In Progress` max 2-3 kart.
- Her issue bir karta bağlı olsun.

---

## 6) PR Rules

- PR title format: `feat(mobile): <short-title>`
- Scope küçük olsun (tek feature/tek problem).
- UI değiştiyse screenshot zorunlu.
- Merge şartı: review + CI green.

---

## 7) Issue Creation Workflow

Sıra:
1. Önce 8 epic issue aç.
2. Sonra sprint bazlı feature task issue'larını aç.
3. Her task issue'yu parent epic'e bağla.
4. Milestone ve labels ekle.

Bu issue metinleri hazır olarak:
- `mobile/plan/09-ISSUE-BACKLOG.md`

