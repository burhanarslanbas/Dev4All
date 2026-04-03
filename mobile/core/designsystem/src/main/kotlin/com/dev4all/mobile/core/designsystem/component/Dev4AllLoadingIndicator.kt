package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme

@Composable
fun Dev4AllLoadingIndicator(
    modifier: Modifier = Modifier,
    message: String? = null
) {
    Column(
        modifier = modifier,
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.spacedBy(12.dp)
    ) {
        CircularProgressIndicator()
        if (message != null) {
            Text(
                text = message,
                style = MaterialTheme.typography.bodyMedium
            )
        }
    }
}

@Preview(showBackground = true)
@Composable
private fun Dev4AllLoadingIndicatorPreview() {
    Dev4AllTheme {
        Dev4AllLoadingIndicator(
            modifier = Modifier.padding(24.dp),
            message = "Loading projects..."
        )
    }
}
