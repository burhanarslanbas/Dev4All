# Kullanılan Kütüphaneler (NuGet Paketleri) ve Detayları

Dev4All projesinde Layered (Onion) Architecture yapısına uygun olarak her katmanın sorumluluğuna göre farklı kütüphaneler kullanılmıştır. Bu kütüphanelerin ne işe yaradığı, neden seçildikleri ve nasıl kullanıldıkları mülakatlarda veya proje tanıtımlarında kullanabilmeniz için aşağıda açıklanmıştır:

---

## 1. Application Katmanı Kütüphaneleri

Bu katman sistemin senaryolarını (Use Cases) barındırdığı için sadece işlem yönetimi (CQRS, MediatR) ve doğrulama (Validation) kütüphanelerine bağımlıdır.

### 1.1. `MediatR`
- **Ne İşe Yarar?** Mediator design pattern'in (Arabulucu Tasarım Deseni) .NET üzerindeki popüler bir uygulamasıdır. Nesnelerin (sınıfların) birbirleriyle doğrudan iletişim kurmasını engelleyip, tüm iletişimi tek bir merkezi nesne (Mediator) üzerinden yönetmeyi sağlar.
- **Neden Kullanıyoruz?** CQRS (Command Query Responsibility Segregation) prensibini uygulamak ve API (Presentation) katmanını, Application katmanına sıkı sıkıya (Tightly Coupled) bağlamaktan kaçınmak için kullanıyoruz. API'den gönderilen Request'ler doğrudan servis sınıflarının new'lenmesi veya inject edilmesi yerine MediatR üzerinden ilgili `Handler` sınıfına yönlendirilir.
- **Kullanım Formülü:** 
  `Request (Command/Query) nesnesi gönderilir -> MediatR bu isteği yakalar -> İlgili Response dönen Handler nesnesi çalışır.`

### 1.2. `FluentValidation` & `FluentValidation.DependencyInjectionExtensions`
- **Ne İşe Yarar?** Veri doğrulama (Validation) işlemlerini if-else bloklarına boğmadan, akıcı (fluent) bir dil (Builder pattern) ile tanımlamamızı sağlayan açık kaynaklı bir kütüphanedir.
- **Neden Kullanıyoruz?** Data Annotations (`[Required]`, `[MinLength]`) gibi attribute tabanlı doğrulamalar Entity veya DTO sınıflarını kirlettiği için tercih edilmez. FluentValidation sayesinde doğrulama kurallarımızı DTO'lardan ayırarak farklı bir sınıf (Validator) içerisine yazarız (Seperation of Concerns).
- **Kullanımı:** `AbstractValidator<T>` sınıfından miras alınarak ilgili sınıf için kural kümesi oluşturulur (`RuleFor(x => x.Email).NotEmpty().EmailAddress();`).

### 1.3. `Microsoft.Extensions.DependencyInjection.Abstractions`
- **Ne İşe Yarar?** ASP.NET Core'un Dependency Injection (DI) altyapısının temel interface'lerini (Örn: `IServiceCollection`) barındıran kütüphanedir.
- **Neden Kullanıyoruz?** Application katmanındaki tüm Mediator ve Validator kayıtlarını tek bir extension method (Örn: `AddApplicationServices`) içinde toplayıp DI konteynerına verebilmek için bu arayüzü referans almamız gerekir.

---

## 2. Infrastructure Katmanı Kütüphaneleri

Dış servis entegrasyonları, e-posta, arka plan görevleri ve Auth işlemlerinin teknik altyapıları bu katmandadır.

### 2.1. `MailKit`
- **Ne İşe Yarar?** .NET platformunda e-posta gönderimi ve alımı için kullanılan `System.Net.Mail` kütüphanesinin yerini alan, modern, hızlı ve çapraz platform destekli kütüphanedir.
- **Neden Kullanıyoruz?** Şifre sıfırlama, hesaba hoşgeldin, sözleşme güncellemeleri veya müşteri etkileşimleri için SMTP üzerinden asenkron şekilde mail gönderebilmekte kullanıyoruz.

### 2.2. `Quartz` & `Quartz.Extensions.Hosting`
- **Ne İşe Yarar?** Gelişmiş, açık kaynaklı bir görev zamanlama (Job Scheduling) aracıdır.
- **Neden Kullanıyoruz?** Projemizde periyodik şekilde çalışması gereken belli işler (Background Jobs) için kullanılır (Örneğin: "Ödemesi yapılmayan sözleşmeleri haftada bir iptal et" veya "Her gece GitHub'dan Log çek"). `Extensions.Hosting` paketi sayesinde ASP.NET Core'un HostedService altyapısına doğrudan kolay bir entegrasyon sağlanır.

