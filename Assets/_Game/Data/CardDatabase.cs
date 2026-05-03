using System.Collections.Generic;
using UnityEngine;

// CardDatabase — tüm CardData asset'lerini runtime'da tek noktadan sunar.
// Inspector'da "allCards" listesine mevcut kart asset'leri sürükle-bırak ile eklenir.
// Shop, AIOpponent, RunManager ve diğer sistemler bu ScriptableObject'e referans alır;
// her sistem kendi yolunu bulmak yerine buradan okur.
//
// Kullanım:
//   Project penceresinde sağ tık → Card Battler → Card Database ile bir asset oluştur.
//   Oluşturulan asset'i Inspector'da açıp allCards listesini doldur.

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Card Battler/Card Database")]
public class CardDatabase : ScriptableObject
{
    [Tooltip("Oyundaki tüm kartların listesi. Yeni kart eklendiğinde buraya da ekle.")]
    public List<CardData> allCards = new List<CardData>();

    /// <summary>
    /// Belirtilen nadirliğe sahip kartları döndürür.
    /// </summary>
    public List<CardData> GetByNadirlik(CardNadirlik nadirlik)
    {
        return allCards.FindAll(card => card.cardNadirlik == nadirlik);
    }

    /// <summary>
    /// Belirtilen tipe sahip kartları döndürür.
    /// </summary>
    public List<CardData> GetByTip(CardTip tip)
    {
        return allCards.FindAll(card => card.cardTip == tip);
    }
}
