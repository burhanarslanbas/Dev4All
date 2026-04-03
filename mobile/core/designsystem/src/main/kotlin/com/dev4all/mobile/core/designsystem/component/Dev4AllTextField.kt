package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme

@Composable
fun Dev4AllTextField(
    value: String,
    onValueChange: (String) -> Unit,
    modifier: Modifier = Modifier,
    label: String,
    placeholder: String? = null,
    enabled: Boolean = true,
    isError: Boolean = false,
    supportingText: String? = null,
    singleLine: Boolean = true
) {
    OutlinedTextField(
        value = value,
        onValueChange = onValueChange,
        modifier = modifier,
        enabled = enabled,
        label = { Text(text = label) },
        placeholder = placeholder?.let { { Text(text = it) } },
        isError = isError,
        supportingText = supportingText?.let { { Text(text = it) } },
        singleLine = singleLine
    )
}

@Preview(showBackground = true)
@Composable
private fun Dev4AllTextFieldPreview() {
    Dev4AllTheme {
        var email by remember { mutableStateOf("") }

        Column(
            modifier = Modifier.padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            Dev4AllTextField(
                value = email,
                onValueChange = { email = it },
                label = "Email",
                placeholder = "name@company.com",
                modifier = Modifier.fillMaxWidth()
            )
            Dev4AllTextField(
                value = "",
                onValueChange = {},
                label = "Password",
                isError = true,
                supportingText = "Password is required",
                modifier = Modifier.fillMaxWidth()
            )
        }
    }
}
