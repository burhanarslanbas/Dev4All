# 05. UI Implementation Plan

> Ekran tasarımları, Navigation grafiği, Compose bileşen detayları, tema sistemi ve responsive tasarım.

---

## 1. Renk Paleti — Tech Blue Theme

Tasarım kılavuzundaki (docs/design/06-stitch-mobile-prompts.md) **"güven veren lacivert ve canlı tech-blue tonları"** direktifine uygun:

### 1.1. Light Theme

```kotlin
// Primary — Tech Blue
val TechBlue = Color(0xFF1565C0)          // Primary
val TechBlueLight = Color(0xFF1E88E5)     // Primary variant / lighter
val TechBlueDark = Color(0xFF0D47A1)      // Primary dark / app bar

// Secondary — Accent
val AccentBlue = Color(0xFF42A5F5)        // Secondary
val AccentTeal = Color(0xFF26A69A)        // Tertiary / success accent

// Surface & Background
val SurfaceWhite = Color(0xFFFFFFFF)      // Surface
val BackgroundGray = Color(0xFFF5F7FA)    // Background
val CardWhite = Color(0xFFFFFFFF)         // Card surface

// Text
val TextPrimary = Color(0xFF1A1A2E)       // On-background primary
val TextSecondary = Color(0xFF6B7280)     // On-background secondary
val TextOnPrimary = Color(0xFFFFFFFF)     // On-primary

// Status Colors
val StatusOpen = Color(0xFF1E88E5)        // Blue — Open
val StatusAwaitingContract = Color(0xFFFFA726)  // Orange — AwaitingContract
val StatusOngoing = Color(0xFF66BB6A)     // Green — Ongoing
val StatusCompleted = Color(0xFF26A69A)   // Teal — Completed
val StatusExpired = Color(0xFFBDBDBD)     // Gray — Expired
val StatusCancelled = Color(0xFFEF5350)   // Red — Cancelled

// Semantic
val ErrorRed = Color(0xFFD32F2F)
val WarningOrange = Color(0xFFF57C00)
val SuccessGreen = Color(0xFF388E3C)
val InfoBlue = Color(0xFF1976D2)
```

### 1.2. Dark Theme

```kotlin
val DarkSurface = Color(0xFF1A1A2E)
val DarkBackground = Color(0xFF0F0F1E)
val DarkCard = Color(0xFF232340)
val DarkTextPrimary = Color(0xFFE0E0E0)
val DarkTextSecondary = Color(0xFF9E9E9E)
// Primary and status renkler aynı kalır (zaten yeterince parlak)
```

---

## 2. Tipografi

```kotlin
val Dev4AllTypography = Typography(
    displayLarge = TextStyle(
        fontFamily = FontFamily.Default,  // veya Google Fonts'tan "Inter"
        fontWeight = FontWeight.Bold,
        fontSize = 32.sp,
        lineHeight = 40.sp,
    ),
    headlineLarge = TextStyle(
        fontWeight = FontWeight.Bold,
        fontSize = 24.sp,
        lineHeight = 32.sp,
    ),
    headlineMedium = TextStyle(
        fontWeight = FontWeight.SemiBold,
        fontSize = 20.sp,
        lineHeight = 28.sp,
    ),
    titleLarge = TextStyle(
        fontWeight = FontWeight.SemiBold,
        fontSize = 18.sp,
        lineHeight = 24.sp,
    ),
    titleMedium = TextStyle(
        fontWeight = FontWeight.Medium,
        fontSize = 16.sp,
        lineHeight = 22.sp,
    ),
    bodyLarge = TextStyle(
        fontWeight = FontWeight.Normal,
        fontSize = 16.sp,
        lineHeight = 24.sp,
    ),
    bodyMedium = TextStyle(
        fontWeight = FontWeight.Normal,
        fontSize = 14.sp,
        lineHeight = 20.sp,
    ),
    bodySmall = TextStyle(
        fontWeight = FontWeight.Normal,
        fontSize = 12.sp,
        lineHeight = 16.sp,
    ),
    labelLarge = TextStyle(
        fontWeight = FontWeight.Medium,
        fontSize = 14.sp,
        lineHeight = 20.sp,
    ),
    labelMedium = TextStyle(
        fontWeight = FontWeight.Medium,
        fontSize = 12.sp,
        lineHeight = 16.sp,
    ),
)
```

