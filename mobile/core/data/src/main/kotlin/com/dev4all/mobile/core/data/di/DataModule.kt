package com.dev4all.mobile.core.data.di

import com.dev4all.mobile.core.data.auth.DataStoreTokenProvider
import com.dev4all.mobile.core.data.repository.AuthRepositoryImpl
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.network.auth.TokenProvider
import dagger.Binds
import dagger.Module
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
abstract class DataModule {

    @Binds
    @Singleton
    abstract fun bindAuthRepository(impl: AuthRepositoryImpl): AuthRepository

    @Binds
    @Singleton
    abstract fun bindTokenProvider(impl: DataStoreTokenProvider): TokenProvider
}
