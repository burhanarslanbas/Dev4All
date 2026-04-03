package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme

enum class Dev4AllButtonVariant {
    Primary,
    Secondary,
    Outline,
    Danger
}

@Composable
fun Dev4AllButton(
    text: String,
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
    enabled: Boolean = true,
    variant: Dev4AllButtonVariant = Dev4AllButtonVariant.Primary
) {
    when (variant) {
        Dev4AllButtonVariant.Primary -> Button(
            onClick = onClick,
            modifier = modifier,
            enabled = enabled
        ) {
            Text(text = text)
        }

        Dev4AllButtonVariant.Secondary -> Button(
            onClick = onClick,
            modifier = modifier,
            enabled = enabled,
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.secondary,
                contentColor = MaterialTheme.colorScheme.onSecondary
            )
        ) {
            Text(text = text)
        }

        Dev4AllButtonVariant.Outline -> OutlinedButton(
            onClick = onClick,
            modifier = modifier,
            enabled = enabled
        ) {
            Text(text = text)
        }

        Dev4AllButtonVariant.Danger -> Button(
            onClick = onClick,
            modifier = modifier,
            enabled = enabled,
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.error,
                contentColor = MaterialTheme.colorScheme.onError
            )
        ) {
            Text(text = text)
        }
    }
}

@Preview(showBackground = true)
@Composable
private fun Dev4AllButtonPreview() {
    Dev4AllTheme {
        Column(
            modifier = Modifier.padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            Dev4AllButton(
                text = "Primary",
                onClick = {},
                modifier = Modifier.fillMaxWidth()
            )
            Dev4AllButton(
                text = "Secondary",
                onClick = {},
                variant = Dev4AllButtonVariant.Secondary,
                modifier = Modifier.fillMaxWidth()
            )
            Dev4AllButton(
                text = "Outline",
                onClick = {},
                variant = Dev4AllButtonVariant.Outline,
                modifier = Modifier.fillMaxWidth()
            )
            Dev4AllButton(
                text = "Danger",
                onClick = {},
                variant = Dev4AllButtonVariant.Danger,
                modifier = Modifier.fillMaxWidth()
            )
        }
    }
}
