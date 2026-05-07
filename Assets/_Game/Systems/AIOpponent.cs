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

    
}