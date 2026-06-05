using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Savaşı başlatmaktan sorumlu köprü bileşeni.
///
/// Sorumluluklar:
///   1. HandUI'dan placed == true olan kartları toplar.
///   2. CardDataConverter ile her CartData → CharacterData'ya dönüştürür.
///   3. AIOpponent'dan rakip desteyi alır ve aynı dönüşümü uygular.
///   4. BattleManager.StartBattle() çağırır.
///
/// Inspector bağlantıları:
///   • handUI        — sahne içindeki HandUI bileşeni
///   • battleManager — sahne içindeki BattleManager bileşeni
///   • aiOpponent    — sahne içindeki AIOpponent bileşeni
///
/// Konum: Assets/_Game/Combat/BattleStarter.cs
/// </summary>
public class BattleStarter : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Bağlantılar")]
    [SerializeField] private HandUI       handUI;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private AIOpponent   aiOpponent;

    [Header("Ayarlar")]
    [Tooltip("Savaş başlatmak için sahaya sürülmesi gereken minimum kart sayısı.")]
    [SerializeField] private int minPlacedCards = 1;

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Savaşı başlatır. UI "Savaşa Başla" butonuna bağlanabilir.
    /// </summary>
    public void StartBattle()
    {
        // ── 1. Bağlantı kontrolü ──────────────────────────────────────────────
        if (!ValidateDependencies()) return;

        // ── 2. Yerleştirilmiş (placed == true) kartları topla ─────────────────
        List<CardData> placedCards = GetPlacedCards();

        if (placedCards.Count < minPlacedCards)
        {
            Debug.LogWarning($"[BattleStarter] En az {minPlacedCards} kart sahaya sürülmeli. " +
                             $"Şu an: {placedCards.Count}");
            return;
        }

        // ── 3. CardData → CharacterData dönüşümü (oyuncu takımı) ─────────────
        List<CharacterData> playerDeck = CardDataConverter.ConvertAll(placedCards);

        if (playerDeck.Count == 0)
        {
            Debug.LogError("[BattleStarter] Dönüşüm sonrası oyuncu destesi boş — savaş iptal.");
            return;
        }

        // ── 4. AI destesini al ve dönüştür ────────────────────────────────────
        List<CardData> aiDeck = aiOpponent.GetRandomDeck();
        List<CharacterData> enemyDeck = CardDataConverter.ConvertAll(aiDeck);

        if (enemyDeck.Count == 0)
        {
            Debug.LogError("[BattleStarter] AI destesi boş — savaş iptal.");
            return;
        }

        // ── 5. Savaşı başlat ──────────────────────────────────────────────────
        Debug.Log($"[BattleStarter] Savaş başlatılıyor — " +
                  $"Oyuncu: {playerDeck.Count} kart | AI: {enemyDeck.Count} kart");

        battleManager.StartBattle(playerDeck, enemyDeck);
    }

    // ─── Yardımcılar ──────────────────────────────────────────────────────────

    /// <summary>HandUI'daki placed == true kartların CardData listesini döndürür.</summary>
    private List<CardData> GetPlacedCards()
    {
        var result = new List<CardData>();

        foreach (CardView cv in handUI.GetHand())
        {
            if (cv != null && cv.placed && cv.data != null)
                result.Add(cv.data);
        }

        Debug.Log($"[BattleStarter] Yerleştirilmiş kart sayısı: {result.Count}");
        return result;
    }

    /// <summary>Gerekli bileşen referanslarının atanıp atanmadığını doğrular.</summary>
    private bool ValidateDependencies()
    {
        bool ok = true;

        if (handUI == null)
        {
            Debug.LogError("[BattleStarter] HandUI atanmamış!");
            ok = false;
        }
        if (battleManager == null)
        {
            Debug.LogError("[BattleStarter] BattleManager atanmamış!");
            ok = false;
        }
        if (aiOpponent == null)
        {
            Debug.LogError("[BattleStarter] AIOpponent atanmamış!");
            ok = false;
        }

        return ok;
    }

    // ─── Auto-Find (opsiyonel) ────────────────────────────────────────────────

    private void Awake()
    {
        // Referanslar Inspector'dan atanmamışsa sahnede ara — bağlantıyı zorunlu kıl.
        if (handUI        == null) handUI        = FindObjectOfType<HandUI>();
        if (battleManager == null) battleManager = FindObjectOfType<BattleManager>();
        if (aiOpponent    == null) aiOpponent    = FindObjectOfType<AIOpponent>();
    }
}