---

## 3. Spacing System

```kotlin
object Spacing {
    val xs = 4.dp
    val sm = 8.dp
    val md = 16.dp
    val lg = 24.dp
    val xl = 32.dp
    val xxl = 48.dp

    val screenPadding = 16.dp
    val cardPadding = 16.dp
    val sectionGap = 24.dp
    val itemGap = 12.dp
}
```

---

## 4. Navigation Graph

### 4.1. Tam Navigation Yapısı

```
NavHost (startDestination = splash)
│
├── splash (SplashScreen)
│   └── → onboarding | auth/login | customer_main | developer_main
│
├── onboarding (OnboardingScreen)
│   └── → auth/login
│
├── auth/ (Nested Graph)
│   ├── login (LoginScreen)
│   │   └── → auth/register | customer_main | developer_main
│   └── register (RegisterScreen)
│       └── → auth/login | customer_main | developer_main
│
├── customer_main/ (Nested Graph — Bottom Nav)
│   ├── home (CustomerDashboardScreen)
│   │   └── → project/create | project/{id}
│   ├── my_projects (MyProjectsScreen)
│   │   └── → project/{id}
│   ├── notifications (NotificationsScreen)
│   └── profile (ProfileScreen)
│       └── → settings
│
├── developer_main/ (Nested Graph — Bottom Nav)
│   ├── explore (ExploreScreen)
│   │   └── → project/{id}
│   ├── my_bids (MyBidsScreen)
│   │   └── → project/{id}
│   ├── my_projects (DevProjectsScreen)
│   │   └── → project/{id}
│   └── profile (ProfileScreen)
│       └── → settings
│
├── project/ (Shared Screens)
│   ├── {id} (ProjectDetailScreen)
│   │   └── → contract/{projectId} | bid/submit/{projectId}
│   ├── create (CreateProjectScreen)
│   └── edit/{id} (EditProjectScreen)
│
├── contract/
│   ├── {projectId} (ContractScreen)
│   │   └── → contract/revise/{projectId} | contract/history/{projectId}
│   ├── revise/{projectId} (ReviseContractScreen)
│   └── history/{projectId} (RevisionHistoryScreen)
│
├── bid/
│   └── submit/{projectId} (SubmitBidBottomSheet)
│
└── settings (SettingsScreen)
```

### 4.2. Navigation Route Tanımları

```kotlin
sealed class Route(val route: String) {
    // Root
    data object Splash : Route("splash")
    data object Onboarding : Route("onboarding")

    // Auth
    data object Login : Route("auth/login")
    data object Register : Route("auth/register")

    // Customer Main (Bottom Nav)
    data object CustomerHome : Route("customer/home")
    data object CustomerProjects : Route("customer/projects")
    data object CustomerNotifications : Route("customer/notifications")

    // Developer Main (Bottom Nav)
    data object DeveloperExplore : Route("developer/explore")
    data object DeveloperBids : Route("developer/bids")
    data object DeveloperProjects : Route("developer/projects")

    // Shared
    data object Profile : Route("profile")
    data object Settings : Route("settings")

    // Project
    data class ProjectDetail(val id: String) : Route("project/$id") {
        companion object { const val ROUTE = "project/{projectId}" }
    }
    data object CreateProject : Route("project/create")
    data class EditProject(val id: String) : Route("project/edit/$id") {
        companion object { const val ROUTE = "project/edit/{projectId}" }
    }

    // Contract
    data class ContractView(val projectId: String) : Route("contract/$projectId") {
        companion object { const val ROUTE = "contract/{projectId}" }
    }
    data class ReviseContract(val projectId: String) : Route("contract/revise/$projectId") {
        companion object { const val ROUTE = "contract/revise/{projectId}" }
    }
    data class RevisionHistory(val projectId: String) : Route("contract/history/$projectId") {
        companion object { const val ROUTE = "contract/history/{projectId}" }
    }

    // Bid
    data class SubmitBid(val projectId: String) : Route("bid/submit/$projectId") {
        companion object { const val ROUTE = "bid/submit/{projectId}" }
    }
}
```

