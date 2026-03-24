# 01. Business Requirements Document (BRD) - Dev4All

## 1. Yönetici Özeti (Executive Summary)

**Dev4All**, yazılım hizmeti almak isteyen müşteriler (bireysel veya kurumsal) ile bağımsız geliştiricileri (freelance yazılım uzmanları, küçük takımlar) güvenli bir ekosistemde bir araya getiren **B2B/B2C tabanlı bir Proje Yönetim ve Eşleşme Platformudur.**

Platform; iletişim kopukluklarını, proje iptallerini ve takipsizliği ortadan kaldırmayı hedefler. Teklif yönetiminden GitHub destekli geliştirme sürecinin şeffaf izlenmesine kadar her adım, tek bir merkezi sistemde yürütülür.

> **Kapsam Notu:** Bu doküman yalnızca iş gereksinimlerini (Business Requirements) kapsar. Teknik ve işlevsel gereksinimler sırasıyla `02-frd.md` ve `03-nfr.md` dokümanlarında ele alınmaktadır.

---

## 2. Problem Tanımı

Yazılım projelerinde müşteri ile geliştirici arasındaki süreç bugün sistematik bir altyapıdan yoksundur:

| Sorun Alanı | Mevcut Durum (As-Is) | Dev4All ile Hedef (To-Be) |
|-------------|---------------------|--------------------------|
| **İletişim** | WhatsApp, e-posta, ofis araçları üzerinden dağınık şekilde yürür. | Tüm proje iletişimi tek platformda toplanır. |
| **Teklif Yönetimi** | Teklifler PDF/Excel formatında kalır; revizyon takibi imkânsızlaşır. | Teklifler karşılaştırmalı grid üzerinde şeffaf biçimde sunulur. |
| **Süreç Takibi** | Proje başladıktan sonra müşteri "kara kutu" içinde kalır; ilerlemeyi haftalarca e-posta sorarak öğrenir. | GitHub entegrasyonu ile commit aktiviteleri müşteriye canlı yansır. |
| **Güven** | Sözleşme ve teslim standartları yoktur; taraflar risk altındadır. | Rol tabanlı yetki sistemi ve durum makinesiyle süreç denetlenebilir hâle gelir. |

---

## 3. Vizyon ve Başarı Kriterleri (Vision & KPIs)

### 3.1. Vizyon

> Yazılım projelerinin fikir aşamasından kodlanıp teslim edilmesine kadar geçen sürecin, tek bir şeffaf merkezden güvenilir ve denetlenebilir biçimde yönetilmesini sağlamak.

### 3.2. Başarı Kriterleri (KPIs)

| KPI | Hedef | Ölçüm Yöntemi |
|-----|-------|---------------|
| **Kapsam Standardizasyonu** | Müşterilerin bütçe, teknoloji ve süre beklentilerini şablona uygun (ProjectRequest) şekilde iletebilmesi. | Oluşturulan proje ilanı sayısı ve tamamlanma oranı. |
| **Süreç Şeffaflığı** | Kabul edilen projelerin commit geçmişinin teknik bilgisi olmayan müşteriler tarafından "aktivite" boyutunda izlenebilmesi. | Aktivite timeline görüntüleme sıklığı. |
| **Hızlı Eşleşme** | Developer'ların uygun ilanlara hızlıca teklif dönebildiği rekabetçi bir ortamın oluşması. | İlan başına ortalama teklif sayısı ve teklif-kabul süresi. |
| **Platform Güvenilirliği** | MVP aşamasında %99 uptime ve <200ms API yanıt süresi hedefi. | Azure App Service monitoring verileri. |

---

## 4. Paydaş Analizi (Stakeholders)

