package com.dev4all.mobile.core.domain.model

data class User(
    val id: String,
    val name: String,
    val email: String,
    val role: UserRole,
)
