package com.dev4all.mobile.core.domain.usecase.bid

import com.dev4all.mobile.core.domain.repository.BidRepository
import com.dev4all.mobile.core.domain.result.Result

class AcceptBidUseCase(private val repo: BidRepository) {
    suspend operator fun invoke(bidId: String): Result<Unit> =
        repo.acceptBid(bidId)
}
