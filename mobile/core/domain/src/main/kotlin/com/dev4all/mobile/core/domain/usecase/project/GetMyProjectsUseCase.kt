package com.dev4all.mobile.core.domain.usecase.project

import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.result.Result

class GetMyProjectsUseCase(private val repo: ProjectRepository) {
    suspend operator fun invoke(): Result<List<Project>> =
        repo.getMyProjects()
}
