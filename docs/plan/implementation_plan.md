# Dev4All Mobil Uygulama — Nihai İmplementasyon Planı

> Backend-uyumlu, Offline-First, 3 Rol (Customer / Developer / Admin) için eksiksiz mobil uygulama.

---

## 1. Amaç

Mevcut mobil uygulamayı:
1. **Tasarım dosyalarına birebir** uygun şekilde yeniden yazma
2. **API bağımsız (offline-first)** çalıştırma — Fake Repository Pattern
3. **Backend entity/DTO/enum'larıyla birebir uyumlu** domain modeli kurma
4. **Tüm roller** (Customer, Developer, Admin) için eksiksiz akış sağlama
5. **İleride tek satır DI değişikliğiyle** gerçek API'ye geçiş imkânı

---

## 2. Mimari

```
┌──────────────┐    ┌──────────────┐    ┌───────────────────┐
│  UI/Feature  │───▶│  Use Cases   │───▶│  Repository IF    │
│   Screens    │    │   (Domain)   │    │   (Domain Layer)  │
└──────────────┘    └──────────────┘    └────────┬──────────┘
                                                 │
                                      ┌──────────▼──────────┐
                                      │     DataModule       │
                                      │   @Binds switch      │
                                      └────┬───────────┬─────┘
                                           │           │
                               ┌───────────▼──┐  ┌─────▼──────────┐
                               │ FakeXxxRepo  │  │ XxxRepoImpl    │
                               │ (in-memory)  │  │ (Retrofit+API) │
                               └──────────────┘  └────────────────┘
```

**Geçiş:** `DataModule.kt` → `@Binds` satırında `FakeXxxRepository` → `XxxRepositoryImpl`.

---

## 3. Backend Çakışma Düzeltmeleri (Uygulanacak)

| # | Çakışma | Düzeltme |
|---|---------|----------|
| 1 | `createdAt`/`updatedAt` vs backend `createdDate`/`updatedDate` | Mobil modellerde `createdDate`/`updatedDate` kullanılacak |
| 2 | `isDeleted: Boolean` vs backend `deletedDate: DateTime?` | Mobil'de `deletedDate: String?` olacak |
| 3 | RefreshToken backend'te var, mobil'de yok | `AuthRepository`'ye `logout()` + `refreshToken()` placeholder eklenir |
| 4 | `core/network/mapper` ve `core/data/mapper` çift mapper | `core/network/mapper` silinecek, `core/data/mapper` tek kaynak olacak |
| 5 | `technologies: List<String>` vs backend `String?` | Mobil'de `technologies: String?`, UI'da parse edilecek |

---

## 4. Tam Ekran Listesi

### Auth (Tüm Roller — 2 Ekran)
| Ekran | Tasarım Kaynağı |
|-------|----------------|
| Register | `dev4all_mobile_register.png` birebir |
| Login | Register ile aynı tasarım dili |

### Customer (4 Ekran)
| Ekran | Kaynak |
|-------|--------|
| Customer Dashboard | Projelerim listesi + FAB |
| Project Create | `dev4all_mobile_project_create.png` birebir |
| Customer Project Detail | Teklifler + Sözleşme + GitHub tabları |
| Bid Evaluation | Gelen teklifleri karşılaştırma + Kabul Et |

### Developer (6 Ekran)
| Ekran | Kaynak |
|-------|--------|
| Explore (Open Projects) | `dev4all_mobile_project_list.png` birebir |
| Developer Project Detail | İlan detayı + Teklif Ver butonu |
| Bid Submit | `dev4all_mobile_bid.png` birebir |
| My Bids | Verdiğim teklifler listesi |
| Assigned Project | GitHub repo bağlama + aktivite timeline |
| Contract View | `dev4all_mobile_contract_1/2.png` birebir |

### Admin (3 Ekran)
| Ekran | Kaynak |
|-------|--------|
| Admin Dashboard | Özet kartlar + hızlı erişim |
| User Management | Kullanıcı listesi + rol değiştir + askıya al |
| Project Oversight | Tüm projeler denetimi |

