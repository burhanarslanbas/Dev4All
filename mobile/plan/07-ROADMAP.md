# 07. Development Roadmap

> Sprint bazlı geliştirme takvimi, milestone'lar ve kabul kriterleri.

---

## Sprint Yapısı

- **Sprint süresi:** 1 hafta
- **Toplam tahmini süre:** 7-8 sprint (7-8 hafta)
- **Geliştirme yaklaşımı:** Feature-by-feature, her sprint sonunda çalışan build

---

## Sprint 1: Foundation (Temel Altyapı)

**Hedef:** Multi-module proje yapısını kurmak, tema ve design system'i oluşturmak, network katmanını hazırlamak.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 1.1 | Multi-module proje yapısını oluştur (settings.gradle.kts, tüm modüller) | Root | 2 saat |
| 1.2 | Version Catalog güncellemesi (tüm bağımlılıklar) | Root | 1 saat |
| 1.3 | Her modülün build.gradle.kts dosyasını yaz | Tüm modüller | 2 saat |
| 1.4 | `:core:common` — Result wrapper, extension functions, constants | common | 2 saat |
| 1.5 | `:core:designsystem` — Tech Blue tema, Color, Typography, Shape, Spacing | designsystem | 3 saat |
| 1.6 | `:core:designsystem` — Temel bileşenler: Button, TextField, Card, TopBar | designsystem | 4 saat |
| 1.7 | `:core:designsystem` — StatusBadge, TechnologyChip, EmptyState, ErrorState, LoadingIndicator | designsystem | 3 saat |
| 1.8 | `:core:network` — OkHttp, Retrofit, Json yapılandırması, NetworkModule | network | 3 saat |
| 1.9 | `:core:network` — AuthInterceptor, ErrorInterceptor | network | 2 saat |
| 1.10 | `:core:network` — Error DTO'lar (ApiErrorResponse, ValidationErrorResponse) | network | 1 saat |
| 1.11 | `:core:datastore` — TokenDataStore, UserSessionDataStore, AppPreferencesDataStore | datastore | 3 saat |
| 1.12 | `:app` — Dev4AllApplication (HiltAndroidApp), AndroidManifest güncellemesi | app | 1 saat |
| 1.13 | Gradle sync + clean build doğrulaması | Tüm | 1 saat |

### Çıktılar
- [x] Tüm modüller compile edilir
- [x] Tema preview'ları çalışır
- [x] Network layer unit test'leri yazılır

### Milestone: `M1 — Foundation Complete`

---

## Sprint 2: Authentication (Kimlik Doğrulama)

**Hedef:** Login ve register ekranlarını geliştirmek, backend auth API ile entegrasyon sağlamak.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 2.1 | `:core:domain` — User, UserRole, AuthToken model'leri | domain | 1 saat |
| 2.2 | `:core:domain` — AuthRepository interface | domain | 30 dk |
| 2.3 | `:core:domain` — LoginUseCase, RegisterUseCase, GetCurrentUserUseCase, LogoutUseCase | domain | 2 saat |
| 2.4 | `:core:domain` — Exception sealed class'ları (AppException) | domain | 1 saat |
| 2.5 | `:core:network` — AuthApiService (Retrofit) | network | 1 saat |
| 2.6 | `:core:network` — Auth DTO'lar (Login/Register Request/Response) | network | 1 saat |
| 2.7 | `:core:data` — AuthRepositoryImpl, UserMapper | data | 2 saat |
| 2.8 | `:core:data` — DataModule (Hilt binding) | data | 30 dk |
| 2.9 | `:app` — LoginScreen + LoginViewModel + UiState/Event | app | 4 saat |
| 2.10 | `:app` — RegisterScreen + RegisterViewModel (rol seçim kartları) | app | 4 saat |
| 2.11 | `:app` — Temel Navigation yapısı (NavHost, auth routes) | app | 2 saat |
| 2.12 | `:app` — SplashScreen (token check + navigation) | app | 2 saat |
| 2.13 | Login/Register ViewModel unit test'leri | test | 3 saat |
| 2.14 | Use case unit test'leri | test | 2 saat |
| 2.15 | Repository unit test'leri | test | 2 saat |

