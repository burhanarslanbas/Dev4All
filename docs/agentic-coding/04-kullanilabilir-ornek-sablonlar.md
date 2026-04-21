# 04 - Örnek Şablonlar (Copy-Paste Prompts)

Agent'ların kalitesi, tamamen sizin verdiğiniz "Prompt" (Talimat) kalitesine bağlıdır. Aşağıda test, demo ve kendi küçük projeleriniz için kullanabileceğiniz 3 farklı temel şablon bulabilirsiniz. 

Bunları doğrudan kopyalayıp GitHub Issue'larınıza gövde (Body) metni olarak yapıştırın!

---

## 🛠 Şablon 1: Frontend (Arayüz) Component Ekleme

```markdown
Sen, bu projenin Frontend ekibindeki Senior React geliştiricisisin.
Amacımız, var olan Tailwind CSS ve kurallarımızı bozmadan aşağıda istenen ekran bileşenini oluşturmaktır.

**Görev (Task):**
- Modern, şık ve koyu tema (Dark Mode) uyumlu bir [Buraya Component Adı, Örn: Kullanıcı Giriş Formu] tasarla.
- Dosyayı `src/components/` klasörüne oluştur.

**Kurallar & Kısıtlamalar:**
1. Kesinlikle `any` tipi kullanma, TypeScript interfacelerini tam tanımla.
2. Form için React Hook Form ve Zod kütüphanelerini import edip kullan.
3. Kodu yazdıktan sonra build alıp test et ve hata olmadığından emin ol.
4. Issue numarasına referans vererek Commit at.

Çıktı Raporu:
Oluşturduğun veya değiştirdiğin dosyaların kısa bir listesini bana ilet.
```

---

## ⚙️ Şablon 2: Backend (API) Uç Noktası (Endpoint) Ekleme

```markdown
Sen, bu projedeki Senior .NET (veya NodeJS/Pthon vs) geliştiricisin.
Clean Architecture ve SOLID prensiplerine harfiyen uyman gerekmektedir.

**Görev (Task):**
- [Buraya İşlemi Yazın, Örn: Ürünleri kategoriye göre getiren endpointi] yaz.

**Adımlar:**
1. `Domain/Entities` veya ilgili model klasörüne gidip modelde değişiklik gerekiyor mu kontrol et.
2. `Application` katmanına DTO ve Servis / Handler sınıfını oluştur. İş mantığını buraya yaz.
3. `API/Controllers` katmanına gelip uygun Route ile işlemi dışarıya aç.

**Kurallar & Kısıtlamalar:**
1. İş kodunu asla Controller içine yazma.
2. Mutlaka bir Interface üzerinden Dependency Injection kullan.
3. Veritabanı sorgularının Asenkron (async/await) olmasına kesinlikle dikkat et.

Çıktı Raporu:
Kodu yazmayı tamamladıktan sonra servisi ayaklandırıp (Build) herhangi bir hata veya Warning (Uyarı) verip vermediğini bana raporla.
```

---

## 📱 Şablon 3: Mobile (Android / iOS) Sayfa Oluşturma
*(Bizim Dev4All'da telefondan Agent'a verdiğimiz yapının basitleştirilmiş örnek halidir)*

```markdown
<GLOBAL_HEADER>
You are a senior Mobile engineer working in this repository.
General constraints:
- Geliştirmeler sadece bu iş ile sınırlı kalmalıdır.
- Sadece `develop` branch'ına Pull Request (PR) açılmalıdır.
- Compose (Android) veya SwiftUI (iOS) ile geliştirildiğinden emin ol.
</GLOBAL_HEADER>

**Issue Adı:** [Oluşturulacak Sayfa, Örn: Şifremi Unuttum Ekranı]
**Amaç:** Kullanıcının email'ini girerek onay butonuyla yeni şifre bağlantısı talep etmesi.

**Adımlar:**
1. UI (Arayüz) dosyasını `screens/` altına oluştur.
2. ViewModel dosyasını oluştur ve input State'lerini ayarla.
3. Api / Network bağlantı fonksiyonunu bağla.
4. Ekran boyutlarına duyarlı olmasına (Responsive) dikkat et.

**Test:**
İşin bittikten sonra lokal ortamda bir Build alarak projenin sorunsuz derlendiğini doğrula. Kod değişikliği bittiği an işlemi bitir, benden yeni bir onaykleme.
```

---

*Bu şablonları kendi projelerinizin ihtiyaçlarına göre özelleştirebilir; böylece yapay zekanın saçmalamasını engelleyebilir, saatlerce uğraşmak yerine dakikalar içinde sorunsuz çalışan kodlar elde edebilirsiniz.*