### Ortak (2 Ekran)
| Ekran | Kaynak |
|-------|--------|
| Profile | Profil bilgileri |
| Alerts | Bildirimler (placeholder) |

**Toplam: 17 ekran**

---

## 5. Phase 1 — Domain Layer (Backend-Uyumlu)

Modül: `core/domain`

### 5.1 Modeller

#### [NEW] `core/domain/.../model/Project.kt`
```kotlin
data class Project(
    val id: String,
    val customerId: String,
    val assignedDeveloperId: String?,
    val title: String,
    val description: String,
    val budget: Double,
    val deadline: String,
    val bidEndDate: String,
    val technologies: String?,            // Backend: nullable virgüllü string
    val status: ProjectStatus,
    val bidCount: Int = 0,                // UI hesaplama
    val createdDate: String,              // Backend: CreatedDate
    val updatedDate: String,              // Backend: UpdatedDate
    val deletedDate: String? = null,      // Backend: DeletedDate (soft delete)
)
```

#### [NEW] `core/domain/.../model/ProjectStatus.kt`
```kotlin
enum class ProjectStatus {
    Open,               // 0
    AwaitingContract,    // 1
    Ongoing,             // 2
    Completed,           // 3
    Expired,             // 4
    Cancelled,           // 5
}
```

#### [NEW] `core/domain/.../model/Bid.kt`
```kotlin
data class Bid(
    val id: String,
    val projectId: String,
    val developerId: String,
    val bidAmount: Double,
    val proposalNote: String,
    val status: BidStatus,
    val isAccepted: Boolean,
    val createdDate: String,
    val updatedDate: String,
    // UI-only
    val developerName: String = "",
    val projectTitle: String = "",
)
```

#### [NEW] `core/domain/.../model/BidStatus.kt`
```kotlin
enum class BidStatus {
    Pending,    // 0
    Accepted,   // 1
    Rejected,   // 2
}
```

#### [NEW] `core/domain/.../model/Contract.kt`
```kotlin
data class Contract(
    val id: String,
    val projectId: String,
    val content: String,
    val revisionNumber: Int,
    val lastRevisedById: String,
    val status: ContractStatus,
    val isCustomerApproved: Boolean,
    val isDeveloperApproved: Boolean,
    val customerApprovedAt: String?,
    val developerApprovedAt: String?,
    val createdDate: String,
    val updatedDate: String,
    // UI-only
    val projectTitle: String? = null,
)
```

#### [NEW] `core/domain/.../model/ContractStatus.kt`
```kotlin
enum class ContractStatus {
    Draft,          // 0
    UnderReview,    // 1
    BothApproved,   // 2
    Cancelled,      // 3
}
```

#### [NEW] `core/domain/.../model/ContractRevision.kt`
```kotlin
data class ContractRevision(
    val id: String,
    val contractId: String,
    val revisedById: String,
    val contentSnapshot: String,
    val revisionNumber: Int,
    val revisionNote: String?,
    val createdDate: String,
)
```

#### [NEW] `core/domain/.../model/GitHubLog.kt`
```kotlin
data class GitHubLog(
    val id: String,
    val projectId: String,
    val repoUrl: String,
    val branch: String,
    val commitHash: String,
    val commitMessage: String,
    val authorName: String,
    val pushedAt: String,
)
```

### 5.2 Repository Interface'leri

#### [MODIFY] `core/domain/.../repository/AuthRepository.kt`
```kotlin
interface AuthRepository {
    suspend fun login(email: String, password: String): Result<AuthToken>
    suspend fun register(fullName: String, email: String, password: String, role: String): Result<AuthToken>
    suspend fun getCurrentUser(): Result<User>
    suspend fun logout(): Result<Unit>
}
```

