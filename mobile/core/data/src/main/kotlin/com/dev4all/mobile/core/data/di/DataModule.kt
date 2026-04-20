package com.dev4all.mobile.core.data.di

import com.dev4all.mobile.core.data.auth.DataStoreTokenProvider
import com.dev4all.mobile.core.data.fake.FakeAuthRepository
import com.dev4all.mobile.core.data.fake.FakeBidRepository
import com.dev4all.mobile.core.data.fake.FakeContractRepository
import com.dev4all.mobile.core.data.fake.FakeGitHubRepository
import com.dev4all.mobile.core.data.fake.FakeProjectRepository
import com.dev4all.mobile.core.data.fake.FakeUserManagementRepository
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.repository.BidRepository
import com.dev4all.mobile.core.domain.repository.ContractRepository
import com.dev4all.mobile.core.domain.repository.GitHubRepository
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.repository.UserManagementRepository
import com.dev4all.mobile.core.network.auth.TokenProvider
import dagger.Binds
import dagger.Module
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

/**
 * Hilt DI module — binds repository interfaces to Fake implementations.
 *
 * TODO: API'ye geçişte FakeXxxRepository → XxxRepositoryImpl olarak değiştir.
 */
@Module
@InstallIn(SingletonComponent::class)
abstract class DataModule {

    @Binds
    @Singleton
    abstract fun bindAuthRepository(impl: FakeAuthRepository): AuthRepository

    @Binds
    @Singleton
    abstract fun bindProjectRepository(impl: FakeProjectRepository): ProjectRepository

    @Binds
    @Singleton
    abstract fun bindBidRepository(impl: FakeBidRepository): BidRepository

    @Binds
    @Singleton
    abstract fun bindContractRepository(impl: FakeContractRepository): ContractRepository

    @Binds
    @Singleton
    abstract fun bindGitHubRepository(impl: FakeGitHubRepository): GitHubRepository

    @Binds
    @Singleton
    abstract fun bindUserManagementRepository(impl: FakeUserManagementRepository): UserManagementRepository

    @Binds
    @Singleton
    abstract fun bindTokenProvider(impl: DataStoreTokenProvider): TokenProvider
}
