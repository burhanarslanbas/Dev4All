package com.dev4all.mobile.core.network.dto.error

import kotlinx.serialization.Serializable

@Serializable
data class ApiErrorResponse(
    val statusCode: Int,
    val timestamp: String,
    val path: String,
    val message: String,
)
