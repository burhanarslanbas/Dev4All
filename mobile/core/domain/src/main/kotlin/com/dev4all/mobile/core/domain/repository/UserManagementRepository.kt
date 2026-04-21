package com.dev4all.mobile.core.domain.repository

import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.result.Result

/**
 * Repository abstraction for admin user management operations.
 */
interface UserManagementRepository {

    /** Lists all platform users — admin only. */
    suspend fun getAllUsers(): Result<List<User>>

    /** Changes a user's role — admin only. */
    suspend fun changeUserRole(userId: String, newRole: UserRole): Result<Unit>

    /** Suspends a user account — admin only. */
    suspend fun suspendUser(userId: String): Result<Unit>
}
