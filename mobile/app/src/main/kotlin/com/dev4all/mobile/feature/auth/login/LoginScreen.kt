package com.dev4all.mobile.feature.auth.login

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.dev4all.mobile.core.designsystem.component.Dev4AllButton
import com.dev4all.mobile.core.designsystem.component.Dev4AllLoadingIndicator
import com.dev4all.mobile.core.designsystem.component.Dev4AllTextField

@Composable
fun LoginRoute(
    viewModel: LoginViewModel = hiltViewModel(),
    onNavigateByRole: (LoginSideEffect.NavigateByRole) -> Unit,
    modifier: Modifier = Modifier,
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()

    LaunchedEffect(viewModel) {
        viewModel.sideEffects.collect { effect ->
            if (effect is LoginSideEffect.NavigateByRole) {
                onNavigateByRole(effect)
            }
        }
    }

    LoginScreen(
        uiState = uiState,
        onEvent = viewModel::onEvent,
        modifier = modifier,
    )
}

@Composable
fun LoginScreen(
    uiState: LoginUiState,
    onEvent: (LoginEvent) -> Unit,
    modifier: Modifier = Modifier,
) {
    Column(
        modifier = modifier
            .fillMaxSize()
            .padding(24.dp),
        verticalArrangement = Arrangement.Center,
    ) {
        Text(
            text = "Welcome back",
            style = MaterialTheme.typography.headlineSmall,
        )
        Spacer(modifier = Modifier.height(16.dp))
        Dev4AllTextField(
            value = uiState.email,
            onValueChange = { onEvent(LoginEvent.EmailChanged(it)) },
            label = "Email",
            supportingText = uiState.emailError,
            isError = uiState.emailError != null,
            enabled = !uiState.isLoading,
            modifier = Modifier.fillMaxWidth(),
        )
        Spacer(modifier = Modifier.height(12.dp))
        Dev4AllTextField(
            value = uiState.password,
            onValueChange = { onEvent(LoginEvent.PasswordChanged(it)) },
            label = "Password",
            supportingText = uiState.passwordError,
            isError = uiState.passwordError != null,
            isPassword = true,
            enabled = !uiState.isLoading,
            modifier = Modifier.fillMaxWidth(),
        )
        if (uiState.errorMessage != null) {
            Spacer(modifier = Modifier.height(12.dp))
            Text(
                text = uiState.errorMessage,
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodyMedium,
            )
        }
        Spacer(modifier = Modifier.height(16.dp))
        Dev4AllButton(
            text = "Login",
            onClick = { onEvent(LoginEvent.LoginClicked) },
            enabled = !uiState.isLoading,
            modifier = Modifier.fillMaxWidth(),
        )
        if (uiState.isLoading) {
            Spacer(modifier = Modifier.height(16.dp))
            Dev4AllLoadingIndicator(
                modifier = Modifier.fillMaxWidth(),
                message = "Signing in...",
            )
        }
    }
}
