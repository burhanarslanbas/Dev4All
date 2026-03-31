# Dev4All Mobile — All Issue Agent Prompts

Bu dosya, GitHub'daki açık mobile issue'lar için mobil cihazdan bile kolayca kopyalanıp agent'a verilebilecek hazır prompt'ları içerir.

## Kullanım

1. GitHub issue numarasını seç.
2. Aşağıdaki ilgili prompt'u kopyala.
3. Agent ekranında:
   - Base branch: `develop`
   - Model: `GPT-5.3-Codex` (önerilen)
4. Prompt'u yapıştırıp çalıştır.

## Önerilen Uygulama Sırası (Dependency-Aware)

Bu sıra, issue numarasına değil bağımlılıklara göre optimize edilmiştir.

1. `#5` — Epic (Sprint 1 takip)
2. `#10` — Theme foundation
3. `#11` — Reusable UI components
4. `#12` — Network setup
5. `#13` — Datastore
6. `#24` — CI check enforcement
7. `#6` — Epic (Sprint 2 takip)
8. `#14` — Auth domain models/use cases
9. `#15` — Auth API services/DTOs
10. `#16` — Auth repository
11. `#17` — Login screen + ViewModel
12. `#18` — Register screen + role cards
13. `#19` — Splash routing
14. `#7` — Epic (Sprint 3 takip)
15. `#20` — Onboarding
16. `#21` — Customer bottom nav
17. `#22` — Developer bottom nav
18. `#23` — Navigation refactor/typed routes

Not:
- `#8` ve `#9` tamamlandı/merge edildiği için bu listede yoktur.
- Epic issue'lar (`#5`, `#6`, `#7`) koordinasyon ve kapanış kontrolü içindir.

---

## Global Prompt Header (Her issue için üstte kalsın)

```md
You are a senior Kotlin/Android engineer working in repository `Dev4All`.

General constraints:
- Follow `docs/AGENTS.md` and `mobile/plan/*.md`.
- Keep changes scoped to this issue only.
- Use Conventional Commits.
- Open PR to `develop`.
- Include `Closes #<ISSUE_NUMBER>` in PR body.
- Run and report:
  - `./gradlew tasks --no-daemon`
  - `./gradlew :app:assembleDebug --no-daemon` (if local SDK exists)

Output format required:
1) Summary
2) Files changed
3) Validation steps + results
4) Known blockers (if any)
```

---

## #5 — epic(mobile): sprint 1 foundation and core setup

```md
<GLOBAL HEADER>

Issue: #5 (Epic)
Goal:
- Track Sprint 1 foundation progress.
- Ensure child issues are linked and progress is visible.

Tasks:
1) Add/update a progress comment on issue #5 summarizing status of #8, #9, #10, #11, #12, #13, #24.
2) Ensure project board status reflects current work (In Progress/Ready/Done).
3) If scope changed, update acceptance checklist in issue #5.

Constraints:
- No product code changes unless strictly needed.
- Focus on planning/governance alignment.
```

---

## #6 — epic(mobile): sprint 2 authentication flow

```md
<GLOBAL HEADER>

Issue: #6 (Epic)
Goal:
- Prepare Sprint 2 auth execution and dependencies.

Tasks:
1) Audit readiness of prerequisite issues (#14-#19).
2) Add a kickoff comment in #6 with execution order:
   - #14 -> #15 -> #16 -> #17 -> #18 -> #19
3) If needed, add missing acceptance criteria in #6.

Constraints:
- No large implementation in this epic task.
- Keep this as orchestration/documentation update.
```

---

## #7 — epic(mobile): sprint 3 onboarding and role-based navigation

```md
<GLOBAL HEADER>

Issue: #7 (Epic)
Goal:
- Prepare Sprint 3 execution map for onboarding/navigation.

Tasks:
1) Validate child issue sequence:
   - #20 -> #21 -> #22 -> #23
2) Add implementation notes for route strategy consistency.
3) Update epic checklist and post status comment.
```

---

## #9 — chore(mobile): define version catalog for all dependencies

```md
<GLOBAL HEADER>

Issue: #9
Goal:
- Standardize dependency versions and plugin aliases in `mobile/gradle/libs.versions.toml`.

Tasks:
1) Ensure all required versions/libraries/plugins exist for Sprint 1-2 scope.
2) Remove duplicate or unused aliases.
3) Align module `build.gradle.kts` files with catalog aliases.
4) Verify `./gradlew tasks --no-daemon`.

Key files:
- `mobile/gradle/libs.versions.toml`
- `mobile/build.gradle.kts`
- `mobile/core/*/build.gradle.kts`
```