### 4.3. Bottom Navigation Items

**Customer:**
| Icon | Label | Route |
|------|-------|-------|
| `Icons.Filled.Home` | Ana Sayfa | `customer/home` |
| `Icons.Filled.Folder` | Projelerim | `customer/projects` |
| `Icons.Filled.Notifications` | Bildirimler | `customer/notifications` |
| `Icons.Filled.Person` | Profil | `profile` |

**Developer:**
| Icon | Label | Route |
|------|-------|-------|
| `Icons.Filled.Explore` | Keşfet | `developer/explore` |
| `Icons.Filled.LocalOffer` | Tekliflerim | `developer/bids` |
| `Icons.Filled.Work` | Projelerim | `developer/projects` |
| `Icons.Filled.Person` | Profil | `profile` |

---

## 5. Ekran Detayları

### 5.1. Splash Screen

**Tasarım:**
- Tam ekran, Tech Blue gradient arka plan
- Merkeze Dev4All logosu (animated fade-in)
- 1.5 saniye bekleme → navigation kararı

**Compose:**
```kotlin
@Composable
fun SplashScreen(
    onNavigate: (Route) -> Unit,
) {
    // Check: token var mı? + onboarding tamamlandı mı?
    // → Appropriate destination
}
```

---

### 5.2. Onboarding

**Tasarım:**
- `HorizontalPager` ile 3 sayfa
- Her sayfada: üst yarı illüstrasyon, alt yarı başlık + açıklama
- Alt kısımda: sayfa göstergesi (dots) + "İleri" / "Hemen Başla" butonu
- Son sayfada "Hemen Başla" büyük buton

**Compose Yapısı:**
```
OnboardingScreen
├── HorizontalPager
│   └── OnboardingPage (x3)
│       ├── Image/Illustration
│       ├── Title
│       └── Description
├── PageIndicator (dots)
└── ActionButton ("İleri" | "Hemen Başla")
```

---

### 5.3. Login Screen

**Tasarım:**
- Üst: Dev4All logo + "Hoş Geldin" başlığı
- Orta: Email input (floating label, email keyboard), Password input (toggle visibility)
- Alt: "Giriş Yap" full-width button, "Hesabın yok mu? Kayıt Ol" link

**Compose Yapısı:**
```
LoginScreen
├── Logo
├── WelcomeTitle
├── Dev4AllTextField (Email)
├── Dev4AllTextField (Password, visibility toggle)
├── "Şifremi Unuttum" link (deaktif — MVP dışı)
├── Dev4AllButton ("Giriş Yap")
├── Loading overlay (when state = Loading)
└── "Kayıt Ol" link → navigate to Register
```

---

### 5.4. Register Screen

**Tasarım:**
- Üst: "Kayıt Ol" başlığı + geri butonu
- Rol seçimi: 2 büyük kart yan yana — "Müşteri (Proje Sahibi)" | "Geliştirici (Developer)"
  - Seçili kart: elevated + primary border + check icon
  - Seçilmemiş: outline style
- Form alanları (rol seçildikten sonra görünür): Ad Soyad, Email, Şifre
- Alt: "Kayıt Ol" full-width button, "Zaten hesabın var mı? Giriş Yap" link

**Compose Yapısı:**
```
RegisterScreen
├── TopBar (back arrow + "Kayıt Ol")
├── RoleSelectionRow
│   ├── RoleCard (Customer) — clickable
│   └── RoleCard (Developer) — clickable
├── AnimatedVisibility (expand when role selected)
│   ├── Dev4AllTextField (Name)
│   ├── Dev4AllTextField (Email)
│   └── Dev4AllTextField (Password)
├── Dev4AllButton ("Kayıt Ol")
└── "Giriş Yap" link
```

---

### 5.5. Customer Dashboard

**Tasarım:**
- Üst: "Merhaba, [İsim]" + profil avatarı
- İstatistik kartları (horizontal scroll): "Aktif Projeler (2)", "Bekleyen Teklifler (5)"
- "Son Projeler" listesi (LazyColumn)
- FAB: Yeni proje oluştur (+)

