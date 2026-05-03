using UnityEngine;
[CreateAssetMenu (fileName = "NewCard", menuName = "Card Battler/Card Data")]

public class CardData : ScriptableObject 
{

    public enum CardIrk
    {
    Düşmüş,
    Direnen,
    Gölge
    }
    
    public enum CardTip
    {
    Destek,
    Hasar,
    Suikastçi,
    Tank
    }
    public enum CardNadirlik
    {
    Sıradan,
    Nadir,
    Destansı,
    Efsanevi
    }

    
    [Header("Temel Bilgiler")]
    public string cardName;
    public string cardAciklama;
    public Sprite cardGorsel;
    public CardIrk cardIrk;
    public CardTip cardTip;
    public CardNadirlik cardNadirlik;
    
        
    [Header("İstatistikler")]
    public int cardDeger; //Kart maliyeti. Kartı çağırmak için gereken enerji
    public int cardCan; //Kartın 0 olduğunda öldüğü dayanıklılık değeri
    public bool cardZirh; //Ek can ya da ön can. İlk darbeyi karşılar. 0 olup kırılsa bile o saldırı ana cana vuramaz
    public int cardZirhCani; //Zırhın canı. Zırh kırılmadan önce dayanabileceği hasar miktarı
    public int cardHasar; //Kartın düşman karta saldırdığında vuracağı saf değer. Defans hesaba katılmamış.
    public float cardHiz; //Kartın kaç turda bir saldırı yapacağı. İlk turunda saldırır ondan sonra hesaba katılır.
    public int cardDefans; //Yediği bütün saldırıları azaltır. 120 hasar - 30 defans = 90 can gider.
    
    [Header("Özellikler")]
    public bool cardToplama; //Kart oyundayken birkaç turda bir enerji kazandırır
    public float cardToplamaSuresi; //Kartın kaç turda bir enerji kazandıracağı. ilk eklendiğinde çalışmaz. O değerdeki tur kadar sonra ilk defa kazandırır.
    public bool cardCalma; //Son darbeyi vurduğu rakip kartın değerinin bir kısmını kazandırır
    public bool cardGizlenme; //İlk darbeyi vurmadan önce görünmez
    public bool cardTespit; //Gizlenmeyi iptal eder
    public bool cardYakma; //Vurduğu düşman karakterine 3 tur yakma uygular.
    public bool cardAtesDirenci; //Kartın yanmaya maruz kalmasını engeller.

}
