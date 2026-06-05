using UnityEngine;

/// <summary>
/// Koleksiyon verisi (CardData ScriptableObject) → savaş runtime verisi (CharacterData) dönüşümü.
/// Sadece savaşa giren alanları kopyalar; ışık maliyeti, toplama süresi gibi
/// koleksiyon-spesifik alanlar bilinçli olarak atlanır.
///
/// Kullanım:
///   CharacterData cd = CardDataConverter.Convert(myCardData);
///
/// Konum: Assets/_Game/Data/CardDataConverter.cs
/// </summary>
public static class CardDataConverter
{
    /// <summary>
    /// Bir <see cref="CardData"/> nesnesini savaş sırasında kullanılacak
    /// <see cref="CharacterData"/> nesnesine dönüştürür.
    ///
    /// Alan eşleşmeleri:
    ///   cardCan      → hp
    ///   cardHasar    → atk
    ///   cardDefans   → def
    ///   cardHiz      → attackSpeedTurns  (float → int, min 1)
    ///   cardZirh     → hasArmor
    ///   cardZirhCani → armorHp
    ///   cardGorsel   → cardArtwork
    /// </summary>
    /// <param name="card">Kaynak kart verisi (ScriptableObject). Null olamaz.</param>
    /// <returns>Doldurulmuş <see cref="CharacterData"/> instance'ı.</returns>
    public static CharacterData Convert(CardData card)
    {
        if (card == null)
        {
            Debug.LogError("[CardDataConverter] CardData null — dönüşüm iptal edildi.");
            return null;
        }

        // CharacterData bir ScriptableObject; runtime'da CreateInstance ile üretilmeli.
        var cd = ScriptableObject.CreateInstance<CharacterData>();

        // ── Kimlik ────────────────────────────────────────────────────────────
        cd.cardName    = card.cardName;
        cd.cardIrk     = card.cardIrk;
        cd.cardTip     = card.cardTip;
        cd.cardNadirlik = card.cardNadirlik;

        // ── Temel Savaş İstatistikleri ────────────────────────────────────────
        cd.hp  = card.cardCan;
        cd.atk = card.cardHasar;
        cd.def = card.cardDefans;

        // cardHiz float (kaç turda bir saldırır); en az 1 tur olmalı.
        cd.attackSpeedTurns = Mathf.Max(1, Mathf.RoundToInt(card.cardHiz));

        // ── Zırh ─────────────────────────────────────────────────────────────
        cd.hasArmor = card.cardZirh;
        cd.armorHp  = card.cardZirhCani;

        // ── Özel Yetenekler ───────────────────────────────────────────────────
        cd.hasStealth      = card.cardGizlenme;
        cd.hasDetection    = card.cardTespit;
        cd.canBurn         = card.cardYakma;

        // ── Işık Mekaniği ─────────────────────────────────────────────────────
        cd.hasLightCollection = card.cardToplama;
        cd.canStealLight      = card.cardCalma;

        // ── Görsel ───────────────────────────────────────────────────────────
        // cardGorsel (Sprite) → CharacterData.cardArtwork
        cd.cardArtwork = card.cardGorsel;

        return cd;
    }

    /// <summary>
    /// Birden fazla kartı toplu dönüştürür.
    /// Null kartlar listeye eklenmez; her atlanma için uyarı loglanır.
    /// </summary>
    public static System.Collections.Generic.List<CharacterData> ConvertAll(
        System.Collections.Generic.IEnumerable<CardData> cards)
    {
        var result = new System.Collections.Generic.List<CharacterData>();

        if (cards == null)
        {
            Debug.LogWarning("[CardDataConverter] ConvertAll: kart listesi null.");
            return result;
        }

        foreach (var card in cards)
        {
            var cd = Convert(card);
            if (cd != null)
                result.Add(cd);
        }

        return result;
    }
}
