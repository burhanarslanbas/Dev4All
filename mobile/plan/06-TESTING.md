# 06. Testing Strategy

> Unit test, UI test, integration test stratejisi, test araçları ve kapsam hedefleri.

---

## 1. Test Piramidi

```
          ┌───────────────┐
          │  UI / E2E     │  ← Az sayıda, yavaş, kritik akışlar
          │  (Espresso)   │
          ├───────────────┤
          │  Integration  │  ← Orta sayıda, API mock, Navigation
          │  (Compose UI) │
          ├───────────────┤
          │  Unit Tests   │  ← Çok sayıda, hızlı, ViewModel/UseCase/Repo
          │  (JUnit+MockK)│
          └───────────────┘
```

**Hedef Kapsam:** > 80% line coverage (domain + data katmanları)

---

## 2. Test Araçları ve Kütüphaneleri

| Araç | Kullanım | Gradle Dependency |
|------|----------|------------------|
| **JUnit 4** | Unit test framework | `junit:junit:4.13.2` |
| **MockK** | Kotlin-native mocking | `io.mockk:mockk:1.13.13` |
| **Turbine** | Flow testing | `app.cash.turbine:turbine:1.2.0` |
| **Coroutines Test** | Coroutine test dispatcher | `kotlinx-coroutines-test:1.9.0` |
| **Compose UI Test** | Compose bileşen testleri | `ui-test-junit4` (BOM) |
| **Espresso** | Android instrumented tests | `espresso-core:3.6.1` |
| **Hilt Testing** | DI-aware tests | `hilt-android-testing` |

---

## 3. Test Naming Convention

```
methodName_scenario_expectedResult
```

Örnekler:
```kotlin
fun login_validCredentials_returnsToken()
fun login_invalidEmail_returnsValidationError()
fun register_duplicateEmail_throwsBusinessException()
fun getProjects_emptyList_returnsEmptyState()
fun submitBid_alreadyBidded_throwsConflictException()
```

---

## 4. Unit Tests

### 4.1. ViewModel Tests

Her ViewModel için ayrı bir test sınıfı. MockK ile dependency'ler mock'lanır:

```kotlin
class LoginViewModelTest {

    @get:Rule
    val mainDispatcherRule = MainDispatcherRule()

    private val loginUseCase = mockk<LoginUseCase>()
    private lateinit var viewModel: LoginViewModel

    @Before
    fun setup() {
        viewModel = LoginViewModel(loginUseCase)
    }

    @Test
    fun login_validCredentials_emitsSuccessState() = runTest {
        // Arrange
        val token = AuthToken("jwt-token", "2026-04-01T00:00:00Z", "test@mail.com", "Customer")
        coEvery { loginUseCase("test@mail.com", "Password1") } returns Result.Success(token)

        // Act
        viewModel.onEvent(LoginEvent.EmailChanged("test@mail.com"))
        viewModel.onEvent(LoginEvent.PasswordChanged("Password1"))
        viewModel.onEvent(LoginEvent.LoginClicked)

        // Assert
        viewModel.uiState.test {
            val state = awaitItem()
            assertTrue(state is LoginUiState.Success)
        }
    }

    @Test
    fun login_invalidCredentials_emitsErrorState() = runTest {
        // Arrange
        coEvery { loginUseCase(any(), any()) } returns
            Result.Error(AppException.Forbidden("Geçersiz e-posta veya şifre."))

        // Act
        viewModel.onEvent(LoginEvent.EmailChanged("wrong@mail.com"))
        viewModel.onEvent(LoginEvent.PasswordChanged("WrongPass1"))
        viewModel.onEvent(LoginEvent.LoginClicked)

        // Assert
        viewModel.uiState.test {
            val state = awaitItem()
            assertTrue(state is LoginUiState.Error)
        }
    }

    @Test
    fun login_emptyEmail_emitsValidationError() = runTest {
        // Act
        viewModel.onEvent(LoginEvent.EmailChanged(""))
        viewModel.onEvent(LoginEvent.LoginClicked)

        // Assert — client-side validation should catch
        viewModel.uiState.test {
            val state = awaitItem()
            assertTrue(state is LoginUiState.ValidationError)
        }
    }
}
```

**Test sınıfı listesi (ViewModel):**
| Test Class | Hedef |
|-----------|-------|
| `LoginViewModelTest` | Login akışı, validation, error handling |
| `RegisterViewModelTest` | Register akışı, rol seçimi, validation |
| `CustomerDashboardViewModelTest` | Dashboard yüklenme, istatistikler |
| `ExploreViewModelTest` | Proje listesi, arama, pagination |
| `ProjectDetailViewModelTest` | Proje detay yüklenme, aksiyonlar |
| `CreateProjectViewModelTest` | Form validation, proje oluşturma |
| `SubmitBidViewModelTest` | Teklif validation, gönderim |
| `MyBidsViewModelTest` | Teklif listesi |
| `ContractViewModelTest` | Sözleşme yüklenme, onay, iptal |
| `ReviseContractViewModelTest` | Revizyon validation, kaydetme |
| `ProfileViewModelTest` | Profil yüklenme, çıkış |

