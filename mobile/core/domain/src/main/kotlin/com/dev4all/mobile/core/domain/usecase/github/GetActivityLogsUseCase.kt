package com.dev4all.mobile.core.domain.usecase.github

import com.dev4all.mobile.core.domain.model.GitHubLog
import com.dev4all.mobile.core.domain.repository.GitHubRepository
import com.dev4all.mobile.core.domain.result.Result

class GetActivityLogsUseCase(private val repo: GitHubRepository) {
    suspend operator fun invoke(projectId: String): Result<List<GitHubLog>> =
        repo.getActivityLogs(projectId)
}