#### [NEW] `core/domain/.../repository/ProjectRepository.kt`
```kotlin
interface ProjectRepository {
    suspend fun getOpenProjects(page: Int = 1, pageSize: Int = 20): Result<List<Project>>
    suspend fun getProjectById(id: String): Result<Project>
    suspend fun getMyProjects(): Result<List<Project>>
    suspend fun getAssignedProjects(): Result<List<Project>>
    suspend fun getAllProjects(): Result<List<Project>>
    suspend fun createProject(title: String, description: String, budget: Double,
        deadline: String, bidEndDate: String, technologies: String?): Result<Project>
    suspend fun updateProject(id: String, title: String?, description: String?,
        budget: Double?, technologies: String?): Result<Project>
    suspend fun deleteProject(id: String): Result<Unit>
}
```

#### [NEW] `core/domain/.../repository/BidRepository.kt`
```kotlin
interface BidRepository {
    suspend fun getByProjectId(projectId: String): Result<List<Bid>>
    suspend fun getMyBids(): Result<List<Bid>>
    suspend fun getByDeveloperAndProject(projectId: String): Result<Bid?>
    suspend fun submitBid(projectId: String, amount: Double, note: String): Result<Bid>
    suspend fun updateBid(bidId: String, amount: Double, note: String): Result<Bid>
    suspend fun acceptBid(bidId: String): Result<Unit>
}
```

#### [NEW] `core/domain/.../repository/ContractRepository.kt`
```kotlin
interface ContractRepository {
    suspend fun getByProjectId(projectId: String): Result<Contract>
    suspend fun reviseContract(projectId: String, content: String, note: String?): Result<Contract>
    suspend fun approveContract(projectId: String): Result<Contract>
    suspend fun cancelContract(projectId: String): Result<Unit>
    suspend fun getRevisions(projectId: String): Result<List<ContractRevision>>
}
```

#### [NEW] `core/domain/.../repository/GitHubRepository.kt`
```kotlin
interface GitHubRepository {
    suspend fun linkRepo(projectId: String, repoUrl: String, branch: String = "main"): Result<Unit>
    suspend fun getActivityLogs(projectId: String): Result<List<GitHubLog>>
}
```

#### [NEW] `core/domain/.../repository/UserManagementRepository.kt`
```kotlin
interface UserManagementRepository {
    suspend fun getAllUsers(): Result<List<User>>
    suspend fun changeUserRole(userId: String, newRole: UserRole): Result<Unit>
    suspend fun suspendUser(userId: String): Result<Unit>
}
```

### 5.3 Use Case'ler

**Auth:** `LoginUseCase` (mevcut), `RegisterUseCase` (mevcut), `GetCurrentUserUseCase`, `LogoutUseCase`

**Project:** `GetOpenProjectsUseCase`, `GetProjectByIdUseCase`, `GetMyProjectsUseCase`, `GetAssignedProjectsUseCase`, `CreateProjectUseCase`, `UpdateProjectUseCase`, `DeleteProjectUseCase`

**Bid:** `GetProjectBidsUseCase`, `GetMyBidsUseCase`, `SubmitBidUseCase`, `UpdateBidUseCase`, `AcceptBidUseCase`

**Contract:** `GetContractUseCase`, `ReviseContractUseCase`, `ApproveContractUseCase`, `CancelContractUseCase`, `GetRevisionsUseCase`

**GitHub:** `LinkRepoUseCase`, `GetActivityLogsUseCase`

**Admin:** `GetAllUsersUseCase`, `ChangeUserRoleUseCase`, `SuspendUserUseCase`, `GetAllProjectsUseCase`

---

## 6. Phase 2 — Fake Repository'ler

Modül: `core/data`

### 6.1 Paylaşılan Veri Deposu

