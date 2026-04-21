package com.dev4all.mobile.core.designsystem.component

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.dev4all.mobile.core.designsystem.theme.AccentBlueLight
import com.dev4all.mobile.core.designsystem.theme.AccentGreen
import com.dev4all.mobile.core.designsystem.theme.AccentGreenLight
import com.dev4all.mobile.core.designsystem.theme.AccentOrange
import com.dev4all.mobile.core.designsystem.theme.AccentOrangeLight
import com.dev4all.mobile.core.designsystem.theme.AccentRed
import com.dev4all.mobile.core.designsystem.theme.AccentRedLight
import com.dev4all.mobile.core.designsystem.theme.BidAccepted
import com.dev4all.mobile.core.designsystem.theme.BidPending
import com.dev4all.mobile.core.designsystem.theme.BidRejected
import com.dev4all.mobile.core.designsystem.theme.ContractBothApproved
import com.dev4all.mobile.core.designsystem.theme.ContractCancelled
import com.dev4all.mobile.core.designsystem.theme.ContractDraft
import com.dev4all.mobile.core.designsystem.theme.ContractUnderReview
import com.dev4all.mobile.core.designsystem.theme.StatusAwaitingContract
import com.dev4all.mobile.core.designsystem.theme.StatusCancelled
import com.dev4all.mobile.core.designsystem.theme.StatusCompleted
import com.dev4all.mobile.core.designsystem.theme.StatusExpired
import com.dev4all.mobile.core.designsystem.theme.StatusOngoing
import com.dev4all.mobile.core.designsystem.theme.StatusOpen
import com.dev4all.mobile.core.designsystem.theme.TechBlue
import com.dev4all.mobile.core.domain.model.BidStatus
import com.dev4all.mobile.core.domain.model.ContractStatus
import com.dev4all.mobile.core.domain.model.ProjectStatus

@Composable
fun ProjectStatusBadge(status: ProjectStatus, modifier: Modifier = Modifier) {
    val (text, textColor, bgColor) = when (status) {
        ProjectStatus.Open           -> Triple("Açık", StatusOpen, AccentBlueLight)
        ProjectStatus.AwaitingContract -> Triple("Sözleşme Bekleniyor", StatusAwaitingContract, AccentOrangeLight)
        ProjectStatus.Ongoing        -> Triple("Devam Ediyor", StatusOngoing, AccentGreenLight)
        ProjectStatus.Completed      -> Triple("Tamamlandı", StatusCompleted, AccentGreenLight)
        ProjectStatus.Expired        -> Triple("Süresi Doldu", StatusExpired, Color(0xFFF1F5F9))
        ProjectStatus.Cancelled      -> Triple("İptal Edildi", StatusCancelled, AccentRedLight)
    }
    StatusChip(text = text, textColor = textColor, bgColor = bgColor, modifier = modifier)
}

@Composable
fun BidStatusBadge(status: BidStatus, modifier: Modifier = Modifier) {
    val (text, textColor, bgColor) = when (status) {
        BidStatus.Pending  -> Triple("Bekliyor", BidPending, AccentOrangeLight)
        BidStatus.Accepted -> Triple("Kabul Edildi", BidAccepted, AccentGreenLight)
        BidStatus.Rejected -> Triple("Reddedildi", BidRejected, AccentRedLight)
    }
    StatusChip(text = text, textColor = textColor, bgColor = bgColor, modifier = modifier)
}

@Composable
fun ContractStatusBadge(status: ContractStatus, modifier: Modifier = Modifier) {
    val (text, textColor, bgColor) = when (status) {
        ContractStatus.Draft         -> Triple("Taslak", ContractDraft, Color(0xFFF1F5F9))
        ContractStatus.UnderReview   -> Triple("İncelemede", ContractUnderReview, AccentOrangeLight)
        ContractStatus.BothApproved  -> Triple("Onaylandı", ContractBothApproved, AccentGreenLight)
        ContractStatus.Cancelled     -> Triple("İptal Edildi", ContractCancelled, AccentRedLight)
    }
    StatusChip(text = text, textColor = textColor, bgColor = bgColor, modifier = modifier)
}

@Composable
fun UrgentBadge(modifier: Modifier = Modifier) {
    StatusChip(text = "ACİL", textColor = AccentRed, bgColor = AccentRedLight, modifier = modifier)
}

@Composable
private fun StatusChip(
    text: String,
    textColor: Color,
    bgColor: Color,
    modifier: Modifier = Modifier,
) {
    Box(
        modifier = modifier
            .clip(RoundedCornerShape(20.dp))
            .background(bgColor)
            .padding(horizontal = 10.dp, vertical = 4.dp),
        contentAlignment = Alignment.Center,
    ) {
        Text(
            text = text,
            color = textColor,
            fontSize = 11.sp,
            fontWeight = FontWeight.SemiBold,
            maxLines = 1,
        )
    }
}
