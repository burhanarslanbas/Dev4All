package com.dev4all.mobile.core.domain.repository

import com.dev4all.mobile.core.domain.model.Contract
import com.dev4all.mobile.core.domain.model.ContractRevision
import com.dev4all.mobile.core.domain.result.Result

/**
 * Repository abstraction for contract (sözleşme) operations.
 * Aligned with backend IContractReadRepository + IContractWriteRepository.
 */
interface ContractRepository {

    /** Gets contract by project ID — backend: GetByProjectIdAsync. */
    suspend fun getByProjectId(projectId: String): Result<Contract>

    /** Revises contract content — resets other party's approval. */
    suspend fun reviseContract(projectId: String, content: String, note: String?): Result<Contract>

    /** Approves contract — if both approve → BothApproved + project → Ongoing. */
    suspend fun approveContract(projectId: String): Result<Contract>

    /** Cancels contract — contract + project → Cancelled. */
    suspend fun cancelContract(projectId: String): Result<Unit>

    /** Gets revision history — backend: GetByIdWithRevisionsAsync. */
    suspend fun getRevisions(projectId: String): Result<List<ContractRevision>>
}
