package com.dev4all.mobile.core.domain.model

/**
 * Domain model for a bid (teklif).
 * Field names match backend Dev4All.Domain.Entities.Bid.
 */
data class Bid(
    val id: String,
    val projectId: String,
    val developerId: String,
    val bidAmount: Double,
    val proposalNote: String,
    val status: BidStatus = BidStatus.Pending,
    val isAccepted: Boolean = false,
    val createdDate: String = "",
    val updatedDate: String = "",
    // UI-only enrichment
    val developerName: String = "",
    val projectTitle: String = "",
)
