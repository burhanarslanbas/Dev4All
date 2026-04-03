package com.dev4all.mobile.core.network.dto.auth

import kotlinx.serialization.Serializable

@Serializable
data class LoginResponse(
    val token: String,
    val expiresAt: String,
    val email: String,
    val role: String,
)
