package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.Bid
import com.dev4all.mobile.core.domain.model.BidStatus
import com.dev4all.mobile.core.domain.model.Contract
import com.dev4all.mobile.core.domain.model.ContractStatus
import com.dev4all.mobile.core.domain.model.ProjectStatus
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.repository.BidRepository
import com.dev4all.mobile.core.domain.result.Result
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FakeBidRepository @Inject constructor() : BidRepository {

    override suspend fun getByProjectId(projectId: String): Result<List<Bid>> {
        val bids = FakeData.bids.filter { it.projectId == projectId }
        return Result.Success(bids)
    }

    override suspend fun getMyBids(): Result<List<Bid>> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val bids = FakeData.bids
            .filter { it.developerId == user.id }
            .sortedByDescending { it.createdDate }
        return Result.Success(bids)
    }

    override suspend fun getByDeveloperAndProject(projectId: String): Result<Bid?> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        val bid = FakeData.bids.find { it.projectId == projectId && it.developerId == user.id }
        return Result.Success(bid)
    }

    override suspend fun submitBid(projectId: String, amount: Double, note: String): Result<Bid> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))

        if (user.role != UserRole.Developer) {
            return Result.Error(AppException.Forbidden("Yalnızca geliştirici rolü teklif verebilir."))
        }

        val project = FakeData.projects.find { it.id == projectId }
            ?: return Result.Error(AppException.NotFound("Proje bulunamadı."))

        if (project.status != ProjectStatus.Open) {
            return Result.Error(AppException.Unknown("Yalnızca açık projelere teklif verilebilir."))
        }

        if (FakeData.bids.any { it.projectId == projectId && it.developerId == user.id }) {
            return Result.Error(AppException.Unknown("Bu ilana zaten teklif verdiniz."))
        }

        if (amount <= 0) return Result.Error(AppException.Unknown("Teklif tutarı sıfırdan büyük olmalıdır."))
        if (note.length < 10 || note.length > 1000) return Result.Error(AppException.Unknown("Öneri notu 10-1000 karakter olmalıdır."))

        val bid = Bid(
            id = FakeData.generateId(), projectId = projectId, developerId = user.id,
            bidAmount = amount, proposalNote = note, status = BidStatus.Pending,
            createdDate = FakeData.now(), updatedDate = FakeData.now(),
            developerName = user.name, projectTitle = project.title,
        )
        FakeData.bids.add(bid)

        val pIdx = FakeData.projects.indexOfFirst { it.id == projectId }
        if (pIdx != -1) {
            FakeData.projects[pIdx] = FakeData.projects[pIdx].copy(bidCount = FakeData.projects[pIdx].bidCount + 1)
        }

        return Result.Success(bid)
    }

    override suspend fun updateBid(bidId: String, amount: Double, note: String): Result<Bid> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))

        val idx = FakeData.bids.indexOfFirst { it.id == bidId && it.developerId == user.id }
        if (idx == -1) return Result.Error(AppException.NotFound("Teklif bulunamadı veya yetkiniz yok."))

        val old = FakeData.bids[idx]
        if (old.status != BidStatus.Pending) return Result.Error(AppException.Unknown("Yalnızca bekleyen teklifler güncellenebilir."))
        if (amount <= 0) return Result.Error(AppException.Unknown("Teklif tutarı sıfırdan büyük olmalıdır."))
        if (note.length < 10 || note.length > 1000) return Result.Error(AppException.Unknown("Öneri notu 10-1000 karakter olmalıdır."))

        val updated = old.copy(bidAmount = amount, proposalNote = note, updatedDate = FakeData.now())
        FakeData.bids[idx] = updated
        return Result.Success(updated)
    }

    override suspend fun acceptBid(bidId: String): Result<Unit> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))

        val bidIdx = FakeData.bids.indexOfFirst { it.id == bidId }
        if (bidIdx == -1) return Result.Error(AppException.NotFound("Teklif bulunamadı."))

        val bid = FakeData.bids[bidIdx]
        val projectIdx = FakeData.projects.indexOfFirst { it.id == bid.projectId }
        if (projectIdx == -1) return Result.Error(AppException.NotFound("İlan bulunamadı."))

        val project = FakeData.projects[projectIdx]

        if (project.customerId != user.id) {
            return Result.Error(AppException.Forbidden("Yalnızca ilan sahibi teklif kabul edebilir."))
        }
        if (project.status != ProjectStatus.Open) {
            return Result.Error(AppException.Unknown("Yalnızca açık projelerdeki teklifler kabul edilebilir."))
        }

        // 1. Accept this bid
        FakeData.bids[bidIdx] = bid.copy(status = BidStatus.Accepted, isAccepted = true, updatedDate = FakeData.now())

        // 2. Reject all other pending bids
        FakeData.bids.forEachIndexed { i, b ->
            if (b.projectId == bid.projectId && b.id != bidId && b.status == BidStatus.Pending) {
                FakeData.bids[i] = b.copy(status = BidStatus.Rejected, updatedDate = FakeData.now())
            }
        }

        // 3. Project → AwaitingContract + assign developer
        FakeData.projects[projectIdx] = project.copy(
            status = ProjectStatus.AwaitingContract,
            assignedDeveloperId = bid.developerId,
            updatedDate = FakeData.now(),
        )

        // 4. Auto-create Contract draft
        val developer = FakeData.users.find { it.id == bid.developerId }
        val contract = Contract(
            id = FakeData.generateId(), projectId = project.id,
            content = buildString {
                appendLine("1. TARAFLAR")
                appendLine("Bu sözleşme ${user.name} (Müşteri) ile ${developer?.name ?: "Geliştirici"} (Geliştirici) arasında akdedilmiştir.")
                appendLine()
                appendLine("2. İŞİN KONUSU")
                appendLine(project.title)
                appendLine()
                appendLine("3. BÜTÇE")
                appendLine("Toplam proje bedeli: ₺${"%,.0f".format(bid.bidAmount)}")
                appendLine()
                appendLine("4. TESLİM TARİHİ")
                appendLine(project.deadline)
                if (!project.technologies.isNullOrBlank()) {
                    appendLine()
                    appendLine("5. TEKNOLOJİLER")
                    appendLine(project.technologies)
                }
            },
            revisionNumber = 1, lastRevisedById = "system",
            status = ContractStatus.Draft,
            createdDate = FakeData.now(), updatedDate = FakeData.now(),
            projectTitle = project.title,
        )
        FakeData.contracts.add(contract)

        return Result.Success(Unit)
    }
}
