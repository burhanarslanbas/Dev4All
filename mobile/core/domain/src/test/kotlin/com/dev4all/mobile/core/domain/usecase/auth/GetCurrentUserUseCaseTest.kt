package com.dev4all.mobile.core.domain.usecase.auth

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.result.Result
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Test

class GetCurrentUserUseCaseTest {

    @Test
    fun invoke_repositorySuccess_returnsUser() = runTest {
        val expected = Result.Success(
            User(
                id = "user-id",
                name = "Test User",
                email = "user@mail.com",
                role = UserRole.Customer,
            )
        )
        val repository = FakeAuthRepository(currentUserResult = expected)
        val useCase = GetCurrentUserUseCase(authRepository = repository)

        val result = useCase()

        assertEquals(expected, result)
    }

    @Test
    fun invoke_repositoryError_returnsError() = runTest {
        val expected = Result.Error(AppException.Unauthorized("Authentication required"))
        val repository = FakeAuthRepository(currentUserResult = expected)
        val useCase = GetCurrentUserUseCase(authRepository = repository)

        val result = useCase()

        assertEquals(expected, result)
    }
}
