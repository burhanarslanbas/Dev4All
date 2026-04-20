package com.dev4all.mobile.core.domain.model

/**
 * Contract lifecycle states — matches backend Dev4All.Domain.Enums.ContractStatus.
 *
 * Draft(0) → UnderReview(1) → BothApproved(2) / Cancelled(3)
 */
enum class ContractStatus {
    Draft,
    UnderReview,
    BothApproved,
    Cancelled,
}
