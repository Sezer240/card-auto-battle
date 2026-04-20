# 🃏 Card Battler

> Kart tabanlı otomatik savaş oyunu — Unity ile geliştirilmekte olan portföy projesi.

Oyuncu elindeki kartlarla bir takım kurar, savaşı başlatır ve karakterler otomatik olarak çarpışır. Strateji tamamen savaş öncesi hazırlıktadır. Her savaşın sonunda yeni kartlar kazanılır ve koleksiyon büyütülür.

---

## 📋 İçindekiler

- [Oyun Hakkında](#-oyun-hakkında)
- [Özellikler](#-özellikler)
- [Kurulum](#-kurulum)
- [Proje Yapısı](#-proje-yapısı)
- [Geliştirme Süreci](#-geliştirme-süreci)
  - [Branch Stratejisi](#branch-stratejisi)
  - [Commit Kuralları](#commit-kuralları)
  - [Pull Request Kuralları](#pull-request-kuralları)
  - [Unity Merge Çakışmalarını Önlemek İçin](#unity-merge-çakışmalarını-önlemek-için)
  - [Sprint Planı](#sprint-planı)
- [Ekip](#-ekip)

---

## 🎮 Oyun Hakkında

**Tür:** Kart tabanlı auto-battler  
**Motor:** Unity 6 (C#)  
**Platform:** PC (Windows), WebGL  
**Durum:** Geliştirme aşamasında

Clash Royale'den ilham alan kart estetiği ile Teamfight Tactics tarzı otomatik savaş mekaniğini birleştiren bu oyunda:

- Elindeki kartları savaş alanına yerleştir
- Savaşı başlat — karakterler otomatik olarak çarpışır
- Kazanırsan yeni kartlar kazan, koleksiyonunu büyüt
- Synergy bonuslarını keşfet (3 Şövalye = +savunma, vb.)
- Roguelite sefer modunda her seferinde farklı bir deneyim yaşa

---

## ✨ Özellikler

### Mevcut (Sprint 1–2)
- [ ] Kart veri modeli (ScriptableObject tabanlı)
- [ ] El yönetimi ve sürükle-bırak yerleşim sistemi
- [ ] Otomatik savaş döngüsü (hedef seçimi, hasar, ölüm)
- [ ] Temel AI rakip

### Geliştirme Aşamasında (Sprint 3–4)
- [ ] Kart kazanma ve koleksiyon sistemi
- [ ] 20+ kart ile sınıf / synergy bonusları
- [ ] Roguelite sefer modu
- [ ] Altın ekonomisi ve shop
- [ ] Save / load sistemi

### Planlanan (Sprint 5+)
- [ ] UI polish ve kart animasyonları
- [ ] Ses efektleri ve müzik
- [ ] WebGL build (tarayıcıda oyna)
- [ ] Online multiplayer (kapsam dışı — zaman kalırsa)

---

## 🛠 Kurulum

### Gereksinimler

- [Unity 6](https://unity.com/releases/editor/archive) (6000.0.x veya üstü)
- Git + [Git LFS](https://git-lfs.com/) (büyük asset'ler için)

### Adımlar

```bash
# 1. Repoyu klonla
git clone https://github.com/Sezer240/card-auto-battle.git
cd card-auto-battle

# 2. Git LFS dosyalarını çek
git lfs pull

# 3. Unity Hub'dan projeyi aç
#    File → Open Project → card-auto-battle klasörünü seç

# 4. Battle sahnesini aç
#    Assets/Scenes/Battle.unity
```

> ⚠️ Unity versiyonu farklıysa proje açılışında upgrade uyarısı çıkabilir. Ekiple aynı versiyonu kullanmaya özen gösterin.

---

## 📁 Proje Yapısı

```
Assets/
├── _Game/
│   ├── Cards/          # ScriptableObject kart tanımları
│   ├── Characters/     # Karakter prefab'ları ve script'leri
│   ├── Combat/         # BattleManager, TargetSelector
│   ├── Economy/        # ShopManager, GoldSystem
│   ├── Progression/    # RunManager (roguelite sefer)
│   ├── UI/             # CardView, HandUI, BattleHUD
│   └── Audio/          # Ses efektleri ve müzik
├── Scenes/
│   ├── MainMenu
│   ├── Battle
│   └── Shop
└── Tests/              # EditMode + PlayMode testleri
```

---

## 🔄 Geliştirme Süreci

### Branch Stratejisi

| Branch | Amaç |
|--------|------|
| `main` | Her zaman oynanabilir, korumalı |
| `develop` | Aktif geliştirme — sprint sonunda `main`'e merge |
| `feature/özellik-adı` | Her özellik kendi dalında |
| `fix/hata-adı` | Hata düzeltmeleri için ayrı dal |

### Commit Kuralları

Commit mesajlarında aşağıdaki prefix'leri kullanın:

```
feat: yeni özellik eklendi
fix: hata düzeltildi
refactor: kod yeniden düzenlendi
art: asset / görsel güncellendi
docs: dokümantasyon güncellendi
```

### Pull Request Kuralları

- `main` ve `develop` dallarına doğrudan push yapılmaz
- Her PR en az **1 kişi tarafından review** edilmeli ve onaylanmalıdır
- PR açıklamasına şunları yazın:
  - Ne yaptı?
  - Nasıl test edildi?
  - UI değişikliği varsa ekran görüntüsü

### Unity Merge Çakışmalarını Önlemek İçin

- `.meta` dosyalarını `.gitignore`'a **eklemeyin** — commit'leyin
- Büyük dosyalar (ses, sprite, animasyon) için Git LFS kullanın
- Aynı sahneyi **aynı anda iki kişi düzenlemesin** — sahne sahipliğini kanalda duyurun

### Sprint Planı

| Sprint | Hafta | Odak | Teslim |
|--------|-------|------|--------|
| 1 | 1–2 | Temel altyapı | El'deki kartları alana yerleştir |
| 2 | 3–4 | Savaş motoru | İki takım otomatik çarpışıyor |
| 3 | 5–6 | Kart sistemi | Synergy bonusları çalışıyor |
| 4 | 7–8 | Oyun döngüsü | Baştan sona oynanabilir sefer |
| 5 | 9–10 | Cila & build | Demo-ready, portföy materyali hazır |

---

## 👥 Ekip

| Rol | Sorumluluk |
|-----|------------|
| **Gameplay** | Savaş motoru, hedef algoritması, sınıf bonusları |
| **UI/UX** | Kart prefab'ları, sürükle-bırak, menüler, animasyonlar |
| **Content** | Kart tasarımı, balans tablosu, ScriptableObject veri girişi |
| **Systems** | Save/load, shop, roguelite ilerleme, AI rakip |
| **DevOps** | GitHub Actions CI, build pipeline, code review |

---

## 📄 Lisans

Bu proje, Unity ve Git iş akışını birlikte öğrenmek amacıyla geliştirilmiş kapalı bir portföy çalışmasıdır.
