package com.dev4all.mobile.core.data.repository

import com.dev4all.mobile.core.data.mapper.toDomainAuthToken
import com.dev4all.mobile.core.data.mapper.toDomainUser
import com.dev4all.mobile.core.data.mapper.toDomainUserRole
import com.dev4all.mobile.core.data.mapper.toDisplayName
import com.dev4all.mobile.core.data.mapper.toRegisterRoleCode
import com.dev4all.mobile.core.data.mapper.toSession
import com.dev4all.mobile.core.datastore.TokenDataStore
import com.dev4all.mobile.core.datastore.UserSessionDataStore
import com.dev4all.mobile.core.datastore.model.UserSession
import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.exception.FieldError
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result
import com.dev4all.mobile.core.network.api.AuthApiService
import com.dev4all.mobile.core.network.dto.auth.LoginRequest
import com.dev4all.mobile.core.network.dto.auth.RegisterRequest
import com.dev4all.mobile.core.network.exception.BadRequestException
import com.dev4all.mobile.core.network.exception.ForbiddenException
import com.dev4all.mobile.core.network.exception.NotFoundException
import com.dev4all.mobile.core.network.exception.ServerException
import com.dev4all.mobile.core.network.exception.UnauthorizedException
import kotlinx.coroutines.flow.first
import java.io.IOException
import javax.inject.Inject

class AuthRepositoryImpl @Inject constructor(
    private val authApiService: AuthApiService,
    private val tokenDataStore: TokenDataStore,
    private val userSessionDataStore: UserSessionDataStore,
) : AuthRepository {

    override suspend fun login(email: String, password: String): Result<AuthToken> = runCatching {
        val loginResponse = authApiService.login(
            request = LoginRequest(
                email = email,
                password = password,
            )
        )

        val authToken = loginResponse.toDomainAuthToken()
        tokenDataStore.saveToken(authToken.token)

        val currentUser = authApiService.getCurrentUser()
        userSessionDataStore.saveSession(
            UserSession(
                userId = currentUser.userId,
                name = email.toDisplayName(),
                email = currentUser.email,
                role = currentUser.role.toDomainUserRole().name,
                isLoggedIn = true,
            )
        )

        Result.Success(authToken)
    }.getOrElse { throwable ->
        mapToResultError(throwable = throwable)
    }

    override suspend fun register(
        fullName: String,
        email: String,
        password: String,
        role: String,
    ): Result<AuthToken> = runCatching {
        val registerRole = role.toDomainUserRole().toRegisterRoleCode()
        val registerResponse = authApiService.register(
            request = RegisterRequest(
                name = fullName,
                email = email,
                password = password,
                role = registerRole,
            )
        )

        val loginResponse = authApiService.login(
            request = LoginRequest(
                email = email,
                password = password,
            )
        )

        val authToken = loginResponse.toDomainAuthToken()
        tokenDataStore.saveToken(authToken.token)
        userSessionDataStore.saveSession(
            registerResponse.toSession(role = authToken.role.name)
        )

        Result.Success(authToken)
    }.getOrElse { throwable ->
        mapToResultError(throwable = throwable)
    }

    override suspend fun getCurrentUser(): Result<User> = runCatching {
        val currentUserResponse = authApiService.getCurrentUser()
        val sessionName = userSessionDataStore.getSession().first()?.name
            ?: currentUserResponse.email.toDisplayName()

        Result.Success(currentUserResponse.toDomainUser(sessionName = sessionName))
    }.getOrElse { throwable ->
        mapToResultError(throwable = throwable)
    }

    override suspend fun logout(): Result<Unit> = runCatching {
        clearAuthState()
        Result.Success(Unit)
    }.getOrElse { throwable ->
        mapToResultError(throwable = throwable)
    }

    private suspend fun mapToResultError(throwable: Throwable): Result.Error {
        val appException = when (throwable) {
            is UnauthorizedException -> {
                clearAuthState()
                AppException.Unauthorized(throwable.message ?: "Unauthorized", throwable)
            }
            is ForbiddenException -> AppException.Forbidden(throwable.message ?: "Forbidden", throwable)
            is NotFoundException -> AppException.NotFound(throwable.message ?: "Not found", throwable)
            is BadRequestException -> AppException.Validation(
                errors = throwable.fieldErrors.map { FieldError(field = it.field, message = it.message) },
                cause = throwable,
            )
            is ServerException -> AppException.Server(throwable.message ?: "Server error", throwable)
            is IOException -> AppException.Network(throwable.message ?: "Network error", throwable)
            is AppException -> throwable
            else -> AppException.Unknown(throwable.message ?: "Unknown error", throwable)
        }

        return Result.Error(appException)
    }

    private suspend fun clearAuthState() {
        tokenDataStore.clearToken()
        userSessionDataStore.clearSession()
    }
}