---

## #10 — feat(mobile): implement tech-blue theme in designsystem

```md
<GLOBAL HEADER>

Issue: #10
Goal:
- Implement Dev4All tech-blue design system theme (light + dark baseline).

Tasks:
1) Create/complete theme files under `mobile/core/designsystem`:
   - `theme/Color.kt`
   - `theme/Typography.kt`
   - `theme/Shape.kt` (optional but recommended)
   - `theme/Dev4AllTheme.kt`
2) Define status colors for project states.
3) Expose theme wrapper composable for app usage.
4) Add minimal previews.

Constraints:
- Keep API clean and reusable.
- No screen-level business logic.
```

---

## #11 — feat(mobile): create reusable compose ui components

```md
<GLOBAL HEADER>

Issue: #11
Goal:
- Build reusable UI primitives in `:core:designsystem`.

Tasks:
1) Implement components:
   - Dev4AllButton
   - Dev4AllTextField
   - Dev4AllCard
   - StatusBadge
   - Dev4AllLoadingIndicator
   - Dev4AllEmptyState
   - Dev4AllErrorState
2) Keep components stateless and previewable.
3) Add basic interaction tests where applicable.

Constraints:
- No feature-screen code.
- Use theme tokens from #10.
```

---

## #12 — feat(mobile): setup retrofit okhttp and interceptors

```md
<GLOBAL HEADER>

Issue: #12
Goal:
- Build network foundation with Retrofit + OkHttp + interceptors.

Tasks:
1) Create API base setup in `:core:network`.
2) Implement:
   - AuthInterceptor
   - ErrorInterceptor
3) Add error DTO contracts for backend middleware response.
4) Add DI module for Retrofit/OkHttp provisioning.
5) Validate with unit tests (interceptor mapping).

Key files:
- `mobile/core/network/src/main/kotlin/...`
- `mobile/core/network/build.gradle.kts`
```

---

## #13 — feat(mobile): implement datastore for token and session

```md
<GLOBAL HEADER>

Issue: #13
Goal:
- Implement local session/token/preferences storage in `:core:datastore`.

Tasks:
1) Create:
   - TokenDataStore
   - UserSessionDataStore
   - AppPreferencesDataStore
2) Provide DI module wiring.
3) Expose flow-based read APIs and clear methods.
4) Add unit tests for save/read/clear behavior.

Constraints:
- Keep Android-specific storage only in datastore module.
```

---

## #14 — feat(mobile): add auth domain models and use cases

```md
<GLOBAL HEADER>

Issue: #14
Goal:
- Build auth domain contracts and use cases in `:core:domain`.

Tasks:
1) Add models:
   - User
   - UserRole
   - AuthToken
2) Add `AuthRepository` interface.
3) Add use cases:
   - LoginUseCase
   - RegisterUseCase
   - GetCurrentUserUseCase
   - LogoutUseCase
4) Add unit tests for use cases.

Constraints:
- Domain module must stay framework-agnostic.
```

---

## #15 — feat(mobile): implement auth api services and dto contracts

```md
<GLOBAL HEADER>

Issue: #15
Goal:
- Implement auth API layer matching backend contract.

Tasks:
1) Add `AuthApiService`.
2) Add DTOs for:
   - register/login requests-responses
   - current user response
3) Ensure backend-compatible field naming and parsing.
4) Cover 400/403/401 error mapping alignment.

Reference:
- `backend/src/Presentation/Dev4All.WebAPI/Controllers/v1/AuthController.cs`
```

---

## #16 — feat(mobile): implement auth repository with token persistence

