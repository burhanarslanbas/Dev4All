package com.dev4all.mobile.core.domain.repository

import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.result.Result

/**
 * Repository abstraction for project operations.
 * Aligned with backend IProjectReadRepository + IProjectWriteRepository.
 */
interface ProjectRepository {

    /** Lists open projects with pagination — backend: GetOpenProjectsAsync. */
    suspend fun getOpenProjects(page: Int = 1, pageSize: Int = 20): Result<List<Project>>

    /** Gets a single project by ID — backend: GetByIdAsync. */
    suspend fun getProjectById(id: String): Result<Project>

    /** Lists projects created by the current customer — backend: GetByCustomerIdAsync. */
    suspend fun getMyProjects(): Result<List<Project>>

    /** Lists projects assigned to the current developer. */
    suspend fun getAssignedProjects(): Result<List<Project>>

    /** Lists all projects (admin only). */
    suspend fun getAllProjects(): Result<List<Project>>

    /** Creates a new project — only Customer role. */
    suspend fun createProject(
        title: String,
        description: String,
        budget: Double,
        deadline: String,
        bidEndDate: String,
        technologies: String?,
    ): Result<Project>

    /** Updates an existing project owned by the current customer. */
    suspend fun updateProject(
        id: String,
        title: String? = null,
        description: String? = null,
        budget: Double? = null,
        technologies: String? = null,
    ): Result<Project>

    /** Soft-deletes a project — sets deletedDate. */
    suspend fun deleteProject(id: String): Result<Unit>
}
