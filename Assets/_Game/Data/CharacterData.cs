using UnityEngine;

/// <summary>
/// Bir kartın / karakterin tüm temel istatistiklerini tutan ScriptableObject.
/// Yeni kart eklemek için: Assets/_Game/Cards klasöründe sağ tık → Create → CardBattler → CharacterData
/// CSV sütunları: Kart adı | Nadirlik | Irkı | Tipi | Işık değeri | Can | Zırh | Zırh canı |
///                Hasar | Saldırı hızı | Defans | Işık toplama | Toplama süresi |
///                Işık çalma | Gizlenme | Yakma | Yakma süresi | Tespit
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "CardBattler/CharacterData")]
public class CharacterData : ScriptableObject
{
    // ─── Kimlik ───────────────────────────────────────────────────────────────
    [Header("Kimlik")]
    public string cardName       = "Yeni Karakter";
    public CardIrk  CardIrk        = CardIrk.Direnen;
    public CardTip    CardTip          = CardTip.hasar;
    public CardNadirlik CardNadirlik     = CardNadirlik.Sıradan;
    [Tooltip("Işık değeri — kartı oyuna sürmek için gereken ışık maliyeti")]
    public int     lightCost     = 50;

    // ─── Temel Savaş İstatistikleri ───────────────────────────────────────────
    [Header("Temel Savaş İstatistikleri")]
    [Tooltip("Maksimum can")]
    public int hp        = 300;
    [Tooltip("Fiziksel hasar")]
    public int atk       = 100;
    [Tooltip("Defans — alınan hasarı azaltır")]
    public int def       = 25;
    [Tooltip("Saldırı hızı: kaç turda bir saldırır (1 = her tur, 2 = iki turda bir…)")]
    public int attackSpeedTurns = 1;

    // ─── Zırh ─────────────────────────────────────────────────────────────────
    [Header("Zırh")]
    public bool hasArmor    = false;
    [Tooltip("Zırh kırılmadan önce taşıyabileceği hasar")]
    public int  armorHp     = 0;

    // ─── Işık Mekaniği ────────────────────────────────────────────────────────
    [Header("Işık Mekaniği")]
    public bool hasLightCollection = false;
    [Tooltip("Işık toplamak için gereken tur sayısı")]
    public int  lightCollectionTurns = 3;
    [Tooltip("Düşmandan ışık çalabilir mi?")]
    public bool canStealLight = false;

    // ─── Özel Yetenekler ──────────────────────────────────────────────────────
    [Header("Özel Yetenekler")]
    [Tooltip("Gizlenme — düşman hedef alamaz")]
    public bool hasStealth  = false;
    [Tooltip("Yakma efekti uygular")]
    public bool canBurn     = false;
    [Tooltip("Yakma kaç tur sürer")]
    public int  burnDuration = 0;
    [Tooltip("Gizlenmiş karakterleri tespit edebilir")]
    public bool hasDetection = false;

    // ─── Görsel / Ses (ilerleyen sprintler için) ──────────────────────────────
    [Header("Görsel & Ses")]
    public Sprite cardArtwork;
    public RuntimeAnimatorController animatorController;
}

