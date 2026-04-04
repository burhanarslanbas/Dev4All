package com.dev4all.mobile.feature.auth.register

import com.dev4all.mobile.core.domain.model.UserRole

sealed interface RegisterEvent {
    data class FullNameChanged(val value: String) : RegisterEvent

    data class EmailChanged(val value: String) : RegisterEvent

    data class PasswordChanged(val value: String) : RegisterEvent

    data class RoleSelected(val value: RegisterRole) : RegisterEvent

    data object RegisterClicked : RegisterEvent
}

enum class RegisterRole(val role: UserRole) {
    Customer(UserRole.Customer),
    Developer(UserRole.Developer),
}
