package com.dev4all.mobile.core.datastore

import kotlinx.coroutines.flow.first
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNull
import org.junit.Test

class TokenDataStoreTest {

    private val preferencesDataStore = TestPreferencesDataStore()
    private val tokenDataStore = TokenDataStore(dataStore = preferencesDataStore)

    @Test
    fun saveAccessToken_tokenSaved_tokenFlowReturnsValue() = runTest {
        tokenDataStore.saveAccessToken("access-token")

        val token = tokenDataStore.accessToken.first()

        assertEquals("access-token", token)
    }

    @Test
    fun clearAccessToken_tokenCleared_tokenFlowReturnsNull() = runTest {
        tokenDataStore.saveAccessToken("access-token")

        tokenDataStore.clearAccessToken()

        val token = tokenDataStore.accessToken.first()
        assertNull(token)
    }
}
