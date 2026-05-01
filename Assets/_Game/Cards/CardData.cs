using UnityEngine;
[CreateAssetMenu (fileName = "NewCard", menuName = "Card Battler/Card Data")]

public class CardData : ScriptableObject 
{
    
    [Header("Temel Bilgiler")]
    public string cardName;
    public enum cardIrk;
    {
    Düşmüş
    Direnen,
    Gölge
    }
    
    public enum cardTip
    {
    Destek,
    Hasar,
    Suikastçi,
    Tank
    }
    public enum cardNadirlik;
    {
    Sıradan,
    Nadir,
    Destansı,
    Efsanevi
    }
    
    [Header("İstatistikler")]
    public int cardDeger; //Kart maliyeti. Kartı çağırmak için gereken enerji
    public int cardCan;
    public bool cardZirh; //Ek can ya da ön can. İlk darbeyi karşılar. 0 olup kırılsa bile o saldırı ana cana vuramaz
    public int cardHasar;
    public float cardHiz;
    public int cardDefans; //Yediği bütün saldırıları azaltır. 120 hasar - 30 defans = 90 can gider.
    
    [Header("Özellikler")]
    public bool cardToplama; //Kart oyundayken birkaç turda bir enerji kazandırır
    public int cardToplamaSuresi;
    public bool cardCalma; //Son darbeyi vurduğu rakip kartın değerinin bir kısmını kazandırır
    public bool cardGizlenme; //İlk darbeyi vurmadan önce görünmez
    public bool cardTespit; //Gizlenmeyi iptal eder
    public bool cardYakma;

}
