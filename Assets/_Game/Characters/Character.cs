using UnityEngine;
using System;

/// <summary>
/// Savaş sahasındaki canlı bir karakteri temsil eder.
/// CharacterData (ScriptableObject) → runtime istatistiklerine kopyalanır.
/// Konumu: Assets/_Game/Characters/Character.cs
/// </summary>
public class Character : MonoBehaviour
{
    // ─── Veri ─────────────────────────────────────────────────────────────────
    [SerializeField] private CharacterData data;

    // ─── Runtime İstatistikleri ───────────────────────────────────────────────
    public string CharacterName  { get; private set; }
    public int    MaxHP          { get; private set; }
    public int    CurrentHP      { get; private set; }
    public int    ATK            { get; private set; }
    public int    DEF            { get; private set; }
    public int    AttackCooldown { get; private set; }   // kaç turda bir saldırır
    public bool   IsAlive        => CurrentHP > 0;

    // Zırh
    public bool   HasArmor       { get; private set; }
    public int    ArmorHP        { get; private set; }
    public int    CurrentArmorHP { get; private set; }

    // Özel
    public bool   IsStealth      { get; private set; }
    public bool   HasDetection   { get; private set; }
    public bool   CanBurn        { get; private set; }
    public int    BurnDuration   { get; private set; }

    // Işık
    public bool   HasLightCollection  { get; private set; }
    public bool   CanStealLight       { get; private set; }

    // Takım (0 = oyuncu, 1 = rakip)
    public int    TeamId         { get; private set; }

    // ─── Olaylar (BattleManager dinler) ──────────────────────────────────────
    public event Action<int>      OnDamageTaken;   // (hasar miktarı)
    public event Action<int>      OnHealed;        // (iyileşme miktarı)
    public event Action<Character> OnDied;         // (ölen karakter)

    // ─── Başlatma ─────────────────────────────────────────────────────────────
    /// <summary>CharacterData'dan runtime değerlerini başlat.</summary>
    public void Initialize(CharacterData characterData, int teamId)
    {
        data = characterData;
        TeamId = teamId;

        CharacterName     = data.cardName;
        MaxHP             = data.hp;
        CurrentHP         = data.hp;
        ATK               = data.atk;
        DEF               = data.def;
        AttackCooldown    = data.attackSpeedTurns;

        HasArmor          = data.hasArmor;
        ArmorHP           = data.armorHp;
        CurrentArmorHP    = data.armorHp;

        IsStealth         = data.hasStealth;
        HasDetection      = data.hasDetection;
        CanBurn           = data.canBurn;
        BurnDuration      = data.burnDuration;
        HasLightCollection = data.hasLightCollection;
        CanStealLight     = data.canStealLight;

        Debug.Log($"[Character] {CharacterName} (Takım {TeamId}) başlatıldı — HP:{MaxHP} ATK:{ATK} DEF:{DEF}");
    }

    // ─── Hasar & İyileşme ─────────────────────────────────────────────────────
    /// <summary>
    /// Karaktere ham hasar uygular. Önce zırh, sonra HP kontrol edilir.
    /// DEF değeri brüt hasardan düşülür (minimum 1 hasar).
    /// </summary>
    public void TakeDamage(int rawDamage)
    {
        int netDamage = Mathf.Max(1, rawDamage - DEF);

        // Zırh varsa önce zırh absorbe eder
        if (HasArmor && CurrentArmorHP > 0)
        {
            int armorAbsorb = Mathf.Min(CurrentArmorHP, netDamage);
            CurrentArmorHP -= armorAbsorb;
            netDamage      -= armorAbsorb;

            if (CurrentArmorHP <= 0)
            {
                HasArmor = false;
                Debug.Log($"[Character] {CharacterName} zırhı kırıldı!");
            }
        }

        if (netDamage <= 0) return;

        CurrentHP -= netDamage;
        CurrentHP  = Mathf.Max(0, CurrentHP);

        OnDamageTaken?.Invoke(netDamage);
        Debug.Log($"[Character] {CharacterName} {netDamage} hasar aldı — Kalan HP: {CurrentHP}/{MaxHP}");

        if (CurrentHP <= 0) Die();
    }

    /// <summary>Karakteri iyileştirir (MaxHP'yi aşamaz).</summary>
    public void Heal(int amount)
    {
        int healed = Mathf.Min(amount, MaxHP - CurrentHP);
        CurrentHP += healed;
        OnHealed?.Invoke(healed);
        Debug.Log($"[Character] {CharacterName} {healed} HP iyileşti — Kalan HP: {CurrentHP}/{MaxHP}");
    }

    // ─── Ölüm ─────────────────────────────────────────────────────────────────
    private void Die()
    {
        Debug.Log($"[Character] {CharacterName} öldü.");
        OnDied?.Invoke(this);
        gameObject.SetActive(false);  // animasyon / efekt için override edilebilir
    }

    // ─── Yardımcılar ──────────────────────────────────────────────────────────
    public CharacterData GetData() => data;

    public override string ToString() =>
        $"{CharacterName} [Takım {TeamId}] HP:{CurrentHP}/{MaxHP} ATK:{ATK} DEF:{DEF}";
}
