# Dev4All Git Guide

Bu doküman, Dev4All için temel ve sık kullanılan Git komutlarını tek yerde toplar.
Komutlar PowerShell/terminal için yazılmıştır.

---

## 1) Temel Kavramlar

- `main`: Üretime yakın/stabil hat.
- `develop`: Günlük geliştirme hattı.
- `feature/*`: Özellik geliştirme branch'leri.
- `origin`: Uzak repo (GitHub).

---

## 2) Günlük En Sık Kullanılan Komutlar

```bash
git status
git add .
git add <file>
git restore --staged <file>
git commit -m "feat(auth): add login handler"
git pull origin develop
git push origin develop
git log --oneline -20
git diff
git diff --staged
```

---

## 3) Branch Yönetimi

### Yeni branch açma

```bash
git checkout develop
git pull origin develop
git checkout -b feature/auth-controller
```

### Branch listesi

```bash
git branch
git branch -a
```

### Branch silme

```bash
git branch -d feature/auth-controller
git push origin --delete feature/auth-controller
```

---

## 4) Commit Standartları (Conventional Commits)

Önerilen tipler:

- `feat:` yeni özellik
- `fix:` bug düzeltmesi
- `docs:` dokümantasyon değişikliği
- `refactor:` davranışı değiştirmeden yeniden düzenleme
- `test:` test ekleme/düzeltme
- `chore:` bakım/altyapı işleri

Örnek:

```text
feat(auth): complete week 2 auth architecture and flow
```

---

## 5) Değişiklikleri Güvenli Gönderme Akışı

```bash
git status
git add <dosyalar>
git commit -m "feat(...): ..."
git pull --rebase origin develop
git push origin develop
```

> Not: Ekip politikası farklı değilse `pull --rebase` geçmişi temiz tutar.

---

## 6) Merge Senaryoları

### A) Feature -> Develop

```bash
git checkout develop
git pull origin develop
git merge feature/my-feature
git push origin develop
```

### B) Develop -> Main (Release)

```bash
git checkout main
git pull origin main
git merge develop
git push origin main
```

### C) Main -> Develop (Back-merge / hizalama)

```bash
git checkout develop
git pull origin develop
git merge main
git push origin develop
```

---

## 7) Sık Kullanılan Senaryo (Özel Bölüm)

## develop'a güncelleme attıktan sonra yapılacaklar

Bu akış, senin özellikle istediğin **develop push -> main merge -> tekrar develop hizalama** senaryosudur:

```bash
# 1) develop'da değişiklikleri commit et ve pushla
git checkout develop
git status
git add <dosyalar>
git commit -m "feat(...): ..."
git push origin develop

# 2) main'e geç, güncelle, develop'u main'e merge et, pushla
git checkout main
git pull origin main
git merge develop
git push origin main

# 3) tekrar develop'a dön ve main ile hizala (back-merge)
git checkout develop
git pull origin develop
git merge main
git push origin develop
```

> Bu 3. adım çok önemlidir: `develop`, `main` gerisinde kalmaz.

---

## 8) Conflict Çözümü (Kısa Akış)

```bash
git status
# conflictli dosyaları düzenle
git add <resolved-files>
git commit
git push origin <branch>
```

Conflict sırasında iptal etmek için:

```bash
git merge --abort
```

---

## 9) Geri Alma / Temizleme Komutları

### Staged'den çıkar (dosya silmeden)

```bash
git restore --staged <file>
```

### Working tree'deki değişikliği geri al (dikkat: kaybolur)

```bash
git restore <file>
```

### Son commit mesajını düzelt (push edilmediyse)

```bash
git commit --amend
```

---

## 10) İnceleme ve Kontrol

```bash
git log --oneline --decorate --graph -20
git show <commit-hash>
git diff origin/develop...HEAD
```

---

## 11) İyi Pratikler

- Sık ve küçük commit at.
- Her commit tek bir amaca hizmet etsin.
- Commit atmadan önce `git diff --staged` kontrolü yap.
- `main`'e doğrudan geliştirme yapma; `develop`/feature üzerinden ilerle.
- Merge sonrası mutlaka build/test çalıştır.

---

## 12) Özet

En kritik release akışı:

1. `develop` commit + push
2. `develop -> main` merge + push
3. `main -> develop` back-merge + push

Bu akış, branch'lerin her zaman senkron kalmasını sağlar.

