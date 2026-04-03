package com.dev4all.mobile.core.network.interceptor

import com.dev4all.mobile.core.network.dto.error.ApiErrorResponse
import com.dev4all.mobile.core.network.dto.error.ValidationErrorResponse
import com.dev4all.mobile.core.network.exception.BadRequestException
import com.dev4all.mobile.core.network.exception.FieldError
import com.dev4all.mobile.core.network.exception.ForbiddenException
import com.dev4all.mobile.core.network.exception.NotFoundException
import com.dev4all.mobile.core.network.exception.ServerException
import com.dev4all.mobile.core.network.exception.UnauthorizedException
import kotlinx.serialization.json.Json
import okhttp3.Interceptor
import okhttp3.Response
import javax.inject.Inject

class ErrorInterceptor @Inject constructor(
    private val json: Json,
) : Interceptor {

    override fun intercept(chain: Interceptor.Chain): Response {
        val response = chain.proceed(chain.request())
        if (response.isSuccessful) {
            return response
        }

        val statusCode = response.code
        val rawBody = response.body?.string()
        response.close()

        throw mapException(statusCode = statusCode, body = rawBody)
    }

    private fun mapException(statusCode: Int, body: String?): Throwable {
        val validationError = body.deserializeOrNull<ValidationErrorResponse>()
        val apiError = body.deserializeOrNull<ApiErrorResponse>()
        val message = apiError?.message ?: validationError?.message ?: defaultMessage(statusCode)

        return when (statusCode) {
            400 -> {
                val fields = validationError
                    ?.errors
                    .orEmpty()
                    .map { FieldError(field = it.field, message = it.message) }

                BadRequestException(message = message, fieldErrors = fields)
            }
            401 -> UnauthorizedException(message = message)
            403 -> ForbiddenException(message = message)
            404 -> NotFoundException(message = message)
            else -> ServerException(message = message, statusCode = statusCode)
        }
    }

    private inline fun <reified T> String?.deserializeOrNull(): T? {
        if (this.isNullOrBlank()) {
            return null
        }

        return runCatching { json.decodeFromString<T>(this) }.getOrNull()
    }

    private fun defaultMessage(statusCode: Int): String {
        return when (statusCode) {
            400 -> "Bad request"
            401 -> "Unauthorized"
            403 -> "Forbidden"
            404 -> "Not found"
            else -> "Server error ($statusCode)"
        }
    }
}
