package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result

class LogoutUseCase(
    private val authRepository: AuthRepository,
) {
    suspend operator fun invoke(): Result<Unit> = authRepository.logout()
}
