# 01. Mobile Architecture Plan

> Dev4All Android uygulamasının mimari kararları, katman yapısı, modül organizasyonu ve veri akış desenleri.

---

## 1. Mimari Yaklaşım: Clean Architecture + MVVM + UDF

### 1.1. Neden Clean Architecture?

Backend projesi (`.NET Onion Architecture`) ile tutarlılık sağlamak ve her katmanın tek bir sorumluluğa sahip olması için **Clean Architecture** tercih edilmiştir. Bu yaklaşım:

- **Testability:** İş mantığı (domain/use case) framework bağımlılığı olmadan test edilir.
- **Scalability:** Yeni feature'lar mevcut katmanları bozmadan eklenir.
- **Separation of Concerns:** UI, iş mantığı ve veri erişimi birbirinden bağımsızdır.

### 1.2. Neden MVVM + UDF (Unidirectional Data Flow)?

Jetpack Compose'un **reactive** yapısıyla en uyumlu desen MVVM'dir. UDF ile:

- State her zaman ViewModel'den akar → UI sadece state'i render eder.
- Kullanıcı aksiyonları (Intent/Event) → ViewModel'e gider → State güncellenir → UI yeniden çizilir.
- Tek yönlü veri akışı, bug'ları minimize eder.

```
┌─────────────┐     Event      ┌──────────────┐    State     ┌─────────────┐
│   Compose    │ ──────────────►│  ViewModel   │ ────────────►│   Compose   │
│   Screen     │                │  (StateFlow) │              │   Screen    │
│  (renders)   │◄───────────────│              │              │  (renders)  │
└─────────────┘    StateFlow   └──────────────┘              └─────────────┘
                                      │
                                      │ calls
                                      ▼
                               ┌──────────────┐
                               │   Use Case   │
                               │  (Domain)    │
                               └──────┬───────┘
                                      │ calls
                                      ▼
                               ┌──────────────┐
                               │  Repository  │
                               │  (Data)      │
                               └──────────────┘
```

---

## 2. Katman Yapısı

### 2.1. Katman Hiyerarşisi

```
┌─────────────────────────────────────────────────┐
│                  :app (Presentation)             │
│  Activities, Navigation, Compose Screens,        │
│  ViewModels, DI Modules                          │
├─────────────────────────────────────────────────┤
│                 :core:domain                     │
│  Entities, Use Cases, Repository Interfaces,     │
│  Value Objects — SIFIR bağımlılık                │
├─────────────────────────────────────────────────┤
│                 :core:data                       │
│  Repository Implementations, API Service,        │
│  DTO ↔ Entity Mappers, DataStore                 │
├─────────────────────────────────────────────────┤
│                :core:network                     │
│  Retrofit Setup, OkHttp Interceptors,            │
│  Auth Token Management, Error Handling           │
├─────────────────────────────────────────────────┤
│               :core:common                       │
│  Shared utilities, Result wrapper,               │
│  Extension functions, Constants                  │
├─────────────────────────────────────────────────┤
│              :core:designsystem                  │
│  Material 3 Theme, Colors, Typography,           │
│  Reusable Compose Components                     │
├─────────────────────────────────────────────────┤
│              :core:datastore                     │
│  DataStore Preferences, Token Storage,           │
│  User Session Persistence                        │
└─────────────────────────────────────────────────┘
```

### 2.2. Bağımlılık Akışı (Strict — Asla İhlal Edilmez)

```
:core:domain        ← SIFIR dış bağımlılık (sadece Kotlin stdlib)
:core:common        ← Kotlin stdlib
:core:network       ← :core:common
:core:datastore     ← :core:common
:core:data          ← :core:domain, :core:network, :core:datastore, :core:common
:core:designsystem  ← :core:common (Compose bağımlılıkları)
:app                ← :core:data, :core:domain, :core:designsystem, :core:common
```

**YASAK bağımlılıklar:**
- `:core:domain` → başka hiçbir modüle bağımlı OLAMAZ
- `:core:network` → `:core:domain`'e bağımlı OLAMAZ (DTO'lar network'te, entity'ler domain'de)
- `:core:designsystem` → `:core:data` veya `:core:domain`'e bağımlı OLAMAZ

---

## 3. Modül Detayları

### 3.1. `:core:domain` — İş Mantığı Katmanı

**Sorumluluk:** Saf iş mantığı, entity tanımları, use case'ler ve repository arayüzleri.

**İçerik:**

