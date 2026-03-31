# 04. Feature Implementation Plan

> Her özellik modülünün detaylı uygulama planı: dosya listesi, class hiyerarşisi, iş kuralları ve bağımlılıklar.

---

## Feature Geliştirme Sırası (Öncelik)

| # | Feature | Bağımlılık | Açıklama |
|---|---------|------------|----------|
| 1 | **Core Infrastructure** | — | Multi-module setup, theme, design system, network layer |
| 2 | **Onboarding** | Core | Splash, karşılama ekranları, ilk açılış deneyimi |
| 3 | **Auth (Login & Register)** | Core, Network | JWT tabanlı kimlik doğrulama |
| 4 | **Customer Dashboard** | Auth | Müşteri ana sayfa, proje özeti |
| 5 | **Developer Explore** | Auth | Geliştirici keşfet, ilan listesi |
| 6 | **Project Management** | Auth, Dashboard | Proje oluşturma, düzenleme, silme |
| 7 | **Bid Management** | Auth, Projects | Teklif verme, güncelleme, kabul etme |
| 8 | **Contract Management** | Bids | Sözleşme görüntüleme, revize, onay, iptal |
| 9 | **GitHub Integration** | Contracts | Repo bağlama, aktivite timeline |
| 10 | **Profile & Settings** | Auth | Kullanıcı profili, uygulama ayarları |

---

## Feature 1: Core Infrastructure

### Amaç
Multi-module proje yapısının kurulması, temel bileşenlerin oluşturulması.

### Oluşturulacak Dosyalar

#### `:core:common`
```
com/dev4all/mobile/common/
├── result/Result.kt
├── extension/StringExtensions.kt
├── extension/DateExtensions.kt
├── extension/FlowExtensions.kt
├── constant/AppConstants.kt
└── util/NetworkMonitor.kt
```

#### `:core:designsystem`
```
com/dev4all/mobile/designsystem/
├── theme/Color.kt
├── theme/Typography.kt
├── theme/Shape.kt
├── theme/Spacing.kt
├── theme/Dev4AllTheme.kt
├── component/Dev4AllButton.kt
├── component/Dev4AllTextField.kt
├── component/Dev4AllCard.kt
├── component/Dev4AllTopBar.kt
├── component/Dev4AllBottomBar.kt
├── component/Dev4AllLoadingIndicator.kt
├── component/Dev4AllErrorState.kt
├── component/Dev4AllEmptyState.kt
├── component/StatusBadge.kt
├── component/TechnologyChip.kt
└── icon/Dev4AllIcons.kt
```

#### `:core:network`
```
com/dev4all/mobile/network/
├── dto/error/ApiErrorResponse.kt
├── dto/error/ValidationErrorResponse.kt
├── interceptor/AuthInterceptor.kt
├── interceptor/ErrorInterceptor.kt
└── di/NetworkModule.kt
```

#### `:core:datastore`
```
com/dev4all/mobile/datastore/
├── TokenDataStore.kt
├── UserSessionDataStore.kt
├── AppPreferencesDataStore.kt
└── di/DataStoreModule.kt
```

### Kabul Kriterleri
- [ ] Tüm modüller Gradle sync başarılı
- [ ] Tema (Tech Blue palette) uygulanmış
- [ ] Temel Compose bileşenler (button, text field, card) çalışır
- [ ] OkHttp + Retrofit yapılandırılmış
- [ ] DataStore token CRUD çalışır
- [ ] Network monitor çevrimiçi/çevrimdışı algılar

---

## Feature 2: Onboarding

### Ekranlar
1. **Splash Screen** — Uygulama logosu, kısa animasyon
2. **Onboarding Pages** — 3 sayfa (HorizontalPager)
   - Sayfa 1: "Yazılım Projelerin İçin Doğru Ekibi Bul"
   - Sayfa 2: "Freelance Geliştiriciler İçin Yeni Fırsatlar"
   - Sayfa 3: "GitHub Entegrasyonlu Şeffaf Süreç"
3. **"Hemen Başla" butonu** → Auth ekranlarına yönlendirir

### Oluşturulacak Dosyalar

```
app/src/main/kotlin/com/dev4all/mobile/feature/onboarding/
├── OnboardingScreen.kt
├── OnboardingPage.kt           # Tek sayfa composable
├── OnboardingViewModel.kt
└── model/OnboardingPageData.kt # title, description, imageRes
```

