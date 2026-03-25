Dev4All Web Uygulaması Özellikleri ve Geliştirme Planı
1. Problem Tanımı
Yazılım hizmeti almak isteyenlerle geliştiriciler farklı platformlarda iletişim kurmakta, bu su reçte gu ven, su reç takibi
ve iş ilerleme kontrolu gibi sorunlar yaşanmaktadır. Proje talepleri, teklifler ve so zleşme su reçleri çoğunlukla
sistematik şekilde takip edilmemekte, bu da gecikmelere ve yanlış anlaşılmalara yol açmaktadır. Mu şteriler, teknik
su reç nedeniyle projenin ilerleyişini yeterince izleyememektedir. Tu m bu problemler, teklif ve proje takibini merkezi
bir platformda yo netme ihtiyacını ortaya çıkarmaktadır.
2. Hedef Kullanıcı
Dev4All platformunun hedef kullanıcı kitlesi aşağıdaki gruplardan oluşmaktadır:
• Yazılım projesi geliştirmek isteyen bireyler veya kurumlar (mu şteriler)
• Freelance çalışan yazılım geliştiriciler
• Ku çu k yazılım ekipleri
Bu kullanıcılar, yazılım projelerinin teklif, so zleşme ve geliştirme su reçlerini daha du zenli ve gu venilir bir şekilde
yo netmek isteyen kişilerden oluşmaktadır.
3. Problemin Önemi ve Etkisi
Yazılım projelerinde teklif ve so zleşme su reçlerinin sistematik yu ru tu lmemesi, taraflar arasında iletişim ve ilerleme
takibinde sorunlara yol açmakta; o zellikle freelance projelerde mu şteri ve geliştirici açısından riskler doğurmaktadır.
Dev4All platformu, bu problemlerin o nu ne geçerek proje su reçlerini tek bir sistemde gu venli ve du zenli şekilde
yo netmeyi hedeflemektedir.
4. Uygulamanın Kapsamı
Dev4All platformu aşağıdaki temel işlevleri içerecektir:
• Mu şterilerin yazılım proje talepleri oluşturabilmesi
• Yazılımcıların oluşturulan proje taleplerine teklif verebilmesi
• Mu şteri ve geliştirici arasında teklif değerlendirme ve onay su recinin yu ru tu lmesi
• Tarafların anlaşması sonucunda proje geliştirme su recinin başlatılması
• GitHub entegrasyonu aracılığıyla proje deposunun sisteme bağlanması
• Proje geliştirme su recinin commit ve depo aktiviteleri u zerinden takip edilebilmesi
5. Teknoloji Yığını
5.1 Backend Teknik Altyapı
• .NET Core Web API Uygulaması
• Onion Architecture
• Global Exception Middleware
• CQRS, Repository, Unit of Work Desenleri
• Entity Framework Core, .Net Core Identity, MediatR, FluentValidation, MailKit, Quartz Ku tu phaneleri
• JWT Authentication & Authorization
• PostgreSQL Veritabanı
• Azure App Service (CI/CD)
• Swagger API Documentation
5.2 Frontend Teknik Altyapı
• React + Vite
• Tailwind CSS
• TypeScript
6. 8 Haftalık Web & Backend Geliştirme Planı
1. Hafta — Proje Analizi ve Sistem Akışı (16 Mart - 22 Mart)
• Projenin amacını ve problem tanımını netleştirme
• Kullanıcı rollerini belirleme (Customer, Developer)
• Platformun temel o zelliklerini listeleme
• Sistem akışını çizme
Temel akış:
1. Customer → Project Request oluşturur
2. Developer → Teklif verir
3. Customer → Teklifi kabul eder
4. Project → Başlar
5. Repo → Bag lanır
2. Hafta — Sistem Mimarisi ve Proje Altyapısının Oluşturulması (23 Mart - 29 Mart)
• Solution ve proje yapısının oluşturulması
• Katmanlı mimarinin kurulması
Oluşturulacak katmanlar:
• Domain
• Application
• Infrastructure / Persistence
• Web API
• Domain katmanındaki entity sınıflarının oluşturulması
• Global middleware yapılandırmalarının eklenmesi (exception handling vb.)
3. Hafta — Entity Framework ve Veritabanı (30 Mart – 5 Nisan)
• DbContext oluşturma
• Entity Framework kurulumu
• Migration oluşturma
• Veritabanını oluşturma
• I lk seed verilerinin eklenmesi
4. Hafta — Frontend Mimarisinin Oluşturulması (6 Nisan – 12 Nisan)
• Frontend projesinin oluşturulması ve klaso r yapısının planlanması
• Routing ve temel sayfa yo nlendirme yapısının kurulması
• API iletişimi için servis yapısının hazırlanması
• Ortak UI bileşenleri ve genel stil yapısının oluşturulması
5. Hafta — Landing Page ve Tasarım Ekranları (13 Nisan – 19 Nisan)
• Landing page tasarımının oluşturulması
• Login ve Register sayfalarının geliştirilmesi
• Uygulama genel layout ve navigasyon yapısının kurulması
• Proje listeleme ekranının (dummy veri ile) hazırlanması
ÖNEMLİ NOT:
6. Haftadan itibaren geliştirilen backend modüllerinin web tarafındaki entegrasyonları da birlikte
yapılacaktır.
6. Hafta — Kullanıcı Sistemi (20 Nisan – 26 Nisan)
• Kullanıcı kayıt API'si
• Kullanıcı giriş API'si
• JWT authentication kurulumu
• Basit kullanıcı profil endpoint'i
7. Hafta — Proje Talebi Sistemi (27 Nisan – 3 Mayıs)
• ProjectRequest oluşturma
• ProjectRequest listeleme
• ProjectRequest detay go ru ntu leme
• Kullanıcının kendi taleplerini go rmesi
8. Hafta — Teklif Sistemi (4 Mayıs - 10 Mayıs)
• Developer'ın projelere teklif verebilmesi
• Tekliflerin listelenmesi
• Kullanıcının verdig i teklifleri go rmesi
9. Hafta — Teklif Onay ve Proje Başlatma (11 Mayıs - 17 Mayıs)
• Mu şterinin teklifleri go ru ntu lemesi
• Teklif kabul / reddetme
• Kabul edilen tekliften proje oluşturma
• Project durum yo netimi
10. Hafta — GitHub Repo Takibi (18 Mayıs - 24 Mayıs)
• Proje için GitHub repo bag lantısı ekleme
• Repo bilgisini sistemde saklama
• Repo aktivitelerini go ru ntu leme
• Proje detay sayfasında repo bilgisi go sterme