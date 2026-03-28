# ASP.NET Core Web API: Güvenli ve Ölçeklenebilir Yapılandırma (Configuration) Rehberi

Bir ASP.NET Core backend projesini GitHub'a (veya herhangi bir kaynak kontrol sistemine) göndermeden önce, yapılandırmanın (configuration) ve hassas verilerin (secrets) doğru yönetilmesi hayati önem taşır. Bu doküman, uygulamamızı sıfırdan "Production-Ready" (üretime hazır) standartlarda nasıl yapılandıracağımızı anlatır.

## 1. Yapılandırma Mimarisi (Configuration Architecture)

ASP.NET Core'da yapılandırma katmanlı bir yapıda çalışır. Ayarlar farklı kaynaklardan okunur ve son okunan değer, öncekini ezer (Override). 

Temel yapımız şu şekilde olmalıdır:
* `appsettings.json`: Temel ayarlar (Tüm ortamlar için geçerli, hassas olmayan varsayılan değerler).
* `appsettings.Development.json`: Yalnızca bilgisayarımızda (Local) çalışırken ezilecek geliştirme ayarları.
* `appsettings.Production.json`: Canlı ortamda çalışırken kullanılacak ayarlar.

**Ezme (Override) Hiyerarşisi (En düşük öncelikten en yükseğe):**
1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Sadece `Development` ortamında geçerlidir)
4. Environment Variables (Ortam Değişkenleri)
5. Command-line arguments (Komut satırı argümanları)

---

## 2. Hassas Veri Yönetimi (Secret Management)

**Kritik Kural:** Hiçbir şifre, API anahtarı veya bağlantı dizesi (Connection String) açık metin olarak `appsettings*.json` dosyalarında GitHub'a KESİNLİKLE gönderilmemelidir.

### Çözüm Yöntemi:
* **Geliştirme (Development) Ortamı:** `.NET User Secrets` kullanılır. Bu araç, şifreleri projenin bulunduğu klasörde değil, bilgisayarınızda (işletim sistemine ait özel ve güvenli bir dizinde) saklar. (.gitignore'a ihtiyaç bile duymaz çünkü proje dosyasında yer almaz).
* **Canlı (Production) Ortamı:** `Environment Variables` (Ortam Değişkenleri) veya Azure Key Vault / AWS Secrets Manager gibi bulut tabanlı şifre yöneticileri kullanılır.

---

## 3. Güçlü Tipli Yapılandırma (Strongly Typed Configuration)

Ayarları kod içerisinde `_configuration["Jwt:Key"]` şeklinde okumak yerine, **IOptions Pattern** kullanarak nesnelere (sınıflara) eşlemeliyiz. Bu sayede tip güvenliği (Type Safety) sağlarız.

### Adım 1: Option Sınıflarını Oluşturun

```csharp
// Core/MyProject.Application/Options/DatabaseOptions.cs
public class DatabaseOptions
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
}

// Core/MyProject.Application/Options/JwtOptions.cs
public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty; // Hassas Veri!
    public int ExpiryInMinutes { get; set; } = 60;
}
```

---

## 4. Güvenlik Kuralları ve Git Düzeni

### GitHub'a Gönderilecek Örnek `appsettings.json`
GitHub'da takım arkadaşlarınızın hangi ayarlara ihtiyaç duyduğunu bilmesi için "şablon" (`template`) niteliğinde değerler bırakmalısınız.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Database": {
    "ConnectionString": "", // Değer verilmez, Secret/Env beklenecek!
    "MaxRetryCount": 3
  },
  "Jwt": {
    "Issuer": "MyProject",
    "Audience": "MyProjectUsers",
    "SecretKey": "", // BOŞ BURAKIN Kullanıcı sırrından gelecek
    "ExpiryInMinutes": 120
  }
}
```

**Git Kuralları:**
* `appsettings.json`, `appsettings.Development.json` dosyaları Git tabanlı sürüm kontrolüne (GitHub) **gönderilir**. Ancak içlerinde şifre olmamalıdır!

---

## 5. Proje Yapısı ve Program.cs Entegrasyonu

Options nesnelerini `Program.cs`'de (veya Dependency Injection modüllerinizde) DI Container'a kaydetmelisiniz. (Ayrıca "Bonus" başlığındaki Validator kısmına bakın).

### Adım 1: Option'ları IoC (DI) Container'a Kaydetmek
```csharp
// Presentation/MyProject.API/Program.cs veya AddApiServices metodu içerisinde:

// Basit IOptions kaydı:
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

// Validation (Anotasyon doğrulaması) içeren güvenli kayıt (Önerilen)
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations() // Eksik config olursa proje başlamadan/istek gelince patlar
    .ValidateOnStart(); // Uygulama kalkarken ayarları test et!
