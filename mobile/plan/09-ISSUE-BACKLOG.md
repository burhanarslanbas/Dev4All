# 09. Ready-to-Create Issue Backlog (Top 20)

> Aşağıdaki 20 issue metni, roadmap'e göre önceliklendirilmiş ve doğrudan GitHub'a açılacak şekilde hazırlanmıştır.

---

## 1) Epic — Sprint 1 Foundation

- **Title:** `epic(mobile): sprint 1 foundation and core setup`
- **Labels:** `type:epic`, `area:mobile`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Objective
Establish multi-module architecture, core design system, and base networking/storage foundations.

## Scope
- Multi-module Gradle setup
- Core common module
- Design system baseline
- Retrofit/OkHttp + interceptors
- DataStore baseline

## Acceptance Criteria
- [ ] All modules compile successfully
- [ ] Android CI is green
- [ ] Core components are preview/testable
- [ ] Child issues are completed
```

---

## 2) Setup Multi-Module Gradle Structure

- **Title:** `feat(mobile): setup multi-module gradle architecture`
- **Labels:** `type:feature`, `area:mobile`, `area:data`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Convert current single-module mobile project into multi-module clean architecture structure.

## Implementation Details
- Update `settings.gradle.kts` includes:
  - `:core:common`
  - `:core:domain`
  - `:core:data`
  - `:core:network`
  - `:core:datastore`
  - `:core:designsystem`
- Add module folders and baseline `build.gradle.kts`.
- Keep app module building after migration.

## Acceptance Criteria
- [ ] Gradle sync succeeds
- [ ] `./gradlew :app:assembleDebug` passes
- [ ] All modules are visible in Android Studio
- [ ] No dependency cycle exists
```

---

## 3) Version Catalog and Dependency Baseline

- **Title:** `chore(mobile): define version catalog for all dependencies`
- **Labels:** `type:chore`, `area:mobile`, `area:data`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Create a single source of truth in `gradle/libs.versions.toml`.

## Implementation Details
- Add Compose BOM, Navigation, Lifecycle, Hilt, Retrofit, OkHttp, Serialization, DataStore, Testing dependencies.
- Add plugin versions (AGP, Kotlin, Hilt, KSP).
- Ensure all modules consume catalog aliases.

## Acceptance Criteria
- [ ] No hardcoded dependency versions remain in module files
- [ ] Build passes with version catalog only
- [ ] CI workflow still succeeds
```

---

## 4) Design System Theme Foundation

- **Title:** `feat(mobile): implement tech-blue theme in designsystem`
- **Labels:** `type:feature`, `area:mobile`, `area:ui`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Implement Dev4All theme (light + dark) aligned with tech-blue palette.

## Implementation Details
- Add `Color.kt`, `Typography.kt`, `Shape.kt`, `Spacing.kt`, `Dev4AllTheme.kt`.
- Introduce status colors for project lifecycle badges.
- Apply theme in `MainActivity`.

## Acceptance Criteria
- [ ] Light and dark themes render correctly
- [ ] Status colors are defined and reusable
- [ ] Compose previews are available
```

---

## 5) Build Reusable UI Components

- **Title:** `feat(mobile): create reusable compose ui components`
- **Labels:** `type:feature`, `area:mobile`, `area:ui`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Create foundational reusable components in `:core:designsystem`.

## Components
- Dev4AllButton
- Dev4AllTextField
- Dev4AllCard
- Dev4AllTopBar
- Dev4AllBottomBar
- StatusBadge
- TechnologyChip
- EmptyState / ErrorState / LoadingIndicator

