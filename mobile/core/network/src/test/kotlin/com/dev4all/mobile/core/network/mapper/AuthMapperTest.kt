package com.dev4all.mobile.core.network.mapper

import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.network.dto.auth.CurrentUserResponse
import com.dev4all.mobile.core.network.dto.auth.LoginResponse
import org.junit.Assert.assertEquals
import org.junit.Test

class AuthMapperTest {

    @Test
    fun toDomain_loginResponse_mapsAuthTokenFields() {
        val response = LoginResponse(
            token = "jwt-token",
            expiresAt = "2026-04-03T12:00:00Z",
            email = "user@mail.com",
            role = "Developer",
        )

        val result = response.toDomain()

        assertEquals("jwt-token", result.token)
        assertEquals("2026-04-03T12:00:00Z", result.expiresAt)
        assertEquals("user@mail.com", result.email)
        assertEquals(UserRole.Developer, result.role)
    }

    @Test
    fun toDomain_currentUserResponse_mapsUserFields() {
        val response = CurrentUserResponse(
            userId = "6c6f620f-0e61-4a0a-b6f2-3f6b9d4c22de",
            email = "customer@mail.com",
            role = "Customer",
        )

        val result = response.toDomain(name = "Customer Name")

        assertEquals("6c6f620f-0e61-4a0a-b6f2-3f6b9d4c22de", result.id)
        assertEquals("Customer Name", result.name)
        assertEquals("customer@mail.com", result.email)
        assertEquals(UserRole.Customer, result.role)
    }

    @Test
    fun toDomain_unknownRole_defaultsToCustomer() {
        val response = LoginResponse(
            token = "jwt-token",
            expiresAt = "2026-04-03T12:00:00Z",
            email = "user@mail.com",
            role = "UnexpectedRole",
        )

        val result = response.toDomain()

        assertEquals(UserRole.Customer, result.role)
    }
}
