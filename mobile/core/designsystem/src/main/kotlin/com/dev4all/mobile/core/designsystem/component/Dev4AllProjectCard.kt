package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.ExperimentalLayoutApi
import androidx.compose.foundation.layout.FlowRow
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.outlined.AccessTime
import androidx.compose.material.icons.outlined.ArrowForward
import androidx.compose.material.icons.outlined.PeopleAlt
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.dev4all.mobile.core.designsystem.theme.DividerColor
import com.dev4all.mobile.core.designsystem.theme.OnLight
import com.dev4all.mobile.core.designsystem.theme.OnLightSecondary
import com.dev4all.mobile.core.designsystem.theme.TechBlue
import com.dev4all.mobile.core.designsystem.theme.TechBlueDark
import com.dev4all.mobile.core.domain.model.Project
import java.time.Instant
import java.time.ZoneId
import java.time.temporal.ChronoUnit

@OptIn(ExperimentalLayoutApi::class)
@Composable
fun Dev4AllProjectCard(
    project: Project,
    onCardClick: (String) -> Unit,
    onActionClick: ((String) -> Unit)? = null,
    actionLabel: String = "Teklif Ver →",
    modifier: Modifier = Modifier,
) {
    val daysLeft = daysUntil(project.bidEndDate)
    val isUrgent = daysLeft in 1..3

    Card(
        modifier = modifier
            .fillMaxWidth()
            .clickable { onCardClick(project.id) },
        shape = RoundedCornerShape(16.dp),
        colors = CardDefaults.cardColors(containerColor = Color.White),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp, hoveredElevation = 6.dp),
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            // Top row: Status badge + Urgent badge
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically,
            ) {
                ProjectStatusBadge(project.status)
                if (isUrgent) UrgentBadge()
            }

            Spacer(Modifier.height(10.dp))

            // Title
            Text(
                text = project.title,
                fontSize = 16.sp,
                fontWeight = FontWeight.SemiBold,
                color = OnLight,
                maxLines = 2,
                overflow = TextOverflow.Ellipsis,
            )

            Spacer(Modifier.height(4.dp))

            // Description
            Text(
                text = project.description,
                fontSize = 13.sp,
                color = OnLightSecondary,
                maxLines = 2,
                overflow = TextOverflow.Ellipsis,
                lineHeight = 18.sp,
            )

            // Technology tags
            if (!project.technologies.isNullOrBlank()) {
                Spacer(Modifier.height(10.dp))
                FlowRow(horizontalArrangement = Arrangement.spacedBy(6.dp)) {
                    project.technologies.split(",").take(4).forEach { tech ->
                        TechTag(tech.trim())
                    }
                }
            }

            Spacer(Modifier.height(12.dp))

            // Divider
            Box(modifier = Modifier.fillMaxWidth().height(1.dp).background(DividerColor))

            Spacer(Modifier.height(12.dp))

            // Bottom info row
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically,
            ) {
                // Budget + deadline info
                Column {
                    Text(
                        text = "₺${"%,.0f".format(project.budget)}",
                        fontSize = 15.sp,
                        fontWeight = FontWeight.Bold,
                        color = TechBlue,
                    )
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Icon(
                            imageVector = Icons.Outlined.AccessTime,
                            contentDescription = null,
                            tint = OnLightSecondary,
                            modifier = Modifier.size(12.dp),
                        )
                        Spacer(Modifier.width(3.dp))
                        Text(
                            text = if (daysLeft > 0) "Son $daysLeft gün" else "Süresi doldu",
                            fontSize = 11.sp,
                            color = OnLightSecondary,
                        )
                        Spacer(Modifier.width(8.dp))
                        Icon(
                            imageVector = Icons.Outlined.PeopleAlt,
                            contentDescription = null,
                            tint = OnLightSecondary,
                            modifier = Modifier.size(12.dp),
                        )
                        Spacer(Modifier.width(3.dp))
                        Text(
                            text = "${project.bidCount} teklif",
                            fontSize = 11.sp,
                            color = OnLightSecondary,
                        )
                    }
                }

                // Action button
                if (onActionClick != null) {
                    Surface(
                        modifier = Modifier
                            .clip(RoundedCornerShape(20.dp))
                            .clickable { onActionClick(project.id) },
                        color = TechBlue,
                        shape = RoundedCornerShape(20.dp),
                    ) {
                        Row(
                            modifier = Modifier.padding(horizontal = 14.dp, vertical = 7.dp),
                            verticalAlignment = Alignment.CenterVertically,
                        ) {
                            Text(
                                text = actionLabel,
                                fontSize = 12.sp,
                                fontWeight = FontWeight.SemiBold,
                                color = Color.White,
                            )
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun TechTag(text: String, modifier: Modifier = Modifier) {
    Box(
        modifier = modifier
            .clip(RoundedCornerShape(6.dp))
            .background(Color(0xFFEFF6FF))
            .padding(horizontal = 8.dp, vertical = 3.dp),
    ) {
        Text(text = text, fontSize = 11.sp, color = TechBlue, fontWeight = FontWeight.Medium)
    }
}

private fun daysUntil(isoDate: String): Long {
    return try {
        val target = Instant.parse(isoDate).atZone(ZoneId.systemDefault()).toLocalDate()
        val today = java.time.LocalDate.now()
        ChronoUnit.DAYS.between(today, target)
    } catch (e: Exception) {
        0L
    }
}