---

### 4.2. Use Case Tests

Her use case için birim testi. Repository mock'lanır:

```kotlin
class LoginUseCaseTest {

    private val authRepository = mockk<AuthRepository>()
    private lateinit var useCase: LoginUseCase

    @Before
    fun setup() {
        useCase = LoginUseCase(authRepository)
    }

    @Test
    fun invoke_validCredentials_returnsSuccessWithToken() = runTest {
        // Arrange
        val expected = AuthToken("jwt", "2026-04-01T00:00:00Z", "user@mail.com", "Customer")
        coEvery { authRepository.login("user@mail.com", "Pass1234") } returns Result.Success(expected)

        // Act
        val result = useCase("user@mail.com", "Pass1234")

        // Assert
        assertTrue(result is Result.Success)
        assertEquals(expected, (result as Result.Success).data)
    }

    @Test
    fun invoke_networkError_returnsError() = runTest {
        // Arrange
        coEvery { authRepository.login(any(), any()) } returns
            Result.Error(AppException.Network("No connection"))

        // Act
        val result = useCase("user@mail.com", "Pass1234")

        // Assert
        assertTrue(result is Result.Error)
        assertTrue((result as Result.Error).exception is AppException.Network)
    }
}
```

**Test sınıfı listesi (UseCase):**
| Test Class | Hedef |
|-----------|-------|
| `LoginUseCaseTest` | Login flow |
| `RegisterUseCaseTest` | Register flow |
| `GetCurrentUserUseCaseTest` | Current user fetch |
| `LogoutUseCaseTest` | Token temizleme |
| `GetProjectsUseCaseTest` | Proje listesi |
| `GetProjectDetailUseCaseTest` | Proje detay |
| `CreateProjectUseCaseTest` | Proje oluşturma |
| `SubmitBidUseCaseTest` | Teklif verme |
| `AcceptBidUseCaseTest` | Teklif kabul |
| `GetContractUseCaseTest` | Sözleşme getirme |
| `ApproveContractUseCaseTest` | Sözleşme onay |
| `GetGitHubLogsUseCaseTest` | GitHub log listesi |

---

### 4.3. Repository Tests

Repository implementation'ları test edilir. API service + DataStore mock'lanır:

```kotlin
class AuthRepositoryImplTest {

    private val authApiService = mockk<AuthApiService>()
    private val tokenDataStore = mockk<TokenDataStore>(relaxed = true)
    private val userSessionDataStore = mockk<UserSessionDataStore>(relaxed = true)
    private lateinit var repository: AuthRepositoryImpl

    @Before
    fun setup() {
        repository = AuthRepositoryImpl(authApiService, tokenDataStore, userSessionDataStore)
    }

    @Test
    fun login_successfulResponse_savesTokenAndReturnsSuccess() = runTest {
        // Arrange
        val response = LoginResponse("jwt-token", "2026-04-01T00:00:00Z", "user@mail.com", "Customer")
        coEvery { authApiService.login(any()) } returns response

        // Act
        val result = repository.login("user@mail.com", "Pass1234")

        // Assert
        assertTrue(result is Result.Success)
        coVerify { tokenDataStore.saveToken("jwt-token") }
        coVerify { userSessionDataStore.saveSession(any(), "user@mail.com", "Customer") }
    }

    @Test
    fun login_networkFailure_returnsNetworkError() = runTest {
        // Arrange
        coEvery { authApiService.login(any()) } throws
            AppException.Network("No internet connection")

        // Act
        val result = repository.login("user@mail.com", "Pass1234")

        // Assert
        assertTrue(result is Result.Error)
    }
}
```

---

### 4.4. Mapper Tests

DTO → Domain entity dönüşümleri test edilir:

```kotlin
class UserMapperTest {

    @Test
    fun loginResponseToDomain_mapsCorrectly() {
        val dto = LoginResponse("jwt", "2026-04-01T00:00:00Z", "user@mail.com", "Customer")
        val domain = dto.toDomain()

        assertEquals("jwt", domain.token)
        assertEquals("user@mail.com", domain.email)
        assertEquals("Customer", domain.role)
    }
}
```

---

## 5. Compose UI Tests

### 5.1. Component Tests

Her reusable bileşen için render + interaction testi:

