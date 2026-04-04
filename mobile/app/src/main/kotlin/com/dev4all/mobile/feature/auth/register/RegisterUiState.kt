package com.dev4all.mobile.feature.auth.register

sealed interface RegisterUiState {
    data object Idle : RegisterUiState

    data object Loading : RegisterUiState

    data class ValidationError(val errors: Map<String, String>) : RegisterUiState

    data class Error(val message: String) : RegisterUiState

    data object Success : RegisterUiState
}
