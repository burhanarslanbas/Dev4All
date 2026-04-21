package com.dev4all.mobile.core.domain.exception

sealed class AppException(
    message: String,
    cause: Throwable? = null,
) : Exception(message, cause) {
    class Network(
        message: String,
        cause: Throwable? = null,
    ) : AppException(message, cause)

    class Unauthorized(
        message: String,
        cause: Throwable? = null,
    ) : AppException(message, cause)

    class Forbidden(
        message: String,
        cause: Throwable? = null,
    ) : AppException(message, cause)

    class NotFound(
        message: String,
        cause: Throwable? = null,
    ) : AppException(message, cause)

    class Validation(
        val errors: List<FieldError>,
        cause: Throwable? = null,
    ) : AppException(message = "Validation failed", cause = cause)

    class Server(
        message: String,
        cause: Throwable? = null,
    ) : AppException(message, cause)

    class Unknown(
        message: String,
        cause: Throwable? = null,
    ) : AppException(message, cause)
}

data class FieldError(
    val field: String,
    val message: String,
)