```
core/domain/src/main/kotlin/com/dev4all/mobile/domain/
├── model/                          # Domain entities
│   ├── User.kt                     # data class User(id, name, email, role)
│   ├── UserRole.kt                 # enum class UserRole { Customer, Developer, Admin }
│   ├── Project.kt                  # data class Project(id, title, description, budget, ...)
│   ├── ProjectStatus.kt            # enum class ProjectStatus { Open, AwaitingContract, Ongoing, Completed, Expired, Cancelled }
│   ├── Bid.kt                      # data class Bid(id, projectId, developerId, amount, note, status)
│   ├── BidStatus.kt                # enum class BidStatus { Pending, Accepted, Rejected }
│   ├── Contract.kt                 # data class Contract(id, projectId, content, status, revisionNumber, ...)
│   ├── ContractStatus.kt           # enum class ContractStatus { Draft, UnderReview, BothApproved, Cancelled }
│   ├── ContractRevision.kt         # data class ContractRevision(id, contractId, content, revisionNumber, ...)
│   ├── GitHubLog.kt                # data class GitHubLog(id, projectId, commitHash, message, author, pushedAt)
│   └── AuthToken.kt                # data class AuthToken(token, expiresAt, email, role)
│
├── repository/                     # Repository interfaces (contract)
│   ├── AuthRepository.kt           # interface AuthRepository
│   ├── ProjectRepository.kt        # interface ProjectRepository
│   ├── BidRepository.kt            # interface BidRepository
│   ├── ContractRepository.kt       # interface ContractRepository
│   └── GitHubLogRepository.kt      # interface GitHubLogRepository
│
├── usecase/                        # Use Cases (interactors)
│   ├── auth/
│   │   ├── LoginUseCase.kt
│   │   ├── RegisterUseCase.kt
│   │   ├── GetCurrentUserUseCase.kt
│   │   └── LogoutUseCase.kt
│   ├── project/
│   │   ├── GetProjectsUseCase.kt
│   │   ├── GetProjectDetailUseCase.kt
│   │   ├── CreateProjectUseCase.kt
│   │   ├── UpdateProjectUseCase.kt
│   │   └── DeleteProjectUseCase.kt
│   ├── bid/
│   │   ├── GetBidsForProjectUseCase.kt
│   │   ├── SubmitBidUseCase.kt
│   │   ├── UpdateBidUseCase.kt
│   │   └── AcceptBidUseCase.kt
│   ├── contract/
│   │   ├── GetContractUseCase.kt
│   │   ├── ReviseContractUseCase.kt
│   │   ├── ApproveContractUseCase.kt
│   │   ├── CancelContractUseCase.kt
│   │   └── GetContractRevisionsUseCase.kt
│   └── github/
│       └── GetGitHubLogsUseCase.kt
│
└── exception/                      # Domain exceptions
    ├── AuthException.kt            # sealed class: InvalidCredentials, SessionExpired
    ├── NetworkException.kt         # sealed class: NoConnection, Timeout, ServerError
    └── BusinessException.kt        # sealed class: ValidationFailed, Forbidden, NotFound
```

**Kurallar:**
- Entity'ler `data class` olarak tanımlanır, immutable'dır.
- Use case'ler tek bir `operator fun invoke()` metodu içerir.
- Repository interface'leri `suspend fun` veya `Flow<T>` döner.
- **YASAK:** Android framework import'u, Retrofit, Hilt annotation.

---

### 3.2. `:core:data` — Veri Erişim Katmanı

**Sorumluluk:** Repository implementasyonları, DTO-Entity mapping, veri kaynakları koordinasyonu.

**İçerik:**

```
core/data/src/main/kotlin/com/dev4all/mobile/data/
├── repository/                     # Repository implementations
│   ├── AuthRepositoryImpl.kt
│   ├── ProjectRepositoryImpl.kt
│   ├── BidRepositoryImpl.kt
│   ├── ContractRepositoryImpl.kt
│   └── GitHubLogRepositoryImpl.kt
│
├── mapper/                         # DTO → Domain Entity mappers
│   ├── UserMapper.kt
│   ├── ProjectMapper.kt
│   ├── BidMapper.kt
│   ├── ContractMapper.kt
│   └── GitHubLogMapper.kt
│
└── di/                             # Hilt DI modules
    └── DataModule.kt               # @Module @InstallIn(SingletonComponent)
```

**Kurallar:**
- Repository impl'ler `@Inject constructor` kullanır.
- DTO → Entity dönüşümü mapper extension function'larıyla yapılır.
- Tüm API çağrıları `try-catch` + `Result` wrapper ile sarılır.

---

### 3.3. `:core:network` — Network Katmanı