```md
<GLOBAL HEADER>

Issue: #16
Goal:
- Implement `AuthRepositoryImpl` in `:core:data` using network + datastore.

Tasks:
1) Implement login/register/me/logout flows.
2) Persist token/session on successful login.
3) Clear storage on logout/auth-invalid states.
4) Add mapper(s) from DTO to domain.
5) Add repository unit tests with fakes/mocks.

Dependencies:
- #13 datastore
- #14 domain contracts
- #15 auth api/dto
```

---

## #17 — feat(mobile): implement login screen and viewmodel

```md
<GLOBAL HEADER>

Issue: #17
Goal:
- Build login feature UI and ViewModel with proper state handling.

Tasks:
1) Create:
   - LoginScreen
   - LoginViewModel
   - LoginUiState
   - LoginEvent / SideEffect
2) Add validation and loading/error/success states.
3) Wire to LoginUseCase.
4) Add ViewModel tests + basic UI test.
```

---

## #18 — feat(mobile): implement register screen with role selection cards

```md
<GLOBAL HEADER>

Issue: #18
Goal:
- Build register UI with customer/developer role selection cards.

Tasks:
1) Create:
   - RegisterScreen
   - RegisterViewModel
   - RegisterUiState
   - RegisterEvent
2) Add form validation aligned with backend rules.
3) Implement role card selection UX.
4) Add tests for validation and success/error flows.
```

---

## #19 — feat(mobile): add splash routing based on onboarding and session state

```md
<GLOBAL HEADER>

Issue: #19
Goal:
- Implement splash decision logic and startup routing.

Routing rules:
1) First launch -> Onboarding
2) Token missing -> Login
3) Token exists -> Verify with `/auth/me`
4) Invalid/expired -> clear session + Login

Tasks:
1) Create SplashScreen + startup coordinator logic.
2) Integrate AppPreferencesDataStore + TokenDataStore.
3) Add routing unit/integration tests.
```

---

## #20 — feat(mobile): implement onboarding screens and completion flag

```md
<GLOBAL HEADER>

Issue: #20
Goal:
- Build 3-page onboarding and persist completion flag.

Tasks:
1) Implement pager-based onboarding screens.
2) Add page indicators and CTA behavior.
3) Persist `isOnboardingComplete`.
4) Add tests for first-run and post-completion behavior.
```

---

## #21 — feat(mobile): create customer bottom navigation shell

```md
<GLOBAL HEADER>

Issue: #21
Goal:
- Build customer tab shell and placeholder destinations.

Tabs:
- Home
- My Projects
- Notifications
- Profile

Tasks:
1) Create customer nav graph and scaffold.
2) Implement tab state handling.
3) Verify back stack behavior.
```

---

## #22 — feat(mobile): create developer bottom navigation shell

```md
<GLOBAL HEADER>

Issue: #22
Goal:
- Build developer tab shell and placeholder destinations.

Tabs:
- Explore
- My Bids
- My Projects
- Profile

Tasks:
1) Create developer nav graph and scaffold.
2) Implement tab state handling.
3) Verify back stack behavior.
```

---

## #23 — refactor(mobile): standardize navigation graph and typed routes

```md
<GLOBAL HEADER>

Issue: #23
Goal:
- Refactor routing into centralized typed route definitions.

Tasks:
1) Centralize all routes in one route model.
2) Remove duplicated string routes.
3) Validate route args and defaults.
4) Update existing nav usages to typed routes.
```

---

## #24 — chore(mobile): enforce ci checks for build and tests on mobile changes

```md
<GLOBAL HEADER>

Issue: #24
Goal:
- Ensure mobile CI gate is strict and predictable.

Tasks:
1) Validate workflows:
   - `.github/workflows/android.yml`
   - `.github/workflows/android-develop.yml`
2) Confirm branch protection required check names match workflow job names.
3) Update docs if check names/rules differ.
4) Verify a sample PR triggers expected checks.
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
- [x] ./gradlew tasks --no-daemon
- [x] ./gradlew :app:assembleDebug --no-daemon

## Notes
- <risk/blocker notes if any>
```

