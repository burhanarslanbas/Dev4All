package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.exception.AppException
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.repository.AuthRepository
import com.dev4all.mobile.core.domain.result.Result
import javax.inject.Inject
import javax.inject.Singleton

/**
 * Fake AuthRepository that works purely with in-memory [FakeData].
 * Replaces network-dependent AuthRepositoryImpl for offline-first development.
 */
@Singleton
class FakeAuthRepository @Inject constructor() : AuthRepository {

    override suspend fun login(email: String, password: String): Result<AuthToken> {
        val expectedPassword = FakeData.passwords[email]
            ?: return Result.Error(AppException.Unauthorized("Geçersiz e-posta veya şifre."))

        if (expectedPassword != password) {
            return Result.Error(AppException.Unauthorized("Geçersiz e-posta veya şifre."))
        }

        val user = FakeData.users.find { it.email == email }
            ?: return Result.Error(AppException.NotFound("Kullanıcı bulunamadı."))

        FakeData.currentUser = user

        return Result.Success(
            AuthToken(
                token = "fake-jwt-token-${user.id}-${System.currentTimeMillis()}",
                expiresAt = "2026-12-31T23:59:59Z",
                email = user.email,
                role = user.role,
            )
        )
    }

    override suspend fun register(
        fullName: String,
        email: String,
        password: String,
        role: String,
    ): Result<AuthToken> {
        // Validation — FRD §8.1
        if (fullName.length < 2 || fullName.length > 100) {
            return Result.Error(AppException.Unknown("Ad 2-100 karakter arası olmalıdır."))
        }
        if (!email.contains("@")) {
            return Result.Error(AppException.Unknown("Geçerli bir e-posta adresi giriniz."))
        }
        if (password.length < 8) {
            return Result.Error(AppException.Unknown("Şifre en az 8 karakter olmalıdır."))
        }
        if (password.none { it.isUpperCase() }) {
            return Result.Error(AppException.Unknown("Şifre en az 1 büyük harf içermelidir."))
        }
        if (password.none { it.isDigit() }) {
            return Result.Error(AppException.Unknown("Şifre en az 1 rakam içermelidir."))
        }
        if (FakeData.users.any { it.email == email }) {
            return Result.Error(AppException.Unknown("Bu e-posta adresi zaten kayıtlı."))
        }

        val userRole = when (role.lowercase()) {
            "customer" -> UserRole.Customer
            "developer" -> UserRole.Developer
            else -> return Result.Error(AppException.Forbidden("Geçersiz rol. Sadece 'Customer' veya 'Developer' seçilebilir."))
        }

        val newUser = User(
            id = FakeData.generateId(),
            name = fullName,
            email = email,
            role = userRole,
        )

        FakeData.users.add(newUser)
        FakeData.passwords[email] = password
        FakeData.currentUser = newUser

        return Result.Success(
            AuthToken(
                token = "fake-jwt-token-${newUser.id}-${System.currentTimeMillis()}",
                expiresAt = "2026-12-31T23:59:59Z",
                email = newUser.email,
                role = newUser.role,
            )
        )
    }

    override suspend fun getCurrentUser(): Result<User> {
        val user = FakeData.currentUser
            ?: return Result.Error(AppException.Unauthorized("Oturum açılmamış."))
        return Result.Success(user)
    }

    override suspend fun logout(): Result<Unit> {
        FakeData.currentUser = null
        return Result.Success(Unit)
    }
}
