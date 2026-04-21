package com.dev4all.mobile.core.domain.model

/**
 * Domain model for a project listing.
 * Field names match backend Dev4All.Domain.Entities.Project (via BaseEntity).
 */
data class Project(
    val id: String,
    val customerId: String,
    val assignedDeveloperId: String? = null,
    val title: String,
    val description: String,
    val budget: Double,
    val deadline: String,
    val bidEndDate: String,
    val technologies: String? = null,
    val status: ProjectStatus = ProjectStatus.Open,
    val createdDate: String = "",
    val updatedDate: String = "",
    val deletedDate: String? = null,
    // UI-computed fields
    val bidCount: Int = 0,
)
