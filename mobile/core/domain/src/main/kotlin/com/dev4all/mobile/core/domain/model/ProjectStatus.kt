package com.dev4all.mobile.core.domain.model

/**
 * Project lifecycle states — matches backend Dev4All.Domain.Enums.ProjectStatus.
 *
 * Open(0) → AwaitingContract(1) → Ongoing(2) → Completed(3)
 *                                              / Expired(4)
 *                                              / Cancelled(5)
 */
enum class ProjectStatus {
    Open,
    AwaitingContract,
    Ongoing,
    Completed,
    Expired,
    Cancelled,
}
