package com.dev4all.mobile.core.datastore.impl

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import com.dev4all.mobile.core.datastore.TokenDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

class PreferencesTokenDataStore(
    private val dataStore: DataStore<Preferences>,
) : TokenDataStore {

    override suspend fun saveToken(token: String) {
        dataStore.edit { preferences ->
            preferences[TOKEN_KEY] = token
        }
    }

    override fun getToken(): Flow<String?> = dataStore.data.map { preferences ->
        preferences[TOKEN_KEY]
    }

    override suspend fun clearToken() {
        dataStore.edit { preferences ->
            preferences.remove(TOKEN_KEY)
        }
    }

    private companion object {
        val TOKEN_KEY = stringPreferencesKey("access_token")
    }
}
