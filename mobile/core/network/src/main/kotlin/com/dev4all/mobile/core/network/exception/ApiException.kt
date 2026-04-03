package com.dev4all.mobile.core.network.exception

import java.io.IOException

sealed class ApiException(
    message: String,
    val statusCode: Int,
    cause: Throwable? = null,
) : IOException(message, cause)

data class FieldError(
    val field: String,
    val message: String,
)

class BadRequestException(
    message: String,
    val fieldErrors: List<FieldError> = emptyList(),
) : ApiException(message = message, statusCode = 400)

class UnauthorizedException(
    message: String,
) : ApiException(message = message, statusCode = 401)

class ForbiddenException(
    message: String,
) : ApiException(message = message, statusCode = 403)

class NotFoundException(
    message: String,
) : ApiException(message = message, statusCode = 404)

class ServerException(
    message: String,
    statusCode: Int,
) : ApiException(message = message, statusCode = statusCode)
