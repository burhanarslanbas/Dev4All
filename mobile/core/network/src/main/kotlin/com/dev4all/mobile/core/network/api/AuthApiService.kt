package com.dev4all.mobile.core.network.api

import com.dev4all.mobile.core.network.dto.auth.CurrentUserResponse
import com.dev4all.mobile.core.network.dto.auth.LoginRequest
import com.dev4all.mobile.core.network.dto.auth.LoginResponse
import com.dev4all.mobile.core.network.dto.auth.RegisterRequest
import com.dev4all.mobile.core.network.dto.auth.RegisterResponse
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface AuthApiService {
    @POST("api/v1/auth/register")
    suspend fun register(@Body request: RegisterRequest): RegisterResponse

    @POST("api/v1/auth/login")
    suspend fun login(@Body request: LoginRequest): LoginResponse

    @GET("api/v1/auth/me")
    suspend fun getCurrentUser(): CurrentUserResponse
}
