using UnityEngine;
using System.Collections.Generic;

public class AIOpponent : MonoBehaviour
{
    [Header("Furkan'ın Kart Verileri (Taslak Liste)")]
    // Claude'un notu: "Sabit bir CardData listesinden rastgele 3 kart seç"
    public List<CardData> availableCards; 

    // Claude'un notu: "BattleManager.StartBattle(playerDeck, aiDeck) imzasını bekle"
    // Bu metod o imza geldiğinde aiDeck'i sağlamak için hazırda bekleyecek.
    public List<CardData> GetRandomDeck()
    {
        List<CardData> aiDeck = new List<CardData>();

        if (availableCards == null || availableCards.Count == 0)
        {
            Debug.LogWarning("[AIOpponent] Kart listesi boş, stub çalışamıyor!");
            return aiDeck;
        }

        // Rastgele 3 kart seçimi (Sprint 1 için yeterli olan kısım)
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            aiDeck.Add(availableCards[randomIndex]);
        }

        Debug.Log("[AIOpponent] Rastgele 3 kartlık deste hazırlandı. (Asıl AI mantığı Sprint 2'de eklenecek)");
        return aiDeck;
    }

    /* 
    ===================================================================
    ZULA (SPRINT 2 İÇİN SAKLANAN TERMİNATÖR AI ALGORİTMASI)
    ===================================================================
    float CalculateCardScore(CardData card) {
        float score = 0f;
        float turnDps = card.cardHasar / Mathf.Max(1f, card.cardHiz);
        score += turnDps * 2.5f; 
        score += card.cardCan * 0.5f; 
        score += card.cardDefans * 3f; 
        if (card.cardZirh) { score += card.cardZirhCani * 0.5f + 20f; }
        score -= card.cardDeger * 1.5f;
        if (card.cardYakma) score += 25f;       
        if (card.cardGizlenme) score += 15f;    
        // ... Sprint 2 gelince bu mantık devreye alınacak!
    }
    ===================================================================
    */
}