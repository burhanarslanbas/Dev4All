# Onion Architecture: Mülakat Hazırlık Rehberi

Bu doküman, Dev4All projesinde kullanılan Onion Architecture (Soğan Mimarisi) hakkında genel bilgiler, katmanların sorumlulukları ve Clean Architecture ile olan farklılıklarını içermektedir. Mülakat süreçlerinde referans olarak kullanılabilir.

## 1. Neden Onion Architecture Kullanıyoruz?

Modern yazılım geliştirme süreçlerinde iş kurallarının (Business Logic) dış etkenlerden (veritabanı, UI, dış servisler vb.) izole edilmesi kritik öneme sahiptir. Onion Architecture bu izolasyonu sağlamak için tasarlanmıştır.

**Temel Faydaları:**
- **İzolasyon ve Test Edilebilirlik:** İş kuralları (Domain) hiçbir teknolojiye bağımlı olmadığı için, veritabanı veya web sunucusuna ihtiyaç duymadan unit testler ile kolayca sınanabilir.
- **Esneklik ve Değiştirilebilirlik:** Veritabanı (Örn: SQL Server'dan PostgreSQL'e geçiş) veya dış servis (Örn: RabbitMQ'dan Kafka'ya geçiş) değişiklikleri merkeze (Domain ve Application katmanlarına) dokunmadan yapılabilir.
- **Sürdürülebilirlik:** Bağımlılıklar içe doğru aktığı için spagetti kod oluşumunu engeller; projeye yeni dahil olan geliştiricilerin sistemi anlamasını kolaylaştırır.
- **Framework Bağımsızlık:** İş kuralları framework'ün kısıtlamalarına göre değil, saf nesne yönelimli programlama (OOP) ve Domain-Driven Design (DDD) prensiplerine göre yazılır.

---

## 2. Katmanlar ve Sorumlulukları

Onion Architecture içten dışa doğru katmanlardan oluşur ve en önemli kuralı **Bağımlılığın İçe Doğru Olmasıdır (Inversion of Control)**. Dış katmanlar iç katmanları bilir, ancak iç katmanlar asla dış katmanları bilemez.