### State Management
```kotlin
data class OnboardingUiState(
    val currentPage: Int = 0,
    val isLastPage: Boolean = false,
)
```

### İş Kuralları
- Onboarding yalnızca ilk açılışta gösterilir (`AppPreferencesDataStore.isOnboardingComplete`)
- "Hemen Başla" → `isOnboardingComplete = true` kaydedilir
- Sonraki açılışlarda direkt Login/Home'a yönlendirilir

### Kabul Kriterleri
- [ ] 3 sayfalı horizontal pager çalışır
- [ ] Sayfa göstergesi (dots) aktif sayfayı gösterir
- [ ] Son sayfada "Hemen Başla" butonu gösterilir
- [ ] İlk açılış sonrası onboarding tekrar gösterilmez

---

## Feature 3: Authentication (Login & Register)

### Ekranlar
1. **Login Screen** — Email, password, "Giriş Yap" butonu, "Kayıt Ol" linki
2. **Register Screen** — Rol seçimi (Customer/Developer kartları), ad, email, şifre, "Kayıt Ol" butonu

### Oluşturulacak Dosyalar

#### Domain
```
core/domain/src/main/kotlin/com/dev4all/mobile/domain/
├── model/User.kt
├── model/UserRole.kt
├── model/AuthToken.kt
├── repository/AuthRepository.kt
├── usecase/auth/LoginUseCase.kt
├── usecase/auth/RegisterUseCase.kt
├── usecase/auth/GetCurrentUserUseCase.kt
└── usecase/auth/LogoutUseCase.kt
```

#### Network
```
core/network/src/main/kotlin/com/dev4all/mobile/network/
├── api/AuthApiService.kt
└── dto/auth/
    ├── LoginRequest.kt
    ├── LoginResponse.kt
    ├── RegisterRequest.kt
    ├── RegisterResponse.kt
    └── CurrentUserResponse.kt
```

#### Data
```
core/data/src/main/kotlin/com/dev4all/mobile/data/
├── repository/AuthRepositoryImpl.kt
├── mapper/UserMapper.kt
└── di/DataModule.kt          # AuthRepository binding
```

#### Presentation (App)
```
app/src/main/kotlin/com/dev4all/mobile/feature/auth/
├── login/
│   ├── LoginScreen.kt
│   ├── LoginViewModel.kt
│   ├── LoginUiState.kt
│   └── LoginEvent.kt
└── register/
    ├── RegisterScreen.kt
    ├── RegisterViewModel.kt
    ├── RegisterUiState.kt
    └── RegisterEvent.kt
```

### Backend API Eşleştirmesi
| Mobil Aksiyon | Backend Endpoint | Request DTO | Response DTO |
|---------------|-----------------|-------------|--------------|
| Giriş Yap | `POST /api/v1/auth/login` | `LoginRequest(email, password)` | `LoginResponse(token, expiresAt, email, role)` |
| Kayıt Ol | `POST /api/v1/auth/register` | `RegisterRequest(name, email, password, role)` | `RegisterResponse(userId, email, name)` |
| Profil Getir | `GET /api/v1/auth/me` | — | `CurrentUserResponse(userId, email, role)` |

### Client-Side Validation (Backend ile Uyumlu)

**Login:**
| Alan | Kural |
|------|-------|
| Email | Boş olamaz, geçerli email formatı |
| Password | Boş olamaz |

**Register:**
| Alan | Kural |
|------|-------|
| Name | Boş olamaz, 2-100 karakter |
| Email | Boş olamaz, geçerli email formatı |
| Password | Min 8 karakter, en az 1 büyük harf, en az 1 rakam |
| Role | Customer veya Developer seçilmeli |

### UiState Tanımları

```kotlin
// Login
sealed interface LoginUiState {
    data object Idle : LoginUiState
    data object Loading : LoginUiState
    data class ValidationError(val errors: Map<String, String>) : LoginUiState
    data class Error(val message: String) : LoginUiState
    data object Success : LoginUiState
}

// Register
sealed interface RegisterUiState {
    data object Idle : RegisterUiState
    data object Loading : RegisterUiState
    data class ValidationError(val errors: Map<String, String>) : RegisterUiState
    data class Error(val message: String) : RegisterUiState
    data object Success : RegisterUiState
}
```