**Compose Yapısı:**
```
CustomerDashboardScreen
├── Scaffold
│   ├── TopBar ("Merhaba, İsim" + avatar)
│   ├── Content
│   │   ├── StatisticsRow (horizontal)
│   │   │   ├── StatCard (Aktif Projeler)
│   │   │   └── StatCard (Bekleyen Teklifler)
│   │   └── LazyColumn ("Son Projeler")
│   │       └── ProjectCard (x n)
│   ├── FAB (+ icon, "Yeni Proje")
│   └── BottomBar (Customer tabs)
```

---

### 5.6. Developer Explore

**Tasarım:**
- Üst: Büyük arama çubuğu + filtre ikonu
- Proje kartları (LazyColumn), her kart:
  - Proje başlığı
  - Bütçe ($2,500)
  - Kalan süre badge ("Son 2 Gün")
  - Teknoloji chip'leri (React, .NET)
  - "Teklif Ver" yönlendirme oku

**Compose Yapısı:**
```
ExploreScreen
├── Scaffold
│   ├── Content
│   │   ├── SearchBar + FilterIcon
│   │   └── LazyColumn
│   │       └── ExploreProjectCard (x n)
│   │           ├── Title
│   │           ├── BudgetText
│   │           ├── RemainingTimeBadge
│   │           ├── TechChipRow
│   │           └── "Teklif Ver" arrow
│   └── BottomBar (Developer tabs)
```

---

### 5.7. Project Detail

**Tasarım:**
- Üst: Proje başlığı + status badge (Open/Ongoing/...)
- Teknoloji chip'leri (horizontal scroll)
- Açıklama (expandable text)
- Bütçe, deadline, teklif bitiş tarihi bilgileri
- **Customer görünümü:** Teklifler listesi + kabul butonu
- **Developer görünümü:** Teklif ver bottom sheet

**Compose Yapısı:**
```
ProjectDetailScreen
├── Scaffold
│   ├── TopBar (back + title)
│   └── LazyColumn
│       ├── StatusBadge
│       ├── TechChipGroup
│       ├── DescriptionSection
│       ├── InfoGrid (Budget, Deadline, BidEndDate)
│       ├── if (isCustomer && project.status == Open)
│       │   └── BidListSection
│       │       └── BidCard (x n) with "Kabul Et" button
│       ├── if (isDeveloper && project.status == Open)
│       │   └── SubmitBidButton → opens BottomSheet
│       ├── if (project.status >= AwaitingContract)
│       │   └── ContractSection → navigate to Contract
│       ├── if (project.status == Ongoing)
│       │   └── GitHubSection
│       │       ├── ConnectRepoSection (if no repo)
│       │       └── ActivityTimelineSection (if repo connected)
```

---

### 5.8. Submit Bid — Bottom Sheet

**Tasarım:**
- Ekranın alt 1/3'ünden yukarı açılır
- Başlık: "Teklif Ver"
- Büyük rakamlı BidAmount input ($)
- ProposalNote textarea
- "Teklifi Gönder" full-width button

**Compose:**
```kotlin
@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SubmitBidBottomSheet(
    onDismiss: () -> Unit,
    onSubmit: (amount: Double, note: String) -> Unit,
) {
    ModalBottomSheet(onDismissRequest = onDismiss) {
        // content
    }
}
```

---

### 5.9. Contract Screen

**Tasarım:**
- Üst: "Proje Sözleşmesi" başlığı
- Sözleşme metni: kaydırılabilir, serif tipografi, ciddi görünüm
- Revizyon numarası: "v1", "v2", ...
- Durum etiketi: "Awaiting Approval", "Under Review", vb.
- Onay durumları: Customer ✓/✗, Developer ✓/✗
- Alt butonlar:
  - "Revize Et" (Outline)
  - "İptal Et" (Red/Warning)
  - "Sözleşmeyi Onayla" (Filled primary — en belirgin)

---

### 5.10. GitHub Activity Timeline

