package com.dev4all.mobile.core.domain.usecase.admin

import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.repository.UserManagementRepository
import com.dev4all.mobile.core.domain.result.Result

class GetAllUsersUseCase(private val repo: UserManagementRepository) {
    suspend operator fun invoke(): Result<List<User>> =
        repo.getAllUsers()
}
