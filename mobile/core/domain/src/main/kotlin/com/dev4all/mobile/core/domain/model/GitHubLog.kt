package com.dev4all.mobile.core.domain.model

/**
 * Domain model for a GitHub activity log entry (commit).
 * Field names match backend Dev4All.Domain.Entities.GitHubLog.
 */
data class GitHubLog(
    val id: String,
    val projectId: String,
    val repoUrl: String,
    val branch: String = "main",
    val commitHash: String,
    val commitMessage: String,
    val authorName: String,
    val pushedAt: String,
)