### 2.1. Domain Layer (Çekirdek - En İç Katman)
Tüm sistemin kalbidir. Sisteme dair hiçbir teknolojik altyapıyı veya framework'ü tanımaz.
- **İçerik:** Entity'ler, Value Object'ler, Domain Exception'ları, Interface'ler (Repository soyutlamaları buraya konulabileceği gibi bizim yapımızda Application katmanındadır), Enum'lar.
- **Kural:** Entity Framework, ASP.NET Core veya herhangi bir 3. parti kütüphane referansı barındırmaz (sadece temel C# tipleri). İş kuralları burada tanımlıdır.

### 2.2. Application Layer (Uygulama Mantığı)
Sistemin senaryolarını (Use Cases) tanımlar.
- **İçerik:** CQRS operasyonları (Command, Query handlerları), DTO'lar, Validation'lar (FluentValidation v.b.), Domain servisleri, arayüz soyutlamaları (Loglama, Email atma, Veritabanı işleri için Interface'ler).
- **Bağımlılık:** Sadece **Domain** katmanını referans alır.
- **Kural:** Nereden veri çekileceğini veya verinin nereye yazılacağını bilmez. Sadece arayüzleri (Interface) kullanarak I/O işlemlerini dışarıya delege eder. Veritabanı referansı Application katmanında olmaz.

### 2.3. Infrastructure & Persistence Layer (Altyapı ve Veri Kalıcılığı)
Somut implementasyonların yer aldığı dış katmandır.
- **İçerik:** Entity Framework Core (DbContext, Migration'lar), MailKit (E-posta gönderimi yapılandırmaları), Identity servisleri, cache entegrasyonları, 3. parti API çağrıları.
- **Bağımlılık:** **Application** katmanında tanımlı arayüzleri uygular (implement eder). Domain katmanını dolaylı yoldan (Application üzerinden) bilir.
- **Kural:** Veritabanına veri nasıl yazılacak, email nasıl gidecek gibi teknik "nasıl" sorularının cevabı buradadır. Application katmanındaki sözleşmeleri (interface) yerine getirir.

### 2.4. Presentation / API Layer (Sunum Katmanı)
Kullanıcının veya dış uygulamaların bizim sistemimizle konuştuğu dış kabuktur.
- **İçerik:** Web API Controller'ları, Global Exception Middleware'leri, Request/Response objeleri, Authentication/Authorization filter'ları, Swagger konfigürasyonu.
- **Bağımlılık:** İdeal olarak sadece **Application** katmanını referans etmelidir. (Sisteme DI container ayağa kalkarken Injection yapabilmek için bazen Infrastructure katmanı da referans eklenir, ancak kod içinde nesneleri direkt olarak new'lenmez).
- **Kural:** Controller'ların içinde asla business logic barındırılmaz. Sadece "Request'i al, Application katmanındaki handler'a (MediatR) gönder ve Response dön" sorumluluğundadır.

---

## 3. Clean Architecture vs Onion Architecture

Hem Clean (Temiz) Architecture hem de Onion (Soğan) Architecture aslında **Ports and Adapters (Hexagonal)** mimarisinin farklı felsefelerle anlatılmış türevleridir. Amacı ikisinin de aynıdır: İş kurallarını detaylardan ayırmak.

### Benzerlikleri:
1. **Dependency Rule:** Her iki mimaride de en önemli prensiptir. Kaynak kod bağımlılıkları sadece içeri (yüksek seviyeli politikalara/iş kurallarına) doğru işaret etmelidir.
2. **Framework'ten Bağımsızlık:** İkisi de uygulamanın çekirdeğini herhangi bir web çatısı veya veritabanı kütüphanesinden izole eder.
3. **Test Edilebilirlik:** İkisi de DI (Dependency Injection) prensibi ile dış katmanların mock'lanmasını teşvik edip, unit testleri basitleştirir.

### Temel Farkları:
- **Terminoloji ve Odak:** Clean Architecture genellikle Use-Case'ler (Interactors) üzerinden hikayeyi anlatırken; Onion Architecture, ismini merkezdeki "Domain" etrafında soğan zarı gibi iç içe geçen katmanlardan alır.
- **Soyutlamaların Yeri (Interfaces):**
  - **Onion Architecture'da** bazen repository sözleşmeleri (interfaces) en iç katman olan Domain'e konulur. (Ancak Dev4All gibi bir çok projede Application seviyesine taşınabilmektedir, bu ekip tercihidir).
  - **Clean Architecture'da** ise genel olarak "Use Cases (Application)" katmanı, veri erişim katmanının Interface'lerini tanımlar, Entities katmanında sadece düz nesneler (POCO) bulundurulur. (Bizim projemizde uyguladığımız yöntem daha çok Clean Architecture esintileri taşıyan hibrit bir Onion yapısıdır).

---

## 4. Olası Mülakat Soruları ve Cevapları

**Soru: Katmanlar arası bağımlılık kuralını ihlal ettiğimizi nasıl anlarız?**
> *Cevap:* Eğer Domain veya Application katmanlarında `Microsoft.EntityFrameworkCore` kullanarak bir Context çağırıyorsak, HTTP Client kullanarak dış bir web isteği atıyorsak veya Configuration okuyorsak katmanlar arası bağımlılık kuralını ihlal etmiş ve UI/Altyapı bileşenlerini içeri taşımışız demektir.

**Soru: Veritabanından gelen entity'leri doğrudan API response'u olarak dönmek (expose etmek) Onion mimaride neden yanlış kabul edilir?**
> *Cevap:* Entity'lerimiz sistemimizin iç modelidir. API response'ları dış dünya sözleşmelerimizdir (Contract). Entity'leri dışarı açmak; veritabanı tasarımımız değiştiğinde API sözleşmelerimizin kırılmasına sebep olur (Coupling). Ayrıca, ilişkili verilerde "Circular reference" limitasyonları ile karşılaşabiliriz, bazen de UI'ın görmemesi gereken verileri dışarı açabiliriz. DTO (Data Transfer Object) kullanımı bu yüzden elzemdir.

**Soru: CQRS ve MediatR ile Onion Architecture'ı neden birlikte kullandık?**
> *Cevap:* CQRS okuma (Query) ve yazma (Command) operasyonlarını fiziksel veya mantıksal olarak ayırmamızı sağlar. Application katmanı bu operasyonların birer `Use Case` (Handler) olarak tek sorumluluk prensibi (SRP) gereği ayrışmasını destekler. MediatR ise API controller'larımızın Application katmanındaki Handler'lara sıkı sıkıya bağlanmasını engelleyen bir mesajlaşma görevi görür, böylelikle Presentation katmanı temiz kalır.
