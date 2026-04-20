package com.dev4all.mobile.core.domain.model

/**
 * Domain model for a contract revision snapshot.
 * Field names match backend Dev4All.Domain.Entities.ContractRevision.
 */
data class ContractRevision(
    val id: String,
    val contractId: String,
    val revisedById: String,
    val contentSnapshot: String,
    val revisionNumber: Int,
    val revisionNote: String? = null,
    val createdDate: String = "",
)