#### [NEW] `core/data/.../fake/FakeData.kt`
```kotlin
object FakeData {
    val users = mutableListOf(
        User(id = "u1", name = "Ahmet Yılmaz", email = "customer@dev4all.com", role = UserRole.Customer),
        User(id = "u2", name = "Elif Demir",   email = "developer@dev4all.com", role = UserRole.Developer),
        User(id = "u3", name = "Admin User",   email = "admin@dev4all.com",     role = UserRole.Admin),
    )
    val passwords = mutableMapOf("customer@dev4all.com" to "Test1234", ...)
    var currentUser: User? = null

    val projects = mutableListOf<Project>(/* 5-6 örnek proje */)
    val bids = mutableListOf<Bid>(/* 3-4 örnek teklif */)
    val contracts = mutableListOf<Contract>(/* 1-2 örnek sözleşme */)
    val contractRevisions = mutableListOf<ContractRevision>()
    val gitHubLogs = mutableListOf<GitHubLog>(/* 5-6 örnek commit */)
}
```

### 6.2 Fake Repository'ler

#### [NEW] `core/data/.../fake/FakeAuthRepository.kt`
- Login: email/password eşleşmesi, başarılıysa sahte JWT token üretimi + `currentUser` set
- Register: validasyon (email benzersiz, password 8+ karakter), listeye ekle
- getCurrentUser: `FakeData.currentUser` dön
- logout: `currentUser = null`

#### [NEW] `core/data/.../fake/FakeProjectRepository.kt`
İş kuralları uygulanır:
- `createProject` → sadece Customer (FR-PR-01), validasyon (FR-PR-02)
- `updateProject` → teklif geldiyse bütçe sadece artırılabilir (FR-PR-05)
- `deleteProject` → Ongoing/Completed silinemez (FR-PR-06), `deletedDate` set edilir
- `getOpenProjects` → status=Open ve deletedDate=null olanlar (FR-PR-07)
- `getMyProjects` → currentUser.id == customerId
- `getAssignedProjects` → currentUser.id == assignedDeveloperId

#### [NEW] `core/data/.../fake/FakeBidRepository.kt`
İş kuralları uygulanır:
- `submitBid` → sadece Developer (FR-BID-01), aynı ilana tek teklif (FR-BID-02)
- BidEndDate geçmişse teklif verilemez (FR-BID-05)
- `acceptBid` →  transaction: proje→AwaitingContract, bid→Accepted, diğerleri→Rejected, Contract oluştur (FR-BID-06)

#### [NEW] `core/data/.../fake/FakeContractRepository.kt`
İş kuralları uygulanır:
- `reviseContract` → revisionNumber++, karşı tarafın onayı sıfırlanır, status=UnderReview (FR-CONTRACT-03)
- `approveContract` → iki taraf da true ise status=BothApproved + proje→Ongoing (FR-CONTRACT-04)
- `cancelContract` → contract→Cancelled + proje→Cancelled (FR-CONTRACT-05)

#### [NEW] `core/data/.../fake/FakeGitHubRepository.kt`
- `linkRepo` → Ongoing projede atanmış developer ise repo bilgisi kaydedilir
- `getActivityLogs` → projectId'ye göre fake GitHubLog listesi

#### [NEW] `core/data/.../fake/FakeUserManagementRepository.kt`
- `getAllUsers` → FakeData.users listesi
- `changeUserRole` → Admin kontrolü + rol güncelleme
- `suspendUser` → deletedDate set

### 6.3 DI Güncellemesi

#### [MODIFY] `core/data/.../di/DataModule.kt`
```kotlin
@Module
@InstallIn(SingletonComponent::class)
abstract class DataModule {
    @Binds @Singleton
    abstract fun bindAuthRepository(impl: FakeAuthRepository): AuthRepository

    @Binds @Singleton
    abstract fun bindProjectRepository(impl: FakeProjectRepository): ProjectRepository

    @Binds @Singleton
    abstract fun bindBidRepository(impl: FakeBidRepository): BidRepository

    @Binds @Singleton
    abstract fun bindContractRepository(impl: FakeContractRepository): ContractRepository

    @Binds @Singleton
    abstract fun bindGitHubRepository(impl: FakeGitHubRepository): GitHubRepository

    @Binds @Singleton
    abstract fun bindUserManagementRepository(impl: FakeUserManagementRepository): UserManagementRepository
    // TODO: API aktifleştiğinde FakeXxx → XxxRepositoryImpl olarak değiştir
}
```

