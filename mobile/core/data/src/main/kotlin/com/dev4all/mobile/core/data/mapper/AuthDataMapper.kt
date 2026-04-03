package com.dev4all.mobile.core.data.mapper

import com.dev4all.mobile.core.datastore.model.UserSession
import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.exception.FieldError
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.network.dto.auth.CurrentUserResponse
import com.dev4all.mobile.core.network.dto.auth.LoginResponse
import com.dev4all.mobile.core.network.dto.auth.RegisterResponse

fun LoginResponse.toDomainAuthToken(): AuthToken = AuthToken(
    token = token,
    expiresAt = expiresAt,
    email = email,
    role = role.toDomainUserRole(),
)

fun RegisterResponse.toSession(): UserSession = UserSession(
    userId = userId,
    name = name,
    email = email,
    role = UserRole.Customer.name,
    isLoggedIn = true,
)

fun CurrentUserResponse.toDomainUser(sessionName: String): User = User(
    id = userId,
    name = sessionName,
    email = email,
    role = role.toDomainUserRole(),
)

fun UserRole.toRegisterRoleCode(): Int = when (this) {
    UserRole.Customer -> 0
    UserRole.Developer -> 1
    UserRole.Admin -> throw AppException.Validation(
        errors = listOf(
            FieldError(
                field = "role",
                message = "Admin role cannot be selected during register",
            )
        )
    )
}

fun String.toDomainUserRole(): UserRole = when (trim().lowercase()) {
    "developer" -> UserRole.Developer
    "admin" -> UserRole.Admin
    else -> UserRole.Customer
}
