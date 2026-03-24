# 03. Non-Functional Requirements Document (NFR) - Dev4All

## 1. Giriş ve Amaç

Bu doküman, Dev4All platformunun **işlevsel olmayan (non-functional)** gereksinimlerini tanımlar. İşlevsel gereksinimler sisteme *ne yapması gerektiğini* söylerken; NFR'lar sisteme *nasıl çalışması gerektiğini* söyler. Bu gereksinimler; performans, güvenlik, kullanılabilirlik, güvenilirlik ve sistem bağımlılıklarını kapsar.

> Geliştirme kararları bu gereksinimlere aykırı olamaz. Mimariye dair değişiklikler bu doküman ile çapraz kontrol edilmelidir.

---

## 2. Performans ve Ölçeklenebilirlik (Performance & Scalability)

### 2.1. API Yanıt Süresi

| İşlem Tipi | Hedef Yanıt Süresi | Ölçüm Koşulu |
|------------|-------------------|--------------|
| Temel CRUD (Listeleme, Detay) | ≤ 200 ms | Ortalama (p50), veritabanı dahil |
| Teklif Kabul (Transaction) | ≤ 500 ms | Ortalama (p95), eş zamanlı erişim altında |
| GitHub Webhook İşleme | ≤ 1000 ms | Background job dahil |
| E-posta Kuyruk Ekleme | ≤ 100 ms | Sadece kuyruğa yazma süresi |

### 2.2. Eşzamanlı Kullanıcı Kapasitesi

- **MVP Hedefi:** Aynı anda **100 eş zamanlı kullanıcı** sorunsuz şekilde ilan görüntüleme ve teklif oluşturma işlemini gerçekleştirebilmelidir.
- **Ölçüm Yöntemi:** Load test araçları (örn. k6, Apache JMeter) ile MVP öncesi temel yük testi yapılacaktır.

### 2.3. Ölçeklenebilirlik Stratejisi

- **.NET Core Web API** ve **PostgreSQL** yapılandırmaları, artan trafik durumunda **Docker container'ları** aracılığıyla yatay (horizontal) ölçeklendirmeye uygun tasarlanacaktır.
- Veritabanı bağlantı havuzu (Connection Pooling) Entity Framework Core üzerinden yapılandırılacaktır.
- Sık sorgulanan listeler (açık ilanlar) için uygulama katmanında **in-memory cache** (IMemoryCache) uygulanabilir; bu MVP sonrası bir optimizasyondur.

---

## 3. Güvenlik (Security)

### 3.1. Kimlik Doğrulama (Authentication)

| Gereksinim | Detay |
|-----------|-------|
| **Altyapı** | ASP.NET Core Identity |
| **Token Standardı** | JWT (JSON Web Token) — `HS256` algoritması |
| **Token Süresi** | Access Token: 60 dakika |
| **Token İçeriği** | `UserId`, `Email`, `Role` claim'leri |
| **Güvenli İletim** | Token yalnızca HTTPS üzerinden iletilir; `Authorization: Bearer <token>` header'ı ile gönderilir. |

### 3.2. Yetkilendirme (Authorization)

- **Role-Based Access Control (RBAC)** uygulanır. `Customer` ve `Developer` rolleri katı biçimde ayrılır.
- Her endpoint, yalnızca yetkili rol tarafından erişilebilir (`[Authorize(Roles = "Customer")]`).
- Bir kullanıcı başkasına ait kaynağa (ilan, teklif, proje) erişmeye çalıştığında `403 Forbidden` döner.

### 3.3. Veri ve İletişim Güvenliği

| Alan | Uygulama |
|------|----------|
| **Şifre Saklama** | `bcrypt` (Hash + Salt) — düz metin şifre asla yazılmaz. |
| **HTTPS Zorunluluğu** | Tüm API uçları yalnızca HTTPS iletişime açıktır; HTTP istekleri HTTPS'e yönlendirilir. |
| **KVKK Uyumu** | Kullanıcıların e-posta ve iletişim bilgileri, API response'larında gerekmedikçe maskelenerek döndürülür. |
| **Webhook İmzası** | GitHub Webhook payload'ları `HMAC-SHA256` ile doğrulanır; imzasız istekler `401 Unauthorized` ile reddedilir. |
| **Input Güvenliği** | FluentValidation ile tüm giriş verileri doğrulanır; SQL Injection ve XSS saldırılarına karşı EF Core parametreli sorgular kullanılır. |

---

## 4. Kullanılabilirlik (Usability)

### 4.1. Responsive Tasarım