### 6.4 Mapper Temizliği

#### [DELETE] `core/network/.../mapper/AuthMapper.kt`
Çift mapper sorunu çözülür. Tüm mapping'ler `core/data/mapper/` altında kalır.

#### [MODIFY] `core/data/.../mapper/AuthDataMapper.kt`
Mevcut mapper korunur, network mapper'daki fonksiyonlar birleştirilir.

---

## 7. Phase 3 — Design System

Modül: `core/designsystem`

### 7.1 Tema Güncellemeleri

#### [MODIFY] `Color.kt`
```
Primary Blue: #2563EB
Primary Light: #3B82F6
Background Gradient: #EFF6FF → #DBEAFE
Card Background: #FFFFFF
Accent Orange: #F59E0B (ACİL badge)
Accent Red: #EF4444 (Danger)
Success Green: #10B981
Text Primary: #1E293B
Text Secondary: #64748B
```

#### [MODIFY] `Typography.kt`
Tasarımdaki font boyutları ve ağırlıkları

#### [MODIFY] `Shape.kt`
Rounded köşeler: 12dp (card), 24dp (button), 16dp (input)

### 7.2 Yeni Component'ler

#### [NEW] `Dev4AllBottomBar.kt`
- 4 tab: KEŞFET, PROJELER, BİLDİRİM, PROFİL
- Rol bazlı dinamik: Customer→tab1=Dashboard, Developer→tab1=Explore
- Aktif tab mavi, pasif gri

#### [NEW] `Dev4AllTopBar.kt`
- Varyant A: Avatar + "Dev4All" + notification bell
- Varyant B: Geri ok + başlık text

#### [NEW] `Dev4AllSearchBar.kt`
- "Proje veya teknoloji ara..." placeholder
- Outlined rounded style

#### [NEW] `Dev4AllFilterChip.kt`
- Hepsi, Web Geliştirme, Mobil Uygulama, Yapay Zeka, Blockchain
- Seçili: filled mavi | Seçilmemiş: outline

#### [NEW] `Dev4AllProjectCard.kt`
- ACİL badge (kırmızı) + "Son X Gün" (turuncu)
- Başlık + açıklama + teknoloji tag'leri + bütçe aralığı
- "Teklif Ver →" butonu

#### [NEW] `Dev4AllBidCard.kt`
- Developer adı, teklif tutarı, öneri notu, BidStatus badge

#### [NEW] `Dev4AllContractSection.kt`
- Sözleşme maddesi card, revizyon badge, onay durumu göstergeleri

#### [NEW] `Dev4AllStatusBadge.kt`
- ProjectStatus, BidStatus, ContractStatus için renkli badge'ler
- Open=mavi, Ongoing=yeşil, Expired=gri, Cancelled=kırmızı, vb.

#### [NEW] `Dev4AllTimelineItem.kt`
- Commit hash kısaltması, mesaj, yazar, zaman, additions/deletions göstergesi

#### [MODIFY] `Dev4AllButton.kt`
- Tam genişlik rounded gradient mavi buton
- İkon + text varyantı
- Outline varyantı (Revize Et) ve Danger text varyantı (İptal Et)

#### [MODIFY] `Dev4AllTextField.kt`
- Label üstte, rounded köşeli, açık gri arka plan
- Multiline text area varyantı

#### [NEW] `Dev4AllRoleCard.kt`
- Register ekranındaki Müşteri/Geliştirici seçim kartları
- İkon + açıklama + seçim checkmark

---

## 8. Phase 4 — Auth Ekranları (Tasarıma Birebir)