## Acceptance Criteria
- [ ] All components are stateless and previewable
- [ ] Variants are supported where applicable
- [ ] UI tests cover core interaction components
```

---

## 6) Network Layer Foundation

- **Title:** `feat(mobile): setup retrofit okhttp and interceptors`
- **Labels:** `type:feature`, `area:mobile`, `area:data`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Set up networking foundation with JSON parsing, auth header support, and error mapping.

## Implementation Details
- Create `NetworkModule` (Retrofit, OkHttp, Json).
- Add `AuthInterceptor`.
- Add `ErrorInterceptor`.
- Add DTOs for API error responses.

## Acceptance Criteria
- [ ] Retrofit client is injectable
- [ ] Auth header is added when token exists
- [ ] HTTP errors map to domain/app exceptions
- [ ] Unit tests cover interceptor behavior
```

---

## 7) DataStore Session and Token Storage

- **Title:** `feat(mobile): implement datastore for token and session`
- **Labels:** `type:feature`, `area:mobile`, `area:data`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Implement secure local storage baseline for auth/session/preferences.

## Implementation Details
- TokenDataStore
- UserSessionDataStore
- AppPreferencesDataStore (onboarding/dark mode)
- DI module for stores

## Acceptance Criteria
- [ ] Save/read/clear token works
- [ ] Session persistence works
- [ ] Preference flags are flow-based
- [ ] Unit tests added
```

---

## 8) Epic — Sprint 2 Auth

- **Title:** `epic(mobile): sprint 2 authentication flow`
- **Labels:** `type:epic`, `area:mobile`, `area:auth`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Objective
Deliver full login/register flow with backend integration and session persistence.

## Scope
- Auth API integration
- Domain models/use cases
- Login/Register screens
- Validation and error UX
- Token/session handling

## Acceptance Criteria
- [ ] Login and register works end-to-end
- [ ] Error states are user-friendly
- [ ] Tests pass with expected coverage
```

---

## 9) Domain Auth Models and Use Cases

- **Title:** `feat(mobile): add auth domain models and use cases`
- **Labels:** `type:feature`, `area:mobile`, `area:auth`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Summary
Create auth domain entities and use cases.

## Implementation Details
- User, UserRole, AuthToken models
- AuthRepository interface
- LoginUseCase
- RegisterUseCase
- GetCurrentUserUseCase
- LogoutUseCase

## Acceptance Criteria
- [ ] Domain module has zero Android framework imports
- [ ] Use cases are unit tested
- [ ] Error contract is consistent
```

---

## 10) Implement Auth API Service and DTOs

- **Title:** `feat(mobile): implement auth api services and dto contracts`
- **Labels:** `type:feature`, `area:mobile`, `area:auth`, `area:data`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Summary
Integrate backend auth endpoints and DTO mapping.

## Endpoints
- POST `/api/v1/auth/register`
- POST `/api/v1/auth/login`
- GET `/api/v1/auth/me`

## Acceptance Criteria
- [ ] DTOs match backend JSON contract
- [ ] API layer handles 400/403/401 correctly
- [ ] Mapper tests are added
```

---

## 11) Implement Auth Repository

- **Title:** `feat(mobile): implement auth repository with token persistence`
- **Labels:** `type:feature`, `area:mobile`, `area:auth`, `area:data`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Summary
Implement `AuthRepositoryImpl` to coordinate API + DataStore.

## Implementation Details
- login -> store token/session
- register -> return mapped result
- getCurrentUser -> map profile
- logout -> clear token/session

## Acceptance Criteria
- [ ] Successful login persists token/session
- [ ] Logout clears all auth state
- [ ] Repository tests cover success/failure
```

---

## 12) Build Login Screen + ViewModel

- **Title:** `feat(mobile): implement login screen and viewmodel`
- **Labels:** `type:feature`, `area:mobile`, `area:auth`, `area:ui`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Summary
Create login UI with validation and state-driven behavior.

## Acceptance Criteria
- [ ] Email/password validation exists
- [ ] Loading state shown during request
- [ ] Error message shown on failure
- [ ] Success triggers navigation to role-based home
- [ ] ViewModel unit tests added
```

---

## 13) Build Register Screen + Role Selection

- **Title:** `feat(mobile): implement register screen with role selection cards`
- **Labels:** `type:feature`, `area:mobile`, `area:auth`, `area:ui`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Summary
Create register screen with customer/developer role cards and validation.

