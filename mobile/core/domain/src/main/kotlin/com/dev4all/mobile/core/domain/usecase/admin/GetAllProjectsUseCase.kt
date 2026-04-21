package com.dev4all.mobile.core.domain.usecase.admin

import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.result.Result

class GetAllProjectsUseCase(private val repo: ProjectRepository) {
    suspend operator fun invoke(): Result<List<Project>> =
        repo.getAllProjects()
}