**Sorumluluk:** HTTP istemci yapılandırması, Retrofit service tanımları, DTO'lar, interceptor'lar.

**İçerik:**

```
core/network/src/main/kotlin/com/dev4all/mobile/network/
├── api/                            # Retrofit service interfaces
│   ├── AuthApiService.kt
│   ├── ProjectApiService.kt
│   ├── BidApiService.kt
│   ├── ContractApiService.kt
│   └── GitHubLogApiService.kt
│
├── dto/                            # Data Transfer Objects
│   ├── auth/
│   │   ├── RegisterRequest.kt
│   │   ├── RegisterResponse.kt
│   │   ├── LoginRequest.kt
│   │   ├── LoginResponse.kt
│   │   └── CurrentUserResponse.kt
│   ├── project/
│   │   ├── ProjectListResponse.kt
│   │   ├── ProjectDetailResponse.kt
│   │   ├── CreateProjectRequest.kt
│   │   └── UpdateProjectRequest.kt
│   ├── bid/
│   │   ├── BidListResponse.kt
│   │   ├── SubmitBidRequest.kt
│   │   └── UpdateBidRequest.kt
│   ├── contract/
│   │   ├── ContractResponse.kt
│   │   ├── ReviseContractRequest.kt
│   │   └── ContractRevisionListResponse.kt
│   ├── github/
│   │   └── GitHubLogListResponse.kt
│   └── error/
│       ├── ApiErrorResponse.kt     # { statusCode, timestamp, path, message }
│       └── ValidationErrorResponse.kt # { statusCode, ..., errors: [{field, message}] }
│
├── interceptor/
│   ├── AuthInterceptor.kt          # JWT token'ı header'a ekler
│   └── ErrorInterceptor.kt         # HTTP hata kodlarını parse eder
│
├── authenticator/
│   └── TokenAuthenticator.kt       # 401 durumunda token yenileme (future)
│
└── di/
    └── NetworkModule.kt            # OkHttp, Retrofit, API service DI
```

**Kurallar:**
- DTO'lar `@Serializable` annotation'ı ile Kotlin Serialization kullanır.
- DTO field isimleri `camelCase` — backend JSON naming policy ile uyumlu.
- Network modülü domain entity'lerini **BİLMEZ** — sadece DTO döner.

---

### 3.4. `:core:datastore` — Yerel Depolama Katmanı

**Sorumluluk:** JWT token, kullanıcı oturumu ve uygulama ayarları saklama.

**İçerik:**

```
core/datastore/src/main/kotlin/com/dev4all/mobile/datastore/
├── TokenDataStore.kt               # JWT token CRUD
├── UserSessionDataStore.kt         # Oturum bilgisi (userId, email, role, isLoggedIn)
├── AppPreferencesDataStore.kt      # Dark mode, onboarding tamamlandı mı, vs.
└── di/
    └── DataStoreModule.kt
```

**Kurallar:**
- `DataStore<Preferences>` kullanılır (SharedPreferences **YASAK**).
- Token encrypted olarak saklanır (`EncryptedSharedPreferences` veya `androidx.security.crypto`).
- Tüm okuma işlemleri `Flow<T>` döner.

---

### 3.5. `:core:designsystem` — Tasarım Sistemi

**Sorumluluk:** Uygulama genelinde kullanılan renk paleti, tipografi, tema ve yeniden kullanılabilir Compose bileşenleri.

**İçerik:**

```
core/designsystem/src/main/kotlin/com/dev4all/mobile/designsystem/
├── theme/
│   ├── Color.kt                    # Tech Blue palette
│   ├── Typography.kt               # Font ailesi ve stiller
│   ├── Shape.kt                    # RoundedCornerShape tanımları
│   ├── Spacing.kt                  # Padding/margin değerleri
│   └── Dev4AllTheme.kt             # MaterialTheme wrapper
│
├── component/
│   ├── Dev4AllButton.kt            # Primary, Secondary, Outline, Danger button variants
│   ├── Dev4AllTextField.kt         # Floating label text field
│   ├── Dev4AllCard.kt              # Standart kart bileşeni
│   ├── Dev4AllTopBar.kt            # Top app bar
│   ├── Dev4AllBottomBar.kt         # Bottom navigation bar
│   ├── Dev4AllLoadingIndicator.kt  # Circular + Skeleton loading
│   ├── Dev4AllErrorState.kt        # Hata durumu bileşeni (retry button ile)
│   ├── Dev4AllEmptyState.kt        # Boş liste durumu (ikon + mesaj)
│   ├── StatusBadge.kt              # Open, Ongoing, Completed, Expired pill badge
│   ├── TechnologyChip.kt          # React, .NET, vb. teknoloji etiketi
│   └── ProjectCard.kt             # Proje listesi için kart bileşeni
│
└── icon/
    └── Dev4AllIcons.kt             # Özel ikon referansları
```

