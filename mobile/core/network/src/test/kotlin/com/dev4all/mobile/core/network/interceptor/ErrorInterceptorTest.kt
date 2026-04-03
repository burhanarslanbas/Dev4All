package com.dev4all.mobile.core.network.interceptor

import com.dev4all.mobile.core.network.exception.BadRequestException
import com.dev4all.mobile.core.network.exception.ForbiddenException
import com.dev4all.mobile.core.network.exception.ServerException
import com.dev4all.mobile.core.network.exception.UnauthorizedException
import kotlinx.serialization.json.Json
import okhttp3.Call
import okhttp3.Connection
import okhttp3.Interceptor
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.Protocol
import okhttp3.Request
import okhttp3.Response
import okhttp3.ResponseBody.Companion.toResponseBody
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import org.junit.Test

class ErrorInterceptorTest {

    private val json = Json {
        ignoreUnknownKeys = true
        isLenient = true
        coerceInputValues = true
    }

    @Test
    fun intercept_validationError_throwsBadRequestWithFieldErrors() {
        val interceptor = ErrorInterceptor(json = json)
        val body = """
            {
              "statusCode": 400,
              "timestamp": "2026-04-03T00:00:00Z",
              "path": "/api/v1/auth/register",
              "message": "Validation failed",
              "errors": [
                {"field": "email", "message": "Email is required"}
              ]
            }
        """.trimIndent()

        val thrown = runCatching { interceptor.intercept(FakeChain(code = 400, body = body)) }.exceptionOrNull()

        assertTrue(thrown is BadRequestException)
        val badRequest = thrown as BadRequestException
        assertEquals("Validation failed", badRequest.message)
        assertEquals(1, badRequest.fieldErrors.size)
        assertEquals("email", badRequest.fieldErrors.first().field)
    }

    @Test
    fun intercept_unauthorizedError_throwsUnauthorizedException() {
        val interceptor = ErrorInterceptor(json = json)
        val body = """
            {
              "statusCode": 401,
              "timestamp": "2026-04-03T00:00:00Z",
              "path": "/api/v1/auth/me",
              "message": "Authentication required"
            }
        """.trimIndent()

        val thrown = runCatching { interceptor.intercept(FakeChain(code = 401, body = body)) }.exceptionOrNull()

        assertTrue(thrown is UnauthorizedException)
        assertEquals("Authentication required", thrown?.message)
    }

    @Test
    fun intercept_forbiddenError_throwsForbiddenException() {
        val interceptor = ErrorInterceptor(json = json)
        val body = """
            {
              "statusCode": 403,
              "timestamp": "2026-04-03T00:00:00Z",
              "path": "/api/v1/auth/login",
              "message": "Geçersiz e-posta veya şifre."
            }
        """.trimIndent()

        val thrown = runCatching { interceptor.intercept(FakeChain(code = 403, body = body)) }.exceptionOrNull()

        assertTrue(thrown is ForbiddenException)
        assertEquals("Geçersiz e-posta veya şifre.", thrown?.message)
    }

    @Test
    fun intercept_unparseableError_usesDefaultServerMessage() {
        val interceptor = ErrorInterceptor(json = json)

        val thrown = runCatching {
            interceptor.intercept(FakeChain(code = 500, body = "<html>500 error</html>"))
        }.exceptionOrNull()

        assertTrue(thrown is ServerException)
        assertEquals("Server error (500)", thrown?.message)
    }

    private class FakeChain(
        private val code: Int,
        private val body: String?,
    ) : Interceptor.Chain {
        private val request = Request.Builder()
            .url("https://example.com")
            .build()

        override fun request(): Request = request

        override fun proceed(request: Request): Response {
            val responseBody = body?.toResponseBody("application/json".toMediaType())
            return Response.Builder()
                .request(request)
                .protocol(Protocol.HTTP_1_1)
                .code(code)
                .message("HTTP $code")
                .apply {
                    if (responseBody != null) {
                        body(responseBody)
                    }
                }
                .build()
        }

        override fun call(): Call {
            throw UnsupportedOperationException()
        }

        override fun connection(): Connection? = null

        override fun connectTimeoutMillis(): Int = 0

        override fun withConnectTimeout(timeout: Int, unit: java.util.concurrent.TimeUnit): Interceptor.Chain = this

        override fun readTimeoutMillis(): Int = 0

        override fun withReadTimeout(timeout: Int, unit: java.util.concurrent.TimeUnit): Interceptor.Chain = this

        override fun writeTimeoutMillis(): Int = 0

        override fun withWriteTimeout(timeout: Int, unit: java.util.concurrent.TimeUnit): Interceptor.Chain = this
    }
}
