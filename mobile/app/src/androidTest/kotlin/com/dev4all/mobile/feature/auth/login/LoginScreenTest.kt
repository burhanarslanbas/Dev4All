package com.dev4all.mobile.feature.auth.login

import androidx.compose.ui.test.assertExists
import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithText
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme
import org.junit.Rule
import org.junit.Test

class LoginScreenTest {

    @get:Rule
    val composeRule = createComposeRule()

    @Test
    fun loginScreen_displaysFieldsAndButton() {
        composeRule.setContent {
            Dev4AllTheme {
                LoginScreen(
                    uiState = LoginUiState(),
                    onEvent = {},
                )
            }
        }

        composeRule.onNodeWithText("Email").assertExists()
        composeRule.onNodeWithText("Password").assertExists()
        composeRule.onNodeWithText("Login").assertExists()
    }
}