---

### 3.6. `:core:common` — Paylaşılan Araçlar

**İçerik:**

```
core/common/src/main/kotlin/com/dev4all/mobile/common/
├── result/
│   └── Result.kt                   # sealed class Result<T> { Success, Error, Loading }
├── extension/
│   ├── StringExtensions.kt         # Email validation, truncate, vs.
│   ├── DateExtensions.kt           # ISO 8601 parse, relative time ("2 saat önce")
│   └── FlowExtensions.kt          # Flow utility'leri
├── constant/
│   └── AppConstants.kt             # ANIMATION_DURATION, PAGINATION_SIZE, vb.
└── util/
    └── NetworkMonitor.kt           # ConnectivityManager ile çevrimiçi/çevrimdışı izleme
```

---

## 4. Veri Akış Deseni (UDF — Unidirectional Data Flow)

### 4.1. UI State Pattern

Her ekran için tek bir sealed interface `UiState` tanımlanır:

```kotlin
sealed interface LoginUiState {
    data object Idle : LoginUiState
    data object Loading : LoginUiState
    data class Success(val token: AuthToken) : LoginUiState
    data class Error(val message: String) : LoginUiState
}
```

### 4.2. Event Pattern

Kullanıcı aksiyonları sealed class ile modellenir:

```kotlin
sealed interface LoginEvent {
    data class EmailChanged(val email: String) : LoginEvent
    data class PasswordChanged(val password: String) : LoginEvent
    data object LoginClicked : LoginEvent
    data object NavigateToRegister : LoginEvent
}
```

### 4.3. ViewModel Pattern

```kotlin
@HiltViewModel
class LoginViewModel @Inject constructor(
    private val loginUseCase: LoginUseCase,
) : ViewModel() {

    private val _uiState = MutableStateFlow<LoginUiState>(LoginUiState.Idle)
    val uiState: StateFlow<LoginUiState> = _uiState.asStateFlow()

    private val _email = MutableStateFlow("")
    val email: StateFlow<String> = _email.asStateFlow()

    private val _password = MutableStateFlow("")
    val password: StateFlow<String> = _password.asStateFlow()

    fun onEvent(event: LoginEvent) {
        when (event) {
            is LoginEvent.EmailChanged -> _email.value = event.email
            is LoginEvent.PasswordChanged -> _password.value = event.password
            is LoginEvent.LoginClicked -> login()
            is LoginEvent.NavigateToRegister -> { /* navigation side-effect */ }
        }
    }

    private fun login() {
        viewModelScope.launch {
            _uiState.value = LoginUiState.Loading
            loginUseCase(_email.value, _password.value)
                .onSuccess { _uiState.value = LoginUiState.Success(it) }
                .onFailure { _uiState.value = LoginUiState.Error(it.message ?: "Unknown error") }
        }
    }
}
```

### 4.4. Screen Pattern

```kotlin
@Composable
fun LoginScreen(
    viewModel: LoginViewModel = hiltViewModel(),
    onLoginSuccess: () -> Unit,
    onNavigateToRegister: () -> Unit,
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val email by viewModel.email.collectAsStateWithLifecycle()
    val password by viewModel.password.collectAsStateWithLifecycle()

    LaunchedEffect(uiState) {
        if (uiState is LoginUiState.Success) onLoginSuccess()
    }

    LoginContent(
        email = email,
        password = password,
        uiState = uiState,
        onEvent = viewModel::onEvent,
    )
}

@Composable
private fun LoginContent(
    email: String,
    password: String,
    uiState: LoginUiState,
    onEvent: (LoginEvent) -> Unit,
) {
    // Stateless composable — preview'larda kolayca test edilir
}
```

---

## 5. Side Effect Yönetimi

### 5.1. One-Time Events (Navigation, Snackbar, Toast)

`Channel` kullanarak tek seferlik event'leri yönet:

```kotlin
private val _sideEffect = Channel<LoginSideEffect>(Channel.BUFFERED)
val sideEffect = _sideEffect.receiveAsFlow()

sealed interface LoginSideEffect {
    data object NavigateToHome : LoginSideEffect
    data class ShowSnackbar(val message: String) : LoginSideEffect
}
```

