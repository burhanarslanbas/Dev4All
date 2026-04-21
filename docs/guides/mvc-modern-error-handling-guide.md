# ASP.NET Core MVC için Modern Hata Yönetimi Rehberi (2026)

Bu doküman, ASP.NET Core MVC uygulamalarında güncel ve sürdürülebilir hata yönetimi yaklaşımını açıklar. Hedefimiz:

- Kullanıcıya güvenli ve anlaşılır hata mesajı göstermek
- Geliştiriciye merkezi log ve izlenebilirlik sağlamak
- Controller seviyesindeki tekrar eden `try-catch` bloklarını azaltmak
- API ve MVC katmanları arasında tutarlı bir hata stratejisi kurmak

## Kısa Cevap: En Modern Yaklaşım Nedir?

2026 itibarıyla ASP.NET Core tarafında en modern ve önerilen temel yapı:

1. **Global exception handling middleware** (`UseExceptionHandler`)
2. **Merkezi exception handler sınıfı** (`IExceptionHandler`)
3. **Standart hata kontratı** (`ProblemDetails`, RFC 7807)
4. **Beklenen iş/validasyon hatalarını lokal olarak ele alma**, beklenmeyen hataları globale bırakma

Bu kombinasyon, hem API hem MVC senaryolarında temiz mimari ve operasyonel gözlemlenebilirlik sağlar.

## Neden Bu Yaklaşım?

- **Tek noktadan yönetim:** Beklenmeyen tüm hatalar tek yerde ele alınır.
- **Tutarlılık:** Farklı controller'larda farklı hata formatları oluşmaz.
- **Güvenlik:** Stack trace gibi hassas detaylar son kullanıcıya sızmaz.
- **Bakım kolaylığı:** `try-catch` tekrarları azalır, controller'lar sadeleşir.
- **Gözlemlenebilirlik:** Log, correlation ID, trace bilgisi merkezi yönetilir.

## Mimari Karar: Middleware mi, Exception Filter mı?

Modern varsayılan yaklaşım:

- **Birincil katman:** Middleware tabanlı global handler (`UseExceptionHandler` + `IExceptionHandler`)
- **İkincil (opsiyonel):** MVC Exception Filter, sadece action'a özgü bir davranış gerçekten gerekiyorsa

Pratikte çoğu projede middleware yeterlidir. Filter yaklaşımı daha niş senaryolar için kullanılmalıdır.

## MVC Uygulamalarında Doğru Desen

MVC uygulamalarında iki farklı hata tipi vardır:

1. **Beklenen hatalar (business/integration):**  
   Örnek: Login başarısız, doğrulama hatası, yetkisiz erişim  
   -> Controller veya servis katmanında kullanıcıya anlamlı mesaj üretilebilir.

2. **Beklenmeyen hatalar (bug/infra):**  
   Örnek: NullReferenceException, ağ kesintisi, serialization hatası  
   -> Global handler tarafından loglanmalı, güvenli fallback response verilmelidir.

Bu yüzden doğru model: **“local handling for expected errors + global handling for unexpected errors”**.

## Önerilen Uygulama Planı (.NET 8/9/10)

### 1) Program.cs servis kayıtları

```csharp
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
```

### 2) Pipeline

```csharp
app.UseExceptionHandler();
```

> Not: `UseExceptionHandler` pipeline'da erken konumlandırılmalıdır ki aşağıdaki middleware/controller hatalarını yakalayabilsin.

### 3) IExceptionHandler implementasyonu

Handler içinde:

- Exception type -> HTTP status code eşlemesi
- `ProblemDetails` üretimi
- Structured logging (ErrorId/TraceId/Path)
- Production'da hassas detayları gizleme

### 4) MVC response stratejisi

MVC uygulaması için iki çıktı stratejisi önerilir:

- **HTML request** ise kullanıcı dostu error page (`/Home/Error` gibi)
- **JSON/AJAX request** ise `ProblemDetails` JSON

Bu ayrımı `Accept` header veya endpoint karakteristiği ile yapabilirsiniz.

## Dev4All için Önerilen Strateji

Sizin yapı için ideal model:

- `backend/src/Presentation/Dev4All.WebAPI`: mevcut global middleware yaklaşımı korunmalı (zaten doğru yönde)
- `frontend/Dev4All.Web`: mevcut `UseExceptionHandler("/Home/Error")` devam etmeli
- `frontend` controller'larındaki geniş `catch (Exception)` blokları azaltılmalı
- Status code bazlı beklenen durumlar (401, 400 validation gibi) lokal olarak ele alınmalı

Bu sayede kullanıcı mesajları bozulmadan kod sadeleşir.

## Controller Seviyesinde Ne Kalmalı?

Kalmalı:

- `HttpRequestException` (401/403/400 gibi beklenen senaryolar)
- Validation parse ve `ModelState` doldurma
- Kullanıcıya dönülecek domain dostu mesaj

Kaldırılabilir veya minimize edilebilir:

- Genel `catch (Exception)` fallback blokları (global handler üstlensin)

## Logging ve Operasyonel Best Practice

- Her hata için `TraceIdentifier` veya correlation ID loglayın.
- Log seviyelerini ayırın:
  - Validation/business -> Warning
  - Unexpected/system -> Error
- Tek format kullanın (structured logging): `{@Exception}`, `Path`, `UserId`, `TraceId`
- Uygulama içi gözlem için merkezi log sistemi (Seq, ELK, Grafana Loki, Azure App Insights vb.) kullanın.

## Güvenlik Kuralları

- Production ortamında stack trace ve iç exception mesajlarını kullanıcıya göstermeyin.
- Hata mesajlarında connection string, token, secret, PII sızdırmayın.
- Error page üzerinde generic mesaj kullanın, teknik detayı yalnızca log'da tutun.

## Aşamalı Geçiş Planı (Risk Düşük)

1. Global handler'ı devreye alın (`IExceptionHandler` + `AddProblemDetails`).
2. Sadece bir controller'da (ör. Auth) geniş `catch (Exception)` bloklarını kaldırın.
3. Beklenen exception mapping'lerini koruyun.
4. Staging ortamında log ve UX çıktısını doğrulayın.
5. Diğer controller'lara aynı deseni yaygınlaştırın.

## Sonuç

Evet, **try-catch ağırlıklı yaklaşımdan global exception handling'e geçmek doğru karar**.  
Ancak MVC uygulamalarında en iyi sonuç için yaklaşım hibrit olmalıdır:

- **Global:** beklenmeyen sistem hataları
- **Local:** kullanıcıya işlevsel geri bildirim gereken beklenen hatalar

Bu model, temiz mimari prensipleriyle uyumlu, modern ve uzun vadede sürdürülebilir yaklaşımdır.

## Kaynaklar

- [Microsoft Learn - Handle errors in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-9.0)
- [Microsoft Learn - Handle errors in ASP.NET Core APIs](https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-8.0)
- [GitHub Docs Source - ASP.NET Core error-handling.md](https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/fundamentals/error-handling.md)
- [Microsoft API Reference - IExceptionHandler](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.diagnostics.iexceptionhandler?view=aspnetcore-9.0)
