package com.dev4all.mobile.core.datastore

import kotlinx.coroutines.flow.Flow

interface TokenDataStore {
    suspend fun saveToken(token: String)

    fun getToken(): Flow<String?>

    suspend fun clearToken()
}
