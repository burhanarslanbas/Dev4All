# 03 - Adım Adım Kullanım Rehberi (Web & Mobil)

Sistemi anladık! Kuralları böldük, promptları (talimatları) hazırladık. Ajan da ("Agent") hazır emir beliyoruz. Peki Agent'ı çalıştırırken nasıl bir yol izleyeceğiz?

Yalnızca masaüstünden (Web'den) değil, mobil telefondan da sisteme iş yaptırabilirsiniz! Sadece bu yapı bile, otobüste veya kafede otururken cep telefonunuzdan yapay zekaya "Uygulamanın şifremi unuttum sayfasını kodla" komutunu verebileceğiniz anlamına gelir.

---

## 💻 Bölüm 1: Web (Bilgisayar) Üzerinden İşleyiş

Dev4All üzerinden örnekleyelim:

**1. GitHub'da Issue Açın:**
GitHub sayfanıza (Repository) gidin, "Issues" sekmesine tıklayıp yeni bir görev (`New Issue`) oluşturun. Görevinize sadece "Login Ekranını Yap" demeyin. İsteklerinizi, uygulamanın rengini, gerekiyorsa modelini adım adım anlatın (Bkz: `04-kullanilabilir-ornek-sablonlar.md`).  
Bu Issue bir numaraya sahip olacak (Örn: `#15`).

**2. Ajan'ı Başlatın:**
Masaüstünde VS Code tabanlı "Cursor", "Roo Code" veya direkt GitHub üzerindeki "GitHub Copilot Workspace" arayüzünü açın.
Agent bölümününe gidin.

**3. Prompt'u (Talimatı) Kopyalayıp Agent'a Verin:**
Şu talimatı agent kısmına yapıştırın ve çalıştırın:
> "Lütfen GitHub'daki `#15` numaralı issue'da yazılan tüm talimatları birebir uygula. Önce istenen dokümanları oku, ardından ilgili dosyaları oluştur. İşin bitince testini yap ve değişiklikleri bana onaya gönder."

**4. Arkanıza Yaslanın!**
Agent klasörleri tarar, dosyaları bulur, `LoginScreen.kt` dosyasını sıfırdan sizin yerinize yazar, gerekli kütüphaneleri ekler. Bittikten sonra size kodları gösterir; tek yapmanız gereken onaylamaktır!

---

## 📱 Bölüm 2: Cep Telefonundan İşleyiş (Agentic Mobil Coding)

Bilgisayar o an yanınızda bile değil! Kod yazmanız gereken işler var!

1. **GitHub Mobil Uygulamasını İndirin** (veya telefon tarayıcısından girin).
2. Projenizin `Issue` sekmesine girin.
3. Bizim kendi oluşturduğumuz `docs/plan/13-all-issue-agent-prompts.md` örneğindeki gibi **hazır Prompt belgelerimizden birine** telefondayken girin. İlgili adımdaki Prompt kodunu kopyalayın.
4. **GitHub Copilot Workspace**'e telefondan giriş yapın (Sadece tarayıcı üzerinden çalışır, kurulum gerektirmez!). Agent sayfasında base branch'ı `develop` seçin.
5. Telefondaki klavyenizi kullanarak:
   - "Aşağıdaki prompt'u uygula" yazın ve az önce kopyaladığınız uzun talimatı Agent input'una YAPIŞTIRIN!
6. Enter'a (veya gönder butonuna) basın.
7. Workspace'te agent'ın telefonda kendi kendine adım adım kod oluşturmasını film izler gibi izleyin.
8. İşlem bitince telefondan **"Create Pull Request" (Değişiklik İsteği Oluştur)** tuşuna basarak işi tamamlayın!

Müthiş değil mi? Geleneksel olarak günlerinizi alabilecek kodlama ve commit süreci, sadece metin kopyala ve yapıştır yaparak, siz kahvenizi içerken halledilmiş oldu!

Sıradaki bölüme ([**Örnek Şablonlar**](./04-kullanilabilir-ornek-sablonlar.md)) geçerek kullanabileceğiniz direkt talimat setlerini kopyalayabilirsiniz!