| Rol | Tanım | Temel Beklenti |
|-----|-------|---------------|
| **Customer (Müşteri / İşveren)** | Yazılım ürününe ihtiyacı olan, platformda proje ilanı açan bireysel veya kurumsal kullanıcı. | Güvenilir geliştiricilere hızlıca ulaşmak ve proje ilerleyişini şeffaf şekilde takip etmek. |
| **Developer (Geliştirici)** | Müşteri taleplerini inceleyen, teklif veren ve anlaşma sağlandığında kodlamayı gerçekleştiren freelance uzman veya küçük takım. | Gerçek projelere erişmek, teklif sürecini kolayca yönetmek ve ödeme güvencesi sağlamak. |
| **Administrator (Sistem Yöneticisi)** | Platform güvenliğini, kullanıcı ve ilan denetimini sağlayan yetkili teknik personel. | Sistemi güvenli ve istikrarlı tutmak, kurallara aykırı içerikleri hızla kaldırmak. |
| **Geliştirici Takımı (Internal)** | Dev4All'ı inşa eden backend ve frontend geliştiricileri. | Açık, test edilebilir ve sürdürülebilir bir mimari üzerinde çalışmak. |

---

## 5. Proje Kapsamı

### 5.1. Kapsam Dahilinde (In-Scope — MVP)

| # | Özellik | Açıklama |
|---|---------|----------|
| 1 | **Kimlik Doğrulama** | JWT tabanlı, rol yetkili (Customer / Developer) kayıt ve giriş sistemi. |
| 2 | **Proje Talebi Yönetimi** | Müşterinin bütçe, deadline ve teknoloji yığınını belirttiği proje ilanı oluşturması, düzenlemesi ve listelemesi. |
| 3 | **Teklif Sistemi** | Developer'ların açık ilanlara fiyat ve süre teklifi (Bid) verebilmesi, tekliflerini güncelleyebilmesi. |
| 4 | **Teklif Onayı ve Proje Başlatma** | Müşterinin teklifi kabul etmesi, projenin "Aktif" duruma geçmesi ve Developer'a atanması. |
| 5 | **GitHub Entegrasyonu** | Aktif projeye GitHub Repo bağlanması; commit ve push aktivitelerinin Webhook aracılığıyla platforma aktarılması. |
| 6 | **E-posta Bildirimleri** | Kritik olaylar (yeni teklif, kabul/red, proje başlangıcı) için otomatik bildirimler. |
| 7 | **Zamanlama Servisleri** | Süresi dolan ilanların otomatik olarak `Expired` statüsüne alınması (Quartz.NET). |

### 5.2. Kapsam Dışında (Out-of-Scope — MVP Sonrası)

| Özellik | Kapsam Dışı Olma Gerekçesi |
|---------|--------------------------|
| **Escrow / Ödeme Sistemi** | Ödemenin platformda emanet tutulması ve projenin bitimiyle yazılımcıya aktarılması, finansal regülasyon ve yasal uyum gerektirmektedir. |
| **Platform İçi Canlı Chat** | Anlık mesajlaşma altyapısı (WebSocket/SignalR) MVP kapsam ve zaman çizelgesini aşmaktadır; iletişim şimdilik teklif notları ve e-posta ile sağlanır. |
| **Profil Değerlendirme / Derecelendirme** | Müşteri-geliştirici karşılıklı yorum ve puan sistemi ilerleyen versiyonlara bırakılmıştır. |
| **OAuth / Sosyal Giriş** | Google, GitHub OAuth entegrasyonu MVP'de yer almaz; yalnızca e-posta+şifre ile kimlik doğrulama desteklenir. |
| **Mobil Uygulama** | Native iOS/Android uygulaması ilk fazda geliştirilmeyecektir. |

---

## 6. İş Süreçleri ve Sistem Akışı

Platform üzerindeki temel iş süreci aşağıdaki aşamalardan oluşur:

```
1. Kayıt & Giriş
   └─ Customer veya Developer rolüyle platforma üye olunur.

2. Proje İlanı Açma
   └─ Customer, bütçe / deadline / teknoloji bilgilerini doldurarak ilan oluşturur.
      İlan "Open" statüsünde yayına girer.

3. Teklif Verme
   └─ Developer, açık ilanları listeler ve ilgilendiği ilana BidAmount + ProposalNote ile teklif verir.
      Customer, yeni teklif geldiğinde e-posta bildirimi alır.

4. Teklif Değerlendirme & Kabul
   └─ Customer, gelen teklifleri karşılaştırır ve uygun olanı "Kabul Et" butonuyla onaylar.
      Sistem projeyi "Ongoing" statüsüne alır; diğer teklifler otomatik reddedilir.

5. Geliştirme Süreci
   └─ Developer, projeye GitHub reposunu bağlar.
      Her commit/push, Webhook aracılığıyla platforma yansır.

6. Proje Takibi
   └─ Customer, Aktivite Timeline üzerinden commit geçmişini canlı izler.
      Proje tamamlandığında "Completed" statüsüne alınır.
```

