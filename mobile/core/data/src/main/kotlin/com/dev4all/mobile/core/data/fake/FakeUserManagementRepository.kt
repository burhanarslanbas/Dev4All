package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.repository.UserManagementRepository
import com.dev4all.mobile.core.domain.result.Result
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FakeUserManagementRepository @Inject constructor() : UserManagementRepository {

    override suspend fun getAllUsers(): Result<List<User>> {
        val user = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        if (user.role != UserRole.Admin) {
            return Result.Error(AppException.Forbidden("Bu işlem için admin yetkisi gereklidir."))
        }
        return Result.Success(FakeData.users.toList())
    }

    override suspend fun changeUserRole(userId: String, newRole: UserRole): Result<Unit> {
        val admin = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        if (admin.role != UserRole.Admin) {
            return Result.Error(AppException.Forbidden("Bu işlem için admin yetkisi gereklidir."))
        }

        val idx = FakeData.users.indexOfFirst { it.id == userId }
        if (idx == -1) return Result.Error(AppException.NotFound("Kullanıcı bulunamadı."))

        FakeData.users[idx] = FakeData.users[idx].copy(role = newRole)
        return Result.Success(Unit)
    }

    override suspend fun suspendUser(userId: String): Result<Unit> {
        val admin = FakeData.currentUser ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        if (admin.role != UserRole.Admin) {
            return Result.Error(AppException.Forbidden("Bu işlem için admin yetkisi gereklidir."))
        }
        if (userId == admin.id) {
            return Result.Error(AppException.Unknown("Kendi hesabınızı askıya alamazsınız."))
        }

        val idx = FakeData.users.indexOfFirst { it.id == userId }
        if (idx == -1) return Result.Error(AppException.NotFound("Kullanıcı bulunamadı."))

        FakeData.users.removeAt(idx)
        return Result.Success(Unit)
    }
}