```

---

## 6. Geliştirici Deneyimi (Developer Experience)

Bir geliştirici projeyi bilgisayarına ilk defa ("git clone" ile) indirdiğinde yapması gerekenler:

### 1. User Secrets'ı Başlatmak (CLI)
Projenin API katmanına gidin ve secrets mekanizmasını başlatın:
```bash
cd src/Presentation/MyProject.API
dotnet user-secrets init
```
*(Bu komut .csproj dosyanıza benzersiz bir `<UserSecretsId>` ekler.)*

### 2. Hassas Ayarları Geliştirici Bilgisayarına Eklemek
```bash
# Database connection string belirleme
dotnet user-secrets set "Database:ConnectionString" "Host=localhost;Database=myprojectdb;Username=postgres;Password=123"

# JWT Secret belirleme
dotnet user-secrets set "Jwt:SecretKey" "cok-gizli-super-uzun-bir-jwt-imza-anahtari-1234!!!"
```

### 3. Kod İçinde Okumak
Herhangi bir Controller veya Service içinde constructor (Dependency Injection) üzerinden IOptions, IOptionsSnapshot veya IOptionsMonitor ile okuyoruz:

```csharp
public class JwtProvider
{
    private readonly JwtOptions _jwtOptions;

    // IOptions yerine IOptionsSnapshot tavsiye edilir, ayar değişirse runtime'da güncellenir
    public JwtProvider(IOptionsSnapshot<JwtOptions> jwtOptions)
    {
         _jwtOptions = jwtOptions.Value;
    }
    
    public string GenerateToken()
    {
         // _jwtOptions.SecretKey burada güvenle kullanılır.
    }
}
```

---

## 7. Üretime Hazırlık (Production Readiness)

Sistem canlıya çıktığında `User Secrets` YOKTUR. Bunun yerine ortam değişkenleri (Environment Variables) devreye girer.

### Sunucuda (veya Docker/CI-CD)'de Değerleri Verme
ASP.NET Core'da nokta (`.`) veya iki nokta (`:`) yerine ortam değişkenlerinde `__` (iki adet alt tire) kullanılır.

**Linux / macOS Export (Örnek):**
```bash
export ASPNETCORE_ENVIRONMENT=Production
export Database__ConnectionString="Server=prod_ip;Database=proddb;User Id=dbuser;Password=super_secure_pw;"
export Jwt__SecretKey="prod-icin-uretilen-cok-daha-karma-sik-key"
```

**Docker / docker-compose.yml Örneği:**
```yaml
services:
  api:
    image: myproject:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Database__ConnectionString=Host=postgres;Database=myprojectdb;Username=prd;Password=prd_pass
      - Jwt__SecretKey=live-secret-key-buraya
```

Sistem çalışırken önce `appsettings.json` okunur (içindeki SecretKey boştur). Daha sonra Environment Variables okunur ve o boşluğu ezerek gerçek değeri doldurur. Kodda hiçbir şey değiştirmenize gerek kalmaz.

---

## 8. Bonus: Best Practices (En İyi Uygulamalar)

### A. DataAnnotations ile Fail-Fast (Erken Patlama) Sistemi
Eğer bir sistem ayağa kalkmak için JWT şifresine %100 ihtiyaç duyuyorsa ve birisi bunu canlı sunucuya girmeyi unuttuysa, hatayı *ilk kullanıcı login olduğunda (NullReferenceException)* fırlatmak **KÖTÜ UYGULAMADIR.**

Sistem başlatılır başlatılmaz çökmeli ve konsola "Jwt Secret Key bulunamadı!" yazmalıdır (Fail-Fast prensibi).

**Option Sınıfında Anotasyon:**
```csharp
using System.ComponentModel.DataAnnotations;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    
    [Required] // Zorunlu alan
    [MinLength(32, ErrorMessage = "JWT anahtarı yeterince güvenli değil (min 32 kar).")]
    public string SecretKey { get; set; } = string.Empty;
}
```

**Program.cs'te ValidateOnStart Kullanımı (Bölüm 5'te gösterildi):**
`.ValidateDataAnnotations().ValidateOnStart();` metodu çağrıldığında sunucu çalışır çalışmaz konfigürasyon doğrulanır. Eğer eksik ortam değişkeni (Environment variable) veya Secret varsa, proje `OptionsValidationException` fırlatır ve kapanır. Sorunu anında fark edersiniz!

### B. ConnectionStrings Standardı
Eğer eski nesil Entity Framework ayar mimarisini kullanıyorsanız, `ConnectionStrings` bloğu da özel bir yapı olarak geçer:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```
Bunu sunucuda ezmek için `ConnectionStrings__DefaultConnection` environment variable anahtarını kullanmalısınız.

### Özetle;
Bu adımları izleyerek projede:
1. Kaynak koda (GitHub'a) şifre sızmasını önlediniz.
2. Magic stringleri `"Jwt:Key"` bırakıp, `IOptions` standardına geçtiniz.
3. Eksik konfigürasyonda uygulamanın çalışma anına gelmeden çökmesini (`Fail-Fast`) sağladınız.
4. Geliştirici ve Üretim (Dev/Prod) ayrımını profesyonelce tamamladınız.
