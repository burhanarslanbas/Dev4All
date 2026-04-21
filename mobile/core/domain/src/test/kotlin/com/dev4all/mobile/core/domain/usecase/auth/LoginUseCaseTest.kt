package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.result.Result
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Test

class LoginUseCaseTest {

    @Test
    fun invoke_validCredentials_returnsRepositoryResult() = runTest {
        val expected = Result.Success(
            AuthToken(
                token = "jwt-token",
                expiresAt = "2026-04-01T00:00:00Z",
                email = "user@mail.com",
                role = UserRole.Customer,
            )
        )
        val repository = FakeAuthRepository(loginResult = expected)
        val useCase = LoginUseCase(authRepository = repository)

        val result = useCase(email = "user@mail.com", password = "Pass1234")

        assertEquals(expected, result)
    }

    @Test
    fun invoke_repositoryError_returnsError() = runTest {
        val expected = Result.Error(AppException.Forbidden("Invalid credentials"))
        val repository = FakeAuthRepository(loginResult = expected)
        val useCase = LoginUseCase(authRepository = repository)

        val result = useCase(email = "user@mail.com", password = "wrong")

        assertEquals(expected, result)
    }
}
