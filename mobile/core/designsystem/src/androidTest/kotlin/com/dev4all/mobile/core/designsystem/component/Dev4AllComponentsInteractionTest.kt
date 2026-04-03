package com.dev4all.mobile.core.designsystem.component

import androidx.compose.ui.test.assertExists
import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithText
import androidx.compose.ui.test.performClick
import com.dev4all.mobile.core.designsystem.theme.Dev4AllTheme
import org.junit.Assert.assertEquals
import org.junit.Rule
import org.junit.Test

class Dev4AllComponentsInteractionTest {

    @get:Rule
    val composeRule = createComposeRule()

    @Test
    fun dev4AllButton_whenClicked_invokesOnClick() {
        var clickCount = 0

        composeRule.setContent {
            Dev4AllTheme {
                Dev4AllButton(
                    text = "Tap Me",
                    onClick = { clickCount++ }
                )
            }
        }

        composeRule.onNodeWithText("Tap Me").performClick()

        assertEquals(1, clickCount)
    }

    @Test
    fun dev4AllErrorState_whenRetryClicked_invokesOnRetry() {
        var retryCount = 0

        composeRule.setContent {
            Dev4AllTheme {
                Dev4AllErrorState(
                    title = "Error",
                    description = "Something failed",
                    onRetryClick = { retryCount++ },
                    retryButtonText = "Try Again"
                )
            }
        }

        composeRule.onNodeWithText("Try Again").performClick()

        assertEquals(1, retryCount)
    }

    @Test
    fun statusBadge_displaysStatusLabel() {
        composeRule.setContent {
            Dev4AllTheme {
                StatusBadge(status = ProjectStatus.AwaitingContract)
            }
        }

        composeRule.onNodeWithText("Awaiting Contract").assertExists()
    }
}
