package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.Contract
import com.dev4all.mobile.core.domain.model.ContractRevision
import com.dev4all.mobile.core.domain.model.ContractStatus
import com.dev4all.mobile.core.domain.model.ProjectStatus
import com.dev4all.mobile.core.domain.repository.ContractRepository
import com.dev4all.mobile.core.domain.result.Result
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FakeContractRepository @Inject constructor() : ContractRepository {

    override suspend fun getByProjectId(projectId: String): Result<Contract> {
        val contract = FakeData.contracts.find { it.projectId == projectId }
            ?: return Result.Error(AppException.NotFound("Sözleşme bulunamadı."))
        return Result.Success(contract)
    }

    override suspend fun reviseContract(projectId: String, content: String, note: String?): Result<Contract> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val idx = FakeData.contracts.indexOfFirst { it.projectId == projectId }
        if (idx == -1) return Result.Error(AppException.NotFound("Sözleşme bulunamadı."))

        val old = FakeData.contracts[idx]

        if (old.status == ContractStatus.BothApproved || old.status == ContractStatus.Cancelled) {
            return Result.Error(AppException.Unknown("Onaylanmış veya iptal edilmiş sözleşme düzenlenemez."))
        }
        if (content.length < 50) return Result.Error(AppException.Unknown("Sözleşme metni en az 50 karakter olmalıdır."))

        val snapshot = ContractRevision(
            id = FakeData.generateId(), contractId = old.id, revisedById = user.id,
            contentSnapshot = old.content, revisionNumber = old.revisionNumber,
            revisionNote = note, createdDate = FakeData.now(),
        )
        FakeData.contractRevisions.add(snapshot)

        val project = FakeData.projects.find { it.id == projectId }
        val isCustomer = project?.customerId == user.id

        val updated = old.copy(
            content = content,
            revisionNumber = old.revisionNumber + 1,
            lastRevisedById = user.id,
            status = ContractStatus.UnderReview,
            isDeveloperApproved = if (isCustomer) false else old.isDeveloperApproved,
            isCustomerApproved = if (!isCustomer) false else old.isCustomerApproved,
            updatedDate = FakeData.now(),
        )
        FakeData.contracts[idx] = updated
        return Result.Success(updated)
    }

    override suspend fun approveContract(projectId: String): Result<Contract> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val idx = FakeData.contracts.indexOfFirst { it.projectId == projectId }
        if (idx == -1) return Result.Error(AppException.NotFound("Sözleşme bulunamadı."))

        val old = FakeData.contracts[idx]
        if (old.status == ContractStatus.BothApproved || old.status == ContractStatus.Cancelled) {
            return Result.Error(AppException.Unknown("Bu sözleşme tekrar onaylanamaz."))
        }

        val project = FakeData.projects.find { it.id == projectId }
        val isCustomer = project?.customerId == user.id
        val now = FakeData.now()

        val newCustomerApproved = if (isCustomer) true else old.isCustomerApproved
        val newDeveloperApproved = if (!isCustomer) true else old.isDeveloperApproved
        val bothApproved = newCustomerApproved && newDeveloperApproved

        val updated = old.copy(
            isCustomerApproved = newCustomerApproved,
            isDeveloperApproved = newDeveloperApproved,
            customerApprovedAt = if (isCustomer) now else old.customerApprovedAt,
            developerApprovedAt = if (!isCustomer) now else old.developerApprovedAt,
            status = if (bothApproved) ContractStatus.BothApproved else old.status,
            updatedDate = now,
        )
        FakeData.contracts[idx] = updated

        if (bothApproved && project != null) {
            val pIdx = FakeData.projects.indexOfFirst { it.id == projectId }
            if (pIdx != -1) {
                FakeData.projects[pIdx] = FakeData.projects[pIdx].copy(
                    status = ProjectStatus.Ongoing, updatedDate = now,
                )
            }
        }

        return Result.Success(updated)
    }

    override suspend fun cancelContract(projectId: String): Result<Unit> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val idx = FakeData.contracts.indexOfFirst { it.projectId == projectId }
        if (idx == -1) return Result.Error(AppException.NotFound("Sözleşme bulunamadı."))

        val old = FakeData.contracts[idx]
        if (old.status == ContractStatus.BothApproved || old.status == ContractStatus.Cancelled) {
            return Result.Error(AppException.Unknown("Onaylanmış veya zaten iptal edilmiş sözleşme iptal edilemez."))
        }

        val now = FakeData.now()
        FakeData.contracts[idx] = old.copy(status = ContractStatus.Cancelled, updatedDate = now)

        val pIdx = FakeData.projects.indexOfFirst { it.id == projectId }
        if (pIdx != -1) {
            FakeData.projects[pIdx] = FakeData.projects[pIdx].copy(
                status = ProjectStatus.Cancelled, updatedDate = now,
            )
        }

        return Result.Success(Unit)
    }

    override suspend fun getRevisions(projectId: String): Result<List<ContractRevision>> {
        val contract = FakeData.contracts.find { it.projectId == projectId }
            ?: return Result.Error(AppException.NotFound("Sözleşme bulunamadı."))
        val revisions = FakeData.contractRevisions
            .filter { it.contractId == contract.id }
            .sortedByDescending { it.revisionNumber }
        return Result.Success(revisions)
    }
}
