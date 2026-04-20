package com.dev4all.mobile.di

import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.repository.BidRepository
import com.dev4all.mobile.core.domain.repository.ContractRepository
import com.dev4all.mobile.core.domain.repository.GitHubRepository
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.repository.UserManagementRepository
import com.dev4all.mobile.core.domain.usecase.admin.GetAllProjectsUseCase
import com.dev4all.mobile.core.domain.usecase.admin.GetAllUsersUseCase
import com.dev4all.mobile.core.domain.usecase.auth.GetCurrentUserUseCase
import com.dev4all.mobile.core.domain.usecase.auth.LoginUseCase
import com.dev4all.mobile.core.domain.usecase.auth.LogoutUseCase
import com.dev4all.mobile.core.domain.usecase.auth.RegisterUseCase
import com.dev4all.mobile.core.domain.usecase.bid.AcceptBidUseCase
import com.dev4all.mobile.core.domain.usecase.bid.GetMyBidsUseCase
import com.dev4all.mobile.core.domain.usecase.bid.GetProjectBidsUseCase
import com.dev4all.mobile.core.domain.usecase.bid.SubmitBidUseCase
import com.dev4all.mobile.core.domain.usecase.contract.ApproveContractUseCase
import com.dev4all.mobile.core.domain.usecase.contract.CancelContractUseCase
import com.dev4all.mobile.core.domain.usecase.contract.GetContractUseCase
import com.dev4all.mobile.core.domain.usecase.contract.ReviseContractUseCase
import com.dev4all.mobile.core.domain.usecase.github.GetActivityLogsUseCase
import com.dev4all.mobile.core.domain.usecase.github.LinkRepoUseCase
import com.dev4all.mobile.core.domain.usecase.project.CreateProjectUseCase
import com.dev4all.mobile.core.domain.usecase.project.DeleteProjectUseCase
import com.dev4all.mobile.core.domain.usecase.project.GetAssignedProjectsUseCase
import com.dev4all.mobile.core.domain.usecase.project.GetMyProjectsUseCase
import com.dev4all.mobile.core.domain.usecase.project.GetOpenProjectsUseCase
import com.dev4all.mobile.core.domain.usecase.project.GetProjectByIdUseCase
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object DomainModule {

    // ── Auth ──────────────────────────────────────────────────────────
    @Provides @Singleton
    fun provideLoginUseCase(r: AuthRepository) = LoginUseCase(authRepository = r)

    @Provides @Singleton
    fun provideRegisterUseCase(r: AuthRepository) = RegisterUseCase(authRepository = r)

    @Provides @Singleton
    fun provideGetCurrentUserUseCase(r: AuthRepository) = GetCurrentUserUseCase(r)

    @Provides @Singleton
    fun provideLogoutUseCase(r: AuthRepository) = LogoutUseCase(r)

    // ── Project ───────────────────────────────────────────────────────
    @Provides @Singleton
    fun provideGetOpenProjectsUseCase(r: ProjectRepository) = GetOpenProjectsUseCase(r)

    @Provides @Singleton
    fun provideGetProjectByIdUseCase(r: ProjectRepository) = GetProjectByIdUseCase(r)

    @Provides @Singleton
    fun provideGetMyProjectsUseCase(r: ProjectRepository) = GetMyProjectsUseCase(r)

    @Provides @Singleton
    fun provideGetAssignedProjectsUseCase(r: ProjectRepository) = GetAssignedProjectsUseCase(r)

    @Provides @Singleton
    fun provideCreateProjectUseCase(r: ProjectRepository) = CreateProjectUseCase(r)

    @Provides @Singleton
    fun provideDeleteProjectUseCase(r: ProjectRepository) = DeleteProjectUseCase(r)

    // ── Bid ───────────────────────────────────────────────────────────
    @Provides @Singleton
    fun provideGetProjectBidsUseCase(r: BidRepository) = GetProjectBidsUseCase(r)

    @Provides @Singleton
    fun provideGetMyBidsUseCase(r: BidRepository) = GetMyBidsUseCase(r)

    @Provides @Singleton
    fun provideSubmitBidUseCase(r: BidRepository) = SubmitBidUseCase(r)

    @Provides @Singleton
    fun provideAcceptBidUseCase(r: BidRepository) = AcceptBidUseCase(r)

    // ── Contract ──────────────────────────────────────────────────────
    @Provides @Singleton
    fun provideGetContractUseCase(r: ContractRepository) = GetContractUseCase(r)

    @Provides @Singleton
    fun provideReviseContractUseCase(r: ContractRepository) = ReviseContractUseCase(r)

    @Provides @Singleton
    fun provideApproveContractUseCase(r: ContractRepository) = ApproveContractUseCase(r)

    @Provides @Singleton
    fun provideCancelContractUseCase(r: ContractRepository) = CancelContractUseCase(r)

    // ── GitHub ────────────────────────────────────────────────────────
    @Provides @Singleton
    fun provideLinkRepoUseCase(r: GitHubRepository) = LinkRepoUseCase(r)

    @Provides @Singleton
    fun provideGetActivityLogsUseCase(r: GitHubRepository) = GetActivityLogsUseCase(r)

    // ── Admin ─────────────────────────────────────────────────────────
    @Provides @Singleton
    fun provideGetAllUsersUseCase(r: UserManagementRepository) = GetAllUsersUseCase(r)

    @Provides @Singleton
    fun provideGetAllProjectsUseCase(r: ProjectRepository) = GetAllProjectsUseCase(r)
}
