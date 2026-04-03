package com.dev4all.mobile.core.datastore

import kotlinx.coroutines.flow.first
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNull
import org.junit.Assert.assertTrue
import org.junit.Test

class UserSessionDataStoreTest {

    private val preferencesDataStore = TestPreferencesDataStore()
    private val userSessionDataStore = UserSessionDataStore(dataStore = preferencesDataStore)

    @Test
    fun saveSession_sessionSaved_sessionFlowReturnsValues() = runTest {
        userSessionDataStore.saveSession(
            userId = "user-1",
            email = "user@dev4all.com",
            role = "Developer",
        )

        val session = userSessionDataStore.session.first()

        assertEquals("user-1", session.userId)
        assertEquals("user@dev4all.com", session.email)
        assertEquals("Developer", session.role)
        assertTrue(session.isLoggedIn)
    }

    @Test
    fun clearSession_sessionCleared_sessionFlowReturnsDefaultValues() = runTest {
        userSessionDataStore.saveSession(
            userId = "user-1",
            email = "user@dev4all.com",
            role = "Developer",
        )

        userSessionDataStore.clearSession()

        val session = userSessionDataStore.session.first()
        assertNull(session.userId)
        assertNull(session.email)
        assertNull(session.role)
        assertFalse(session.isLoggedIn)
    }
}
