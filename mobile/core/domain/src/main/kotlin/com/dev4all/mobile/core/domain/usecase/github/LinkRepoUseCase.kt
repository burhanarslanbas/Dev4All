package com.dev4all.mobile.core.domain.usecase.github

import com.dev4all.mobile.core.domain.repository.GitHubRepository
import com.dev4all.mobile.core.domain.result.Result

class LinkRepoUseCase(private val repo: GitHubRepository) {
    suspend operator fun invoke(projectId: String, repoUrl: String, branch: String = "main"): Result<Unit> =
        repo.linkRepo(projectId, repoUrl, branch)
}
