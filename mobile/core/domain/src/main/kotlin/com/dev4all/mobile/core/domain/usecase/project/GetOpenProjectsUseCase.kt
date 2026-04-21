package com.dev4all.mobile.core.domain.usecase.project

import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.result.Result

class GetOpenProjectsUseCase(private val repo: ProjectRepository) {
    suspend operator fun invoke(page: Int = 1, pageSize: Int = 20): Result<List<Project>> =
        repo.getOpenProjects(page, pageSize)
}
