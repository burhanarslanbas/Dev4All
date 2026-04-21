package com.dev4all.mobile.feature.auth.login

import com.dev4all.mobile.core.domain.model.UserRole

sealed interface LoginSideEffect {
    data class NavigateByRole(val role: UserRole) : LoginSideEffect
}