**Tasarım:**
- Dikey zaman çizelgesi (sol tarafta çizgi + dot)
- Her commit item:
  - Commit mesajı (bold)
  - Yazar adı + zaman ("Ahmet Yılmaz — 2 saat önce")
  - Branch ikonu
- Temiz, okunabilir, teknik bilgisi olmayan kullanıcı için anlaşılır

**Compose Yapısı:**
```
ActivityTimelineSection
├── "GitHub Aktivite Akışı" header
└── LazyColumn
    └── TimelineItem (x n)
        ├── TimelineDot + Line
        ├── CommitMessage
        ├── AuthorName + RelativeTime
        └── BranchIcon
```

---

## 6. Reusable Compose Components

### 6.1. Dev4AllButton

```kotlin
@Composable
fun Dev4AllButton(
    text: String,
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
    enabled: Boolean = true,
    isLoading: Boolean = false,
    variant: ButtonVariant = ButtonVariant.Primary,
)

enum class ButtonVariant { Primary, Secondary, Outline, Danger, Text }
```

### 6.2. Dev4AllTextField

```kotlin
@Composable
fun Dev4AllTextField(
    value: String,
    onValueChange: (String) -> Unit,
    label: String,
    modifier: Modifier = Modifier,
    error: String? = null,
    keyboardType: KeyboardType = KeyboardType.Text,
    isPassword: Boolean = false,
    leadingIcon: ImageVector? = null,
    maxLines: Int = 1,
)
```

### 6.3. StatusBadge

```kotlin
@Composable
fun StatusBadge(
    status: String,
    modifier: Modifier = Modifier,
)

// Otomatik renk eşleştirmesi:
// "Open" → StatusOpen (blue pill)
// "AwaitingContract" → StatusAwaitingContract (orange pill)
// "Ongoing" → StatusOngoing (green pill)
// "Completed" → StatusCompleted (teal pill)
// "Expired" → StatusExpired (gray pill)
// "Cancelled" → StatusCancelled (red pill)
```

### 6.4. TechnologyChip

```kotlin
@Composable
fun TechnologyChip(
    label: String,
    modifier: Modifier = Modifier,
    onRemove: (() -> Unit)? = null,  // null = read-only mode
)
```

### 6.5. ProjectCard

```kotlin
@Composable
fun ProjectCard(
    project: ProjectSummary,
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
)
// İçerik: başlık, bütçe, status badge, teknoloji chip'leri, kalan süre
```

### 6.6. EmptyState

```kotlin
@Composable
fun Dev4AllEmptyState(
    icon: ImageVector,
    title: String,
    description: String,
    modifier: Modifier = Modifier,
    actionText: String? = null,
    onAction: (() -> Unit)? = null,
)
```

### 6.7. ErrorState

```kotlin
@Composable
fun Dev4AllErrorState(
    message: String,
    modifier: Modifier = Modifier,
    onRetry: (() -> Unit)? = null,
)
```

---

## 7. Responsive Tasarım Notları

- Tüm spacing değerleri `Spacing` object'inden çekilir, hardcoded **YASAK**
- `fillMaxWidth()` tercih edilir, sabit genişlik sadece ikonlarda
- Liste item'ları esnek height ile tasarlanır
- Bottom sheet'ler ekranın %40-60'ını kaplar
- FAB her zaman sağ altta sabit konumda
- Keyboard açıldığında form alanları görünür kalır (`imePadding()`)
- Edge-to-edge display (`enableEdgeToEdge()`) aktif

---

## 8. Animasyon Planı

| Animasyon | Kullanım Yeri | Tür |
|-----------|--------------|-----|
| Fade-in | Splash logo | `animateFloatAsState` |
| Slide horizontal | Onboarding pager | `HorizontalPager` built-in |
| Content size | Rol seçimi → form alanları | `AnimatedVisibility` |
| Shared element | Liste → detay | `SharedTransitionLayout` (Compose 1.7+) |
| Loading shimmer | Skeleton loading | `ShimmerEffect` custom |
| Button loading | Submit buttons | Spinner inside button |
| Slide up | Bottom sheet | `ModalBottomSheet` built-in |
| Timeline appear | GitHub commits | Staggered `AnimatedVisibility` |
