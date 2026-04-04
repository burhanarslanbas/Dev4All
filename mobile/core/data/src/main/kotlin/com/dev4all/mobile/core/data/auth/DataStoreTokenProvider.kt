package com.dev4all.mobile.core.data.auth

import com.dev4all.mobile.core.datastore.TokenDataStore
import com.dev4all.mobile.core.network.auth.TokenProvider
import kotlinx.coroutines.flow.first
import javax.inject.Inject

class DataStoreTokenProvider @Inject constructor(
    private val tokenDataStore: TokenDataStore,
) : TokenProvider {
    override suspend fun getAccessToken(): String? = tokenDataStore.getToken().first()
}