---

## 7. Proje Riskleri ve Varsayımlar

### 7.1. Risk Tablosu

| # | Risk | Olasılık | Etki | Azaltma Stratejisi |
|---|------|----------|------|-------------------|
| R-01 | **GitHub API Rate Limit Aşımı** — Yoğun kullanımda GitHub API kota sınırına ulaşılması. | Orta | Yüksek | Webhook tabanlı mimari kullanılarak yalnızca değişen veriler çekilir; rate limit proaktif şekilde izlenir. |
| R-02 | **Webhook Güvenliği** — Sahte payload enjeksiyonu ile yetkisiz commit kaydı oluşturulması. | Düşük | Yüksek | HMAC-SHA256 imza doğrulaması zorunlu tutulur; imzasız istekler reddedilir. |
| R-03 | **Veri Tutarsızlığı** — Teklif kabul işlemi sırasında eşzamanlı (concurrent) güncelleme çakışması. | Orta | Yüksek | Kabul işlemi veritabanı transaction'ı (Unit of Work) içinde atomik olarak gerçekleştirilir. |
| R-04 | **E-posta Gönderim Başarısızlığı** — SMTP sunucu kesintisi nedeniyle bildirimlerin gönderilememesi. | Düşük | Orta | E-postalar Quartz.NET tabanlı kuyruk (Job Queue) ile asenkron gönderilir; başarısız görevler yeniden denenir. |
| R-05 | **Kapsam Sürüklenmesi (Scope Creep)** — Geliştirme sürecinde yeni özellik taleplerinin MVP takvimini aksatması. | Orta | Orta | Kapsam değişiklikleri bu BRD üzerinden değerlendirilir; MVP dışı talepler "backlog" olarak işaretlenir. |

### 7.2. Varsayımlar

| # | Varsayım |
|---|----------|
| A-01 | Platforma dahil olan Developer'ların Git ve GitHub kullanımına hâkim oldukları varsayılmaktadır. |
| A-02 | MVP aşamasında eş zamanlı maksimum 100 aktif kullanıcı öngörülmektedir. |
| A-03 | E-posta gönderimi için geçerli bir SMTP sunucusuna (MailKit uyumlu) erişim sağlanmış olacaktır. |
| A-04 | Veritabanı olarak PostgreSQL 15+ kullanılacak ve ortam Azure App Service üzerinde barındırılacaktır. |

---

## 8. Teknoloji Yığını (Özet)

| Katman | Teknoloji |
|--------|-----------|
| **Backend** | .NET Core Web API, Onion Architecture, CQRS (MediatR), EF Core, FluentValidation, JWT, Serilog, Quartz.NET, MailKit |
| **Veritabanı** | PostgreSQL 15+ |
| **Frontend** | React + Vite, TypeScript, Tailwind CSS |
| **Entegrasyon** | GitHub Webhook API, MailKit (SMTP) |
| **Altyapı** | Azure App Service, CI/CD Pipeline, Swagger (API Dokümantasyon) |

> Detaylı mimari ve veri modeli için `04-sadm.md`; entegrasyon spesifikasyonları için `05-integration.md` dokümanlarına bakınız.

---

## 9. Bağlantılı Dokümanlar

| Doküman | Açıklama |
|---------|----------|
| `02-frd.md` | Functional Requirements Document — Kullanıcı hikayeleri, Use Case'ler, iş kuralları. |
| `03-nfr.md` | Non-Functional Requirements — Performans, güvenlik, loglama gereksinimleri. |
| `04-sadm.md` | Sistem Mimarisi ve Veri Modeli — Onion Architecture, ER diyagramı, CQRS özeti. |
| `05-integration.md` | Entegrasyon Spesifikasyonları — GitHub Webhook ve MailKit detayları. |
