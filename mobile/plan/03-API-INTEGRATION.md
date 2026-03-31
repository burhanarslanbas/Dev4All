# 03. API Integration Plan

> Backend API ile iletişim katmanının tam detayları: endpoint sözleşmeleri, DTO tanımları, token yönetimi, hata işleme.

---

## 1. Base Configuration

### 1.1. Base URL

```
Production:  https://api.dev4all.com/api
Development: http://10.0.2.2:5000/api   (Android Emulator → localhost)
```

`BuildConfig.API_BASE_URL` üzerinden okunur; `local.properties` ile override edilebilir.

### 1.2. OkHttp Client Yapılandırması

```kotlin
@Module
@InstallIn(SingletonComponent::class)
object NetworkModule {

    @Provides
    @Singleton
    fun provideOkHttpClient(
        authInterceptor: AuthInterceptor,
        errorInterceptor: ErrorInterceptor,
    ): OkHttpClient = OkHttpClient.Builder()
        .addInterceptor(authInterceptor)
        .addInterceptor(errorInterceptor)
        .addInterceptor(HttpLoggingInterceptor().apply {
            level = if (BuildConfig.DEBUG)
                HttpLoggingInterceptor.Level.BODY
            else
                HttpLoggingInterceptor.Level.NONE
        })
        .connectTimeout(30, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .writeTimeout(30, TimeUnit.SECONDS)
        .build()

    @Provides
    @Singleton
    fun provideRetrofit(okHttpClient: OkHttpClient): Retrofit = Retrofit.Builder()
        .baseUrl(BuildConfig.API_BASE_URL)
        .client(okHttpClient)
        .addConverterFactory(Json.asConverterFactory("application/json".toMediaType()))
        .build()
}
```

### 1.3. Kotlin Serialization Json Konfigürasyonu

```kotlin
val json = Json {
    ignoreUnknownKeys = true
    isLenient = true
    encodeDefaults = true
    coerceInputValues = true
}
```

---

## 2. Auth Interceptor (JWT Token Ekleme)

```kotlin
class AuthInterceptor @Inject constructor(
    private val tokenDataStore: TokenDataStore,
) : Interceptor {

    override fun intercept(chain: Interceptor.Chain): Response {
        val token = runBlocking { tokenDataStore.getToken() }
        val request = if (token != null) {
            chain.request().newBuilder()
                .addHeader("Authorization", "Bearer $token")
                .build()
        } else {
            chain.request()
        }
        return chain.proceed(request)
    }
}
```

**Not:** Public endpoint'ler (register, login) token gerektirmez. Interceptor, token `null` ise header eklemez.

---

## 3. Error Interceptor (Hata Parse)

```kotlin
class ErrorInterceptor @Inject constructor() : Interceptor {

    override fun intercept(chain: Interceptor.Chain): Response {
        val response = chain.proceed(chain.request())

        if (!response.isSuccessful) {
            val errorBody = response.body?.string()
            val exception = parseError(response.code, errorBody)
            throw exception
        }

        return response
    }

    private fun parseError(code: Int, body: String?): AppException {
        if (body.isNullOrBlank()) {
            return when (code) {
                401 -> AppException.Unauthorized("Authentication required")
                403 -> AppException.Forbidden("Access denied")
                404 -> AppException.NotFound("Resource not found")
                else -> AppException.Server("Server error ($code)")
            }
        }

        return try {
            val json = Json { ignoreUnknownKeys = true }

            // Validation error'u kontrol et (errors array mevcut mu?)
            val validationError = try {
                json.decodeFromString<ValidationErrorResponse>(body)
            } catch (_: Exception) { null }

            if (validationError?.errors?.isNotEmpty() == true) {
                AppException.Validation(
                    validationError.errors.map { FieldError(it.field, it.message) }
                )
            } else {
                val apiError = json.decodeFromString<ApiErrorResponse>(body)
                when (code) {
                    400 -> AppException.Server(apiError.message)
                    401 -> AppException.Unauthorized(apiError.message)
                    403 -> AppException.Forbidden(apiError.message)
                    404 -> AppException.NotFound(apiError.message)
                    409 -> AppException.Server(apiError.message)
                    else -> AppException.Server(apiError.message)
                }
            }
        } catch (_: Exception) {
            AppException.Server("Unexpected error ($code)")
        }
    }
}
```

---

## 4. API Endpoint Sözleşmeleri ve DTO'lar

### 4.1. Authentication

#### `POST /api/v1/auth/register`

