package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.GitHubLog
import com.dev4all.mobile.core.domain.model.ProjectStatus
import com.dev4all.mobile.core.domain.repository.GitHubRepository
import com.dev4all.mobile.core.domain.result.Result
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FakeGitHubRepository @Inject constructor() : GitHubRepository {

    override suspend fun linkRepo(projectId: String, repoUrl: String, branch: String): Result<Unit> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val project = FakeData.projects.find { it.id == projectId }
            ?: return Result.Error(AppException.NotFound("Proje bulunamadı."))

        if (project.assignedDeveloperId != user.id) {
            return Result.Error(AppException.Forbidden("Yalnızca atanmış geliştirici repo bağlayabilir."))
        }
        if (project.status != ProjectStatus.Ongoing) {
            return Result.Error(AppException.Unknown("Yalnızca devam eden projelere repo bağlanabilir."))
        }
        if (!repoUrl.contains("github.com")) {
            return Result.Error(AppException.Unknown("Geçerli bir GitHub URL'si giriniz."))
        }

        val log = GitHubLog(
            id = FakeData.generateId(), projectId = projectId, repoUrl = repoUrl,
            branch = branch, commitHash = "0000000000000000000000000000000000000000",
            commitMessage = "Repository bağlandı: $repoUrl ($branch)",
            authorName = user.name, pushedAt = FakeData.now(),
        )
        FakeData.gitHubLogs.add(log)
        return Result.Success(Unit)
    }

    override suspend fun getActivityLogs(projectId: String): Result<List<GitHubLog>> {
        val logs = FakeData.gitHubLogs
            .filter { it.projectId == projectId }
            .sortedByDescending { it.pushedAt }
        return Result.Success(logs)
    }
}
