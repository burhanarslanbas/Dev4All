package com.dev4all.mobile.core.data.fake

import com.dev4all.mobile.core.domain.model.Bid
import com.dev4all.mobile.core.domain.model.BidStatus
import com.dev4all.mobile.core.domain.model.Contract
import com.dev4all.mobile.core.domain.model.ContractRevision
import com.dev4all.mobile.core.domain.model.ContractStatus
import com.dev4all.mobile.core.domain.model.GitHubLog
import com.dev4all.mobile.core.domain.model.Project
import com.dev4all.mobile.core.domain.model.ProjectStatus
import com.dev4all.mobile.core.domain.model.User
import com.dev4all.mobile.core.domain.model.UserRole
import java.util.UUID

/**
 * Shared in-memory data store for all fake repositories.
 * Provides realistic sample data aligned with backend entity structure.
 */
object FakeData {

    // ── Users ────────────────────────────────────────────────────────
    val passwords = mutableMapOf(
        "customer@dev4all.com" to "Test1234",
        "developer@dev4all.com" to "Test1234",
        "admin@dev4all.com" to "Test1234",
    )

    val users = mutableListOf(
        User(id = "u-cust-1", name = "Ahmet Yılmaz", email = "customer@dev4all.com", role = UserRole.Customer),
        User(id = "u-dev-1", name = "Elif Demir", email = "developer@dev4all.com", role = UserRole.Developer),
        User(id = "u-admin-1", name = "Admin User", email = "admin@dev4all.com", role = UserRole.Admin),
    )

    var currentUser: User? = null

    // ── Projects ─────────────────────────────────────────────────────
    val projects = mutableListOf(
        Project(
            id = "p-1", customerId = "u-cust-1", title = "E-Ticaret Platformu UI/UX Modernizasyonu",
            description = "Mevcut e-ticaret platformunun kullanıcı arayüzünün modern tasarım trendlerine uygun olarak yeniden tasarlanması. Responsive tasarım, erişilebilirlik standartları ve performans optimizasyonu dahildir.",
            budget = 55000.0, deadline = "2026-06-15T00:00:00Z", bidEndDate = "2026-05-01T00:00:00Z",
            technologies = "React, TypeScript, Tailwind CSS, Figma",
            status = ProjectStatus.Open, createdDate = "2026-04-01T10:00:00Z", updatedDate = "2026-04-01T10:00:00Z", bidCount = 3,
        ),
        Project(
            id = "p-2", customerId = "u-cust-1", title = "Mobil Sağlık Takip Uygulaması",
            description = "Hastaların günlük sağlık verilerini (tansiyon, şeker, nabız) takip edebileceği, doktor randevularını yönetebileceği ve ilaç hatırlatıcıları içeren cross-platform mobil uygulama.",
            budget = 80000.0, deadline = "2026-07-30T00:00:00Z", bidEndDate = "2026-05-15T00:00:00Z",
            technologies = "Kotlin, Jetpack Compose, Firebase, FHIR API",
            status = ProjectStatus.Open, createdDate = "2026-04-03T14:30:00Z", updatedDate = "2026-04-03T14:30:00Z", bidCount = 1,
        ),
        Project(
            id = "p-3", customerId = "u-cust-1", title = "Yapay Zeka Destekli Chatbot",
            description = "Müşteri hizmetleri için doğal dil işleme tabanlı akıllı chatbot geliştirme. Çoklu dil desteği, sentiment analizi ve CRM entegrasyonu içerir.",
            budget = 120000.0, deadline = "2026-09-01T00:00:00Z", bidEndDate = "2026-05-20T00:00:00Z",
            technologies = "Python, OpenAI API, FastAPI, PostgreSQL",
            status = ProjectStatus.Open, createdDate = "2026-04-05T09:00:00Z", updatedDate = "2026-04-05T09:00:00Z", bidCount = 5,
        ),
        Project(
            id = "p-4", customerId = "u-cust-1", assignedDeveloperId = "u-dev-1",
            title = "Blockchain Tabanlı Tedarik Zinciri Takip", description = "Ürünlerin üretimden tüketiciye kadar olan yolculuğunu blockchain üzerinde şeffaf olarak takip eden dağıtık uygulama.",
            budget = 200000.0, deadline = "2026-10-15T00:00:00Z", bidEndDate = "2026-04-01T00:00:00Z",
            technologies = "Solidity, Ethereum, React, Node.js",
            status = ProjectStatus.AwaitingContract, createdDate = "2026-03-15T12:00:00Z", updatedDate = "2026-04-08T16:00:00Z", bidCount = 2,
        ),
        Project(
            id = "p-5", customerId = "u-cust-1", assignedDeveloperId = "u-dev-1",
            title = "SaaS Dashboard Analytics Platformu", description = "İşletmelerin verilerini görselleştirmesini sağlayan SaaS tabanlı analitik dashboard. Gerçek zamanlı grafik, rapor oluşturma ve veri dışa aktarma.",
            budget = 95000.0, deadline = "2026-08-01T00:00:00Z", bidEndDate = "2026-03-20T00:00:00Z",
            technologies = ".NET, Blazor, SignalR, PostgreSQL, Chart.js",
            status = ProjectStatus.Ongoing, createdDate = "2026-03-01T08:30:00Z", updatedDate = "2026-04-10T11:00:00Z", bidCount = 4,
        ),
    )

