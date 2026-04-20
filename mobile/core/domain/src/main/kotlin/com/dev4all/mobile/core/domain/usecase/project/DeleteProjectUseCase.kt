package com.dev4all.mobile.core.domain.usecase.project

import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.result.Result

class DeleteProjectUseCase(private val repo: ProjectRepository) {
    suspend operator fun invoke(id: String): Result<Unit> =
        repo.deleteProject(id)
}
