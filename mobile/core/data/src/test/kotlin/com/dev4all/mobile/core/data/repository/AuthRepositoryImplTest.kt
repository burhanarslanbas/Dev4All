package com.dev4all.mobile.core.data.repository

import com.dev4all.mobile.core.datastore.TokenDataStore
import com.dev4all.mobile.core.datastore.UserSessionDataStore
import com.dev4all.mobile.core.datastore.model.UserSession
import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.result.Result
import com.dev4all.mobile.core.network.api.AuthApiService
import com.dev4all.mobile.core.network.dto.auth.CurrentUserResponse
import com.dev4all.mobile.core.network.dto.auth.LoginRequest
import com.dev4all.mobile.core.network.dto.auth.LoginResponse
import com.dev4all.mobile.core.network.dto.auth.RegisterRequest
import com.dev4all.mobile.core.network.dto.auth.RegisterResponse
import com.dev4all.mobile.core.network.exception.UnauthorizedException
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Test

class AuthRepositoryImplTest {

    @Test
    fun login_success_persistsTokenAndSession() = runTest {
        val authApi = FakeAuthApiService().apply {
            loginResponse = LoginResponse(
                token = "jwt-token",
                expiresAt = "2026-04-03T00:00:00Z",
                email = "dev@mail.com",
                role = "Developer",
            )
            currentUserResponse = CurrentUserResponse(
                userId = "u-1",
                email = "dev@mail.com",
                role = "Developer",
            )
        }
        val tokenStore = FakeTokenDataStore()
        val sessionStore = FakeUserSessionDataStore()
        val repository = AuthRepositoryImpl(authApi, tokenStore, sessionStore)

        val result = repository.login(email = "dev@mail.com", password = "Pass1234")

        assertTrue(result is Result.Success)
        assertEquals("jwt-token", tokenStore.token)
        assertEquals("u-1", sessionStore.session?.userId)
        assertEquals("dev@mail.com", sessionStore.session?.email)
        assertEquals("Developer", sessionStore.session?.role)
        assertEquals(true, sessionStore.session?.isLoggedIn)
    }

    @Test
    fun logout_always_clearsTokenAndSession() = runTest {
        val authApi = FakeAuthApiService()
        val tokenStore = FakeTokenDataStore().apply { token = "jwt-token" }
        val sessionStore = FakeUserSessionDataStore().apply {
            session = UserSession(
                userId = "u-1",
                name = "Dev",
                email = "dev@mail.com",
                role = "Developer",
                isLoggedIn = true,
            )
        }
        val repository = AuthRepositoryImpl(authApi, tokenStore, sessionStore)

        val result = repository.logout()

        assertTrue(result is Result.Success)
        assertEquals(null, tokenStore.token)
        assertEquals(null, sessionStore.session)
    }

    @Test
    fun getCurrentUser_unauthorized_clearsAuthStateAndReturnsUnauthorizedError() = runTest {
        val authApi = FakeAuthApiService().apply {
            currentUserThrowable = UnauthorizedException("expired")
        }
        val tokenStore = FakeTokenDataStore().apply { token = "jwt-token" }
        val sessionStore = FakeUserSessionDataStore().apply {
            session = UserSession(
                userId = "u-1",
                name = "Dev",
                email = "dev@mail.com",
                role = "Developer",
                isLoggedIn = true,
            )
        }
        val repository = AuthRepositoryImpl(authApi, tokenStore, sessionStore)

        val result = repository.getCurrentUser()

        assertTrue(result is Result.Error)
        val error = (result as Result.Error).exception
        assertTrue(error is AppException.Unauthorized)
        assertEquals(null, tokenStore.token)
        assertEquals(null, sessionStore.session)
    }

    @Test
    fun register_success_logsInAndPersistsTokenAndSession() = runTest {
        val authApi = FakeAuthApiService().apply {
            registerResponse = RegisterResponse(
                userId = "u-2",
                email = "customer@mail.com",
                name = "Customer User",
            )
            loginResponse = LoginResponse(
                token = "jwt-token-2",
                expiresAt = "2026-05-03T00:00:00Z",
                email = "customer@mail.com",
                role = "Customer",
            )
        }
        val tokenStore = FakeTokenDataStore()
        val sessionStore = FakeUserSessionDataStore()
        val repository = AuthRepositoryImpl(authApi, tokenStore, sessionStore)

        val result = repository.register(
            fullName = "Customer User",
            email = "customer@mail.com",
            password = "Pass1234",
            role = "Customer",
        )

        assertTrue(result is Result.Success)
        assertEquals("jwt-token-2", tokenStore.token)
        assertEquals("u-2", sessionStore.session?.userId)
        assertEquals("Customer User", sessionStore.session?.name)
        assertEquals("customer@mail.com", sessionStore.session?.email)
        assertEquals("Customer", sessionStore.session?.role)

        val registerRequest = authApi.lastRegisterRequest
        checkNotNull(registerRequest)
        assertEquals(0, registerRequest.role)
    }

    private class FakeAuthApiService : AuthApiService {
        var loginResponse: LoginResponse = LoginResponse("", "", "", "Customer")
        var registerResponse: RegisterResponse = RegisterResponse("", "", "")
        var currentUserResponse: CurrentUserResponse = CurrentUserResponse("", "", "Customer")

        var loginThrowable: Throwable? = null
        var registerThrowable: Throwable? = null
        var currentUserThrowable: Throwable? = null

        var lastRegisterRequest: RegisterRequest? = null

        override suspend fun register(request: RegisterRequest): RegisterResponse {
            lastRegisterRequest = request
            registerThrowable?.let { throw it }
            return registerResponse
        }

        override suspend fun login(request: LoginRequest): LoginResponse {
            loginThrowable?.let { throw it }
            return loginResponse
        }

        override suspend fun getCurrentUser(): CurrentUserResponse {
            currentUserThrowable?.let { throw it }
            return currentUserResponse
        }
    }

    private class FakeTokenDataStore : TokenDataStore {
        var token: String? = null

        override suspend fun saveToken(token: String) {
            this.token = token
        }

        override fun getToken(): Flow<String?> = MutableStateFlow(token)

        override suspend fun clearToken() {
            token = null
        }
    }

    private class FakeUserSessionDataStore : UserSessionDataStore {
        var session: UserSession? = null

        override suspend fun saveSession(session: UserSession) {
            this.session = session
        }

        override fun getSession(): Flow<UserSession?> = MutableStateFlow(session)

        override suspend fun clearSession() {
            session = null
        }
    }
}
