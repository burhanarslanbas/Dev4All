package com.dev4all.mobile.feature.auth.register

import androidx.compose.ui.test.assertIsDisplayed
import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithTag
import androidx.compose.ui.test.onNodeWithText
import androidx.compose.ui.test.performClick
import androidx.compose.ui.test.performTextInput
import com.dev4all.mobile.core.domain.model.AuthToken
import com.dev4all.mobile.core.domain.model.UserRole
import com.dev4all.mobile.core.domain.result.Result
import org.junit.Assert.assertEquals
import org.junit.Rule
import org.junit.Test

class RegisterScreenTest {

    @get:Rule
    val composeRule = createComposeRule()

    @Test
    fun registerContent_displaysRoleCardsAndFormFields() {
        composeRule.setContent {
            RegisterContent(
                fullName = "",
                email = "",
                password = "",
                selectedRole = null,
                uiState = RegisterUiState.Idle,
                onEvent = {},
            )
        }

        composeRule.onNodeWithText("Kayıt Ol").assertIsDisplayed()
        composeRule.onNodeWithText("Müşteri").assertIsDisplayed()
        composeRule.onNodeWithText("Geliştirici").assertIsDisplayed()
        composeRule.onNodeWithText("Ad Soyad").assertIsDisplayed()
        composeRule.onNodeWithText("E-posta").assertIsDisplayed()
        composeRule.onNodeWithText("Şifre").assertIsDisplayed()
    }

    @Test
    fun registerContent_customerCardClick_sendsRoleSelectedEvent() {
        var receivedEvent: RegisterEvent? = null

        composeRule.setContent {
            RegisterContent(
                fullName = "",
                email = "",
                password = "",
                selectedRole = null,
                uiState = RegisterUiState.Idle,
                onEvent = { receivedEvent = it },
            )
        }

        composeRule.onNodeWithTag(CUSTOMER_ROLE_CARD_TAG).performClick()

        assertEquals(RegisterEvent.RoleSelected(RegisterRole.Customer), receivedEvent)
    }

    @Test
    fun registerScreen_successFlow_invokesOnRegisterSuccess() {
        var successCallCount = 0
        val viewModel = RegisterViewModel { _, _, _, _ ->
            Result.Success(
                AuthToken(
                    token = "token",
                    expiresAt = "2026-04-01T00:00:00Z",
                    email = "test@mail.com",
                    role = UserRole.Customer,
                )
            )
        }

        composeRule.setContent {
            RegisterScreen(
                viewModel = viewModel,
                onRegisterSuccess = { successCallCount++ },
                onNavigateToLogin = {},
                onBackClick = {},
            )
        }

        composeRule.onNodeWithTag(FULL_NAME_FIELD_TAG).performTextInput("Test User")
        composeRule.onNodeWithTag(EMAIL_FIELD_TAG).performTextInput("test@mail.com")
        composeRule.onNodeWithTag(PASSWORD_FIELD_TAG).performTextInput("Password1")
        composeRule.onNodeWithTag(CUSTOMER_ROLE_CARD_TAG).performClick()
        composeRule.onNodeWithTag(REGISTER_BUTTON_TAG).performClick()

        composeRule.waitUntil(timeoutMillis = 5_000L) { successCallCount == 1 }
    }
}
