package com.dev4all.mobile.core.network.dto.error

import kotlinx.serialization.Serializable

@Serializable
data class ValidationErrorResponse(
    val statusCode: Int,
    val timestamp: String,
    val path: String,
    val message: String,
    val errors: List<ValidationErrorItem> = emptyList(),
)

@Serializable
data class ValidationErrorItem(
    val field: String,
    val message: String,
)
