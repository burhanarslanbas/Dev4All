package com.dev4all.mobile.core.network.interceptor

import com.dev4all.mobile.core.network.auth.TokenProvider
import kotlinx.coroutines.runBlocking
import okhttp3.Interceptor
import okhttp3.Response
import javax.inject.Inject

class AuthInterceptor @Inject constructor(
    private val tokenProvider: TokenProvider,
) : Interceptor {

    override fun intercept(chain: Interceptor.Chain): Response {
        val token = runCatching { runBlocking { tokenProvider.getAccessToken() } }
            .getOrNull()
            ?.takeIf { it.isNotBlank() }

        val request = chain.request().newBuilder().apply {
            if (token != null) {
                header(AUTHORIZATION_HEADER, "$BEARER_PREFIX$token")
            }
        }.build()

        return chain.proceed(request)
    }

    private companion object {
        const val AUTHORIZATION_HEADER = "Authorization"
        const val BEARER_PREFIX = "Bearer "
    }
}
