package com.dev4all.mobile.core.datastore

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

class AppPreferencesDataStore @Inject constructor(
    private val dataStore: DataStore<Preferences>,
) {
    val isOnboardingCompleted: Flow<Boolean> = dataStore.data.map { preferences ->
        preferences[IS_ONBOARDING_COMPLETED_KEY] ?: false
    }

    val isDarkModeEnabled: Flow<Boolean> = dataStore.data.map { preferences ->
        preferences[IS_DARK_MODE_ENABLED_KEY] ?: false
    }

    suspend fun setOnboardingCompleted(isCompleted: Boolean) {
        dataStore.edit { preferences ->
            preferences[IS_ONBOARDING_COMPLETED_KEY] = isCompleted
        }
    }

    suspend fun setDarkModeEnabled(isEnabled: Boolean) {
        dataStore.edit { preferences ->
            preferences[IS_DARK_MODE_ENABLED_KEY] = isEnabled
        }
    }

    suspend fun clearPreferences() {
        dataStore.edit { preferences ->
            preferences.remove(IS_ONBOARDING_COMPLETED_KEY)
            preferences.remove(IS_DARK_MODE_ENABLED_KEY)
        }
    }

    private companion object {
        val IS_ONBOARDING_COMPLETED_KEY = booleanPreferencesKey("is_onboarding_completed")
        val IS_DARK_MODE_ENABLED_KEY = booleanPreferencesKey("is_dark_mode_enabled")
    }
}
