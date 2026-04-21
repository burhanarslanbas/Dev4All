# Dev4All Mobil Uygulama — Görev Takibi

## Phase 1: Domain Layer (Backend-Uyumlu)
- [ ] Yeni modeller: Project, ProjectStatus, Bid, BidStatus, Contract, ContractStatus, ContractRevision, GitHubLog
- [ ] Yeni repository IF'ler: ProjectRepository, BidRepository, ContractRepository, GitHubRepository, UserManagementRepository
- [ ] AuthRepository güncelle: logout() ekle
- [ ] Yeni use case'ler: Project (7), Bid (5), Contract (5), GitHub (2), Admin (3)

## Phase 2: Fake Repository'ler
- [ ] FakeData.kt — paylaşılan in-memory veri deposu
- [ ] FakeAuthRepository.kt
- [ ] FakeProjectRepository.kt
- [ ] FakeBidRepository.kt
- [ ] FakeContractRepository.kt
- [ ] FakeGitHubRepository.kt
- [ ] FakeUserManagementRepository.kt
- [ ] DataModule.kt güncelle — tüm binding'ler
- [ ] core/network/mapper/AuthMapper.kt sil (çift mapper temizliği)

## Phase 3: Design System
- [ ] Color.kt güncelle
- [ ] Typography.kt güncelle
- [ ] Shape.kt güncelle/oluştur
- [ ] Dev4AllBottomBar.kt
- [ ] Dev4AllTopBar.kt
- [ ] Dev4AllSearchBar.kt
- [ ] Dev4AllFilterChip.kt
- [ ] Dev4AllProjectCard.kt
- [ ] Dev4AllBidCard.kt
- [ ] Dev4AllContractSection.kt
- [ ] Dev4AllStatusBadge.kt
- [ ] Dev4AllTimelineItem.kt
- [ ] Dev4AllRoleCard.kt
- [ ] Dev4AllButton.kt güncelle
- [ ] Dev4AllTextField.kt güncelle

## Phase 4: Auth Ekranları
- [ ] RegisterScreen.kt — tasarıma birebir
- [ ] LoginScreen.kt — tasarıma birebir
- [ ] RegisterViewModel.kt — @HiltViewModel + validasyon

## Phase 5: Feature Ekranları
- [ ] Customer Dashboard
- [ ] Project Create
- [ ] Customer Project Detail
- [ ] Bid Evaluation
- [ ] Explore (Developer)
- [ ] Developer Project Detail
- [ ] Bid Submit
- [ ] My Bids
- [ ] Assigned Project
- [ ] Contract Screen
- [ ] Admin Dashboard
- [ ] User Management
- [ ] Project Oversight
- [ ] Profile
- [ ] Alerts

## Phase 6: Navigation & App Shell
- [ ] Screen.kt (routes)
- [ ] BottomNavItem.kt
- [ ] Dev4AllNavHost.kt
- [ ] MainActivity.kt güncelle
- [ ] DomainModule.kt güncelle

## Phase 7: Build & Test
- [ ] gradlew assembleDebug
- [ ] Hata düzeltmeleri
- [ ] Fonksiyonel doğrulama
