package com.dev4all.mobile.core.datastore

import com.dev4all.mobile.core.datastore.model.UserSession
import kotlinx.coroutines.flow.Flow

interface UserSessionDataStore {
    suspend fun saveSession(session: UserSession)

    fun getSession(): Flow<UserSession?>

    suspend fun clearSession()
}