    // ── Bids ─────────────────────────────────────────────────────────
    val bids = mutableListOf(
        Bid(
            id = "b-1", projectId = "p-1", developerId = "u-dev-1", bidAmount = 48000.0,
            proposalNote = "E-ticaret alanında 5 yıllık deneyimim var. React ve Tailwind ile modern, performanslı arayüzler tasarlayabilirim. Accessibility standartlarına tam uyum sağlarım.",
            status = BidStatus.Pending, createdDate = "2026-04-05T11:00:00Z", updatedDate = "2026-04-05T11:00:00Z",
            developerName = "Elif Demir", projectTitle = "E-Ticaret Platformu UI/UX Modernizasyonu",
        ),
        Bid(
            id = "b-2", projectId = "p-4", developerId = "u-dev-1", bidAmount = 185000.0,
            proposalNote = "Solidity ve Ethereum üzerinde 3 yıllık tecrübem bulunmaktadır. Tedarik zinciri projelerinde daha önce çalıştım.",
            status = BidStatus.Accepted, isAccepted = true, createdDate = "2026-03-25T10:00:00Z", updatedDate = "2026-04-08T16:00:00Z",
            developerName = "Elif Demir", projectTitle = "Blockchain Tabanlı Tedarik Zinciri Takip",
        ),
        Bid(
            id = "b-3", projectId = "p-5", developerId = "u-dev-1", bidAmount = 90000.0,
            proposalNote = ".NET ve Blazor konusunda uzmanım. SignalR ile gerçek zamanlı veri akışı sağlayabilirim.",
            status = BidStatus.Accepted, isAccepted = true, createdDate = "2026-03-10T14:00:00Z", updatedDate = "2026-03-20T09:00:00Z",
            developerName = "Elif Demir", projectTitle = "SaaS Dashboard Analytics Platformu",
        ),
    )