### Akış Diyagramı

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ Login Screen │────►│ LoginVM      │────►│ LoginUseCase │
│              │     │              │     │              │
│ email ─────────►│  onEvent()   │────►│  invoke()    │
│ password ──────►│              │     │              │
│              │     │ _uiState ◄──│     │  ┌──────────┤
│ ◄── uiState ─│     │              │     │  │AuthRepo  │
│              │     │  sideEffect ──────►│  │.login()  │
│              │     │  → Navigate  │     │  └──────────┤
└──────────────┘     └──────────────┘     │     │       │
                                          │  ┌──▼──────┐│
                                          │  │API Svc  ││
                                          │  │.login() ││
                                          │  └─────────┘│
                                          │  ┌─────────┐│
                                          │  │TokenDS  ││
                                          │  │.save()  ││
                                          │  └─────────┘│
                                          └──────────────┘
```

### Kabul Kriterleri
- [ ] Login form validation çalışır
- [ ] Başarılı login → token kaydedilir → Home'a yönlendirilir
- [ ] Hatalı login → error mesajı gösterilir
- [ ] Register'da rol seçim kartları çalışır
- [ ] Başarılı register → otomatik login → Home'a yönlendirilir
- [ ] Backend validation hataları field bazlı gösterilir
- [ ] Loading state gösterilir
- [ ] Logout → token temizlenir → Login'e yönlendirilir

---

## Feature 4: Customer Dashboard

### Ekranlar
1. **Customer Home** — "Merhaba, [İsim]", istatistik kartları, son projeler listesi, FAB (+)

### Oluşturulacak Dosyalar

```
app/src/main/kotlin/com/dev4all/mobile/feature/customer/dashboard/
├── CustomerDashboardScreen.kt
├── CustomerDashboardViewModel.kt
├── CustomerDashboardUiState.kt
├── CustomerDashboardEvent.kt
└── component/
    ├── StatisticsCard.kt
    └── RecentProjectItem.kt
```

### State
```kotlin
data class CustomerDashboardUiState(
    val userName: String = "",
    val activeProjectCount: Int = 0,
    val pendingBidCount: Int = 0,
    val recentProjects: List<ProjectSummary> = emptyList(),
    val isLoading: Boolean = false,
    val error: String? = null,
)
```

### Kabul Kriterleri
- [ ] Kullanıcı adı "Merhaba, ..." ile gösterilir
- [ ] Aktif proje ve bekleyen teklif sayıları gösterilir
- [ ] Son projeler listesi yüklenir
- [ ] FAB butonu proje oluşturma ekranına yönlendirir
- [ ] Pull-to-refresh çalışır

---

## Feature 5: Developer Explore

### Ekranlar
1. **Explore Screen** — Arama çubuğu, filtre, açık proje kartları listesi

### Oluşturulacak Dosyalar

```
app/src/main/kotlin/com/dev4all/mobile/feature/developer/explore/
├── ExploreScreen.kt
├── ExploreViewModel.kt
├── ExploreUiState.kt
├── ExploreEvent.kt
└── component/
    ├── SearchBar.kt
    └── ProjectListItem.kt
```

### Domain
```
core/domain/.../
├── model/Project.kt              # (zaten var)
├── model/ProjectStatus.kt        # (zaten var)
├── repository/ProjectRepository.kt
└── usecase/project/GetProjectsUseCase.kt
```

### State
```kotlin
data class ExploreUiState(
    val projects: List<ProjectSummary> = emptyList(),
    val searchQuery: String = "",
    val isLoading: Boolean = false,
    val error: String? = null,
    val currentPage: Int = 1,
    val hasMore: Boolean = true,
)
```

### Kabul Kriterleri
- [ ] Açık projeler listesi yüklenir (pagination)
- [ ] Arama çubuğu ile filtreleme çalışır
- [ ] Proje kartı: başlık, bütçe, kalan süre, teknolojiler gösterilir
- [ ] Karta tıklama → proje detayına yönlendirir
- [ ] Infinite scroll / load more çalışır
- [ ] Empty state gösterilir

---

## Feature 6: Project Management

### Ekranlar
1. **Project Detail** — Proje bilgileri, teklifler, sözleşme durumu, GitHub timeline
2. **Create Project Form** — Yeni proje oluşturma
3. **Edit Project Form** — Proje düzenleme

### Oluşturulacak Dosyalar

#### Domain
```
core/domain/.../usecase/project/
├── GetProjectDetailUseCase.kt
├── CreateProjectUseCase.kt
├── UpdateProjectUseCase.kt
└── DeleteProjectUseCase.kt
```

#### Network
```
core/network/.../api/ProjectApiService.kt
core/network/.../dto/project/
├── CreateProjectRequest.kt
├── UpdateProjectRequest.kt
├── ProjectListResponse.kt
└── ProjectDetailResponse.kt
```

#### Data
```
core/data/.../repository/ProjectRepositoryImpl.kt
core/data/.../mapper/ProjectMapper.kt
```

#### Presentation
```
app/.../feature/project/
├── detail/
│   ├── ProjectDetailScreen.kt
│   ├── ProjectDetailViewModel.kt
│   ├── ProjectDetailUiState.kt
│   └── component/
│       ├── ProjectInfoSection.kt
│       ├── TechnologyChipGroup.kt
│       └── ProjectActionButtons.kt
├── create/
│   ├── CreateProjectScreen.kt
│   ├── CreateProjectViewModel.kt
│   └── CreateProjectUiState.kt
└── edit/
    ├── EditProjectScreen.kt
    ├── EditProjectViewModel.kt
    └── EditProjectUiState.kt
