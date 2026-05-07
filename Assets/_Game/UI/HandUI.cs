 using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public GameObject cardPrefab;
    public int maxCards = 7;
    private List<CardView> hand = new();

    public void AddCard(CardData cardData)
    {
        if (hand.Count >= maxCards) return;
        GameObject go = Instantiate(cardPrefab, transform);
        CardView view = go.GetComponent<CardView>();
        view.Setup(cardData);
        hand.Add(view);
    }

    public void RemoveCard(CardView card)
    {
        hand.Remove(card);
        Destroy(card.gameObject);
    }

    public List<CardView> GetHand() => hand;
}