## Acceptance Criteria
- [ ] Role selection is required
- [ ] Form validation mirrors backend rules
- [ ] Register success behavior is defined (auto-login or redirect)
- [ ] ViewModel and UI tests added
```

---

## 14) Splash and Session Routing

- **Title:** `feat(mobile): add splash routing based on onboarding and session state`
- **Labels:** `type:feature`, `area:mobile`, `area:auth`, `priority:P1`
- **Milestone:** `M2 Auth`
- **Body:**

```md
## Summary
Route user from splash to onboarding/login/home by stored state.

## Rules
- first launch -> onboarding
- token missing -> login
- token exists -> verify with `/auth/me`

## Acceptance Criteria
- [ ] Routing logic works deterministically
- [ ] Invalid token redirects to login
- [ ] Unit/integration test added
```

---

## 15) Epic — Sprint 3 Navigation

- **Title:** `epic(mobile): sprint 3 onboarding and role-based navigation`
- **Labels:** `type:epic`, `area:mobile`, `area:ui`, `priority:P2`
- **Milestone:** `M3 Navigation`
- **Body:**

```md
## Objective
Complete onboarding UX and role-specific bottom navigation shells.

## Acceptance Criteria
- [ ] Onboarding flow complete
- [ ] Customer and developer navigation shells ready
- [ ] Shared profile/settings route working
```

---

## 16) Build Onboarding Flow

- **Title:** `feat(mobile): implement onboarding screens and completion flag`
- **Labels:** `type:feature`, `area:mobile`, `area:ui`, `priority:P2`
- **Milestone:** `M3 Navigation`
- **Body:**

```md
## Summary
Implement 3-page onboarding flow based on design prompts.

## Acceptance Criteria
- [ ] Horizontal pager with page indicators
- [ ] Last page has CTA button
- [ ] Completion flag persisted in preferences
```

---

## 17) Customer Bottom Navigation Shell

- **Title:** `feat(mobile): create customer bottom navigation shell`
- **Labels:** `type:feature`, `area:mobile`, `area:ui`, `priority:P2`
- **Milestone:** `M3 Navigation`
- **Body:**

```md
## Tabs
- Home
- My Projects
- Notifications
- Profile

## Acceptance Criteria
- [ ] Tab switching works
- [ ] Back stack behavior is correct
- [ ] Placeholder screens are wired
```

---

## 18) Developer Bottom Navigation Shell

- **Title:** `feat(mobile): create developer bottom navigation shell`
- **Labels:** `type:feature`, `area:mobile`, `area:ui`, `priority:P2`
- **Milestone:** `M3 Navigation`
- **Body:**

```md
## Tabs
- Explore
- My Bids
- My Projects
- Profile

## Acceptance Criteria
- [ ] Tab switching works
- [ ] Back stack behavior is correct
- [ ] Placeholder screens are wired
```

---

## 19) Navigation Graph Cleanup and Type-Safe Routes

- **Title:** `refactor(mobile): standardize navigation graph and typed routes`
- **Labels:** `type:feature`, `area:mobile`, `area:ui`, `priority:P2`
- **Milestone:** `M3 Navigation`
- **Body:**

```md
## Summary
Refactor routing constants and argument passing into consistent route model.

## Acceptance Criteria
- [ ] All routes centralized
- [ ] Route arguments validated
- [ ] No duplicated route strings in screens
```

---

## 20) Baseline Mobile CI Validation

- **Title:** `chore(mobile): enforce ci checks for build and tests on mobile changes`
- **Labels:** `type:chore`, `area:mobile`, `area:test`, `priority:P1`
- **Milestone:** `M1 Foundation`
- **Body:**

```md
## Summary
Ensure CI runs only when `mobile/**` changes and blocks bad merges.

## Acceptance Criteria
- [ ] Workflow triggers on push/PR for mobile path
- [ ] Build + unit tests are mandatory checks
- [ ] CI status is required in branch protection
```

