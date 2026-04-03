package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result

class FakeAuthRepository(
    private val loginResult: Result<AuthToken> = Result.Error(AppException.Unknown("Not configured")),
    private val registerResult: Result<AuthToken> = Result.Error(AppException.Unknown("Not configured")),
    private val currentUserResult: Result<User> = Result.Error(AppException.Unknown("Not configured")),
    private val logoutResult: Result<Unit> = Result.Error(AppException.Unknown("Not configured")),
) : AuthRepository {
    override suspend fun login(email: String, password: String): Result<AuthToken> = loginResult

    override suspend fun register(
        fullName: String,
        email: String,
        password: String,
        role: String,
    ): Result<AuthToken> = registerResult

    override suspend fun getCurrentUser(): Result<User> = currentUserResult

    override suspend fun logout(): Result<Unit> = logoutResult
}