```

### Client-Side Validation
| Alan | Kural |
|------|-------|
| Title | 3-100 karakter, boş olamaz |
| Description | 10-2000 karakter, boş olamaz |
| Budget | Pozitif sayı, > 0 |
| Deadline | Bugünden sonra |
| BidEndDate | Bugünden sonra, Deadline'dan önce |

### İş Kuralları
- Yalnızca **Customer** rolü proje oluşturabilir/düzenleyebilir
- Teklif geldikten sonra bütçe **yalnızca artırılabilir**
- `Ongoing` veya `Completed` projeler silinemez

### Kabul Kriterleri
- [ ] Proje detayı tüm alanlarıyla gösterilir
- [ ] Status badge doğru renk ve metinle gösterilir
- [ ] Proje oluşturma formu validation ile çalışır
- [ ] Tarih seçici (Date Picker) düzgün çalışır
- [ ] Teknoloji chip'leri eklenebilir/silinebilir
- [ ] Proje silme onay dialogu gösterilir

---

## Feature 7: Bid Management

### Ekranlar
1. **Bid List** (Customer) — Proje detayı altında gelen teklifler
2. **Submit Bid** (Developer) — Bottom sheet ile teklif verme
3. **My Bids** (Developer) — Verilen teklifler listesi

### Oluşturulacak Dosyalar

#### Domain
```
core/domain/.../
├── model/Bid.kt
├── model/BidStatus.kt
├── repository/BidRepository.kt
└── usecase/bid/
    ├── GetBidsForProjectUseCase.kt
    ├── SubmitBidUseCase.kt
    ├── UpdateBidUseCase.kt
    └── AcceptBidUseCase.kt
```

#### Network
```
core/network/.../api/BidApiService.kt
core/network/.../dto/bid/
├── BidListResponse.kt
├── SubmitBidRequest.kt
└── UpdateBidRequest.kt
```

#### Presentation
```
app/.../feature/bid/
├── list/
│   ├── BidListSection.kt          # Proje detayında embed
│   └── BidCard.kt
├── submit/
│   ├── SubmitBidBottomSheet.kt
│   ├── SubmitBidViewModel.kt
│   └── SubmitBidUiState.kt
└── mybids/
    ├── MyBidsScreen.kt
    ├── MyBidsViewModel.kt
    └── MyBidsUiState.kt
