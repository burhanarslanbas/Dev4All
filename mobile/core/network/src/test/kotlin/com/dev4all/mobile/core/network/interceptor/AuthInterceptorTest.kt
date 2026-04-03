package com.dev4all.mobile.core.network.interceptor

import com.dev4all.mobile.core.network.auth.TokenProvider
import okhttp3.Call
import okhttp3.Connection
import okhttp3.Interceptor
import okhttp3.Protocol
import okhttp3.Request
import okhttp3.Response
import okhttp3.ResponseBody.Companion.toResponseBody
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Test

class AuthInterceptorTest {

    @Test
    fun intercept_tokenExists_addsAuthorizationHeader() {
        val interceptor = AuthInterceptor(tokenProvider = FakeTokenProvider("test-token"))
        val chain = FakeChain()

        interceptor.intercept(chain)

        assertEquals("Bearer test-token", chain.proceededRequest.header("Authorization"))
    }

    @Test
    fun intercept_tokenMissing_doesNotAddAuthorizationHeader() {
        val interceptor = AuthInterceptor(tokenProvider = FakeTokenProvider(null))
        val chain = FakeChain()

        interceptor.intercept(chain)

        assertNull(chain.proceededRequest.header("Authorization"))
    }

    private class FakeTokenProvider(
        private val token: String?,
    ) : TokenProvider {
        override suspend fun getAccessToken(): String? = token
    }

    private class FakeChain : Interceptor.Chain {
        private val request = Request.Builder()
            .url("https://example.com")
            .build()

        lateinit var proceededRequest: Request

        override fun request(): Request = request

        override fun proceed(request: Request): Response {
            proceededRequest = request
            return Response.Builder()
                .request(request)
                .protocol(Protocol.HTTP_1_1)
                .code(200)
                .message("OK")
                .body("ok".toResponseBody())
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
