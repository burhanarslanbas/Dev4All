# 01 - Temel Kavramlar ve Kurulum

Eğer daha önce hiç "Agent", "GitHub" veya "Issue" gibi terimler duymadıysanız, hiç endişelenmeyin! Kendi projenizi yapay zekaya kodlatmak için kod yazmayı bilmenize gerek yok, ancak bu sisteme "yöneticilik" yapabilmek için bazı araçların ne anlama geldiğini kavramalısınız.

## 1. GitHub Nedir?
GitHub, yazılımcıların kodlarını tuttuğu, paylaştığı ve beraber aynı proje üzerinde çalıştığı bir bulut deposudur (Google Drive'ın kodlar için olanı gibi düşünebilirsiniz). 
- Biz yapay zekaya bir iş atadığımızda, yapay zeka gelip kodları bu depoya yükler.
- Her şeyi burada saklarız ki dünyanın neresinden olursak olalım, saniyeler içinde işlerimize devam edebilelim.

## 2. GitHub Issue Nedir?
**Issue (Talep/Görev), bu sistemin kalbidir.** Normalde yazılımcılar Issue sistemini "Şurada bir hata var" (bug) demek veya "Şöyle bir yeni özellik istiyorum" diyerek iş listesi (To-do) yapmak için kullanır.
- Biz ise Issue'ları **Yapay Zekaya vereceğimiz talimat listesi** olarak kullanıyoruz.
- Projenizi tek bir büyük istek ("Bana E-ticaret sitesi yap") olarak verirseniz yapay zeka çuvallar. Onun yerine işi küçük Issue'lara bölmeliyiz: "Oturum açma ekranını yap" (Issue 1), "Sepete ekleme olayını yap" (Issue 2).

## 3. AI Agent (Yapay Zeka Ajanı) Nedir?
Normal ChatGPT veya Claude'a girip "Bana kod yaz" dediğinizde size sadece metin olarak kodu geri verir. O kodu alıp doğru dosyalara kopyalamak sizin işinizdir.
- **Agent:** Bu işi otonom (kendi kendine) yapan yapay zeka modelidir. 
- Siz ona bir GitHub Issue'su verirsiniz, o gider sizin klasörlerinizi açar, projeyi okur, gereken dosyaları kendi yaratır, kodunu kendi içine kaydeder, hatta çalıştırıp hata olup olmadığını kontrol eder. Hata varsa kendi kendine düzeltir. Sizin adınıza kod yazıp işi size sadece "İstediğin gibi çalıştı, onaylıyor musun?" diyerek bitirir.

Biz Agent olarak GitHub Copilot Workspace veya Cursor / Roo Code gibi gelişmiş agent araçlarını kullanıyoruz.

---

## 4. GitHub Copilot Student Paketi Nasıl Alınır?
Dünyanın en iyi Agent ve yapay zeka araçları ücretli olsa da, eğer **öğrenciyseniz** GitHub Copilot ve premium araçların birçoğu size **TAMAMEN ÜCRETSİZ!**

### Adım Adım Ücretsiz Hesap Kurulumu:
1. Bir GitHub hesabı açın (github.com).
2. [GitHub Education (education.github.com/pack)](https://education.github.com/pack) adresine gidin.
3. Öğrenci (Student) seçeneğine tıklayarak okulunuzun size verdiği öğrenci mail adresinizle (`.edu.tr` veya okul uzantılı) giriş yapın veya öğrenci belge fotoğrafınızı sisteme yükleyin.
4. Başvurunuz birkaç gün içinde (bazen anında) onaylanır. 
5. Artık GitHub Copilot dahil binlerce dolarlık yazılım üretim araçlarına ömür boyu (öğrenci olduğunuz sürece) ücretsiz erişiminiz var demektir!

Diğer dosyadan (02-bizim-yapi-nasil-calisiyor.md) devam ederek, "Agentic Coding" yönteminde bu yapıyı kendi projemiz için nasıl kullandığımızı görebilirsiniz.
