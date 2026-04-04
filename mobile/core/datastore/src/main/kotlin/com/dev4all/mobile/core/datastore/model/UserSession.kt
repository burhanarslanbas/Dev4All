package com.dev4all.mobile.core.datastore.model

data class UserSession(
    val userId: String,
    val name: String,
    val email: String,
    val role: String,
    val isLoggedIn: Boolean,
)