### [MODIFY] `RegisterScreen.kt`
Tasarım: `dev4all_mobile_register.png`
```
┌─────────────────────────────────┐
│ Dev4All                Giriş Yap│  ← TopBar
├─────────────────────────────────┤
│                                 │
│  Geleceği                       │
│  Birlikte İnşa Edelim.         │  ← Hero (bold kısım mavi)
│                                 │
│  ┌─────────────────────────────┐│
│  │ Hesabınızı Oluşturun       ││
│  │ Hangi rolde devam etmek    ││
│  │ istersiniz?                ││
│  │                            ││
│  │ [📁 Müşteri] [◇ Geliştirici]│  ← RoleCard
│  │                            ││
│  │ Ad Soyad          [_______]││
│  │ E-posta Adresi    [_______]││
│  │ Şifre             [_______]││
│  │                            ││
│  │ ☐ Kullanım Koşulları...    ││
│  │                            ││
│  │ [      Kayıt Ol      ]     ││  ← Gradient mavi buton
│  │                            ││
│  │ Zaten hesabın var mı?      ││
│  │ Giriş Yap                  ││
│  └─────────────────────────────┘│
│                                 │
│ © 2024 Dev4All.                 │
│ Güvenlik · KVKK · İletişim      │
└─────────────────────────────────┘
```

### [MODIFY] `LoginScreen.kt`
Aynı tasarım dili: Hero text + Card (email + şifre + "Giriş Yap" buton + "Kayıt Ol" link)

### [MODIFY] `RegisterViewModel.kt`
- `@HiltViewModel` + `@Inject` eklenir
- Validasyon: Name 2-100, Email format, Password 8+ & 1 büyük harf & 1 rakam

### `LoginViewModel.kt` — mevcut yapı korunur

---

## 9. Phase 5 — Feature Ekranları

### 9A. Customer Ekranları

#### [NEW] `feature/customer/dashboard/CustomerDashboardScreen.kt` + ViewModel + UiState
- Projelerim listesi (LazyColumn)
- Her kart: başlık, status badge, bidCount, bütçe
- FAB: "+ Yeni Proje" → ProjectCreate'e navigate

#### [NEW] `feature/project/create/ProjectCreateScreen.kt` + ViewModel + UiState + Event
Tasarım: `dev4all_mobile_project_create.png`
- Form: Proje Başlığı (3-100), Açıklama (10-2000), Bütçe (>0), Teknolojiler, Teslim Tarihi, Teklif Bitiş Tarihi
- "İlanı Yayınla ▶" butonu
- Validasyon: FRD §8.2

#### [NEW] `feature/project/detail/CustomerProjectDetailScreen.kt` + ViewModel
- Proje bilgileri + status badge
- 3 Tab: Teklifler | Sözleşme | GitHub Aktivite
- Teklifler: BidCard listesi + "Kabul Et" butonu
- Sözleşme: Contract içerik + Onayla/Revize/İptal
- GitHub: Timeline (commit listesi)

#### [NEW] `feature/bid/evaluate/BidEvaluateScreen.kt` + ViewModel
- Gelen teklifler karşılaştırma grid
- "Kabul Et" → AcceptBidUseCase → proje→AwaitingContract

---

### 9B. Developer Ekranları

#### [NEW] `feature/explore/ExploreScreen.kt` + ViewModel + UiState + Event
Tasarım: `dev4all_mobile_project_list.png`
- TopBar: Avatar + "Dev4All" + notification
- SearchBar + Filtrele butonu
- Kategori chip'leri (FilterChip)
- Proje kartları listesi (ProjectCard + LazyColumn)
- Bottom navigation

#### [NEW] `feature/project/detail/DeveloperProjectDetailScreen.kt` + ViewModel
- İlan detayı: başlık, açıklama, bütçe, teknolojiler, deadline, bidEndDate
- "Teklif Ver" butonu → BidSubmit'e navigate
- Atanmış projeler: repo bağlama + timeline

#### [NEW] `feature/bid/submit/BidSubmitScreen.kt` + ViewModel + UiState + Event
Tasarım: `dev4all_mobile_bid.png`
- Teklif Tutarı (₺) — büyük font input
- "Platform komisyonu sonrası kazancınız: ₺X"
- Öneri Notu (10-1000 karakter, multiline)
- Bilgi kutusu
- "Teklifi Gönder ▶" butonu

