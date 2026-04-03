package com.dev4all.mobile.core.datastore.di

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.preferencesDataStoreFile
import androidx.datastore.preferences.core.PreferenceDataStoreFactory
import com.dev4all.mobile.core.datastore.AppPreferencesDataStore
import com.dev4all.mobile.core.datastore.TokenDataStore
import com.dev4all.mobile.core.datastore.UserSessionDataStore
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.android.qualifiers.ApplicationContext
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DataStoreModule {

    @Provides
    @Singleton
    fun providePreferencesDataStore(
        @ApplicationContext context: Context,
    ): DataStore<Preferences> = PreferenceDataStoreFactory.create {
        context.preferencesDataStoreFile(DATASTORE_NAME)
    }

    @Provides
    @Singleton
    fun provideTokenDataStore(
        dataStore: DataStore<Preferences>,
    ): TokenDataStore = TokenDataStore(dataStore = dataStore)

    @Provides
    @Singleton
    fun provideUserSessionDataStore(
        dataStore: DataStore<Preferences>,
    ): UserSessionDataStore = UserSessionDataStore(dataStore = dataStore)

    @Provides
    @Singleton
    fun provideAppPreferencesDataStore(
        dataStore: DataStore<Preferences>,
    ): AppPreferencesDataStore = AppPreferencesDataStore(dataStore = dataStore)

    private const val DATASTORE_NAME = "dev4all_preferences"
}
