package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ErrorOutline
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme

@Composable
fun Dev4AllErrorState(
    title: String,
    description: String,
    onRetryClick: () -> Unit,
    modifier: Modifier = Modifier,
    retryButtonText: String = "Retry",
    icon: ImageVector = Icons.Default.ErrorOutline
) {
    Column(
        modifier = modifier,
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        Icon(
            imageVector = icon,
            contentDescription = null,
            tint = MaterialTheme.colorScheme.error
        )
        Text(
            text = title,
            style = MaterialTheme.typography.titleMedium
        )
        Text(
            text = description,
            style = MaterialTheme.typography.bodyMedium,
            color = MaterialTheme.colorScheme.onSurfaceVariant
        )
        Dev4AllButton(
            text = retryButtonText,
            onClick = onRetryClick,
            variant = Dev4AllButtonVariant.Danger
        )
    }
}

@Preview(showBackground = true)
@Composable
private fun Dev4AllErrorStatePreview() {
    Dev4AllTheme {
        Dev4AllErrorState(
            title = "Something went wrong",
            description = "Please check your connection and try again.",
            onRetryClick = {},
            modifier = Modifier.padding(24.dp)
        )
    }
}
