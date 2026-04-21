package com.dev4all.mobile.core.datastore.di

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.preferencesDataStoreFile
import androidx.datastore.preferences.core.PreferenceDataStoreFactory
import com.dev4all.mobile.core.datastore.TokenDataStore
import com.dev4all.mobile.core.datastore.UserSessionDataStore
import com.dev4all.mobile.core.datastore.impl.PreferencesTokenDataStore
import com.dev4all.mobile.core.datastore.impl.PreferencesUserSessionDataStore
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DataStoreModule {

    @Provides
    @Singleton
    fun providePreferencesDataStore(
        @ApplicationContext context: Context,
    ): DataStore<Preferences> = PreferenceDataStoreFactory.create(
        scope = CoroutineScope(Dispatchers.IO + SupervisorJob()),
        produceFile = { context.preferencesDataStoreFile(PREFERENCES_FILE_NAME) },
    )

    @Provides
    @Singleton
    fun provideTokenDataStore(dataStore: DataStore<Preferences>): TokenDataStore =
        PreferencesTokenDataStore(dataStore = dataStore)

    @Provides
    @Singleton
    fun provideUserSessionDataStore(dataStore: DataStore<Preferences>): UserSessionDataStore =
        PreferencesUserSessionDataStore(dataStore = dataStore)

    private const val PREFERENCES_FILE_NAME = "dev4all_mobile_preferences"
}
