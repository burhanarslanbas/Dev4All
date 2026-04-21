package com.dev4all.mobile.feature.auth.login

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result
import com.dev4all.mobile.core.domain.usecase.auth.LoginUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.cancel
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test

@OptIn(ExperimentalCoroutinesApi::class)
class LoginViewModelTest {

    private val testDispatcher = StandardTestDispatcher()

    @Before
    fun setup() {
        Dispatchers.setMain(testDispatcher)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun onLoginClicked_withBlankEmail_setsValidationError() = runTest(testDispatcher) {
        val viewModel = createViewModel(Result.Success(validToken))

        viewModel.onEvent(LoginEvent.PasswordChanged("Password1"))
        viewModel.onEvent(LoginEvent.LoginClicked)

        assertEquals("Email is required", viewModel.uiState.value.emailError)
        assertFalse(viewModel.uiState.value.isLoading)
    }

    @Test
    fun onLoginClicked_withShortPassword_setsValidationError() = runTest(testDispatcher) {
        val viewModel = createViewModel(Result.Success(validToken))

        viewModel.onEvent(LoginEvent.EmailChanged("user@mail.com"))
        viewModel.onEvent(LoginEvent.PasswordChanged("123"))
        viewModel.onEvent(LoginEvent.LoginClicked)

        assertEquals("Password must be at least 6 characters", viewModel.uiState.value.passwordError)
        assertFalse(viewModel.uiState.value.isLoading)
    }

    @Test
    fun onLoginClicked_whenUseCaseFails_setsErrorMessage() = runTest(testDispatcher) {
        val viewModel = createViewModel(Result.Error(AppException.Forbidden("Invalid credentials")))

        viewModel.onEvent(LoginEvent.EmailChanged("user@mail.com"))
        viewModel.onEvent(LoginEvent.PasswordChanged("Password1"))
        viewModel.onEvent(LoginEvent.LoginClicked)
        testScheduler.advanceUntilIdle()

        assertEquals("Invalid email or password", viewModel.uiState.value.errorMessage)
        assertFalse(viewModel.uiState.value.isLoading)
    }

    @Test
    fun onLoginClicked_whenUseCaseSucceeds_emitsNavigateSideEffectByRole() = runTest(testDispatcher) {
        val viewModel = createViewModel(Result.Success(validToken.copy(role = UserRole.Developer)))
        var received: LoginSideEffect? = null
        val collector = launch {
            received = viewModel.sideEffects.first()
        }

        viewModel.onEvent(LoginEvent.EmailChanged("dev@mail.com"))
        viewModel.onEvent(LoginEvent.PasswordChanged("Password1"))
        viewModel.onEvent(LoginEvent.LoginClicked)
        testScheduler.advanceUntilIdle()

        assertTrue(received is LoginSideEffect.NavigateByRole)
        assertEquals(
            UserRole.Developer,
            (received as LoginSideEffect.NavigateByRole).role
        )
        assertFalse(viewModel.uiState.value.isLoading)
        collector.cancel()
    }

    private fun createViewModel(loginResult: Result<AuthToken>): LoginViewModel {
        val useCase = LoginUseCase(
            authRepository = object : AuthRepository {
                override suspend fun login(email: String, password: String): Result<AuthToken> = loginResult

                override suspend fun register(
                    fullName: String,
                    email: String,
                    password: String,
                    role: String,
                ): Result<AuthToken> = error("Not used in LoginViewModel tests")

                override suspend fun getCurrentUser(): Result<User> = error("Not used in LoginViewModel tests")

                override suspend fun logout(): Result<Unit> = error("Not used in LoginViewModel tests")
            }
        )

        return LoginViewModel(loginUseCase = useCase)
    }

    private companion object {
        val validToken = AuthToken(
            token = "jwt-token",
            expiresAt = "2026-04-05T00:00:00Z",
            email = "user@mail.com",
            role = UserRole.Customer,
        )
    }
}
