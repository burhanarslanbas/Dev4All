package com.dev4all.mobile.core.domain.repository

import com.dev4all.mobile.core.domain.model.Bid
import com.dev4all.mobile.core.domain.result.Result

/**
 * Repository abstraction for bid (teklif) operations.
 * Aligned with backend IBidReadRepository + IBidWriteRepository.
 */
interface BidRepository {

    /** Lists all bids for a project — backend: GetByProjectIdAsync. */
    suspend fun getByProjectId(projectId: String): Result<List<Bid>>

    /** Lists current developer's bids — backend: GetByDeveloperIdAsync. */
    suspend fun getMyBids(): Result<List<Bid>>

    /** Checks if developer already bid on this project — backend: GetByDeveloperAndProjectAsync. */
    suspend fun getByDeveloperAndProject(projectId: String): Result<Bid?>

    /** Submits a new bid — only Developer role, one per project. */
    suspend fun submitBid(projectId: String, amount: Double, note: String): Result<Bid>

    /** Updates an existing bid while project is still Open. */
    suspend fun updateBid(bidId: String, amount: Double, note: String): Result<Bid>

    /** Accepts a bid — transitions project to AwaitingContract, creates Contract draft. */
    suspend fun acceptBid(bidId: String): Result<Unit>
}