#### [NEW] `feature/bid/mybids/MyBidsScreen.kt` + ViewModel
- Verdiğim teklifler listesi (BidCard)
- Status badge: Pending (sarı), Accepted (yeşil), Rejected (kırmızı)

#### [NEW] `feature/project/assigned/AssignedProjectScreen.kt` + ViewModel
- Atandığı Ongoing projeler
- GitHub repo bağlama formu (URL + Branch)
- Commit timeline görüntüleme

#### [NEW] `feature/contract/ContractScreen.kt` + ViewModel + UiState + Event
Tasarım: `dev4all_mobile_contract_1.png` + `contract_2.png`
- "Projelere Dön" link
- "Proje Sözleşmesi" başlık + proje açıklaması
- Revizyon badge (v1, v2...) + Durum badge
- Sözleşme maddeleri card
- Süreç özeti: son güncelleme, onay bekleyenler
- 3 buton: "Sözleşmeyi Onayla" (Primary) / "Revize Et" (Outline) / "İptal Et" (Danger text)

---

### 9C. Admin Ekranları

#### [NEW] `feature/admin/dashboard/AdminDashboardScreen.kt` + ViewModel
- Özet kartlar: toplam kullanıcı, aktif proje, bekleyen teklif, açık sözleşme
- Hızlı erişim butonları

#### [NEW] `feature/admin/users/UserManagementScreen.kt` + ViewModel
- Kullanıcı listesi: Ad, Email, Rol, Durum
- Rol değiştirme dropdown
- Askıya alma butonu

#### [NEW] `feature/admin/projects/ProjectOversightScreen.kt` + ViewModel
- Tüm projeler (tüm statuslar)
- Filtreleme: status, tarih

---

### 9D. Ortak Ekranlar

#### [NEW] `feature/profile/ProfileScreen.kt` + ViewModel
- Kullanıcı bilgileri: ad, email, rol
- Çıkış yap butonu