### Çıktılar
- [x] Login çalışır → token kaydedilir → Home'a yönlendirilir
- [x] Register çalışır → otomatik login → Home'a yönlendirilir
- [x] Hatalı giriş → error mesajı gösterilir
- [x] Client-side validation çalışır
- [x] > 80% test coverage (auth modülü)

### Milestone: `M2 — Auth Complete`

---

## Sprint 3: Onboarding + Main Navigation

**Hedef:** Onboarding deneyimini oluşturmak, Customer ve Developer ana ekranlarını (bottom nav) kurmak.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 3.1 | OnboardingScreen (3 sayfa HorizontalPager) | app | 3 saat |
| 3.2 | OnboardingViewModel (ilk açılış kontrolü) | app | 1 saat |
| 3.3 | Customer BottomNavigation yapısı (4 sekme) | app | 2 saat |
| 3.4 | Developer BottomNavigation yapısı (4 sekme) | app | 2 saat |
| 3.5 | Rol bazlı navigation akışı (login sonrası Customer vs Developer) | app | 2 saat |
| 3.6 | Customer Dashboard placeholder ekranı | app | 2 saat |
| 3.7 | Developer Explore placeholder ekranı | app | 2 saat |
| 3.8 | Profile placeholder ekranı (logout butonu çalışır) | app | 2 saat |
| 3.9 | Full navigation test (splash → onboarding → login → home → tabs) | test | 2 saat |

### Çıktılar
- [x] İlk açılışta onboarding gösterilir, sonraki açılışlarda skip
- [x] Login sonrası role göre doğru main screen'e yönlendirilir
- [x] Bottom navigation sekmeler arası geçiş çalışır
- [x] Logout → Login'e geri dönüş çalışır

### Milestone: `M3 — Navigation Complete`

---

## Sprint 4: Project Management (Proje Yönetimi)

**Hedef:** Proje listeleme, detay, oluşturma ve düzenleme özelliklerini geliştirmek.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 4.1 | `:core:domain` — Project, ProjectStatus model'leri | domain | 1 saat |
| 4.2 | `:core:domain` — ProjectRepository interface | domain | 30 dk |
| 4.3 | `:core:domain` — GetProjects, GetProjectDetail, Create, Update, Delete use case'leri | domain | 2 saat |
| 4.4 | `:core:network` — ProjectApiService, Project DTO'lar | network | 2 saat |
| 4.5 | `:core:data` — ProjectRepositoryImpl, ProjectMapper | data | 2 saat |
| 4.6 | Customer Dashboard (gerçek veri: istatistikler, son projeler) | app | 3 saat |
| 4.7 | Developer Explore (proje listesi, arama, pagination) | app | 4 saat |
| 4.8 | ProjectDetailScreen (tüm bilgiler, status badge) | app | 4 saat |
| 4.9 | CreateProjectScreen (form, validation, date picker, tech chips) | app | 4 saat |
| 4.10 | EditProjectScreen (mevcut veriyle pre-filled form) | app | 2 saat |
| 4.11 | Delete project (soft delete, onay dialogu) | app | 1 saat |
| 4.12 | ViewModel + UseCase + Repository testleri | test | 3 saat |

### Çıktılar
- [x] Customer dashboard gerçek veriyle çalışır
- [x] Developer explore ile projeleri listeleyebilir
- [x] Proje detay sayfası eksiksiz
- [x] Proje oluşturma + düzenleme formları çalışır
- [x] Pagination ve arama çalışır

### Milestone: `M4 — Projects Complete`

---

## Sprint 5: Bid Management (Teklif Sistemi)