- Frontend (React + Tailwind CSS), **Mobile-First** yaklaşımıyla geliştirilir.
- Minimum test edilecek ekran boyutları: 375px (mobil), 768px (tablet), 1280px (masaüstü).

### 4.2. Hata Yönetimi

- Son kullanıcıya asla **Stack Trace** veya ham backend hata kodu gösterilmez.
- Tüm hatalar **Global Exception Middleware** tarafından yakalanır ve standart bir hata yapısıyla döndürülür:

```json
{
  "statusCode": 400,
  "message": "Bütçe alanı sıfırdan büyük olmalıdır.",
  "errors": ["Budget must be greater than 0"]
}
```

- HTTP durum kodları tutarlı biçimde kullanılır:

| HTTP Kodu | Kullanım Senaryosu |
|-----------|-------------------|
| `200 OK` | Başarılı GET/PUT işlemleri |
| `201 Created` | Yeni kaynak oluşturuldu |
| `400 Bad Request` | Doğrulama hatası (FluentValidation) |
| `401 Unauthorized` | Geçersiz veya eksik JWT |
| `403 Forbidden` | Yetersiz rol/yetki |
| `404 Not Found` | Kayıt bulunamadı |
| `409 Conflict` | Duplicate işlem (aynı ilana ikinci teklif vb.) |
| `500 Internal Server Error` | İşlenmeyen hata (Global Middleware yakalar) |

---

## 5. Güvenilirlik ve Loglama (Reliability & Logging)

### 5.1. Sistem Erişilebilirliği (Uptime)

- **Hedef Uptime:** %99 (aylık yaklaşık 7.2 saat planlı/plansız kesinti toleransı).
- **Barındırma:** Azure App Service — yerleşik yedeklilik ve otomatik yeniden başlatma desteği ile.

### 5.2. Loglama Stratejisi

| Seviye | Kullanım Durumu | Hedef |
|--------|----------------|-------|
| `Information` | Başarılı işlem akışları (kullanıcı girişi, ilan oluşturma) | Konsol / Dosya |
| `Warning` | Beklenen ancak dikkat gerektiren durumlar (rate limit yaklaşımı, geçersiz token) | Konsol / Dosya |
| `Error` | İşlenmiş fakat kritik hatalar (e-posta gönderim başarısızlığı) | Dosya / Veritabanı |
| `Fatal` | Sistemin devam edemeyeceği hatalar (DB bağlantı kopması) | Dosya / Veritabanı / Alert |

- Loglama altyapısı olarak **Serilog** kullanılacaktır. Sink'ler yapılandırmaya göre dosya ve/veya veritabanına yazabilir.
- Her log kaydına `CorrelationId` eklenerek istek bazlı log takibi mümkün kılınır.

### 5.3. Arka Plan Servisleri (Background Jobs)

- **Quartz.NET** tabanlı zamanlayıcı görevler:

| Görev | Çalışma Sıklığı | Açıklama |
|-------|----------------|----------|
| `ExpiredBidJob` | Her 15 dakikada bir | Süresi dolan ilanları `Expired` statüsüne alır. |
| `EmailDispatchJob` | Her 1 dakikada bir | E-posta kuyruğundaki bekleyen bildirimleri gönderir; başarısız görevleri yeniden dener (max 3 deneme). |

---

## 6. Sistem Bağımlılıkları (Dependencies)

| Bileşen | Versiyon / Detay | Kritiklik |
|---------|-----------------|-----------|
| **PostgreSQL** | 15+ | Zorunlu — ana veritabanı |
| **SMTP Sunucusu** | MailKit uyumlu herhangi bir sağlayıcı (Gmail, SendGrid vb.) | Zorunlu — e-posta bildirimleri için |
| **GitHub** | REST API v3 / Webhook Events | Zorunlu — proje aktivite takibi için |
| **Azure App Service** | F1 veya üstü plan | Zorunlu — barındırma |
| **.NET SDK** | .NET 8+ | Zorunlu — runtime |
| **Node.js** | 18+ | Frontend build için |

---

## 7. Bağlantılı Dokümanlar

| Doküman | Açıklama |
|---------|----------|
| `01-brd.md` | Business Requirements — İş gereksinimleri ve proje kapsamı. |
| `02-frd.md` | Functional Requirements — Kullanıcı hikayeleri, iş kuralları. |
| `04-sadm.md` | Sistem Mimarisi ve Veri Modeli — Mimari kararlar ve ER diyagramı. |
| `05-integration.md` | Entegrasyon Spesifikasyonları — GitHub Webhook ve MailKit detayları. |
