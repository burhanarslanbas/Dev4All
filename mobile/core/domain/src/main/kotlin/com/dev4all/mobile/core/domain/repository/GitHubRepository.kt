package com.dev4all.mobile.core.domain.repository

import com.dev4all.mobile.core.domain.model.GitHubLog
import com.dev4all.mobile.core.domain.result.Result

/**
 * Repository abstraction for GitHub integration operations.
 * Aligned with backend IGitHubLogReadRepository + IGitHubLogWriteRepository.
 */
interface GitHubRepository {

    /** Links a GitHub repo to an ongoing project — developer only. */
    suspend fun linkRepo(projectId: String, repoUrl: String, branch: String = "main"): Result<Unit>

    /** Gets commit activity logs for a project. */
    suspend fun getActivityLogs(projectId: String): Result<List<GitHubLog>>
}