**Hedef:** Teklif verme, listeleme, güncelleme ve kabul etme özelliklerini geliştirmek.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 5.1 | `:core:domain` — Bid, BidStatus model'leri | domain | 1 saat |
| 5.2 | `:core:domain` — BidRepository interface + use case'ler | domain | 2 saat |
| 5.3 | `:core:network` — BidApiService, Bid DTO'lar | network | 1 saat |
| 5.4 | `:core:data` — BidRepositoryImpl, BidMapper | data | 2 saat |
| 5.5 | SubmitBidBottomSheet (Developer) | app | 3 saat |
| 5.6 | BidListSection (Customer — proje detayında embed) | app | 3 saat |
| 5.7 | AcceptBid akışı (Customer — onay dialogu) | app | 2 saat |
| 5.8 | MyBidsScreen (Developer — verilen teklifler) | app | 3 saat |
| 5.9 | Bid status badge'ları (Pending/Accepted/Rejected) | designsystem | 1 saat |
| 5.10 | Unit + UI testleri | test | 3 saat |

### Çıktılar
- [x] Developer teklif verebilir (bottom sheet)
- [x] Customer gelen teklifleri listeler ve kabul edebilir
- [x] Developer tekliflerini "Tekliflerim" ekranında görür
- [x] Teklif kabul → proje AwaitingContract'a geçer

### Milestone: `M5 — Bids Complete`

---

## Sprint 6: Contract Management (Sözleşme Yönetimi)

**Hedef:** Sözleşme görüntüleme, revize, onay ve iptal akışlarını geliştirmek.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 6.1 | `:core:domain` — Contract, ContractStatus, ContractRevision model'leri | domain | 1 saat |
| 6.2 | `:core:domain` — ContractRepository interface + use case'ler | domain | 2 saat |
| 6.3 | `:core:network` — ContractApiService, Contract DTO'lar | network | 1 saat |
| 6.4 | `:core:data` — ContractRepositoryImpl, ContractMapper | data | 2 saat |
| 6.5 | ContractScreen (metin, revizyon no, onay durumları) | app | 4 saat |
| 6.6 | ReviseContractScreen (düzenleme formu) | app | 3 saat |
| 6.7 | Contract action buttons (onayla, revize, iptal) | app | 2 saat |
| 6.8 | RevisionHistoryScreen | app | 2 saat |
| 6.9 | Sözleşme onayı → Proje Ongoing geçiş akışı | app | 2 saat |
| 6.10 | Unit + UI testleri | test | 3 saat |

### Çıktılar
- [x] Sözleşme metni okunabilir şekilde gösterilir
- [x] Revize, onayla, iptal butonları çalışır
- [x] İki taraf onayladığında proje Ongoing'e geçer
- [x] Revizyon geçmişi listelenebilir

### Milestone: `M6 — Contracts Complete`

---

## Sprint 7: GitHub Integration + Polish

**Hedef:** GitHub repo bağlama ve aktivite timeline'ını geliştirmek, UI polish.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 7.1 | `:core:domain` — GitHubLog model, repository, use case | domain | 1 saat |
| 7.2 | `:core:network` — GitHubLogApiService, DTO'lar | network | 1 saat |
| 7.3 | `:core:data` — GitHubLogRepositoryImpl, mapper | data | 1 saat |
| 7.4 | ConnectRepoSection (Developer — proje detayında) | app | 3 saat |
| 7.5 | ActivityTimelineSection (commit timeline) | app | 4 saat |
| 7.6 | TimelineItem + TimelineLine custom composable | designsystem | 3 saat |
| 7.7 | Relative time formatting ("2 saat önce") | common | 1 saat |
| 7.8 | Pull-to-refresh tüm liste ekranlarına | app | 2 saat |
| 7.9 | Empty state'ler tüm ekranlara | app | 2 saat |
| 7.10 | Loading skeleton'lar | designsystem | 2 saat |
| 7.11 | Error handling iyileştirmeleri (Snackbar, retry) | app | 2 saat |
| 7.12 | Animasyonlar (fade, slide, shared element) | app | 3 saat |

