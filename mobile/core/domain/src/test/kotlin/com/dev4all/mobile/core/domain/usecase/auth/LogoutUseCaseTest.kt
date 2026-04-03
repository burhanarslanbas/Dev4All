package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.result.Result
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Test

class LogoutUseCaseTest {

    @Test
    fun invoke_repositorySuccess_returnsUnitSuccess() = runTest {
        val expected = Result.Success(Unit)
        val repository = FakeAuthRepository(logoutResult = expected)
        val useCase = LogoutUseCase(authRepository = repository)

        val result = useCase()

        assertEquals(expected, result)
    }

    @Test
    fun invoke_repositoryError_returnsError() = runTest {
        val expected = Result.Error(AppException.Network("No connection"))
        val repository = FakeAuthRepository(logoutResult = expected)
        val useCase = LogoutUseCase(authRepository = repository)

        val result = useCase()

        assertEquals(expected, result)
    }
}
