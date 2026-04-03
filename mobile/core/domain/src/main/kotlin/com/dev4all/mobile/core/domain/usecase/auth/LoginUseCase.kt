package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result

class LoginUseCase(
    private val authRepository: AuthRepository,
) {
    suspend operator fun invoke(email: String, password: String): Result<AuthToken> =
        authRepository.login(email = email, password = password)
}