    // ── Contracts ────────────────────────────────────────────────────
    val contracts = mutableListOf(
        Contract(
            id = "c-1", projectId = "p-4",
            content = "1. TARAFLAR\nBu sözleşme Ahmet Yılmaz (Müşteri) ile Elif Demir (Geliştirici) arasında akdedilmiştir.\n\n2. İŞİN KONUSU\nBlockchain tabanlı tedarik zinciri takip uygulamasının geliştirilmesi.\n\n3. BÜTÇE\nToplam proje bedeli: ₺185.000\n\n4. TESLİM TARİHİ\n15 Ekim 2026\n\n5. TEKNOLOJİLER\nSolidity, Ethereum, React, Node.js\n\n6. ÖDEME PLANI\n%30 başlangıç, %40 ara teslim, %30 final teslim.",
            revisionNumber = 1, lastRevisedById = "system", status = ContractStatus.Draft,
            isCustomerApproved = false, isDeveloperApproved = false,
            createdDate = "2026-04-08T16:00:00Z", updatedDate = "2026-04-08T16:00:00Z",
            projectTitle = "Blockchain Tabanlı Tedarik Zinciri Takip",
        ),
        Contract(
            id = "c-2", projectId = "p-5",
            content = "1. TARAFLAR\nBu sözleşme Ahmet Yılmaz (Müşteri) ile Elif Demir (Geliştirici) arasında akdedilmiştir.\n\n2. İŞİN KONUSU\nSaaS Dashboard Analytics Platformu.\n\n3. BÜTÇE\nToplam: ₺90.000\n\n4. TESLİM TARİHİ\n1 Ağustos 2026",
            revisionNumber = 2, lastRevisedById = "u-dev-1", status = ContractStatus.BothApproved,
            isCustomerApproved = true, isDeveloperApproved = true,
            customerApprovedAt = "2026-03-22T10:00:00Z", developerApprovedAt = "2026-03-21T15:00:00Z",
            createdDate = "2026-03-20T09:00:00Z", updatedDate = "2026-03-22T10:00:00Z",
            projectTitle = "SaaS Dashboard Analytics Platformu",
        ),
    )

    val contractRevisions = mutableListOf(
        ContractRevision(
            id = "cr-1", contractId = "c-2", revisedById = "u-dev-1",
            contentSnapshot = "İlk taslak — ödeme planı eklenmeden önceki versiyon.",
            revisionNumber = 1, revisionNote = "Ödeme planını detaylandırdım", createdDate = "2026-03-21T15:00:00Z",
        ),
    )

    // ── GitHub Logs ──────────────────────────────────────────────────
    val gitHubLogs = mutableListOf(
        GitHubLog(id = "gl-1", projectId = "p-5", repoUrl = "https://github.com/elifdemir/saas-dashboard", branch = "main", commitHash = "a1b2c3d4e5f6789012345678901234567890abcd", commitMessage = "feat: dashboard ana sayfa layout tamamlandı", authorName = "Elif Demir", pushedAt = "2026-04-08T14:22:00Z"),
        GitHubLog(id = "gl-2", projectId = "p-5", repoUrl = "https://github.com/elifdemir/saas-dashboard", branch = "main", commitHash = "b2c3d4e5f67890123456789012345678901abcde", commitMessage = "feat: gerçek zamanlı grafik bileşeni eklendi", authorName = "Elif Demir", pushedAt = "2026-04-09T10:15:00Z"),
        GitHubLog(id = "gl-3", projectId = "p-5", repoUrl = "https://github.com/elifdemir/saas-dashboard", branch = "main", commitHash = "c3d4e5f678901234567890123456789012abcdef", commitMessage = "fix: SignalR bağlantı kopması düzeltildi", authorName = "Elif Demir", pushedAt = "2026-04-09T16:45:00Z"),
        GitHubLog(id = "gl-4", projectId = "p-5", repoUrl = "https://github.com/elifdemir/saas-dashboard", branch = "develop", commitHash = "d4e5f6789012345678901234567890123abcdef0", commitMessage = "feat: rapor dışa aktarma (PDF/Excel) eklendi", authorName = "Elif Demir", pushedAt = "2026-04-10T09:30:00Z"),
        GitHubLog(id = "gl-5", projectId = "p-5", repoUrl = "https://github.com/elifdemir/saas-dashboard", branch = "develop", commitHash = "e5f67890123456789012345678901234abcdef01", commitMessage = "test: dashboard widget unit testleri eklendi", authorName = "Elif Demir", pushedAt = "2026-04-10T11:00:00Z"),
    )

    // ── Helpers ───────────────────────────────────────────────────────
    fun generateId(): String = UUID.randomUUID().toString()
    fun now(): String = java.time.Instant.now().toString()
}
