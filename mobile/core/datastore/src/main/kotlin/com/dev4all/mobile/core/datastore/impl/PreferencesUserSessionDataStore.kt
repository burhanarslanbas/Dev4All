package com.dev4all.mobile.core.datastore.impl

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import com.dev4all.mobile.core.datastore.UserSessionDataStore
import com.dev4all.mobile.core.datastore.model.UserSession
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

class PreferencesUserSessionDataStore(
    private val dataStore: DataStore<Preferences>,
) : UserSessionDataStore {

    override suspend fun saveSession(session: UserSession) {
        dataStore.edit { preferences ->
            preferences[USER_ID_KEY] = session.userId
            preferences[NAME_KEY] = session.name
            preferences[EMAIL_KEY] = session.email
            preferences[ROLE_KEY] = session.role
            preferences[IS_LOGGED_IN_KEY] = session.isLoggedIn
        }
    }

    override fun getSession(): Flow<UserSession?> = dataStore.data.map { preferences ->
        val userId = preferences[USER_ID_KEY]
        val name = preferences[NAME_KEY]
        val email = preferences[EMAIL_KEY]
        val role = preferences[ROLE_KEY]
        val isLoggedIn = preferences[IS_LOGGED_IN_KEY] ?: false

        if (userId == null || name == null || email == null || role == null) {
            null
        } else {
            UserSession(
                userId = userId,
                name = name,
                email = email,
                role = role,
                isLoggedIn = isLoggedIn,
            )
        }
    }

    override suspend fun clearSession() {
        dataStore.edit { preferences ->
            preferences.remove(USER_ID_KEY)
            preferences.remove(NAME_KEY)
            preferences.remove(EMAIL_KEY)
            preferences.remove(ROLE_KEY)
            preferences.remove(IS_LOGGED_IN_KEY)
        }
    }

    private companion object {
        val USER_ID_KEY = stringPreferencesKey("session_user_id")
        val NAME_KEY = stringPreferencesKey("session_name")
        val EMAIL_KEY = stringPreferencesKey("session_email")
        val ROLE_KEY = stringPreferencesKey("session_role")
        val IS_LOGGED_IN_KEY = booleanPreferencesKey("session_is_logged_in")
    }
}
