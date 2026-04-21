package com.dev4all.mobile.core.domain.model

/**
 * Bid lifecycle states — matches backend Dev4All.Domain.Enums.BidStatus.
 *
 * Pending(0) → Accepted(1) / Rejected(2)
 */
enum class BidStatus {
    Pending,
    Accepted,
    Rejected,
}
