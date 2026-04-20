package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.model.ProjectStatus
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.repository.ProjectRepository
import com.dev4all.mobile.core.domain.result.Result
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FakeProjectRepository @Inject constructor() : ProjectRepository {

    override suspend fun getOpenProjects(page: Int, pageSize: Int): Result<List<Project>> {
        val open = FakeData.projects
            .filter { it.status == ProjectStatus.Open && it.deletedDate == null }
            .sortedByDescending { it.createdDate }
        val paged = open.drop((page - 1) * pageSize).take(pageSize)
        return Result.Success(paged)
    }

    override suspend fun getProjectById(id: String): Result<Project> {
        val project = FakeData.projects.find { it.id == id && it.deletedDate == null }
            ?: return Result.Error(AppException.NotFound("Proje bulunamadı."))
        return Result.Success(project)
    }

    override suspend fun getMyProjects(): Result<List<Project>> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val mine = FakeData.projects
            .filter { it.customerId == user.id && it.deletedDate == null }
            .sortedByDescending { it.createdDate }
        return Result.Success(mine)
    }

    override suspend fun getAssignedProjects(): Result<List<Project>> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val assigned = FakeData.projects
            .filter { it.assignedDeveloperId == user.id && it.deletedDate == null }
            .sortedByDescending { it.createdDate }
        return Result.Success(assigned)
    }

    override suspend fun getAllProjects(): Result<List<Project>> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        if (user.role != UserRole.Admin) {
            return Result.Error(AppException.Forbidden("Bu işlem için admin yetkisi gereklidir."))
        }
        return Result.Success(FakeData.projects.sortedByDescending { it.createdDate })
    }

    override suspend fun createProject(
        title: String, description: String, budget: Double,
        deadline: String, bidEndDate: String, technologies: String?,
    ): Result<Project> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        if (user.role != UserRole.Customer) {
            return Result.Error(AppException.Forbidden("Yalnızca müşteri rolü proje oluşturabilir."))
        }
        if (title.length < 3 || title.length > 100) return Result.Error(AppException.Unknown("Başlık 3-100 karakter olmalıdır."))
        if (description.length < 10 || description.length > 2000) return Result.Error(AppException.Unknown("Açıklama 10-2000 karakter olmalıdır."))
        if (budget <= 0) return Result.Error(AppException.Unknown("Bütçe sıfırdan büyük olmalıdır."))

        val project = Project(
            id = FakeData.generateId(), customerId = user.id, title = title,
            description = description, budget = budget, deadline = deadline,
            bidEndDate = bidEndDate, technologies = technologies,
            status = ProjectStatus.Open, createdDate = FakeData.now(), updatedDate = FakeData.now(),
        )
        FakeData.projects.add(project)
        return Result.Success(project)
    }

    override suspend fun updateProject(
        id: String, title: String?, description: String?, budget: Double?, technologies: String?,
    ): Result<Project> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val idx = FakeData.projects.indexOfFirst { it.id == id && it.customerId == user.id }
        if (idx == -1) return Result.Error(AppException.NotFound("Proje bulunamadı veya yetkiniz yok."))

        val old = FakeData.projects[idx]
        if (budget != null && old.bidCount > 0 && budget < old.budget) {
            return Result.Error(AppException.Unknown("Teklif geldikten sonra bütçe düşürülemez."))
        }

        val updated = old.copy(
            title = title ?: old.title,
            description = description ?: old.description,
            budget = budget ?: old.budget,
            technologies = technologies ?: old.technologies,
            updatedDate = FakeData.now(),
        )
        FakeData.projects[idx] = updated
        return Result.Success(updated)
    }

    override suspend fun deleteProject(id: String): Result<Unit> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val idx = FakeData.projects.indexOfFirst { it.id == id && it.customerId == user.id }
        if (idx == -1) return Result.Error(AppException.NotFound("Proje bulunamadı veya yetkiniz yok."))

        val project = FakeData.projects[idx]
        if (project.status == ProjectStatus.Ongoing || project.status == ProjectStatus.Completed) {
            return Result.Error(AppException.Unknown("Devam eden veya tamamlanan projeler silinemez."))
        }
        FakeData.projects[idx] = project.copy(deletedDate = FakeData.now())
        return Result.Success(Unit)
    }
}
