package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result

class RegisterUseCase(
    private val authRepository: AuthRepository,
) {
    suspend operator fun invoke(
        fullName: String,
        email: String,
        password: String,
        role: String,
    ): Result<AuthToken> =
        authRepository.register(
            fullName = fullName,
            email = email,
            password = password,
            role = role,
        )
}
