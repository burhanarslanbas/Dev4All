# 02 - Bizim Yapı Nasıl Çalışıyor? (Agentic Coding Metodu)

Bu bölümde, Agentic Coding (Yapay Zeka ile Kodlama) mantığını Dev4All projesi üzerinden nasıl kurguladığımızı anlatıyoruz. Eğer kendi test veya Demo projelerinizi yapmak isterseniz, bu mantığı anlamak işinizi çok kolaylaştıracaktır.

## İşi Küçük Adımlara (Issue) Bölmek
Normal bir "proje" binlerce satır koddan oluşur. Yapay zeka henüz "Bana bir E-ticaret sitesi yap" dediğinizde her şeyi kusursuzca çıkaracak kapasitede değildir. En akıllı yapay zeka modelleri bile küçük, net ve odaklanmış hedeflerde çok daha iyi sonuç verir.

İşte bu yüzden projeyi **GitHub Issue'lara** bölüyoruz!
Örneğin bizim backend API sürecimiz şu şekilde bölünmüştür:
- **Issue 1:** Veritabanına (Entity) Kullanıcı tablosunu oluştur.
- **Issue 2:** Kullanıcı için Kayıt ve Giriş (Auth) Endpoint'lerini yaz.
- **Issue 3:** Email Onay işlemlerini yaz.

Her Issue, bir yapay zeka ajanı için **bir tam günlük iştir** (Oysaki o bu işi bizim için sadece 3-5 dakikada bitirir!).

## "Issue Body" ve "Agent Prompts" (Talimat Setleri)
Elimizde küçük işler var. Ancak ajanı serbest bırakırsak kendi kafasına göre bir yöntem deneyebilir. Bu yüzden **Prompt Engineering** (İstem Mühendisliği) devreye giriyor.

Her Issue'nun içine net kurallar yazıyoruz. Biz Dev4All projesinde her Issue için **Agent'a özel şablonlar (Prompt'lar)** hazırladık.
Agent'a verdiğimiz bir Prompt kabaca şöyle görünür:
1. **Rolün:** Sen bir Senior (Kıdemli) yazılımcısın.
2. **Kural:** Şuradaki Mimari dokümanını (AGENTS.md) oku, kodları asla onun dışına çıkarak yazma.
3. **Görev (Task):** Kayıt ekranını yap.
4. **Adımlar:**
   a) Önce XYZ dosyasını aç.
   b) Form kurallarını kontrol et.
   c) Kodu yazdıktan sonra testleri çalıştır ve uygulamanın çalıştığını onayla.
5. İş bitince bana "Closes #10" yazarak PR (Değişiklik İsteği) aç.

## "Neden bu şekilde yapıyoruz?"
Mobil veya Frontend'i test ve entegrasyon süreçlerinde MVP (Minimum Viable Product - En Erken Ürün) haline getirmek için hız inanılmaz derece önemlidir. Agent kullanmazsak 2 haftada yazacağımız mobil arayüz uygulamasını, hazırladığımız promt'lar (Issue'lar) sayesinde **bilgisayarı bile açmadan cep telefonumuzla Agent'a kopyala-yapıştır ile görev vererek 1 gün içinde tamamlayabiliyoruz.**

Biz Dev4All'da mimari tasarımlar için kendi aklımızı ve yazılımcı vizyonumuzu kullanıyoruz, ancak "amelelik" dediğimiz veya sadece test/demo için hızlıca çıkarılması gereken arayüzler ve bileşenler (frontend & mobile MVP kısımları) için **tamamen bu Agentic Coding yapısını** kullanıyoruz! 

Bir sonraki bölümde [**Adım Adım Kullanım Rehberi**](./03-adim-adim-kullanim-rehberi.md) okuyarak bu yapıyı telefondan bile nasıl uygulayabileceğinizi görebilirsiniz.
