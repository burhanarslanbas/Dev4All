package com.dev4all.mobile.feature.auth.register

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.result.Result
import com.dev4all.mobile.core.domain.usecase.auth.RegisterUseCase
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

class RegisterViewModel(
    private val registerAction: suspend (String, String, String, String) -> Result<*>,
) : ViewModel() {

    constructor(registerUseCase: RegisterUseCase) : this(registerUseCase::invoke)

    private val _uiState = MutableStateFlow<RegisterUiState>(RegisterUiState.Idle)
    val uiState: StateFlow<RegisterUiState> = _uiState.asStateFlow()

    private val _fullName = MutableStateFlow("")
    val fullName: StateFlow<String> = _fullName.asStateFlow()

    private val _email = MutableStateFlow("")
    val email: StateFlow<String> = _email.asStateFlow()

    private val _password = MutableStateFlow("")
    val password: StateFlow<String> = _password.asStateFlow()

    private val _selectedRole = MutableStateFlow<RegisterRole?>(null)
    val selectedRole: StateFlow<RegisterRole?> = _selectedRole.asStateFlow()

    fun onEvent(event: RegisterEvent) {
        when (event) {
            is RegisterEvent.FullNameChanged -> {
                resetTransientState()
                _fullName.update { event.value }
            }

            is RegisterEvent.EmailChanged -> {
                resetTransientState()
                _email.update { event.value }
            }

            is RegisterEvent.PasswordChanged -> {
                resetTransientState()
                _password.update { event.value }
            }

            is RegisterEvent.RoleSelected -> {
                resetTransientState()
                _selectedRole.update { event.value }
            }

            RegisterEvent.RegisterClicked -> register()
        }
    }

    private fun register() {
        val validationErrors = validate(
            fullName = _fullName.value,
            email = _email.value,
            password = _password.value,
            role = _selectedRole.value,
        )

        if (validationErrors.isNotEmpty()) {
            _uiState.value = RegisterUiState.ValidationError(validationErrors)
            return
        }

        viewModelScope.launch {
            _uiState.value = RegisterUiState.Loading
            val role = checkNotNull(_selectedRole.value).role.name
            when (
                val result = registerAction(
                    _fullName.value.trim(),
                    _email.value.trim(),
                    _password.value,
                    role,
                )
            ) {
                is Result.Success -> {
                    _uiState.value = RegisterUiState.Success
                }

                is Result.Error -> {
                    _uiState.value = when (val exception = result.exception) {
                        is AppException.Validation -> {
                            RegisterUiState.ValidationError(
                                exception.errors.associate { error ->
                                    error.field.trim().lowercase() to error.message
                                }
                            )
                        }

                        else -> RegisterUiState.Error(exception.message ?: "Bilinmeyen hata")
                    }
                }
            }
        }
    }

    private fun validate(
        fullName: String,
        email: String,
        password: String,
        role: RegisterRole?,
    ): Map<String, String> {
        val errors = linkedMapOf<String, String>()

        when {
            fullName.isBlank() -> errors["name"] = "Ad boş olamaz."
            fullName.trim().length !in 2..100 -> errors["name"] = "Ad 2-100 karakter arası olmalıdır."
        }

        when {
            email.isBlank() -> errors["email"] = "E-posta boş olamaz."
            !EMAIL_REGEX.matches(email.trim()) -> errors["email"] = "Geçerli bir e-posta adresi giriniz."
        }

        when {
            password.isBlank() -> errors["password"] = "Şifre boş olamaz."
            password.length < 8 -> errors["password"] = "Şifre en az 8 karakter olmalıdır."
            password.none { it.isUpperCase() } -> errors["password"] = "Şifre en az 1 büyük harf içermelidir."
            password.none { it.isDigit() } -> errors["password"] = "Şifre en az 1 rakam içermelidir."
        }

        if (role == null) {
            errors["role"] = "Rol seçimi zorunludur."
        }

        return errors
    }

    private fun resetTransientState() {
        if (_uiState.value is RegisterUiState.Error || _uiState.value is RegisterUiState.ValidationError) {
            _uiState.value = RegisterUiState.Idle
        }
    }

    private companion object {
        val EMAIL_REGEX: Regex = "^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+$".toRegex()
    }
}
