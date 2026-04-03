package com.dev4all.mobile.core.network.auth

interface TokenProvider {
    suspend fun getAccessToken(): String?
}
