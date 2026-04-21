package com.dev4all.mobile.core.domain.usecase.bid

import com.dev4all.mobile.core.domain.model.Bid
import com.dev4all.mobile.core.domain.repository.BidRepository
import com.dev4all.mobile.core.domain.result.Result

class GetProjectBidsUseCase(private val repo: BidRepository) {
    suspend operator fun invoke(projectId: String): Result<List<Bid>> =
        repo.getByProjectId(projectId)
}