### 2.3. `Microsoft.AspNetCore.Authentication.JwtBearer` & `System.IdentityModel.Tokens.Jwt`
- **Ne İşe Yarar?** JSON Web Token (JWT) tabanlı kimlik doğrulama yapılabilmesini sağlar.
- **Neden Kullanıyoruz?** RESTful API uygulamamızın kullanıcı kimliklerini Stateless (durumsuz) olarak yönetebilmesi, Frontend (React) projemizin Header ile göndereceği JWT token'ını doğrulayıp kullanıcının UserId, Role gibi (`Claims`) bilgilerini çıkarmak için kullanılır.

### 2.4. `Microsoft.Extensions.Configuration.Abstractions`
- **Ne İşe Yarar?** `IConfiguration` arayüzünü barındırır.
- **Neden Kullanıyoruz?** Infrastructure katmanındaki nesnelerin (Örneğin: MailServer ayarları veya JwtKey ayarları) `appsettings.json` içerisinden değer okuyabilmesini sağlamak için kullanılır.

---

## 3. Persistence Katmanı Kütüphaneleri

Veritabanı işlemleri ve nesne-ilişkisel eşleme (ORM) bu katmanın sorumluluğundadır.

### 3.1. `Microsoft.EntityFrameworkCore` & `Microsoft.EntityFrameworkCore.Design`
- **Ne İşe Yarar?** Microsoft'un geliştirdiği ileri seviye, performanslı modern ORM (Object-Relational Mapper) aracıdır. `.Design` ise komut satırı aracıyla Code-First migration komutları (Add-Migration, Update-Database) atabilmemizi sağlar.
- **Neden Kullanıyoruz?** Veritabanı ile Entity'lerimiz (C# POCO Sınıflarımız) arasındaki dönüşümü güvenli bir biçimde map etmek ve CRUD işlemleri için saf SQL (Ado.net) yazmaktan kaçınarak Repository'lerimizi asenkron LINQ komutlarıyla besleyebilmek için.

### 3.2. `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Ne İşe Yarar?** Entity Framework Core'un **PostgreSQL** veritabanı ile konuşabilmesi için .NET ekibi ve açık kaynak toplulukları ile birlikte geliştirilen PostgreSQL Provider'ıdır.
- **Neden Kullanıyoruz?** Projemizin veri kaynağı (Database) olarak ücretsiz, gelişmiş veri tiplerini (JSONB gibi) barındıran güçlü bir RDBMS olan PostgreSQL'i seçtiğimiz için.

### 3.3. `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- **Ne İşe Yarar?** ASP.NET Core Identity altyapısının EF Core ile entegre bir şekilde çalışmasını ve Identity tablolarının (`AspNetUsers`, `AspNetRoles` vb.) oluşturulmasını sağlayan araçtır.
- **Neden Kullanıyoruz?** Projede rol bazlı yetkilendirme (Customer, Developer, Admin), parola hash'leme (PasswordHasher), Lockout kuralları gibi işlemleri sıfırdan bizim yazmamız yerine kanıtlanmış bir AAA (Authentication, Authorization, Accounting) framework'ü kullanmak için.

---

## 4. WebAPI (Presentation) Katmanı Kütüphaneleri

### 4.1. `Swashbuckle.AspNetCore`
- **Ne İşe Yarar?** Swagger Open API özelliklerini entegre eder.
- **Neden Kullanıyoruz?** API Endpoint'lerimizi (Controller / Action'ları) dokümante etmek, frontend geliştiricisinin (React) veya üçüncü parti sistemlerin bizim API'mizde hangi verileri istediğimizi ve hangi tiplerde response verdiğimizi görmesi (Swagger UI üzerinden doğrudan test edilebilmesi) için kullanıyoruz.

### 4.2. `Microsoft.AspNetCore.Authentication.JwtBearer` (Yeniden dahil edildi)
- **Neden Kullanıyoruz?** `Program.cs` (.NET 6+) üzerinde `AddAuthentication` ve `AddJwtBearer` middleware'ini HTTP request tarafına ekleyebilmek için WebAPI projesine doğrudan kurmamız gerekir. Bu paket Middleware entegrasyonu içindir.