Screen'de:

```kotlin
LaunchedEffect(Unit) {
    viewModel.sideEffect.collect { effect ->
        when (effect) {
            is LoginSideEffect.NavigateToHome -> onLoginSuccess()
            is LoginSideEffect.ShowSnackbar -> snackbarHostState.showSnackbar(effect.message)
        }
    }
}
```

---

## 6. Error Handling Stratejisi

### 6.1. Result Wrapper

```kotlin
sealed class Result<out T> {
    data class Success<T>(val data: T) : Result<T>()
    data class Error(val exception: AppException) : Result<Nothing>()
}

sealed class AppException(message: String, cause: Throwable? = null) : Exception(message, cause) {
    class Network(message: String) : AppException(message)
    class Unauthorized(message: String) : AppException(message)
    class Forbidden(message: String) : AppException(message)
    class NotFound(message: String) : AppException(message)
    class Validation(val errors: List<FieldError>) : AppException("Validation failed")
    class Server(message: String) : AppException(message)
    class Unknown(message: String, cause: Throwable?) : AppException(message, cause)
}

data class FieldError(val field: String, val message: String)
```

### 6.2. Backend Hata Mapping

Backend `GlobalExceptionMiddleware` aşağıdaki JSON yapısını döner:

```json
{
  "statusCode": 400,
  "timestamp": "2026-03-31T14:00:00Z",
  "path": "/api/v1/auth/register",
  "message": "Validation failed",
  "errors": [
    { "field": "email", "message": "E-posta boş olamaz." }
  ]
}
```

Mobil tarafta `ErrorInterceptor` bu JSON'u parse ederek uygun `AppException` alt tipine dönüştürür:

| HTTP Status | AppException |
|-------------|-------------|
| 400 (with errors array) | `Validation(errors)` |
| 400 (without errors) | `Server(message)` |
| 401 | `Unauthorized(message)` |
| 403 | `Forbidden(message)` |
| 404 | `NotFound(message)` |
| 500 | `Server(message)` |
| No Internet | `Network("No internet connection")` |
| Timeout | `Network("Request timed out")` |

---

## 7. Dependency Injection Stratejisi

Hilt ile katman bazlı modüller:

| Modül | Install In | İçerik |
|-------|-----------|--------|
| `NetworkModule` | `SingletonComponent` | OkHttp, Retrofit, API Service'ler |
| `DataStoreModule` | `SingletonComponent` | DataStore instance'ları |
| `DataModule` | `SingletonComponent` | Repository binding'leri |
| `UseCaseModule` | `ViewModelComponent` | Use case binding'leri (opsiyonel — constructor injection yeterli) |

```kotlin
@Module
@InstallIn(SingletonComponent::class)
abstract class DataModule {

    @Binds
    abstract fun bindAuthRepository(impl: AuthRepositoryImpl): AuthRepository

    @Binds
    abstract fun bindProjectRepository(impl: ProjectRepositoryImpl): ProjectRepository
    
    // ...
}
```

---

## 8. Özet Mimari Diyagramı

```
┌─────────────────────────────────────────────────────────────────┐
│                         :app                                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐       │
│  │ Login    │  │ Register │  │Dashboard │  │ Project  │  ...   │
│  │ Screen   │  │ Screen   │  │ Screen   │  │ Detail   │       │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘       │
│       │              │              │              │             │
│  ┌────▼─────┐  ┌────▼─────┐  ┌────▼─────┐  ┌────▼─────┐       │
│  │ Login    │  │Register │  │Dashboard│  │ProjectDe│       │
│  │ViewModel│  │ViewModel│  │ViewModel│  │ViewModel│       │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘       │
│       └──────────┬───┘──────────┬──┘──────────┬───┘             │
│                  │              │              │                 │
├──────────────────▼──────────────▼──────────────▼─────────────────┤
│                      :core:domain                                │
│         Use Cases, Repository Interfaces, Entities               │
├──────────────────────────────┬───────────────────────────────────┤
│                              │                                   │
│         :core:data           │        :core:datastore            │
│   Repository Impls, Mappers  │   Token, Session Storage          │
│              │               │              │                    │
├──────────────▼───────────────┼──────────────▼────────────────────┤
│                              │                                   │
│        :core:network         │      :core:designsystem           │
│  Retrofit, OkHttp, DTOs     │   Theme, Components               │
│                              │                                   │
├──────────────────────────────┼───────────────────────────────────┤
│                       :core:common                               │
│              Result, Extensions, Constants                        │
└──────────────────────────────────────────────────────────────────┘
```
