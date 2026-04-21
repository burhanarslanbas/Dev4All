package com.dev4all.mobile.core.domain.model

/**
 * Domain model for a project contract.
 * Field names match backend Dev4All.Domain.Entities.Contract.
 */
data class Contract(
    val id: String,
    val projectId: String,
    val content: String,
    val revisionNumber: Int = 1,
    val lastRevisedById: String = "",
    val status: ContractStatus = ContractStatus.Draft,
    val isCustomerApproved: Boolean = false,
    val isDeveloperApproved: Boolean = false,
    val customerApprovedAt: String? = null,
    val developerApprovedAt: String? = null,
    val createdDate: String = "",
    val updatedDate: String = "",
    // UI-only enrichment
    val projectTitle: String? = null,
)
