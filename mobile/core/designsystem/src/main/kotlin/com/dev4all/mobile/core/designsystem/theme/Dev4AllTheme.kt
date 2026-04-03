package com.dev4all.mobile.core.designsystem.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.ColorScheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.CompositionLocalProvider
import androidx.compose.runtime.Immutable
import androidx.compose.runtime.staticCompositionLocalOf
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.tooling.preview.Preview

private val LightColorScheme = lightColorScheme(
    primary = TechBlue,
    onPrimary = Color.White,
    primaryContainer = TechBlueLight,
    onPrimaryContainer = OnLight,
    secondary = TechBlueDark,
    onSecondary = Color.White,
    background = BackgroundLight,
    onBackground = OnLight,
    surface = SurfaceLight,
    onSurface = OnLight,
    surfaceVariant = SurfaceVariantLight,
    onSurfaceVariant = OnLight
)

private val DarkColorScheme = darkColorScheme(
    primary = TechBlueLight,
    onPrimary = Color.Black,
    primaryContainer = TechBlueDark,
    onPrimaryContainer = OnDark,
    secondary = TechBlue,
    onSecondary = Color.White,
    background = BackgroundDark,
    onBackground = OnDark,
    surface = SurfaceDark,
    onSurface = OnDark,
    surfaceVariant = SurfaceVariantDark,
    onSurfaceVariant = OnDark
)

@Immutable
data class Dev4AllStatusColors(
    val open: Color,
    val awaitingContract: Color,
    val ongoing: Color,
    val completed: Color,
    val expired: Color,
    val cancelled: Color
)

private val LightStatusColors = Dev4AllStatusColors(
    open = StatusOpen,
    awaitingContract = StatusAwaitingContract,
    ongoing = StatusOngoing,
    completed = StatusCompleted,
    expired = StatusExpired,
    cancelled = StatusCancelled
)

private val DarkStatusColors = Dev4AllStatusColors(
    open = StatusOpen,
    awaitingContract = StatusAwaitingContract,
    ongoing = StatusOngoing,
    completed = StatusCompleted,
    expired = StatusExpired,
    cancelled = StatusCancelled
)

private val LocalStatusColors = staticCompositionLocalOf { LightStatusColors }

object Dev4AllTheme {
    val colorScheme: ColorScheme
        @Composable
        get() = MaterialTheme.colorScheme

    val typography
        @Composable
        get() = MaterialTheme.typography

    val shapes
        @Composable
        get() = MaterialTheme.shapes

    val statusColors: Dev4AllStatusColors
        @Composable
        get() = LocalStatusColors.current
}

@Composable
fun Dev4AllTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    content: @Composable () -> Unit
) {
    val colorScheme = if (darkTheme) DarkColorScheme else LightColorScheme
    val statusColors = if (darkTheme) DarkStatusColors else LightStatusColors

    CompositionLocalProvider(LocalStatusColors provides statusColors) {
        MaterialTheme(
            colorScheme = colorScheme,
            typography = Dev4AllTypography,
            shapes = Dev4AllShapes,
            content = content
        )
    }
}

@Preview(name = "Dev4AllTheme Light", showBackground = true)
@Composable
private fun Dev4AllThemeLightPreview() {
    Dev4AllTheme(darkTheme = false) {
        Surface {
            Text(
                text = "Dev4All Light",
                color = Dev4AllTheme.statusColors.open,
                style = Dev4AllTheme.typography.bodyLarge
            )
        }
    }
}

@Preview(name = "Dev4AllTheme Dark", showBackground = true)
@Composable
private fun Dev4AllThemeDarkPreview() {
    Dev4AllTheme(darkTheme = true) {
        Surface {
            Text(
                text = "Dev4All Dark",
                color = Dev4AllTheme.statusColors.ongoing,
                style = Dev4AllTheme.typography.bodyLarge
            )
        }
    }
}