### Çıktılar
- [x] Developer repo bağlayabilir
- [x] Commit timeline görüntülenir
- [x] Tüm ekranlarda empty state ve error state var
- [x] Animasyonlar smooth çalışıyor
- [x] Pull-to-refresh çalışıyor

### Milestone: `M7 — GitHub + Polish Complete`

---

## Sprint 8: Profile, Settings, Final QA

**Hedef:** Profil ve ayarlar ekranları, dark mode, son test ve bug fix.

### Görevler

| # | Görev | Modül | Tahmini Süre |
|---|-------|-------|-------------|
| 8.1 | ProfileScreen (kullanıcı bilgileri) | app | 2 saat |
| 8.2 | SettingsScreen (dark mode toggle, çıkış) | app | 2 saat |
| 8.3 | Dark mode tema desteği | designsystem | 3 saat |
| 8.4 | NotificationsScreen (placeholder — MVP'de backend yok) | app | 1 saat |
| 8.5 | Edge case testleri (no internet, token expiry, empty lists) | test | 3 saat |
| 8.6 | Compose UI testleri (screen render testleri) | test | 3 saat |
| 8.7 | Code review ve refactor | Tüm | 3 saat |
| 8.8 | ProGuard / R8 kuralları doğrulama (release build) | app | 2 saat |
| 8.9 | README.md güncelleme | mobile | 1 saat |
| 8.10 | Final build + smoke test | Tüm | 2 saat |

### Çıktılar
- [x] Profil ekranı çalışır
- [x] Dark mode toggle çalışır ve persist edilir
- [x] Release build başarılı
- [x] Tüm kritik akışlar smoke test'ten geçer

### Milestone: `M8 — MVP Release Ready`

---

## Özet Milestone Tablosu

| Milestone | Sprint | Hedef Tarih (Tahmini) | Durum |
|-----------|--------|----------------------|-------|
| M1 — Foundation Complete | Sprint 1 | Hafta 1 sonu | Bekliyor |
| M2 — Auth Complete | Sprint 2 | Hafta 2 sonu | Bekliyor |
| M3 — Navigation Complete | Sprint 3 | Hafta 3 sonu | Bekliyor |
| M4 — Projects Complete | Sprint 4 | Hafta 4 sonu | Bekliyor |
| M5 — Bids Complete | Sprint 5 | Hafta 5 sonu | Bekliyor |
| M6 — Contracts Complete | Sprint 6 | Hafta 6 sonu | Bekliyor |
| M7 — GitHub + Polish Complete | Sprint 7 | Hafta 7 sonu | Bekliyor |
| M8 — MVP Release Ready | Sprint 8 | Hafta 8 sonu | Bekliyor |

---

## Risk ve Bağımlılıklar

| Risk | Olasılık | Etki | Azaltma |
|------|----------|------|---------|
| Backend endpoint'leri henüz hazır değil | Yüksek | Yüksek | MockK ile fake repository kullanarak geliştirmeye devam et; API hazır olunca entegre et |
| Hilt/KSP build hataları | Orta | Orta | Clean build + cache temizleme; KSP versiyonunu Kotlin ile uyumlu tut |
| Compose performans sorunları (büyük listeler) | Düşük | Orta | LazyColumn + key parametresi, `remember` optimizasyonları |
| Token expiry sırasında UX kopukluğu | Orta | Orta | Global error handler, otomatik login ekranına yönlendirme |
| Backend hata formatı değişikliği | Düşük | Yüksek | `ignoreUnknownKeys = true` + defensive parsing |

---

## Geliştirme Prensipleri

1. **Her sprint sonunda çalışan build** — kırık build commit'lenmez
2. **Feature branch'ler** — `feature/auth`, `feature/projects`, vb.
3. **Test-first yaklaşımı** — ViewModel/UseCase testleri feature ile birlikte yazılır
4. **Incremental PR'lar** — dev bir büyük PR yerine küçük, reviewable parçalar
5. **Backend mock'lama** — Backend hazır olmayan feature'lar fake data ile geliştirilir
