using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardView : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData data;
    public Image artwork;
    public TextMeshProUGUI nameText, hpText, dmgText;

    private Transform originalParent;
    private Vector3 originalPos;
    private Canvas rootCanvas;
    [HideInInspector] public bool placed = false;

    void Start()
    {
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void Setup(CardData cardData)
    {
        data = cardData;
        if (cardData.cardGorsel != null)
            artwork.sprite = cardData.cardGorsel;
        nameText.text = cardData.cardName;
        hpText.text   = "HP: " + cardData.cardCan;
        dmgText.text  = "DMG: " + cardData.cardHasar;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        originalParent = transform.parent;
        originalPos    = transform.position;
        transform.SetParent(rootCanvas.transform);
        transform.SetAsLastSibling();
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        transform.position += (Vector3)e.delta;
    }

    public void OnEndDrag(PointerEventData e)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (!placed)
        {
            transform.SetParent(originalParent);
            transform.position = originalPos;
        }
    }
}