```

### İş Kuralları
- Yalnızca **Developer** teklif verebilir
- Aynı ilana yalnızca **1 aktif teklif** — 409 Conflict
- `Open` + `BidEndDate` geçmemiş → teklif verilebilir
- Teklif güncelleme sadece `Pending` durumda + ilan `Open` iken
- Customer teklif kabul edince → proje `AwaitingContract` + diğer teklifler `Rejected`

### Kabul Kriterleri
- [ ] Customer: proje detayında teklif listesi görünür
- [ ] Customer: "Kabul Et" butonu çalışır, onay dialogu gösterilir
- [ ] Developer: Bottom sheet ile teklif verilebilir
- [ ] Developer: BidAmount ve ProposalNote validation
- [ ] Developer: Tekliflerini "Tekliflerim" ekranında listeleyebilir
- [ ] Teklif status badge'ı (Pending/Accepted/Rejected) gösterilir

---

## Feature 8: Contract Management

### Ekranlar
1. **Contract View** — Sözleşme metni, revizyon no, onay durumu
2. **Contract Actions** — Revize, onayla, iptal butonları
3. **Revision History** — Geçmiş sözleşme versiyonları

### Oluşturulacak Dosyalar

#### Domain
```
core/domain/.../
├── model/Contract.kt
├── model/ContractStatus.kt
├── model/ContractRevision.kt
├── repository/ContractRepository.kt
└── usecase/contract/
    ├── GetContractUseCase.kt
    ├── ReviseContractUseCase.kt
    ├── ApproveContractUseCase.kt
    ├── CancelContractUseCase.kt
    └── GetContractRevisionsUseCase.kt
```

#### Presentation
```
app/.../feature/contract/
├── view/
│   ├── ContractScreen.kt
│   ├── ContractViewModel.kt
│   ├── ContractUiState.kt
│   └── component/
│       ├── ContractContentCard.kt
│       ├── ContractStatusHeader.kt
│       └── ContractActionButtons.kt
├── revise/
│   ├── ReviseContractScreen.kt
│   └── ReviseContractViewModel.kt
└── history/
    ├── RevisionHistoryScreen.kt
    ├── RevisionHistoryViewModel.kt
    └── component/RevisionCard.kt
```

### İş Kuralları
- Teklif kabul → sistem otomatik `Draft` sözleşme oluşturur
- Taraflardan biri revize ederse → diğerinin onayı sıfırlanır
- Her iki taraf onaylarsa → `BothApproved` → proje `Ongoing`
- İptal → proje de `Cancelled`
- `Cancelled` / `BothApproved` sözleşme düzenlenemez

### Kabul Kriterleri
- [ ] Sözleşme metni okunabilir şekilde gösterilir
- [ ] Revizyon numarası ve onay durumları gösterilir
- [ ] Revize butonu → edit ekranı açılır
- [ ] Onayla butonu → onay dialogu → API çağrısı
- [ ] İptal butonu → uyarı dialogu → API çağrısı
- [ ] Revizyon geçmişi listelenebilir
- [ ] Rol bazlı buton visibility (Customer vs Developer)

---

## Feature 9: GitHub Integration

### Ekranlar
1. **Connect Repo** (Developer) — Repo URL + branch input, bağla butonu
2. **Activity Timeline** (Customer/Developer) — Commit geçmişi timeline

### Oluşturulacak Dosyalar

#### Domain
```
core/domain/.../
├── model/GitHubLog.kt
├── repository/GitHubLogRepository.kt
└── usecase/github/GetGitHubLogsUseCase.kt
```

#### Presentation
```
app/.../feature/github/
├── connect/
│   ├── ConnectRepoSection.kt     # Proje detayında embed
│   └── ConnectRepoViewModel.kt
└── timeline/
    ├── ActivityTimelineSection.kt # Proje detayında embed
    ├── ActivityTimelineViewModel.kt
    └── component/
        ├── TimelineItem.kt
        └── TimelineLine.kt
```

### Kabul Kriterleri
- [ ] Developer: repo URL + branch girişi çalışır
- [ ] URL formatı doğrulanır (github.com domain)
- [ ] Bağlantı başarılı mesajı gösterilir
- [ ] Commit timeline dikey çizgi tasarımıyla gösterilir
- [ ] Her commit: mesaj, yazar, zaman ("2 saat önce") gösterilir
- [ ] Boş timeline durumu (henüz commit yok) gösterilir

---

## Feature 10: Profile & Settings

### Ekranlar
1. **Profile Screen** — Kullanıcı bilgileri, rol, email
2. **Settings Screen** — Tema (dark/light), çıkış yap

### Oluşturulacak Dosyalar

```
app/.../feature/profile/
├── ProfileScreen.kt
├── ProfileViewModel.kt
└── ProfileUiState.kt

app/.../feature/settings/
├── SettingsScreen.kt
└── SettingsViewModel.kt
```

### Kabul Kriterleri
- [ ] Kullanıcı adı, email ve rol gösterilir
- [ ] Çıkış yap butonu çalışır (token temizleme + navigate to login)
- [ ] Dark mode toggle çalışır (DataStore ile persist)
