# Dev4All Mobile — Master Plan Overview

> Bu doküman, Dev4All Android mobil uygulamasının tüm geliştirme planının **ana indeks** dosyasıdır.
> Her alt plan dosyasının kapsamı ve kullanım amacı burada özetlenmiştir.

---

## Proje Özeti

**Dev4All Mobile**, müşterilerin yazılım proje ilanı açtığı, geliştiricilerin teklif verdiği, sözleşme müzakeresi yaptığı ve GitHub entegrasyonu ile proje ilerleyişini izlediği B2B freelance marketplace platformunun **Android native** istemcisidir.

| Özellik | Değer |
|---------|-------|
| **Platform** | Android (API 24+, Android 7.0 Nougat) |
| **Dil** | Kotlin 2.x |
| **UI Framework** | Jetpack Compose + Material 3 |
| **Mimari** | Clean Architecture (Multi-Module) + MVVM + UDF |
| **DI** | Hilt (Dagger) |
| **Networking** | Retrofit + OkHttp + Kotlin Serialization |
| **Async** | Kotlin Coroutines + Flow |
| **Navigation** | Jetpack Compose Navigation (Type-Safe) |
| **Local Storage** | DataStore (Preferences + Proto) |
| **Build System** | Gradle 8.9, Kotlin DSL, Version Catalog |

---

## Plan Dosyaları

| # | Dosya | Kapsam |
|---|-------|--------|
| 00 | `00-OVERVIEW.md` (bu dosya) | Ana indeks, özet, AI agent talimatları |
| 01 | [`01-ARCHITECTURE.md`](./01-ARCHITECTURE.md) | Clean Architecture katmanları, modül yapısı, bağımlılık akışı, MVVM + UDF pattern |
| 02 | [`02-PROJECT-SETUP.md`](./02-PROJECT-SETUP.md) | Multi-module Gradle yapılandırması, tüm bağımlılıklar, Version Catalog, ProGuard, CI/CD |
| 03 | [`03-API-INTEGRATION.md`](./03-API-INTEGRATION.md) | Backend API sözleşmesi, Retrofit servisleri, DTO'lar, hata yönetimi, token yönetimi |
| 04 | [`04-FEATURES.md`](./04-FEATURES.md) | Özellik bazlı uygulama planı — her feature için dosya/class listesi |
| 05 | [`05-UI-IMPLEMENTATION.md`](./05-UI-IMPLEMENTATION.md) | Ekran tasarımları, Navigation grafiği, Compose bileşenleri, tema sistemi |
| 06 | [`06-TESTING.md`](./06-TESTING.md) | Unit test, UI test, integration test stratejisi |
| 07 | [`07-ROADMAP.md`](./07-ROADMAP.md) | Sprint bazlı geliştirme takvimi, milestone'lar |

---

## Backend API Özeti (Mobil İçin Gerekli Endpoint'ler)

| HTTP | Endpoint | Açıklama |
|------|----------|----------|
| `POST` | `/api/v1/auth/register` | Kullanıcı kaydı (Customer/Developer) |
| `POST` | `/api/v1/auth/login` | JWT token ile giriş |
| `GET` | `/api/v1/auth/me` | Mevcut kullanıcı profili |
| `GET` | `/api/projects` | Açık projeleri listele |
| `POST` | `/api/projects` | Yeni proje oluştur (Customer) |
| `GET` | `/api/projects/{id}` | Proje detayı |
| `PUT` | `/api/projects/{id}` | Proje güncelle |
| `DELETE` | `/api/projects/{id}` | Proje sil (soft-delete) |
| `GET` | `/api/projects/{id}/bids` | Projeye ait teklifleri listele |
| `POST` | `/api/projects/{id}/bids` | Teklif ver (Developer) |
| `PUT` | `/api/bids/{id}` | Teklif güncelle |
| `POST` | `/api/bids/{id}/accept` | Teklifi kabul et (Customer) |
| `GET` | `/api/contracts/{projectId}` | Sözleşmeyi görüntüle |
| `PUT` | `/api/contracts/{projectId}` | Sözleşmeyi revize et |
| `POST` | `/api/contracts/{projectId}/approve` | Sözleşmeyi onayla |
| `POST` | `/api/contracts/{projectId}/cancel` | Sözleşmeyi iptal et |
| `GET` | `/api/contracts/{projectId}/revisions` | Revizyon geçmişi |
| `PUT` | `/api/projects/{id}/repo` | GitHub repo bağla (Developer) |
| `GET` | `/api/projects/{id}/github-logs` | Aktivite timeline |

---

## AI Agent Talimatları

Bu plan dosyaları, bir AI coding agent'ın (Cursor, Copilot, vb.) Dev4All mobil uygulamasını **sıfırdan geliştirmesi** için yeterli bağlamı sağlar. Agent'lar için kurallar:

### Genel Kurallar
1. **Dil:** Kotlin 2.x — tüm yeni dosyalar `.kt` uzantılı olacak.
2. **UI:** Jetpack Compose — XML layout **YASAK**.
3. **DI:** Hilt — manuel bağımlılık yönetimi **YASAK**.
4. **Async:** Coroutines + Flow — RxJava **YASAK**.
5. **Serialization:** Kotlin Serialization — Gson/Moshi **YASAK**.

### Naming Conventions
- **Package:** `com.dev4all.mobile.<module>.<layer>.<feature>`
- **Classes:** `PascalCase` — `LoginViewModel`, `AuthRepository`
- **Functions:** `camelCase` — `getProjects()`, `submitBid()`
- **Constants:** `SCREAMING_SNAKE_CASE` — `BASE_URL`, `TOKEN_KEY`
- **Composables:** `PascalCase` — `LoginScreen`, `ProjectCard`
- **State:** `_uiState` (private MutableStateFlow), `uiState` (public StateFlow)

### Dosya Organizasyonu
- Her feature kendi paketinde yaşar.
- Screen composable'ları `<Feature>Screen.kt` olarak adlandırılır.
- ViewModel'ler `<Feature>ViewModel.kt` olarak adlandırılır.
- Repository interface'leri `domain` katmanında, implementasyonları `data` katmanında bulunur.

### Commit Mesajları
- Conventional Commits: `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:`
- İngilizce yazılır.
- Trailer: `Requested-by: Burhan Arslanbaş`

---

## Bağlantılı Dokümanlar

| Doküman | Konum |
|---------|-------|
| Backend AGENTS.md | `docs/AGENTS.md` |
| Business Requirements | `docs/analyse/01-brd.md` |
| Functional Requirements | `docs/analyse/02-frd.md` |
| Non-Functional Requirements | `docs/analyse/03-nfr.md` |
| System Architecture | `docs/analyse/04-sadm.md` |
| Integration Specs | `docs/analyse/05-integration.md` |
| Mobile UI Design Prompts | `docs/design/06-stitch-mobile-prompts.md` |
| Mobile README | `mobile/README.md` |