**Request:**
```kotlin
@Serializable
data class RegisterRequest(
    val name: String,
    val email: String,
    val password: String,
    val role: Int,  // 0 = Customer, 1 = Developer
)
```

**Response (201 Created):**
```kotlin
@Serializable
data class RegisterResponse(
    val userId: String,
    val email: String,
    val name: String,
)
```

**Error (400 Bad Request — Validation):**
```json
{
  "statusCode": 400,
  "timestamp": "2026-03-31T14:00:00Z",
  "path": "/api/v1/auth/register",
  "message": "Validation failed",
  "errors": [
    { "field": "Email", "message": "E-posta boş olamaz." },
    { "field": "Password", "message": "Şifre en az 8 karakter olmalıdır." }
  ]
}
```

**Error (400 Bad Request — Business Rule):**
```json
{
  "statusCode": 400,
  "timestamp": "2026-03-31T14:00:00Z",
  "path": "/api/v1/auth/register",
  "message": "Passwords must have at least one uppercase ('A'-'Z')."
}
```

---

#### `POST /api/v1/auth/login`

**Request:**
```kotlin
@Serializable
data class LoginRequest(
    val email: String,
    val password: String,
)
```

**Response (200 OK):**
```kotlin
@Serializable
data class LoginResponse(
    val token: String,
    val expiresAt: String,  // ISO 8601 DateTime
    val email: String,
    val role: String,       // "Customer" | "Developer" | "Admin"
)
```

**Error (403 Forbidden):**
```json
{
  "statusCode": 403,
  "timestamp": "2026-03-31T14:00:00Z",
  "path": "/api/v1/auth/login",
  "message": "Geçersiz e-posta veya şifre."
}
```

> **DİKKAT:** Backend `UnauthorizedDomainException` → HTTP **403** döner (401 değil). Mobil tarafta buna göre hata mesajı gösterilmeli.

---

#### `GET /api/v1/auth/me`

**Headers:** `Authorization: Bearer <token>`

**Response (200 OK):**
```kotlin
@Serializable
data class CurrentUserResponse(
    val userId: String,
    val email: String,
    val role: String,
)
```

**Error (401 Unauthorized):** Token eksik veya geçersiz.

---

### 4.2. Projects

#### `GET /api/projects` — Açık Projeleri Listele

**Query Parameters:**
| Param | Type | Default | Description |
|-------|------|---------|-------------|
| `page` | int | 1 | Sayfa numarası |
| `pageSize` | int | 10 | Sayfa başına kayıt |
| `search` | string? | null | Başlık arama |
| `sortBy` | string? | "createdDate" | Sıralama alanı |

**Response (200 OK):**
```kotlin
@Serializable
data class ProjectListResponse(
    val items: List<ProjectSummaryDto>,
    val totalCount: Int,
    val page: Int,
    val pageSize: Int,
)

@Serializable
data class ProjectSummaryDto(
    val id: String,
    val title: String,
    val description: String,
    val budget: Double,
    val deadline: String,
    val bidEndDate: String,
    val status: String,
    val technologies: List<String>,
    val bidCount: Int,
    val createdDate: String,
)
```

---

#### `POST /api/projects` — Yeni Proje Oluştur (Customer)

**Request:**
```kotlin
@Serializable
data class CreateProjectRequest(
    val title: String,
    val description: String,
    val budget: Double,
    val deadline: String,       // ISO 8601
    val bidEndDate: String,     // ISO 8601
    val technologies: List<String>,
)
```

**Response (201 Created):** Project ID döner.

---

#### `GET /api/projects/{id}` — Proje Detayı

**Response (200 OK):**
```kotlin
@Serializable
data class ProjectDetailResponse(
    val id: String,
    val title: String,
    val description: String,
    val budget: Double,
    val deadline: String,
    val bidEndDate: String,
    val status: String,
    val technologies: List<String>,
    val bidCount: Int,
    val customerId: String,
    val customerName: String,
    val assignedDeveloperId: String?,
    val assignedDeveloperName: String?,
    val repoUrl: String?,
    val branch: String?,
    val createdDate: String,
    val updatedDate: String?,
)
```

---

#### `PUT /api/projects/{id}` — Proje Güncelle (Customer)

**Request:**
```kotlin
@Serializable
data class UpdateProjectRequest(
    val title: String,
    val description: String,
    val budget: Double,
    val deadline: String,
    val bidEndDate: String,
    val technologies: List<String>,
)
```

---

#### `DELETE /api/projects/{id}` — Proje Sil (Soft Delete)

