package com.dev4all.mobile.core.domain.result

import com.dev4all.mobile.core.domain.exception.AppException

sealed class Result<out T> {
    data class Success<T>(val data: T) : Result<T>()

    data class Error(val exception: AppException) : Result<Nothing>()
}
