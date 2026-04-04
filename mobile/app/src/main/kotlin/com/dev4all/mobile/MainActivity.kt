package com.dev4all.mobile

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.rememberSaveable
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import com.dev4all.mobile.feature.auth.login.LoginRoute
import com.dev4all.mobile.feature.auth.login.LoginScreen
import com.dev4all.mobile.feature.auth.login.LoginUiState
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            Dev4AllTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    var destination by rememberSaveable { mutableStateOf(AppDestination.Login) }
                    when (destination) {
                        AppDestination.Login -> LoginRoute(
                            onNavigateByRole = { effect ->
                                destination = when (effect.role) {
                                    UserRole.Customer -> AppDestination.CustomerHome
                                    UserRole.Developer -> AppDestination.DeveloperHome
                                    UserRole.Admin -> AppDestination.AdminHome
                                }
                            },
                            modifier = Modifier.padding(innerPadding),
                        )

                        AppDestination.CustomerHome -> RoleHomeScreen("Customer Home", Modifier.padding(innerPadding))
                        AppDestination.DeveloperHome -> RoleHomeScreen("Developer Home", Modifier.padding(innerPadding))
                        AppDestination.AdminHome -> RoleHomeScreen("Admin Home", Modifier.padding(innerPadding))
                    }
                }
            }
        }
    }
}

private enum class AppDestination {
    Login,
    CustomerHome,
    DeveloperHome,
    AdminHome,
}

@Composable
private fun RoleHomeScreen(
    title: String,
    modifier: Modifier = Modifier,
) {
    Box(
        modifier = modifier.fillMaxSize(),
        contentAlignment = Alignment.Center,
    ) {
        Text(text = title)
    }
}

@Composable
private fun LoginScreenPreviewContent(modifier: Modifier = Modifier) {
    LoginScreen(
        uiState = LoginUiState(),
        onEvent = {},
        modifier = modifier,
    )
}

@Preview(showBackground = true)
@Composable
fun LoginScreenPreview() {
    Dev4AllTheme {
        LoginScreenPreviewContent()
    }
}
