package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme

enum class ProjectStatus {
    Open,
    AwaitingContract,
    Ongoing,
    Completed,
    Expired,
    Cancelled
}

@Composable
fun StatusBadge(
    status: ProjectStatus,
    modifier: Modifier = Modifier
) {
    val statusLabel = when (status) {
        ProjectStatus.Open -> "Open"
        ProjectStatus.AwaitingContract -> "Awaiting Contract"
        ProjectStatus.Ongoing -> "Ongoing"
        ProjectStatus.Completed -> "Completed"
        ProjectStatus.Expired -> "Expired"
        ProjectStatus.Cancelled -> "Cancelled"
    }

    val statusColor = when (status) {
        ProjectStatus.Open -> Dev4AllTheme.statusColors.open
        ProjectStatus.AwaitingContract -> Dev4AllTheme.statusColors.awaitingContract
        ProjectStatus.Ongoing -> Dev4AllTheme.statusColors.ongoing
        ProjectStatus.Completed -> Dev4AllTheme.statusColors.completed
        ProjectStatus.Expired -> Dev4AllTheme.statusColors.expired
        ProjectStatus.Cancelled -> Dev4AllTheme.statusColors.cancelled
    }

    Surface(
        modifier = modifier,
        shape = MaterialTheme.shapes.medium,
        color = statusColor.copy(alpha = 0.12f),
        contentColor = statusColor
    ) {
        Text(
            text = statusLabel,
            style = MaterialTheme.typography.labelLarge,
            modifier = Modifier.padding(horizontal = 12.dp, vertical = 6.dp)
        )
    }
}

@Preview(showBackground = true)
@Composable
private fun StatusBadgePreview() {
    Dev4AllTheme {
        Row(
            modifier = Modifier.padding(16.dp),
            horizontalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            StatusBadge(status = ProjectStatus.Open)
            StatusBadge(status = ProjectStatus.Ongoing)
            StatusBadge(status = ProjectStatus.Completed)
            StatusBadge(status = ProjectStatus.Cancelled)
        }
    }
}