```kotlin
class Dev4AllButtonTest {

    @get:Rule
    val composeRule = createComposeRule()

    @Test
    fun primaryButton_clickable_invokesCallback() {
        var clicked = false
        composeRule.setContent {
            Dev4AllButton(text = "Test", onClick = { clicked = true })
        }
        composeRule.onNodeWithText("Test").performClick()
        assertTrue(clicked)
    }

    @Test
    fun button_loading_showsProgress() {
        composeRule.setContent {
            Dev4AllButton(text = "Submit", onClick = {}, isLoading = true)
        }
        composeRule.onNodeWithTag("loading_indicator").assertIsDisplayed()
    }

    @Test
    fun button_disabled_notClickable() {
        var clicked = false
        composeRule.setContent {
            Dev4AllButton(text = "Disabled", onClick = { clicked = true }, enabled = false)
        }
        composeRule.onNodeWithText("Disabled").performClick()
        assertFalse(clicked)
    }
}
```

### 5.2. Screen Tests

Ekran bazlı smoke testleri (ViewModel mock'lanır):

```kotlin
class LoginScreenTest {

    @get:Rule
    val composeRule = createComposeRule()

    @Test
    fun loginScreen_displaysAllElements() {
        composeRule.setContent {
            LoginContent(
                email = "",
                password = "",
                uiState = LoginUiState.Idle,
                onEvent = {},
            )
        }
        composeRule.onNodeWithText("E-posta").assertIsDisplayed()
        composeRule.onNodeWithText("Şifre").assertIsDisplayed()
        composeRule.onNodeWithText("Giriş Yap").assertIsDisplayed()
    }

    @Test
    fun loginScreen_loading_showsProgressIndicator() {
        composeRule.setContent {
            LoginContent(
                email = "test@mail.com",
                password = "Pass1234",
                uiState = LoginUiState.Loading,
                onEvent = {},
            )
        }
        composeRule.onNodeWithTag("loading").assertIsDisplayed()
    }

    @Test
    fun loginScreen_error_showsErrorMessage() {
        composeRule.setContent {
            LoginContent(
                email = "test@mail.com",
                password = "wrong",
                uiState = LoginUiState.Error("Geçersiz e-posta veya şifre."),
                onEvent = {},
            )
        }
        composeRule.onNodeWithText("Geçersiz e-posta veya şifre.").assertIsDisplayed()
    }
}
```

---

## 6. Integration Tests (Instrumented)

### 6.1. Navigation Tests

```kotlin
@HiltAndroidTest
class NavigationTest {

    @get:Rule(order = 0)
    val hiltRule = HiltAndroidRule(this)

    @get:Rule(order = 1)
    val composeRule = createAndroidComposeRule<MainActivity>()

    @Test
    fun splash_navigatesToLogin_whenNotLoggedIn() {
        composeRule.waitForIdle()
        composeRule.onNodeWithText("Giriş Yap").assertIsDisplayed()
    }
}
```

---

## 7. Test Utility Classes

### 7.1. MainDispatcherRule

```kotlin
class MainDispatcherRule(
    private val dispatcher: TestDispatcher = UnconfinedTestDispatcher(),
) : TestWatcher() {

    override fun starting(description: Description) {
        Dispatchers.setMain(dispatcher)
    }

    override fun finished(description: Description) {
        Dispatchers.resetMain()
    }
}
```

### 7.2. Fake DataStore

```kotlin
class FakeTokenDataStore : TokenDataStore {
    private var token: String? = null

    override suspend fun saveToken(token: String) { this.token = token }
    override suspend fun getToken(): String? = token
    override suspend fun clearToken() { token = null }
    override fun hasToken(): Flow<Boolean> = flowOf(token != null)
}
```

---

## 8. Test Kapsam Hedefleri

| Katman | Hedef Kapsam | Öncelik |
|--------|-------------|---------|
| `:core:domain` (Use Cases) | > 90% | Yüksek |
| `:core:data` (Repositories) | > 85% | Yüksek |
| `:core:network` (Mappers) | > 80% | Orta |
| `:app` (ViewModels) | > 80% | Yüksek |
| `:core:designsystem` (Components) | > 60% | Düşük |
| Navigation flows | Smoke tests | Orta |

---

## 9. Test Çalıştırma Komutları

```bash
# Tüm unit testler
./gradlew test

# Belirli modül
./gradlew :core:domain:test
./gradlew :core:data:test
./gradlew :app:test

# Test coverage raporu
./gradlew testDebugUnitTest
./gradlew koverReport  # (Kover plugin eklenirse)

# Instrumented testler (emulator gerekli)
./gradlew connectedCheck

# Belirli test class
./gradlew :app:testDebugUnitTest --tests "com.dev4all.mobile.feature.auth.login.LoginViewModelTest"
```
