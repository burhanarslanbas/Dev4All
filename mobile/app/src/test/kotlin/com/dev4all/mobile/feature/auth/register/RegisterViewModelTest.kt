package com.dev4all.mobile.feature.auth.register

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.exception.FieldError
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.result.Result
import kotlinx.coroutines.test.advanceUntilIdle
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Rule
import org.junit.Test

class RegisterViewModelTest {

    @get:Rule
    val mainDispatcherRule = MainDispatcherRule()

    @Test
    fun register_withoutRole_emitsValidationError() = runTest {
        val viewModel = RegisterViewModel { _, _, _, _ ->
            Result.Success(sampleToken())
        }

        viewModel.onEvent(RegisterEvent.FullNameChanged("Test User"))
        viewModel.onEvent(RegisterEvent.EmailChanged("test@mail.com"))
        viewModel.onEvent(RegisterEvent.PasswordChanged("Password1"))
        viewModel.onEvent(RegisterEvent.RegisterClicked)

        val state = viewModel.uiState.value
        assertTrue(state is RegisterUiState.ValidationError)
        assertEquals("Rol seçimi zorunludur.", (state as RegisterUiState.ValidationError).errors["role"])
    }

    @Test
    fun register_invalidPassword_emitsValidationError() = runTest {
        val viewModel = RegisterViewModel { _, _, _, _ ->
            Result.Success(sampleToken())
        }

        viewModel.onEvent(RegisterEvent.FullNameChanged("Test User"))
        viewModel.onEvent(RegisterEvent.EmailChanged("test@mail.com"))
        viewModel.onEvent(RegisterEvent.RoleSelected(RegisterRole.Customer))
        viewModel.onEvent(RegisterEvent.PasswordChanged("password1"))
        viewModel.onEvent(RegisterEvent.RegisterClicked)

        val state = viewModel.uiState.value
        assertTrue(state is RegisterUiState.ValidationError)
        assertEquals(
            "Şifre en az 1 büyük harf içermelidir.",
            (state as RegisterUiState.ValidationError).errors["password"],
        )
    }

    @Test
    fun register_success_emitsSuccess() = runTest {
        val viewModel = RegisterViewModel { _, _, _, _ ->
            Result.Success(sampleToken())
        }

        viewModel.onEvent(RegisterEvent.FullNameChanged("Test User"))
        viewModel.onEvent(RegisterEvent.EmailChanged("test@mail.com"))
        viewModel.onEvent(RegisterEvent.PasswordChanged("Password1"))
        viewModel.onEvent(RegisterEvent.RoleSelected(RegisterRole.Developer))
        viewModel.onEvent(RegisterEvent.RegisterClicked)
        advanceUntilIdle()

        assertTrue(viewModel.uiState.value is RegisterUiState.Success)
    }

    @Test
    fun register_apiValidationError_emitsValidationState() = runTest {
        val viewModel = RegisterViewModel { _, _, _, _ ->
            Result.Error(
                AppException.Validation(
                    errors = listOf(FieldError(field = "Email", message = "E-posta boş olamaz."))
                )
            )
        }

        viewModel.onEvent(RegisterEvent.FullNameChanged("Test User"))
        viewModel.onEvent(RegisterEvent.EmailChanged("test@mail.com"))
        viewModel.onEvent(RegisterEvent.PasswordChanged("Password1"))
        viewModel.onEvent(RegisterEvent.RoleSelected(RegisterRole.Customer))
        viewModel.onEvent(RegisterEvent.RegisterClicked)
        advanceUntilIdle()

        val state = viewModel.uiState.value
        assertTrue(state is RegisterUiState.ValidationError)
        assertEquals("E-posta boş olamaz.", (state as RegisterUiState.ValidationError).errors["email"])
    }

    @Test
    fun register_serverError_emitsErrorState() = runTest {
        val viewModel = RegisterViewModel { _, _, _, _ ->
            Result.Error(AppException.Server(message = "Sunucu hatası"))
        }

        viewModel.onEvent(RegisterEvent.FullNameChanged("Test User"))
        viewModel.onEvent(RegisterEvent.EmailChanged("test@mail.com"))
        viewModel.onEvent(RegisterEvent.PasswordChanged("Password1"))
        viewModel.onEvent(RegisterEvent.RoleSelected(RegisterRole.Customer))
        viewModel.onEvent(RegisterEvent.RegisterClicked)
        advanceUntilIdle()

        val state = viewModel.uiState.value
        assertTrue(state is RegisterUiState.Error)
        assertEquals("Sunucu hatası", (state as RegisterUiState.Error).message)
    }

    private fun sampleToken(): AuthToken = AuthToken(
        token = "token",
        expiresAt = "2026-04-01T00:00:00Z",
        email = "test@mail.com",
        role = UserRole.Customer,
    )
}
