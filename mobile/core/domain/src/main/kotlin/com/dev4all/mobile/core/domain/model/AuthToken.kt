package com.dev4all.mobile.core.domain.model

data class AuthToken(
    val token: String,
    val expiresAt: String,
    val email: String,
    val role: UserRole,
)
