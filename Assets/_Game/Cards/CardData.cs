[CreateAssetMenu (fileName = "NewCard", menuName = "Card Battler/Card Data")]

public class CardData : ScriptableObject 
{
    
    [Header("Temel Bilgiler")]
    public string cardName;
    public string cardIrk;
    public string cardTip;
    public string cardNadirlik;
    
    [Header("İstatistikler")]
    public int cardDeger;
    public int cardCan;
    public bool cardZirh;
    public int cardHasar;
    public float cardHiz;
    public int cardDefans;
    
    [Header("Özellikler")]
    public bool cardToplama;
    public bool cardCalma;
    public bool cardGizlenme;
    public bool cardTespit;
    public bool cardYakma;

}
