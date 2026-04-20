package com.dev4all.mobile.core.domain.usecase.contract

import com.dev4all.mobile.core.domain.model.Contract
import com.dev4all.mobile.core.domain.repository.ContractRepository
import com.dev4all.mobile.core.domain.result.Result

class ReviseContractUseCase(private val repo: ContractRepository) {
    suspend operator fun invoke(projectId: String, content: String, note: String?): Result<Contract> =
        repo.reviseContract(projectId, content, note)
}
