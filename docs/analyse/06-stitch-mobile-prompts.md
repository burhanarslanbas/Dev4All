# 06. Dev4All - Mobil Tasarım Kılavuzu (Stitch Prompts)

Bu doküman, Dev4All projesinin MVP sonrası hedeflerine uygun olarak mobil arayüzlerinin (iOS/Android) Stitch gibi bir AI UI tasarım aracında eksiksiz ve modern bir şekilde tasarlanabilmesi için oluşturulmuş "Tasarım Prompt (Komut) Setini" içerir.

Renk paleti önerisi: Projeye güven ve profesyonellik katacak **mavi-lacivert tonları (Tech Blue)** ve temiz bir beyaz/açık gri arka plan.

Aşağıdaki komutları sırasıyla kullanarak uygulamanın genel hatlarını çizebilirsiniz:

---

## 1. Ortak Ekranlar (Onboarding & Auth)

### Prompt 1: Splash ve Onboarding (Karşılama)
> "Modern ve profesyonel bir B2B E-Ticaret / Proje Yönetim mobil uygulaması için onboarding ekranları tasarla. Renk paleti güven veren lacivert ve canlı tech-blue tonlarında olsun. Ekran 1: 'Yazılım Projelerin İçin Doğru Ekibi Bul' başlığı ve müşteri/proje konseptli bir illüstrasyon. Ekran 2: 'Freelance Geliştiriciler İçin Yeni Fırsatlar' başlığı ve kodlama/geliştirici konseptli 3D icon. Ekran 3: 'GitHub Entegrasyonlu Şeffaf Süreç' başlığı. Alt kısımda sayfa göstergesi (pagination dots) ve büyük, dikkat çekici bir 'Hemen Başla' (Get Started) butonu olsun."

### Prompt 2: Kayıt ve Giriş Yap Ekranları (Auth Flow)
> "Clean architecture stiline sahip sade ve şık bir 'Kayıt Ol' (Sign Up) ekranı tasarla. Kullanıcı önce iki büyük karttan birini seçecek: 'Müşteri (Proje Sahibi)' veya 'Geliştirici (Developer)'. Bu rol seçiminden sonra Ad Soyad, E-posta ve Şifre giriş alanları (floating label tarzında) çıksın. Alt kısımda 'Kayıt Ol' butonu ve 'Zaten hesabın var mı? Giriş Yap' linki olsun. Login (Giriş) ekranı da aynı tasarım dilinde; sadece E-posta, Şifre alanları ve 'Şifremi Unuttum' linki içersin."

---

## 2. Müşteri (Customer) Akışı Ekranları

### Prompt 3: Müşteri Ana Sayfa (Customer Dashboard) & Alt Navigasyon
> "Müşteri rolündeki bir kullanıcı için modern bir mobil Dashboard ekranı tasarla. En altta 4 sekmeli bir Bottom Navigation Bar olsun: 'Ana Sayfa', 'Projelerim', 'Bildirimler', 'Profil'. En üstte kullanıcıyı karşılayan 'Merhaba, [İsim]' yazısı ve bir profil avatarı. Hemen altında özet istatistik kartları: 'Aktif Projeler (2)', 'Bekleyen Teklifler (5)'. Sayfanın orta kısmında dikey kaydırılabilir 'Son Projelerin' listesi olsun. Sayfanın en çarpıcı öğesi, sağ altta tıklandığında yeni bir proje ilanı açmaya yönlendiren belirgin bir 'Yeni Proje Oluştur' (+) FAB (Floating Action Button) butonu olsun."

### Prompt 4: Yeni Proje Talebi (İlan) Oluşturma Formu
> "Müşterinin yeni bir yazılım projesi ilanı açması için çok şık bir mobil form ekranı tasarla. Ekranın üstünde 'Yeni Proje İlanı' başlığı ve 'Geri' ikonu. Form alanları: 'Proje Başlığı' (Input), 'Proje Detayları' (Büyük TextArea), 'Bütçe ($)' (Input), 'Teslim Tarihi - Deadline' (Date Picker takvim ikonuyla), 'Teklif Bitiş Tarihi' (Date Picker), ve 'Teknolojiler' (Kullanıcının React, .NET gibi etiketleri seçebileceği veya yazabileceği chipler alanı). Alanlar arasına estetik boşluklar koy, tasarım karmaşık görünmesin. En altta geniş ve belirgin bir 'İlanı Yayınla' butonu koy."

### Prompt 5: Proje Detayı ve Gelen Teklifler (Bids) Görüntüleme
> "Müşterinin açık bir ilanının detayını ve gelen teklifleri gördüğü ekranı tasarla. Üst kısımda projenin başlığı, bütçesi ve etiket (badge) şeklinde 'Open (Açık)' statüsü bulunsun. Hemen altında yan yana kaydırılabilir (horizontal scroll) teknoloji chipleri (örn: React, Node.js). Sayfanın alt yarısında 'Gelen Teklifler (Bids)' başlığı altında kart tasarımları liste halinde olsun. Her teklif kartında: Geliştiricinin adı, profil fotoğrafı, 'Teklif Edilen Tutar', 'Öneri Notundan kısa bir özet' ve sağ tarafta dikkat çekici bir 'Kabul Et (Accept)' butonu yer alsın."