#### [NEW] `feature/alerts/AlertsScreen.kt`
- Bildirim listesi (placeholder — fake notification'lar)

---

## 10. Phase 6 — Navigation & App Shell

### [NEW] `navigation/Screen.kt`
```kotlin
sealed class Screen(val route: String) {
    // Auth
    data object Login : Screen("login")
    data object Register : Screen("register")

    // Customer
    data object CustomerDashboard : Screen("customer/dashboard")
    data object ProjectCreate : Screen("project/create")
    data object CustomerProjectDetail : Screen("customer/project/{projectId}")
    data object BidEvaluate : Screen("customer/project/{projectId}/bids")

    // Developer
    data object Explore : Screen("explore")
    data object DeveloperProjectDetail : Screen("developer/project/{projectId}")
    data object BidSubmit : Screen("developer/project/{projectId}/bid")
    data object MyBids : Screen("developer/bids")
    data object AssignedProject : Screen("developer/assigned/{projectId}")

    // Shared
    data object Contract : Screen("contract/{projectId}")
    data object Profile : Screen("profile")
    data object Alerts : Screen("alerts")

    // Admin
    data object AdminDashboard : Screen("admin/dashboard")
    data object UserManagement : Screen("admin/users")
    data object ProjectOversight : Screen("admin/projects")
}
```

### [NEW] `navigation/BottomNavItem.kt`
Rol bazlı tab yapılandırması:
- **Customer:** Dashboard | Explore | Alerts | Profile
- **Developer:** Explore | My Bids | Alerts | Profile
- **Admin:** Dashboard | Projects | Users | Profile

### [NEW] `navigation/Dev4AllNavHost.kt`
```
Auth Graph: login ↔ register
Main Graph: Bottom Bar Scaffold
  ├── Customer tabs → project create, project detail, bid evaluate, contract
  ├── Developer tabs → project detail, bid submit, assigned project, contract
  └── Admin tabs → user management, project oversight
```

### [MODIFY] `MainActivity.kt`
- PlaceholderDestination enum kaldırılır
- Jetpack Navigation Compose + Bottom Bar Scaffold
- Login durumuna göre Auth ↔ Main yönlendirme

### [MODIFY] `DomainModule.kt`
- Tüm yeni UseCase'ler için `@Provides` fonksiyonları eklenir

---

## 11. Phase 7 — Build, Test & Doğrulama

### Build
```bash
cd mobile
./gradlew assembleDebug
./gradlew :app:testDebugUnitTest
```

### Fonksiyonel Test (Offline — İnternet Kapalı)

**Customer akışı:**
1. Register (Customer) → Login (customer@dev4all.com / Test1234)
2. Dashboard → Yeni Proje oluştur (validasyon test)
3. Projeye gelen teklifi gör → Kabul Et
4. Sözleşme taslağı oluşur → Onayla
5. GitHub timeline'da commit geçmişi

**Developer akışı:**
1. Login (developer@dev4all.com / Test1234)
2. Explore → Filtreleme → İlan detay
3. Teklif ver (validasyon: 10-1000 karakter not)
4. My Bids → Durum kontrolü
5. Kabul edilen proje → Sözleşme onayla → GitHub repo bağla

**Admin akışı:**
1. Login (admin@dev4all.com / Test1234)
2. Dashboard → Kullanıcı yönetimi
3. Rol değiştir + Hesap askıya al
4. Proje denetimi

### Tasarım Uyumluluk
- Her ekranı tasarım PNG'siyle yan yana karşılaştır
- Renk, font, spacing, border radius değerlerini doğrula

---

## 12. Dosya Özeti

| Katman | Yeni | Değişen | Silinen |
|--------|------|---------|---------|
| Domain (models, repos, use cases) | ~22 | 1 | 0 |
| Data (fake repos, DI, mapper) | 7 | 2 | 1 |
| Design System (components, theme) | 9 | 4 | 0 |
| Feature Screens (UI + VM + State) | ~30 | 4 | 0 |
| Navigation | 3 | 1 | 0 |
| **TOPLAM** | **~71** | **~12** | **1** |

---

## 13. Backend API Uyumluluk Haritası

| Mobil İşlem | Fake Repository | Backend Endpoint | Response DTO |
|-------------|----------------|------------------|-------------|
| Login | `FakeAuthRepository.login` | `POST /api/v1/auth/login` | `LoginUserResponse(Token, ExpiresAt, Email, Role)` |
| Register | `FakeAuthRepository.register` | `POST /api/v1/auth/register` | `RegisterUserResponse(UserId, Email, Name)` |
| Current User | `FakeAuthRepository.getCurrentUser` | `GET /api/v1/auth/me` | `GetCurrentUserResponse(UserId, Email, Role)` |
| Open Projects | `FakeProjectRepository.getOpenProjects` | `GET /api/v1/projects` | `PagedResult<Project>` |
| Create Project | `FakeProjectRepository.createProject` | `POST /api/v1/projects` | `Project` |
| Project Detail | `FakeProjectRepository.getProjectById` | `GET /api/v1/projects/{id}` | `Project` |
| Submit Bid | `FakeBidRepository.submitBid` | `POST /api/v1/projects/{id}/bids` | `Bid` |
| Accept Bid | `FakeBidRepository.acceptBid` | `POST /api/v1/bids/{id}/accept` | — |
| Get Contract | `FakeContractRepository.getByProjectId` | `GET /api/v1/contracts/{projectId}` | `Contract` |
| Approve Contract | `FakeContractRepository.approveContract` | `POST /api/v1/contracts/{projectId}/approve` | `Contract` |
| Revise Contract | `FakeContractRepository.reviseContract` | `PUT /api/v1/contracts/{projectId}` | `Contract` |
| Cancel Contract | `FakeContractRepository.cancelContract` | `POST /api/v1/contracts/{projectId}/cancel` | — |
| Link Repo | `FakeGitHubRepository.linkRepo` | `PUT /api/v1/projects/{id}/repo` | — |
| Activity Logs | `FakeGitHubRepository.getActivityLogs` | `GET /api/v1/projects/{id}/github-logs` | `List<GitHubLog>` |
