using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Otomatik savaş döngüsünü yöneten ana Manager.
/// Konumu: Assets/_Game/Combat/BattleManager.cs
/// Inspector'dan bağlanacak alanlar: playerTeamSpawns, enemyTeamSpawns
/// 
/// Savaş akışı:
///   StartBattle() → tur döngüsü başlar
///   Her tur: tüm canlı karakterler sırayla saldırır (ATK hızına göre)
///   Bir takım tamamen yok olunca → EndBattle()
/// </summary>
public class BattleManager : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Takım Spawn Noktaları")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Ayarlar")]
    [SerializeField] private float turnDelay = 0.8f;   // Turlar arası bekleme (sn)
    [SerializeField] private GameObject characterPrefab; // Character componenti olan prefab

    // ─── Runtime ──────────────────────────────────────────────────────────────
    private List<Character> playerTeam = new();
    private List<Character> enemyTeam  = new();
    private int  currentTurn = 0;
    private bool battleRunning = false;

    // ─── Durum Makinesi ───────────────────────────────────────────────────────
    public enum BattleState { Idle, Running, PlayerWon, EnemyWon, Draw }
    public BattleState State { get; private set; } = BattleState.Idle;

    // ─── Olaylar (UI / diğer sistemler dinleyebilir) ──────────────────────────
    public static event System.Action<int>         OnTurnStarted;   // tur numarası
    public static event System.Action<BattleState> OnBattleEnded;   // sonuç

    // ─── Başlat / Durdur ──────────────────────────────────────────────────────

    /// <summary>
    /// Savaşı başlatır. CharacterData listelerini dışarıdan al (HandUI / RunManager verir).
    /// </summary>
    public void StartBattle(List<CharacterData> playerCards, List<CharacterData> enemyCards)
    {
        if (battleRunning)
        {
            Debug.LogWarning("[BattleManager] Savaş zaten devam ediyor.");
            return;
        }

        SpawnTeam(playerCards, playerSpawnPoints, playerTeam, teamId: 0);
        SpawnTeam(enemyCards,  enemySpawnPoints,  enemyTeam,  teamId: 1);

        currentTurn    = 0;
        State          = BattleState.Running;
        battleRunning  = true;

        Debug.Log("[BattleManager] Savaş başladı!");
        StartCoroutine(BattleLoop());
    }

    /// <summary>Savaşı erken sonlandır (örneğin oyuncu geri çekilirse).</summary>
    public void AbortBattle()
    {
        StopAllCoroutines();
        battleRunning = false;
        State         = BattleState.Idle;
        Debug.Log("[BattleManager] Savaş iptal edildi.");
    }

    // ─── Ana Döngü ────────────────────────────────────────────────────────────

    private IEnumerator BattleLoop()
    {
        while (battleRunning)
        {
            currentTurn++;
            OnTurnStarted?.Invoke(currentTurn);
            Debug.Log($"[BattleManager] ── TUR {currentTurn} ──");

            // Tüm canlı karakterleri saldırı sırasına al
            var turnOrder = GetTurnOrder();

            foreach (var attacker in turnOrder)
            {
                if (!attacker.IsAlive) continue;

                // Saldırı hızı kontrolü (örn. her 2 turda bir saldırır)
                if (currentTurn % attacker.AttackCooldown != 0) continue;

                // Hangi takımda? → Rakip listeyi belirle
                var enemies = attacker.TeamId == 0 ? enemyTeam : playerTeam;

                Character target = TargetSelector.SelectTarget(attacker, enemies);
                if (target == null) continue;

                ExecuteAttack(attacker, target);
            }

            // Kazanan kontrolü
            BattleState? result = CheckWinCondition();
            if (result.HasValue)
            {
                EndBattle(result.Value);
                yield break;
            }

            yield return new WaitForSeconds(turnDelay);
        }
    }

    // ─── Saldırı ──────────────────────────────────────────────────────────────

    private void ExecuteAttack(Character attacker, Character target)
    {
        Debug.Log($"[BattleManager] {attacker.CharacterName} → {target.CharacterName} saldırıyor (ATK:{attacker.ATK})");
        target.TakeDamage(attacker.ATK);

        // Yakma efekti
        if (attacker.CanBurn)
        {
            // TODO Sprint 3: StatusEffectManager.ApplyBurn(target, attacker.BurnDuration)
            Debug.Log($"[BattleManager] {target.CharacterName} yakma efekti aldı ({attacker.BurnDuration} tur)");
        }
    }

    // ─── Kazanan Kontrolü ─────────────────────────────────────────────────────

    private BattleState? CheckWinCondition()
    {
        bool playerAlive = playerTeam.Any(c => c.IsAlive);
        bool enemyAlive  = enemyTeam.Any(c => c.IsAlive);

        if (!playerAlive && !enemyAlive) return BattleState.Draw;
        if (!enemyAlive)                 return BattleState.PlayerWon;
        if (!playerAlive)                return BattleState.EnemyWon;
        return null;
    }

    private void EndBattle(BattleState result)
    {
        battleRunning = false;
        State         = result;

        string msg = result switch
        {
            BattleState.PlayerWon => "🏆 Oyuncu kazandı!",
            BattleState.EnemyWon  => "💀 Düşman kazandı!",
            BattleState.Draw      => "🤝 Berabere!",
            _                     => "Savaş bitti."
        };

        Debug.Log($"[BattleManager] {msg} (Toplam tur: {currentTurn})");
        OnBattleEnded?.Invoke(result);

        // TODO Sprint 3: Kart kazanma, altın sistemi buradan tetiklenecek
    }

    // ─── Yardımcılar ──────────────────────────────────────────────────────────

    /// <summary>Saldırı sırasını belirler — şimdilik karışık (önce oyuncu).</summary>
    private List<Character> GetTurnOrder()
    {
        var order = new List<Character>();
        order.AddRange(playerTeam.Where(c => c.IsAlive));
        order.AddRange(enemyTeam.Where(c => c.IsAlive));
        return order;
    }

    /// <summary>Bir takımı spawn noktalarına yerleştirir ve listeye ekler.</summary>
    private void SpawnTeam(List<CharacterData> cards, Transform[] spawnPoints,
                           List<Character> teamList, int teamId)
    {
        teamList.Clear();

        for (int i = 0; i < cards.Count && i < spawnPoints.Length; i++)
        {
            GameObject go = Instantiate(characterPrefab, spawnPoints[i].position,
                                        spawnPoints[i].rotation);
            var character = go.GetComponent<Character>();

            if (character == null)
            {
                Debug.LogError("[BattleManager] characterPrefab'da Character componenti yok!");
                continue;
            }

            character.Initialize(cards[i], teamId);
            character.OnDied += OnCharacterDied;
            teamList.Add(character);
        }

        Debug.Log($"[BattleManager] Takım {teamId} oluşturuldu — {teamList.Count} karakter.");
    }

    private void OnCharacterDied(Character dead)
    {
        Debug.Log($"[BattleManager] {dead.CharacterName} savaş alanından çıktı.");
    }

    // ─── Gizmo (Editor görsel yardım) ────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawSpawnGizmos(playerSpawnPoints, Color.cyan);
        DrawSpawnGizmos(enemySpawnPoints,  Color.red);
    }

    private void DrawSpawnGizmos(Transform[] points, Color color)
    {
        if (points == null) return;
        Gizmos.color = color;
        foreach (var t in points)
            if (t != null) Gizmos.DrawWireSphere(t.position, 0.4f);
    }
#endif
}
