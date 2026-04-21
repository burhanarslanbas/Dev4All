package com.dev4all.mobile.core.network.mapper

import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.network.dto.auth.CurrentUserResponse
import com.dev4all.mobile.core.network.dto.auth.LoginResponse

fun LoginResponse.toDomain(): AuthToken = AuthToken(
    token = token,
    expiresAt = expiresAt,
    email = email,
    role = role.toUserRole(),
)

fun CurrentUserResponse.toDomain(name: String): User = User(
    id = userId,
    name = name,
    email = email,
    role = role.toUserRole(),
)

private fun String.toUserRole(): UserRole = when (this.lowercase()) {
    "customer" -> UserRole.Customer
    "developer" -> UserRole.Developer
    "admin" -> UserRole.Admin
    else -> UserRole.Customer
}
