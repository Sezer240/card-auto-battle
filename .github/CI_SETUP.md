# CI Kurulum Rehberi (Sezer · DevOps)

## Gün 1 — GitHub Actions CI

### 1. Unity Lisans Secret'larını Ekle

GitHub repo → **Settings → Secrets and variables → Actions → New repository secret**

| Secret adı | Değer | Nasıl bulunur? |
|---|---|---|
| `UNITY_EMAIL` | Unity hesabının e-posta adresi | Unity Hub giriş bilgisi |
| `UNITY_PASSWORD` | Unity hesabının şifresi | Unity Hub giriş bilgisi |
| `UNITY_LICENSE` | `.ulf` dosyasının XML içeriği | Aşağıdaki adımlar |

#### UNITY_LICENSE nasıl alınır? (Personal lisans)

```bash
# 1. Lokalde Unity'yi komut satırından çalıştır, aktivasyon isteği dosyası oluştur:
Unity.exe -quit -batchmode -createManualActivationFile -logfile

# 2. Oluşturulan Unity_v2022.x.alf dosyasını şu adrese yükle:
#    https://license.unity3d.com/manual
#    → "Activate New License" → Personal → .alf dosyasını yükle → .ulf indir

# 3. İndirilen .ulf dosyasını text editörde aç, tüm XML içeriğini kopyala
# 4. UNITY_LICENSE secret'ına bu içeriği yapıştır
```

> ⚠️ Secret'lar bir kez kaydedildikten sonra tekrar görüntülenemez.
> Şifreni değiştirirsen `UNITY_PASSWORD` secret'ını da güncellemeyi unutma.

---

### 2. CI Workflow'u Test Et

Secret'ları ekledikten sonra:

1. Bu PR'ı merge et (`feature/ci-setup` → `develop`)
2. GitHub → **Actions** sekmesine git
3. `CI` workflow'unun tetiklendiğini ve yeşil yandığını doğrula

---

## Gün 2 — Branch Koruma Kuralları

### `develop` branch için

GitHub repo → **Settings → Branches → Add branch ruleset**

**Branch name pattern:** `develop`

| Kural | Ayar |
|---|---|
| Require a pull request before merging | ✅ Açık |
| Required approvals | **1** |
| Dismiss stale reviews | ✅ Açık |
| Require status checks to pass | ✅ Açık |
| Required status checks | `EditMode Tests`, `Build Check (Windows)` |
| Do not allow bypassing | ✅ Açık |

### `main` branch için

**Branch name pattern:** `main`

| Kural | Ayar |
|---|---|
| Require a pull request before merging | ✅ Açık |
| Required approvals | **2** |
| Dismiss stale reviews | ✅ Açık |
| Require status checks to pass | ✅ Açık |
| Required status checks | `EditMode Tests`, `Build Check (Windows)` |
| Restrict who can push | ✅ Açık — sadece Sezer |
| Do not allow bypassing | ✅ Açık |

### Ekip Duyurusu (Discord/Slack mesajı)

```
@everyone 

✅ GitHub Actions CI artık aktif!

Kurallar:
• develop ve main'e doğrudan push kapalı — her şey PR ile gelecek
• CI (build + test) yeşil olmadan merge yok
• Her PR en az 1 review bekliyor

PR açarken şablonu doldurun, büyük dosyalarda LFS kullanın.
Sorun varsa bana yazın.

— Sezer
```

---

## Gün 3–4 — PR Review Takvimi

### Haftalık Review Ritmi

| Zaman | Görev |
|---|---|
| Her gün 09:00 | Açık PR'lara bak, CI durumunu kontrol et |
| Salı & Perşembe 17:00 | Aktif PR'ları review et (< 24 saat gecikme hedefi) |
| Cuma 15:00 | Haftalık özet: kaç PR merge edildi, CI kırmızı kaldı mı? |
| Hafta sonu | Demo için develop → main merge hazırlığı |

### CI Kırmızı Olduğunda

1. GitHub Actions → başarısız job'a tıkla → log'u oku
2. Hatanın sahibini bul (son commit'i atan kişi)
3. Discord'da doğrudan mesaj: branch adı + hata özeti
4. Sahibi düzeltene kadar o PR'ı `needs-fix` etiketiyle işaretle

### PR Etiketleri

| Etiket | Anlam |
|---|---|
| `needs-review` | Review bekleniyor |
| `needs-fix` | CI kırmızı veya review yorumu var |
| `ready-to-merge` | Onay tamam, merge edilebilir |
| `blocked` | Başka bir PR'a bağımlı |
