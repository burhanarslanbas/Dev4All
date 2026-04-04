package com.dev4all.mobile.feature.auth.register

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.testTag
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle

const val CUSTOMER_ROLE_CARD_TAG = "register_customer_role_card"
const val DEVELOPER_ROLE_CARD_TAG = "register_developer_role_card"
const val FULL_NAME_FIELD_TAG = "register_full_name_field"
const val EMAIL_FIELD_TAG = "register_email_field"
const val PASSWORD_FIELD_TAG = "register_password_field"
const val REGISTER_BUTTON_TAG = "register_submit_button"

@Composable
fun RegisterScreen(
    viewModel: RegisterViewModel,
    onRegisterSuccess: () -> Unit,
    onNavigateToLogin: () -> Unit,
    onBackClick: () -> Unit,
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    val fullName by viewModel.fullName.collectAsStateWithLifecycle()
    val email by viewModel.email.collectAsStateWithLifecycle()
    val password by viewModel.password.collectAsStateWithLifecycle()
    val selectedRole by viewModel.selectedRole.collectAsStateWithLifecycle()

    LaunchedEffect(uiState) {
        if (uiState is RegisterUiState.Success) {
            onRegisterSuccess()
        }
    }

    RegisterContent(
        fullName = fullName,
        email = email,
        password = password,
        selectedRole = selectedRole,
        uiState = uiState,
        onEvent = viewModel::onEvent,
        onNavigateToLogin = onNavigateToLogin,
        onBackClick = onBackClick,
    )
}

@Composable
internal fun RegisterContent(
    fullName: String,
    email: String,
    password: String,
    selectedRole: RegisterRole?,
    uiState: RegisterUiState,
    onEvent: (RegisterEvent) -> Unit,
    onNavigateToLogin: () -> Unit = {},
    onBackClick: () -> Unit = {},
) {
    val validationErrors = (uiState as? RegisterUiState.ValidationError)?.errors.orEmpty()

    Column(
        modifier = Modifier
            .fillMaxWidth()
            .padding(16.dp),
        verticalArrangement = Arrangement.spacedBy(12.dp),
    ) {
        Text(
            text = "Kayıt Ol",
            style = MaterialTheme.typography.headlineSmall,
            modifier = Modifier.clickable(onClick = onBackClick),
        )

        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.spacedBy(12.dp),
        ) {
            RoleCard(
                title = "Müşteri",
                selected = selectedRole == RegisterRole.Customer,
                modifier = Modifier
                    .weight(1f)
                    .testTag(CUSTOMER_ROLE_CARD_TAG),
                onClick = { onEvent(RegisterEvent.RoleSelected(RegisterRole.Customer)) },
            )

            RoleCard(
                title = "Geliştirici",
                selected = selectedRole == RegisterRole.Developer,
                modifier = Modifier
                    .weight(1f)
                    .testTag(DEVELOPER_ROLE_CARD_TAG),
                onClick = { onEvent(RegisterEvent.RoleSelected(RegisterRole.Developer)) },
            )
        }

        validationErrors["role"]?.let { message ->
            Text(
                text = message,
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodySmall,
            )
        }

        OutlinedTextField(
            value = fullName,
            onValueChange = { onEvent(RegisterEvent.FullNameChanged(it)) },
            modifier = Modifier
                .fillMaxWidth()
                .testTag(FULL_NAME_FIELD_TAG),
            label = { Text("Ad Soyad") },
            isError = validationErrors.containsKey("name"),
            singleLine = true,
        )
        validationErrors["name"]?.let { message ->
            Text(text = message, color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        OutlinedTextField(
            value = email,
            onValueChange = { onEvent(RegisterEvent.EmailChanged(it)) },
            modifier = Modifier
                .fillMaxWidth()
                .testTag(EMAIL_FIELD_TAG),
            label = { Text("E-posta") },
            isError = validationErrors.containsKey("email"),
            singleLine = true,
        )
        validationErrors["email"]?.let { message ->
            Text(text = message, color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        OutlinedTextField(
            value = password,
            onValueChange = { onEvent(RegisterEvent.PasswordChanged(it)) },
            modifier = Modifier
                .fillMaxWidth()
                .testTag(PASSWORD_FIELD_TAG),
            label = { Text("Şifre") },
            visualTransformation = PasswordVisualTransformation(),
            isError = validationErrors.containsKey("password"),
            singleLine = true,
        )
        validationErrors["password"]?.let { message ->
            Text(text = message, color = MaterialTheme.colorScheme.error, style = MaterialTheme.typography.bodySmall)
        }

        if (uiState is RegisterUiState.Error) {
            Text(
                text = uiState.message,
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodySmall,
            )
        }

        Button(
            onClick = { onEvent(RegisterEvent.RegisterClicked) },
            modifier = Modifier
                .fillMaxWidth()
                .testTag(REGISTER_BUTTON_TAG),
            enabled = uiState !is RegisterUiState.Loading,
        ) {
            if (uiState is RegisterUiState.Loading) {
                CircularProgressIndicator(
                    modifier = Modifier.size(18.dp),
                    strokeWidth = 2.dp,
                )
                Spacer(modifier = Modifier.size(8.dp))
            }
            Text(text = "Kayıt Ol")
        }

        Text(
            text = "Zaten hesabın var mı? Giriş Yap",
            modifier = Modifier.clickable(onClick = onNavigateToLogin),
            style = MaterialTheme.typography.bodyMedium,
        )
    }
}

@Composable
private fun RoleCard(
    title: String,
    selected: Boolean,
    modifier: Modifier = Modifier,
    onClick: () -> Unit,
) {
    val borderColor = if (selected) {
        MaterialTheme.colorScheme.primary
    } else {
        MaterialTheme.colorScheme.outline
    }

    Card(
        modifier = modifier.clickable(onClick = onClick),
        border = BorderStroke(1.dp, borderColor),
        colors = CardDefaults.cardColors(
            containerColor = if (selected) {
                MaterialTheme.colorScheme.primaryContainer
            } else {
                MaterialTheme.colorScheme.surface
            }
        ),
    ) {
        Text(
            text = title,
            modifier = Modifier.padding(16.dp),
            style = MaterialTheme.typography.bodyLarge,
        )
    }
}
