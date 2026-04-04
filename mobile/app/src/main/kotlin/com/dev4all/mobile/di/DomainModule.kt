package com.dev4all.mobile.di

import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.usecase.auth.LoginUseCase
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DomainModule {

    @Provides
    @Singleton
    fun provideLoginUseCase(authRepository: AuthRepository): LoginUseCase =
        LoginUseCase(authRepository = authRepository)
}
