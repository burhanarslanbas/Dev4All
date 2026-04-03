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

enum class StatusBadgeType {
    Open,
    AwaitingContract,
    Ongoing,
    Completed,
    Expired,
    Cancelled
}

@Composable
fun StatusBadge(
    status: StatusBadgeType,
    modifier: Modifier = Modifier
) {
    val statusLabel = when (status) {
        StatusBadgeType.Open -> "Open"
        StatusBadgeType.AwaitingContract -> "Awaiting Contract"
        StatusBadgeType.Ongoing -> "Ongoing"
        StatusBadgeType.Completed -> "Completed"
        StatusBadgeType.Expired -> "Expired"
        StatusBadgeType.Cancelled -> "Cancelled"
    }

    val statusColor = when (status) {
        StatusBadgeType.Open -> Dev4AllTheme.statusColors.open
        StatusBadgeType.AwaitingContract -> Dev4AllTheme.statusColors.awaitingContract
        StatusBadgeType.Ongoing -> Dev4AllTheme.statusColors.ongoing
        StatusBadgeType.Completed -> Dev4AllTheme.statusColors.completed
        StatusBadgeType.Expired -> Dev4AllTheme.statusColors.expired
        StatusBadgeType.Cancelled -> Dev4AllTheme.statusColors.cancelled
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
            StatusBadge(status = StatusBadgeType.Open)
            StatusBadge(status = StatusBadgeType.Ongoing)
            StatusBadge(status = StatusBadgeType.Completed)
            StatusBadge(status = StatusBadgeType.Cancelled)
        }
    }
}
