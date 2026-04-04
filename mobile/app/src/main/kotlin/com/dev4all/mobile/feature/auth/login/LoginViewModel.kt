package com.dev4all.mobile.feature.auth.login

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.result.Result
import com.dev4all.mobile.core.domain.usecase.auth.LoginUseCase
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject
import kotlinx.coroutines.channels.Channel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.receiveAsFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

@HiltViewModel
class LoginViewModel @Inject constructor(
    private val loginUseCase: LoginUseCase,
) : ViewModel() {

    private val _uiState = MutableStateFlow(LoginUiState())
    val uiState: StateFlow<LoginUiState> = _uiState.asStateFlow()

    private val _sideEffects = Channel<LoginSideEffect>(Channel.BUFFERED)
    val sideEffects = _sideEffects.receiveAsFlow()

    fun onEvent(event: LoginEvent) {
        when (event) {
            is LoginEvent.EmailChanged -> _uiState.update {
                it.copy(email = event.email, emailError = null, errorMessage = null)
            }

            is LoginEvent.PasswordChanged -> _uiState.update {
                it.copy(password = event.password, passwordError = null, errorMessage = null)
            }

            LoginEvent.LoginClicked -> submitLogin()
        }
    }

    private fun submitLogin() {
        val currentState = _uiState.value
        val emailError = validateEmail(currentState.email)
        val passwordError = validatePassword(currentState.password)

        if (emailError != null || passwordError != null) {
            _uiState.update {
                it.copy(
                    emailError = emailError,
                    passwordError = passwordError,
                    errorMessage = null,
                    isLoading = false,
                )
            }
            return
        }

        viewModelScope.launch {
            _uiState.update {
                it.copy(
                    isLoading = true,
                    emailError = null,
                    passwordError = null,
                    errorMessage = null,
                )
            }

            when (val result = loginUseCase(currentState.email.trim(), currentState.password)) {
                is Result.Success -> {
                    _uiState.update { it.copy(isLoading = false) }
                    _sideEffects.send(LoginSideEffect.NavigateByRole(result.data.role))
                }

                is Result.Error -> {
                    _uiState.update {
                        it.copy(
                            isLoading = false,
                            errorMessage = mapError(result.exception),
                        )
                    }
                }
            }
        }
    }

    private fun validateEmail(email: String): String? {
        val trimmedEmail = email.trim()
        if (trimmedEmail.isBlank()) return "Email is required"
        return if (EMAIL_REGEX.matches(trimmedEmail)) null else "Enter a valid email address"
    }

    private fun validatePassword(password: String): String? {
        if (password.isBlank()) return "Password is required"
        return if (password.length >= MIN_PASSWORD_LENGTH) null else "Password must be at least 6 characters"
    }

    private fun mapError(exception: AppException): String = when (exception) {
        is AppException.Forbidden -> "Invalid email or password"
        is AppException.Unauthorized -> "Your session is not authorized"
        is AppException.Network -> "Please check your internet connection"
        is AppException.Validation -> exception.errors.firstOrNull()?.message ?: "Validation failed"
        else -> exception.message ?: "Something went wrong"
    }

    private companion object {
        private const val EMAIL_PATTERN = "^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$"
        private val EMAIL_REGEX = Regex(EMAIL_PATTERN)
        private const val MIN_PASSWORD_LENGTH = 6
    }
}
