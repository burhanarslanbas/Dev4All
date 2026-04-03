package com.dev4all.mobile.core.datastore

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

data class UserSession(
    val userId: String?,
    val email: String?,
    val role: String?,
    val isLoggedIn: Boolean,
)

class UserSessionDataStore @Inject constructor(
    private val dataStore: DataStore<Preferences>,
) {
    val session: Flow<UserSession> = dataStore.data.map { preferences ->
        UserSession(
            userId = preferences[USER_ID_KEY],
            email = preferences[EMAIL_KEY],
            role = preferences[ROLE_KEY],
            isLoggedIn = preferences[IS_LOGGED_IN_KEY] ?: false,
        )
    }

    suspend fun saveSession(
        userId: String,
        email: String,
        role: String,
    ) {
        dataStore.edit { preferences ->
            preferences[USER_ID_KEY] = userId
            preferences[EMAIL_KEY] = email
            preferences[ROLE_KEY] = role
            preferences[IS_LOGGED_IN_KEY] = true
        }
    }

    suspend fun clearSession() {
        dataStore.edit { preferences ->
            preferences.remove(USER_ID_KEY)
            preferences.remove(EMAIL_KEY)
            preferences.remove(ROLE_KEY)
            preferences[IS_LOGGED_IN_KEY] = false
        }
    }

    private companion object {
        val USER_ID_KEY = stringPreferencesKey("user_id")
        val EMAIL_KEY = stringPreferencesKey("user_email")
        val ROLE_KEY = stringPreferencesKey("user_role")
        val IS_LOGGED_IN_KEY = booleanPreferencesKey("is_logged_in")
    }
}
