package com.dev4all.mobile.core.domain.usecase.contract

import com.dev4all.mobile.core.domain.model.Contract
import com.dev4all.mobile.core.domain.repository.ContractRepository
import com.dev4all.mobile.core.domain.result.Result

class GetContractUseCase(private val repo: ContractRepository) {
    suspend operator fun invoke(projectId: String): Result<Contract> =
        repo.getByProjectId(projectId)
}