**Response:** 204 No Content

---

### 4.3. Bids

#### `GET /api/projects/{projectId}/bids` — Proje Tekliflerini Listele

**Response (200 OK):**
```kotlin
@Serializable
data class BidListResponse(
    val items: List<BidDto>,
)

@Serializable
data class BidDto(
    val id: String,
    val projectId: String,
    val developerId: String,
    val developerName: String,
    val bidAmount: Double,
    val proposalNote: String,
    val status: String,         // "Pending" | "Accepted" | "Rejected"
    val isAccepted: Boolean,
    val createdDate: String,
)
```

---

#### `POST /api/projects/{projectId}/bids` — Teklif Ver (Developer)

**Request:**
```kotlin
@Serializable
data class SubmitBidRequest(
    val bidAmount: Double,
    val proposalNote: String,
)
```

---

#### `PUT /api/bids/{bidId}` — Teklif Güncelle (Developer)

**Request:**
```kotlin
@Serializable
data class UpdateBidRequest(
    val bidAmount: Double,
    val proposalNote: String,
)
```

---

#### `POST /api/bids/{bidId}/accept` — Teklif Kabul Et (Customer)

**Response:** 200 OK (proje AwaitingContract'a geçer, sözleşme taslağı oluşturulur)

---

### 4.4. Contracts

#### `GET /api/contracts/{projectId}` — Sözleşme Görüntüle

**Response (200 OK):**
```kotlin
@Serializable
data class ContractResponse(
    val id: String,
    val projectId: String,
    val content: String,
    val status: String,            // "Draft" | "UnderReview" | "BothApproved" | "Cancelled"
    val revisionNumber: Int,
    val isCustomerApproved: Boolean,
    val isDeveloperApproved: Boolean,
    val lastRevisedById: String?,
    val createdDate: String,
    val updatedDate: String?,
)
```

---

#### `PUT /api/contracts/{projectId}` — Sözleşme Revize Et

**Request:**
```kotlin
@Serializable
data class ReviseContractRequest(
    val content: String,
    val revisionNote: String?,
)
```

---

#### `POST /api/contracts/{projectId}/approve` — Sözleşme Onayla

**Response:** 200 OK

---

#### `POST /api/contracts/{projectId}/cancel` — Sözleşme İptal Et

**Response:** 200 OK

---

#### `GET /api/contracts/{projectId}/revisions` — Revizyon Geçmişi

**Response (200 OK):**
```kotlin
@Serializable
data class ContractRevisionListResponse(
    val items: List<ContractRevisionDto>,
)

@Serializable
data class ContractRevisionDto(
    val id: String,
    val contractId: String,
    val content: String,
    val revisionNumber: Int,
    val revisedById: String,
    val revisedByName: String,
    val revisionNote: String?,
    val createdDate: String,
)
```

---

### 4.5. GitHub Integration

#### `PUT /api/projects/{projectId}/repo` — GitHub Repo Bağla (Developer)

**Request:**
```kotlin
@Serializable
data class ConnectRepoRequest(
    val repoUrl: String,
    val branch: String?,
)
```

---

#### `GET /api/projects/{projectId}/github-logs` — Aktivite Timeline

**Response (200 OK):**
```kotlin
@Serializable
data class GitHubLogListResponse(
    val items: List<GitHubLogDto>,
)

@Serializable
data class GitHubLogDto(
    val id: String,
    val projectId: String,
    val commitHash: String,
    val commitMessage: String,
    val authorName: String,
    val pushedAt: String,
)
```

---

## 5. Retrofit Service Interfaces

### 5.1. AuthApiService

```kotlin
interface AuthApiService {

    @POST("v1/auth/register")
    suspend fun register(@Body request: RegisterRequest): RegisterResponse

    @POST("v1/auth/login")
    suspend fun login(@Body request: LoginRequest): LoginResponse

    @GET("v1/auth/me")
    suspend fun getCurrentUser(): CurrentUserResponse
}
```

### 5.2. ProjectApiService

```kotlin
interface ProjectApiService {

    @GET("projects")
    suspend fun getProjects(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 10,
        @Query("search") search: String? = null,
    ): ProjectListResponse

    @POST("projects")
    suspend fun createProject(@Body request: CreateProjectRequest): String

    @GET("projects/{id}")
    suspend fun getProjectDetail(@Path("id") id: String): ProjectDetailResponse

    @PUT("projects/{id}")
    suspend fun updateProject(@Path("id") id: String, @Body request: UpdateProjectRequest)

    @DELETE("projects/{id}")
    suspend fun deleteProject(@Path("id") id: String)

    @PUT("projects/{id}/repo")
    suspend fun connectRepo(@Path("id") id: String, @Body request: ConnectRepoRequest)
}
```

### 5.3. BidApiService

```kotlin
interface BidApiService {

    @GET("projects/{projectId}/bids")
    suspend fun getBidsForProject(@Path("projectId") projectId: String): BidListResponse

    @POST("projects/{projectId}/bids")
    suspend fun submitBid(@Path("projectId") projectId: String, @Body request: SubmitBidRequest)

    @PUT("bids/{bidId}")
    suspend fun updateBid(@Path("bidId") bidId: String, @Body request: UpdateBidRequest)

    @POST("bids/{bidId}/accept")
    suspend fun acceptBid(@Path("bidId") bidId: String)
}
```

### 5.4. ContractApiService

```kotlin
interface ContractApiService {

    @GET("contracts/{projectId}")
    suspend fun getContract(@Path("projectId") projectId: String): ContractResponse

    @PUT("contracts/{projectId}")
    suspend fun reviseContract(@Path("projectId") projectId: String, @Body request: ReviseContractRequest)

    @POST("contracts/{projectId}/approve")
    suspend fun approveContract(@Path("projectId") projectId: String)

    @POST("contracts/{projectId}/cancel")
    suspend fun cancelContract(@Path("projectId") projectId: String)

    @GET("contracts/{projectId}/revisions")
    suspend fun getContractRevisions(@Path("projectId") projectId: String): ContractRevisionListResponse
}
```

### 5.5. GitHubLogApiService

```kotlin
interface GitHubLogApiService {

    @GET("projects/{projectId}/github-logs")
    suspend fun getGitHubLogs(@Path("projectId") projectId: String): GitHubLogListResponse
}
```

---

## 6. Error DTO'ları

```kotlin
@Serializable
data class ApiErrorResponse(
    val statusCode: Int,
    val timestamp: String? = null,
    val path: String? = null,
    val message: String,
)

@Serializable
data class ValidationErrorResponse(
    val statusCode: Int,
    val timestamp: String? = null,
    val path: String? = null,
    val message: String,
    val errors: List<ValidationFieldError> = emptyList(),
)

@Serializable
data class ValidationFieldError(
    val field: String,
    val message: String,
)
```

---

## 7. Token Yönetim Akışı

### 7.1. Login Sonrası Token Saklama

```
User enters credentials
    → LoginUseCase invoked
        → AuthRepository.login() called
            → AuthApiService.login() HTTP call
                → LoginResponse received
        → TokenDataStore.saveToken(response.token)
        → UserSessionDataStore.saveSession(userId, email, role)
    → UI navigates to Home
```

### 7.2. Token Lifecycle

```
App Launch
    → Check TokenDataStore.hasToken()
        → true: Navigate to Home, verify with GET /me
            → 401: Clear token, navigate to Login
            → 200: Continue
        → false: Navigate to Onboarding/Login
```

### 7.3. Token Expiry Handling

Backend'de şu an refresh token mekanizması **YOK**. Token süresi dolduğunda:

1. API 401 döner
2. `ErrorInterceptor` → `AppException.Unauthorized` fırlatır
3. Repository bu exception'ı yakalar
4. `TokenDataStore.clearToken()` çağrılır
5. UI, Login ekranına yönlendirilir

**İleride:** Refresh token endpoint'i eklendiğinde `TokenAuthenticator` (OkHttp Authenticator) devreye girecek.

---

## 8. Network Monitoring

```kotlin
class NetworkMonitor @Inject constructor(
    @ApplicationContext private val context: Context,
) {
    val isOnline: Flow<Boolean> = callbackFlow {
        val connectivityManager = context.getSystemService<ConnectivityManager>()
        val callback = object : ConnectivityManager.NetworkCallback() {
            override fun onAvailable(network: Network) { trySend(true) }
            override fun onLost(network: Network) { trySend(false) }
        }
        val request = NetworkRequest.Builder()
            .addCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET)
            .build()
        connectivityManager?.registerNetworkCallback(request, callback)
        
        // Initial state
        val isConnected = connectivityManager?.activeNetwork != null
        trySend(isConnected)
        
        awaitClose { connectivityManager?.unregisterNetworkCallback(callback) }
    }.distinctUntilChanged()
}
```

Kullanım: Çevrimdışıyken API çağrısı yapmadan önce kullanıcıya bilgi mesajı gösterilir.
