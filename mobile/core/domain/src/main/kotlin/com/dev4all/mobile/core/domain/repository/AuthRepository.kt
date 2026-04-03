package com.dev4all.mobile.core.domain.repository

import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.result.Result

interface AuthRepository {
    suspend fun login(email: String, password: String): Result<AuthToken>

    suspend fun register(
        fullName: String,
        email: String,
        password: String,
        role: String,
    ): Result<AuthToken>

    suspend fun getCurrentUser(): Result<User>

    suspend fun logout(): Result<Unit>
}
