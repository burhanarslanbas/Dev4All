# Yapay Zeka Destekli Geliştirme — Çalışma Rehberi

Bu doküman, **Cursor**, **Antigravity** ve benzeri araçlarla günlük geliştirme yaparken; sohbet yönetimi, model seçimi ve planlı iş (ör. haftalık mimari plandaki **Adım 4**) üzerinde nasıl ilerleneceğini açıklar.

> **Not:** Ürün arayüzündeki model adları (Auto, Sonnet, Opus vb.) zaman içinde değişebilir. Buradaki **rol** tanımları (hız / maliyet / derin düşünme) güncel ürün dokümantasyonuyla birlikte kullanılmalıdır.

---

## 1. Sohbet her şeyi “unutur” mu?

**Kısa cevap:** Evet, her **yeni sohbet** (yeni konuşma oturumu) genelde **önceki sohbetin mesaj geçmişine erişmez**. Model, bağlamı şunlardan **yeniden doldurur**:

| Kaynak | Ne sağlar? |
|--------|----------------|
| **Açık dosyalar ve imleç** | O an düzenlediğiniz kod |
| **@ ile eklenen dosya/klasör** | Seçtiğiniz dosya veya dizin içeriği |
| **Proje kuralları** | Örn. `.cursor/rules/*.mdc` — otomatik veya glob ile |
| **`docs/AGENTS.md`** | Repoda versiyonlanan mimari ve kod kuralları (manuel @ veya ürünün okuduğu yol) |
| **README, plan dokümanları** | `@docs/plan/...` ile bağlam verme |

**Sonuç:** “Projeyi unutma” diye bir şey yoktur; **bağlamı siz ve araç birlikte her oturuma taşırsınız**. Yeni sohbet açmak normaldir; unutkanlık değil, **tasarım gereği temiz bağlam**dır.

---

## 2. Ne sıklıkla sohbet değiştirmeliyim?

Sabit bir “gün” sayısı yok; **sinyallere** göre karar verin:

### Yeni sohbet açmayı düşünün

- **Önceki konuşma çok uzadı** (yüzlerce mesaj) — model bağlamı dolunca özet veya gürültü artar.
- **Tamamen farklı bir iş** (ör. bug’tan sonra yeni feature).
- **Önceki deneme başarısız / yanlış yöne gitti** — temiz başlangıç, kısa özetle yeni istek.
- **Hassas veya farklı gizli bilgi** — önceki mesajlarda kalan detayları yeni sohbete taşımak istemezseniz.

### Aynı sohbette devam edin

- **Tek bir görevin** adımları (ör. “Adım 4’teki tüm dosyaları oluştur, build al”).
- **Hata ayıklama** — aynı hata, aynı dosya; bağlam kopmasın.
- **Küçük iterasyonlar** (lint düzelt, isim değiştir).

**Pratik kural:** Bir işi “tek commit / tek PR” mantığında bitiriyorsanız, **çoğu zaman tek sohbet** yeterli. İş büyüdüyse **yeni sohbet + kısa özet + @plan** ile devam edin.

---

## 3. Hangi durumda hangi model? (Auto vs Sonnet vs Opus)

Bu bölüm **davranışsal** öneridir; Cursor’da veya başka üründe isimler farklı olabilir.

### Auto (veya “önerilen / hızlı”)

- **Ne zaman:** Basit düzenleme, tekrarlayan refaktör, küçük dosya değişiklikleri, “şu satırı düzelt”.
- **Avantaj:** Düşük gecikme, düşük maliyet.
- **Risk:** Çok katmanlı mimari veya nadir edge case’lerde yüzeysel kalabilir.

### Sonnet sınıfı (güçlü, dengeli)

- **Ne zaman:** Çoğu **feature** işi, CQRS handler, validator, repository iskeleti, test yazımı.
- **Avantaj:** Kod kalitesi ve hız dengesi genelde iyi.
- **Bu proje için:** Günlük backend geliştirmenin **varsayılan** seçeneği olarak düşünülebilir.

### Opus sınıfı (en güçlü / derin)

- **Ne zaman:** Büyük mimari karar, zor bug, güvenlik, performans, çok dosyalı refaktör, **belirsiz gereksinim** çözümlemesi.
- **Avantaj:** Daha iyi soyutlama ve uzun vadeli tutarlılık.
- **Maliyet:** Daha yavaş ve pahalı; her küçük görev için gereksiz.

---

## 4. Adım 4’e geçiyorum — nasıl bir yol izlemeliyim?

Aşağıdaki sıra, **2 haftalık plandaki “Adım 4 — Application katmanı soyutlamaları”** gibi net bir plan adımı için önerilir.

### 4.1. Hazırlık (5 dakika)

