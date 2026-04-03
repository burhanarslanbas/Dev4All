package com.dev4all.mobile.core.datastore

import kotlinx.coroutines.flow.first
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class AppPreferencesDataStoreTest {

    private val preferencesDataStore = TestPreferencesDataStore()
    private val appPreferencesDataStore = AppPreferencesDataStore(dataStore = preferencesDataStore)

    @Test
    fun setOnboardingCompleted_valueUpdated_onboardingFlowReturnsValue() = runTest {
        appPreferencesDataStore.setOnboardingCompleted(isCompleted = true)

        val isCompleted = appPreferencesDataStore.isOnboardingCompleted.first()

        assertTrue(isCompleted)
    }

    @Test
    fun setDarkModeEnabled_valueUpdated_darkModeFlowReturnsValue() = runTest {
        appPreferencesDataStore.setDarkModeEnabled(isEnabled = true)

        val isDarkModeEnabled = appPreferencesDataStore.isDarkModeEnabled.first()

        assertTrue(isDarkModeEnabled)
    }

    @Test
    fun clearPreferences_valuesCleared_flowsReturnDefaults() = runTest {
        appPreferencesDataStore.setOnboardingCompleted(isCompleted = true)
        appPreferencesDataStore.setDarkModeEnabled(isEnabled = true)

        appPreferencesDataStore.clearPreferences()

        val isCompleted = appPreferencesDataStore.isOnboardingCompleted.first()
        val isDarkModeEnabled = appPreferencesDataStore.isDarkModeEnabled.first()
        assertFalse(isCompleted)
        assertFalse(isDarkModeEnabled)
    }
}
