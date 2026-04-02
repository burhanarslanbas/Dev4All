# Copilot Coding Agent Instructions

> Bu dosya GitHub Copilot Coding Agent tarafından otomatik olarak okunur.
> Issue'ya atandığında agent bu talimatları takip eder.

## Proje Bağlamı

Dev4All, müşterilerin proje yayınladığı, geliştiricilerin teklif verdiği bir B2B freelance marketplace platformudur.

## Öncelikli Okuma Listesi

Herhangi bir kod yazmadan ÖNCE bu dosyaları oku:
1. `docs/AGENTS.md` — Mimari kurallar, kodlama standartları, katman yapısı
2. Issue body'sindeki talimatlar — Ne yapılacağının detayı

## Prompt Dosyaları

Eğer issue body'si kısa ve yetersizse, detaylı talimatları şu dosyalarda bulabilirsin:
- **Backend issue'ları:** `backend/plan/22-BACKEND-AGENT-PROMPTS.md` — Issue numarasını (örn: `#108`, `#B01`) ara
- **Frontend issue'ları:** `frontend/plan/32-FRONTEND-AGENT-PROMPTS.md` — `#F01`, `#F02` vb. ara
- **Mobile issue'ları:** `docs/plan/13-all-issue-agent-prompts.md` — Issue numarasını ara

## Kritik Kurallar

### Mimari
- Onion Architecture: Domain ← Application ← Persistence/Infrastructure ← WebAPI
- Domain'de ASLA dış bağımlılık olmamalı
- Controller'lar SADECE `ISender.Send()` çağırır
- Her Command'ın FluentValidation Validator'ı OLMALI

### E-posta
Handler içinde `IEmailService.SendAsync()` **ÇAĞIRMA**.
`IEmailNotificationService.QueueXxxEmailAsync()` kullan → EmailQueue → Quartz işler.

### Commit & PR
- Conventional Commits: `feat:`, `fix:`, `test:`, `docs:`
- PR body'sinde mutlaka `Closes #XX` yaz (auto-chain için zorunlu)
- Branch: `feat/issue-{number}-{short-name}` formatında oluştur
- Base branch: `develop`
- Tool attribution trailer EKLEME (`--trailer` kullanma)

### Kodlama
- C# 13 / .NET 10
- File-scoped namespaces
- Primary constructors
- `sealed record` for Commands/Queries/DTOs
- `sealed class` for Handlers
- `CancellationToken` on all async methods
- `async/await` for all I/O

### Test
- xUnit + FluentAssertions + NSubstitute
- Pattern: `MethodName_Scenario_ExpectedResult`
- Her handler ve validator için unit test

## Frontend (ASP.NET Core MVC)
- Bootstrap 5 kullan
- Cookie Authentication ile JWT token sakla
- `IApiClient` üzerinden backend API'ye bağlan
- View'larda model binding + validation summary kullan
- `[ValidateAntiForgeryToken]` POST action'larda zorunlu

## Mobile (Kotlin/Android)
- Jetpack Compose UI
- MVVM + Clean Architecture
- Hilt DI
- Retrofit + OkHttp
- Kotlin Coroutines + Flow

## Build Doğrulama
Her değişiklikten sonra mutlaka `dotnet build` çalıştır.
Test varsa `dotnet test` çalıştır.
