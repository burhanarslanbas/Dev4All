package com.dev4all.mobile.core.network.dto.auth

import kotlinx.serialization.Serializable

@Serializable
data class RegisterResponse(
    val userId: String,
    val email: String,
    val name: String,
)