### Prompt 6: Dijital Sözleşme (Contract) Onay ve Revizyon Ekranı
> "Müşteri teklifi kabul ettikten sonra ortaya çıkan 'Dijital Sözleşme' ekranını tasarla. Üstte 'Proje Sözleşmesi' başlığı. Ortada yasal metinleri andıran, ancak okunulabilir şık bir tipografiye sahip, dikey kaydırılabilir bir metin kutusu (Contract Content). Metnin altında 'Revizyon Numarası: v1' ve durum etiketi ('Awaiting Approval'). En altta üç adet aksiyon butonu: 'Revize Et' (Outline stili), 'İptal Et' (Kırmızımsı/Uyarı stili) ve en belirgin şekilde 'Sözleşmeyi Onayla' (Ana renk doldurulmuş buton). Tasarım hukuki bir ciddiyetle teknolojik bir pratikliği aynı anda versin."

### Prompt 7: Aktif Proje (Ongoing) ve GitHub Aktivite Akışı (Timeline)
> "Sözleşmesi onaylanmış ve başlamış aktif bir projenin ilerleme ekranını tasarla. Ekranın üstünde proje başlığı ve yeşil bir 'Ongoing (Devam Ediyor)' etiketi. Orta kısımda bu uygulamanın en can alıcı özelliği olan 'GitHub Aktivite Akışı (Timeline)' yer alsın. Dikey bir zaman çizelgesi (timeline) tasarımı kullan. Çizelgedeki her bir adım bir 'Commit'i temsil etsin. Örneğin: 'UserService eklendi', altında 'Ahmet Yılmaz - 2 saat önce' yazısı ve yanlarında GitHub repo / branch ikonları yer alsın. Tasarım, teknik bilgisi olmayan bir müşterinin bile yazılımın geliştirildiğini anlayabileceği kadar temiz ve görsel olmalı."

---

## 3. Geliştirici (Developer) Akışı Ekranları

### Prompt 8: Geliştirici Keşfet (Explore / İlan Listesi) Ekranı
> "Geliştirici rolü için ana sayfa (Explore) ekranını tasarla. Altta Bottom Navigation ('Keşfet', 'Tekliflerim', 'Projelerim', 'Profil'). Üstte büyük bir arama çubuğu ve yanına 'Filtrele' ikonu ekle. Altında açık proje ilanlarını gösteren kartlar (Card UI) listelensin. Her bir proje kartında: Proje Başlığı, Bütçe, Kalan Teklif Süresi (Sayaç ikonuyla: 'Son 2 Gün'), ve etiketler (örn: C#, React). Kartın sağ alt köşesinde 'Teklif Ver' yazan küçük bir yönlendirme oku/butonu. Genel görünüm Dribbble/Behance kalitesinde, modern iş bulma (freelance) uygulamalarını andırsın."

### Prompt 9: İlan Detayı ve Teklif Verme (Bid Form)
> "Geliştiricinin bir proje detayını inceleyip teklif verdiği mobil ekran. Üstte projenin tüm açıklamaları okunabilir şekilde yer alsın. Alt kısımda, ekranın 1/3'ünü kaplayacak şekilde aşağıdan yukarıya açılan bir Bottom Sheet (Kayan Panel) modali tasarla. Bu panelin başlığı 'Teklif Ver' olsun. İçinde 'Teklif Tutarı ($)' girilebilecek büyük rakamlı bir input ve 'Müşteriye Öneri Notunuz' için geniş bir metin alanı (textarea) bulunsun. En altta 'Teklifi Gönder' butonu yer alsın."

### Prompt 10: Geliştirici - Aktif Proje ve Repo Bağlama Ekranı
> "Geliştiricinin kazandığı ve başlamaya hazır olduğu bir 'Aktif Proje' yönetim ekranı. Üstte proje detayları özet şeklinde dursun. Uygulamanın en önemli özelliklerinden biri olan 'GitHub Repository Bağlama' alanı ekranın ortasında vurgulansın. Bir kart içerisinde 'Projeyi GitHub ile bağla' yönergesi, altında 'Repo URL' input alanı ve opsiyonel 'Branch Adı' input alanı bulunsun. Yanında GitHub logolu şık bir 'Bağla (Connect)' butonu olsun. Bağlantı kurulduğunda altta başarı mesajı verecek bir gösterge simüle et."

---

## 💡 Stitch Üzerinde Tasarım Yaparken İpuçları (Tips)

1. **Aksiyonların Belirginliği:** FRD'de belirtilen iş kuralları gereği, "Teklifi Kabul Et", "Sözleşmeyi Onayla" gibi kritik butonların her zaman ekranın rahatça erişilebilir en alt kısmında, tam en (full-width) ve dikkat çekici renklerde olmasına dikkat edin.
2. **Durum Bildirimleri (Status Badges):** Projelerde yer alan `Open`, `AwaitingContract`, `Ongoing`, `Completed` gibi statüleri hap (pill) şeklindeki küçük etiket tasarımlarıyla her proje başlığının yanına mutlaka ekletin.
3. **Empty State (Boş Durum) Ekranları:** "Hiç projeniz yok", "Henüz teklif gelmedi" gibi durumlar da hesaba katılarak sevimli ve bilgilendirici empty state ikonografi/çizimleri kullanılmalıdır.