1. Planda ilgili bölümü açın: `docs/plan/2-hafta-backend-mimari-plan.md` → **Adım 4**.
2. **Yeni bir sohbet** açın (veya önceki adım bittiyse temiz oturum).
3. İlk mesajda şunları bağlayın:
   - `@docs/AGENTS.md` veya en azından “kurallara uy” hatırlatması
   - `@docs/plan/2-hafta-backend-mimari-plan.md` içinde **sadece Adım 4** ile ilgili kısmı @ ile işaretleyin veya “Adım 4’ü uygula” deyin
   - `@backend/src/Core/Dev4All.Application` klasörü (mevcut yapıyı görsün)

### 4.2. İstek şablonu (örnek)

Örnek talimat (kopyalayıp uyarlayabilirsiniz):

```text
docs/plan/2-hafta-backend-mimari-plan.md içindeki "Adım 4 — Application Katmanı" bölümünü uygula.
docs/AGENTS.md ve .cursor/rules kurallarına uy.
Mevcut backend yolu: backend/
Her dosya için plandaki imzaları koru; eksik klasörleri oluştur.
İş bitince: dotnet build backend/Dev4All.slnx çalıştır ve hataları düzelt.
```

### 4.3. Model seçimi (bu adım için)

- **Adım 4 tek seferde çok dosya** üretiyorsa: **Sonnet** veya ürününüzdeki “güçlü dengeli” model.
- **Sadece bir iki küçük dosya** eklenecekse: **Auto** yeterli olabilir.
- **Tasarım tartışması** (repository arayüzleri, IUnitOfWork sınırları) gerekiyorsa: **Opus** ile kısa bir “tasarıyı onayla, sonra kodu Sonnet/Auto ile yaz” ayrımı yapılabilir.

### 4.4. Bitiş kontrolü

- `dotnet build backend/Dev4All.slnx` — sıfır hata
- İlgili unit test varsa çalıştırma (Adım 4 sonrası planda test varsa)
- **Commit:** `feat: add application layer abstractions (step 4)` gibi anlamlı mesaj

---

## 5. Bağlamı kaybetmemek için kalıcı araçlar

| Araç | Amaç |
|------|------|
| `docs/AGENTS.md` | Tüm AI araçları için tek kaynak mimari kurallar |
| `.cursor/rules/*.mdc` | Cursor’da dosya türüne göre otomatik kurallar |
| `docs/plan/*.md` | Hangi adımda olduğunuzun resmi tanımı |
| `README.md` | Depo yapısı ve `dotnet build` komutu |
| Git commit mesajları | İnsan ve gelecekteki “sohbetler” için özet |

Yeni sohbette **“Adım 4’ü uygula”** demeniz yeterli; detay **planda** durduğu için “unutma” riski azalır.

---

## 6. Yaygın hatalar

1. **Uzun sohbette her şeyi** aynı thread’de tutup bağlamın dolmasını beklemek → ara sıra **yeni sohbet + @plan**.
2. **Hiç @ kullanmamak** → model sadece tahmin eder; `@docs/AGENTS.md` ve ilgili klasörü eklemek kaliteyi artırır.
3. **Her iş için Opus** → maliyet ve süre; gerektiğinde kullanın.
4. **Plana aykırı kod** → önce planı veya `AGENTS.md`’yi güncelleyin, sonra kodu değiştirin (tek kaynak prensibi).

---

## 7. İlgili dokümanlar

| Dosya | İçerik |
|--------|--------|
| [docs/AGENTS.md](../AGENTS.md) | Mimari, katman kuralları, CQRS kalıpları |
| [docs/plan/2-hafta-backend-mimari-plan.md](../plan/2-hafta-backend-mimari-plan.md) | Haftalık adımlar (Adım 4 dahil) |
| [docs/analyse/04-sadm.md](../analyse/04-sadm.md) | Sistem mimarisi ve veri modeli |
| [docs/guides/README.md](README.md) | Rehberler dizini (tüm `guides` dosyaları) |
| [docs/guides/onion-architecture-guide.md](onion-architecture-guide.md) | Onion / Clean Architecture özeti |
| [docs/guides/nuget-libraries-guide.md](nuget-libraries-guide.md) | NuGet kütüphane notları |
| [docs/guides/configuration/config-walkthrough.md](configuration/config-walkthrough.md) | Güvenli konfigürasyon entegrasyon özeti |
| [docs/guides/configuration/secure-configuration-guide.md](configuration/secure-configuration-guide.md) | Production-ready konfigürasyon kılavuzu |

---

## 8. Özet checklist (her yeni büyük adım için)

- [ ] Plan dokümanında adım numarası ve kapsam net mi?
- [ ] İlk mesajda `@docs/AGENTS.md` veya ilgili `@docs/plan/...` bağlandı mı?
- [ ] Backend kökü `backend/` olarak mı kullanılıyor?
- [ ] Uygun model seçildi mi (basit → Auto, günlük iş → Sonnet, mimari/zor → Opus)?
- [ ] İş bitince `dotnet build backend/Dev4All.slnx` çalıştı mı?
- [ ] Gerekirse commit atıldı mı?

Bu rehber, **süreç** içindir; ürün güncellemelerinde model adları ve menüler değişebilir — **bağlamı dokümanla taşıma** ilkesi kalıcıdır.
