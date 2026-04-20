package com.dev4all.mobile.core.domain.usecase.project

import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.result.Result

class CreateProjectUseCase(private val repo: ProjectRepository) {
    suspend operator fun invoke(
        title: String,
        description: String,
        budget: Double,
        deadline: String,
        bidEndDate: String,
        technologies: String?,
    ): Result<Project> = repo.createProject(title, description, budget, deadline, bidEndDate, technologies)
}
