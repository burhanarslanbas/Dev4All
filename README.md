<div align="center">
  <img src="https://raw.githubusercontent.com/github/explore/80688e429a7d4ef2fca1e82350fe8e3517d3494d/topics/dotnet/dotnet.png" alt=".NET" width="80" height="80"/>
  <img src="https://raw.githubusercontent.com/github/explore/80688e429a7d4ef2fca1e82350fe8e3517d3494d/topics/react/react.png" alt="React" width="80" height="80"/>
  <img src="https://raw.githubusercontent.com/github/explore/80688e429a7d4ef2fca1e82350fe8e3517d3494d/topics/typescript/typescript.png" alt="TypeScript" width="80" height="80"/>
  <img src="https://raw.githubusercontent.com/github/explore/80688e429a7d4ef2fca1e82350fe8e3517d3494d/topics/postgresql/postgresql.png" alt="PostgreSQL" width="80" height="80"/>
  <img src="https://raw.githubusercontent.com/github/explore/80688e429a7d4ef2fca1e82350fe8e3517d3494d/topics/kotlin/kotlin.png" alt="Kotlin" width="80" height="80"/>

  <h1>🚀 Dev4All</h1>
  <p><strong>Yazılım projelerini hayata geçiren güvenilir B2B/B2C eşleşme ve yönetim ekosistemi.</strong></p>

  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET" /></a>
  <a href="https://reactjs.org/"><img src="https://img.shields.io/badge/React-18-61DAFB?style=for-the-badge&logo=react&logoColor=black" alt="React" /></a>
  <a href="https://www.typescriptlang.org/"><img src="https://img.shields.io/badge/TypeScript-5.0-3178C6?style=for-the-badge&logo=typescript&logoColor=white" alt="TypeScript" /></a>
  <a href="https://www.postgresql.org/"><img src="https://img.shields.io/badge/PostgreSQL-15%2B-336791?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" /></a>
  <a href="https://azure.microsoft.com/"><img src="https://img.shields.io/badge/Azure_App_Service-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white" alt="Azure" /></a>
  <a href="https://kotlinlang.org/"><img src="https://img.shields.io/badge/Kotlin-Native-7F52FF?style=for-the-badge&logo=kotlin&logoColor=white" alt="Kotlin" /></a>
</div>

<br />

## 📖 Proje Hakkında

**Dev4All**, yazılım hizmeti almak isteyen müşteriler (bireysel veya kurumsal) ile bağımsız geliştiricileri (freelance yazılım uzmanları, küçük takımlar) güvenli bir ekosistemde bir araya getiren B2B/B2C tabanlı bir **Proje Yönetim ve Eşleşme Platformudur**. 

İletişim kopukluklarını, proje iptallerini ve takipsizliği ortadan kaldırmayı hedefleyen Dev4All; teklif yönetiminden GitHub destekli geliştirme sürecinin şeffaf izlenmesine kadar her adımı tek bir merkezi sistemde yürütür.

---

## ✨ Temel Özellikler

- 🔐 **Kimlik Doğrulama:** JWT tabanlı, rol yetkili (Müşteri / Geliştirici) güvenli giriş ve kayıt altyapısı.
- 📝 **Proje Talebi (İlan) Yönetimi:** Müşterilerin bütçe, deadline ve teknoloji beklentilerini şeffafça belirtebildiği akıllı ilan sistemi.
- 🤝 **Teklif ve Eşleşme:** Açık projelere geliştiricilerin teklif verebilmesi, müşterinin karşılaştırmalı değerlendirme yapıp onaylayabilmesi.
- 🔄 **GitHub Entegrasyonu:** Kabul edilen projelere GitHub Reposu bağlanarak; commit ve push aktivitelerinin Webhook ile canlı izlenebilmesi.
- 📩 **Otomatik Bildirimler:** Yeni teklif, proje başlangıcı veya statü değişikliklerinde MailKit destekli e-posta bildirimleri.
- ⏱️ **Zamanlanmış Görevler:** Quartz.NET ile süresi dolan ilanların otomatik yönetimi.

---

## 🛠️ Kullanılan Teknolojiler

Proje, modern yazılım mimarisi standartlarına uygun olarak en güncel teknolojilerle geliştirilmektedir.

### 🌐 Backend
- **Framework:** .NET 10 (ASP.NET Core Web API)
- **Mimari:** Clean / Onion Architecture, Domain-Driven Design (DDD)
- **Desenler:** CQRS (MediatR), Repository, Unit of Work
- **Veritabanı & ORM:** PostgreSQL 15+, Entity Framework Core
- **Güvenlik:** ASP.NET Core Identity, JWT Bearer
- **Validasyon:** FluentValidation
- **Diğer Bileşenler:** Quartz.NET (Arka plan görevleri), MailKit (SMTP Entegrasyonu), Serilog (Loglama)

### 💻 Frontend
- **Kütüphane:** React
- **Derleyici/Paketleyici:** Vite
- **Dil:** TypeScript
- **Stil Yönetimi:** Tailwind CSS

### 📱 Mobil
- **Dil:** Kotlin
- **Platform:** Android Native

### 🚀 Altyapı & Entegrasyon
- **CI/CD & Hosting:** Azure App Service
- **Versiyon Kontrol Entegrasyonu:** GitHub Webhook API
- **API Dokümantasyonu:** Swagger / OpenAPI

---

## 🏗️ Mimari Yaklaşım

Dev4All backend sistemi katmanlı mimari (Onion Architecture) prensiplerine göre yapılandırılmıştır:

1. **Domain (Core):** Bağımlılıklardan tamamen izole edilmiş, iş kurallarını barındıran POCO sınıfları, enum ve arayüzler.
2. **Application:** CQRS pattern'i kullanılarak Use Case'lerin işletildiği katman. DTO, Validator ve servis arayüzleri burada yer alır.
3. **Infrastructure / Persistence:** DbContext, veritabanı bağlantıları, dış servis entegrasyonları (Email, GitHub vb.) uygulama katmanı logiğini hayata geçirir.
4. **Presentation (API):** Middleware konfigürasyonları, endpoint'lerin dışa sunulduğu merkez yapı.

---

## 🚦 İş Süreci Akışı

1. **Üyelik & İlan Açma:** Müşteri platforma üye olur ve detaylı bir proje ilanı yayınlar.
2. **Teklif Süreci:** Geliştiriciler açık ilanlara bütçe ve süre belirterek `Bid` (Teklif) gönderir.
3. **Onaylama:** Müşteri uygun bulduğu teklifi onaylar, diğer teklifler reddedilir.
4. **Geliştirme & Takip:** Geliştirici GitHub reposunu sisteme bağlar; müşteri platform üzerinden aktivite akışını izler.
5. **Teslimat:** Proje tamamlanır ve başarılı bir ekosistem döngüsü sağlanmış olur.

---

<div align="center">
  <p>Built with ❤️ for a better software development ecosystem.</p>
</div>
