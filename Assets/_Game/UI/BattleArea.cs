using UnityEngine;
using UnityEngine.EventSystems;

public class BattleArea : MonoBehaviour, IDropHandler
{
    // Kart bu alana bırakıldığında Unity otomatik çağırır
    public void OnDrop(PointerEventData eventData)
    {
        // Sürüklenen nesneyi al
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        CardView card = dropped.GetComponent<CardView>();
        if (card == null) return;

        // Kartı yerleştirildi olarak işaretle
        card.placed = true;

        // Kartı BattleArea'nın içine taşı
        card.transform.SetParent(transform);

        Debug.Log(card.data.cardName + " savaş alanına yerleştirildi!");
    }
}