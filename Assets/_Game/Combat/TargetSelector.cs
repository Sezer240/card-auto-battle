using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Bir karakterin saldıracağı hedefi seçen sistem.
/// Sprint 2: TargetPriority enum'u eklendi; Nearest ve LowestHp stratejileri aktif.
/// Konum: Assets/_Game/Combat/TargetSelector.cs
/// </summary>
public static class TargetSelector
{
    // ─── Hedef Önceliği Enum ──────────────────────────────────────────────────

    /// <summary>
    /// Hangi hedefe öncelik verileceğini belirler.
    /// BattleManager veya karakter verisi üzerinden geçilebilir.
    /// </summary>
    public enum TargetPriority
    {
        /// <summary>Pozisyon olarak en yakın canlı düşmanı hedef al.</summary>
        Nearest,

        /// <summary>En az HP'si kalan canlı düşmanı hedef al (öldürme odaklı).</summary>
        LowestHp
    }

    // ─── Genel API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Saldıranın mevcut stratejisine göre hedef döndürür.
    /// Hiç uygun hedef yoksa null döner.
    /// </summary>
    /// <param name="attacker">Saldıran karakter.</param>
    /// <param name="enemies">Rakip takımın karakter listesi.</param>
    /// <param name="priority">
    ///   Hedef seçim stratejisi.
    ///   Varsayılan: <see cref="TargetPriority.Nearest"/> — geriye dönük uyumluluk korunur.
    /// </param>
    public static Character SelectTarget(
        Character attacker,
        List<Character> enemies,
        TargetPriority priority = TargetPriority.Nearest)
    {
        if (attacker == null || enemies == null || enemies.Count == 0)
            return null;

        // Gizlenmiş düşmanları filtrele (attacker Detection yoksa göremez)
        var visibleEnemies = FilterVisible(attacker, enemies);
        if (visibleEnemies.Count == 0) return null;

        return priority switch
        {
            TargetPriority.Nearest  => NearestEnemy(attacker, visibleEnemies),
            TargetPriority.LowestHp => LowestHpEnemy(visibleEnemies),
            _                       => NearestEnemy(attacker, visibleEnemies)
        };
    }

    // ─── Strateji: En Yakın ───────────────────────────────────────────────────

    /// <summary>Pozisyona göre en yakın canlı düşmanı döndürür.</summary>
    private static Character NearestEnemy(Character attacker, List<Character> enemies)
    {
        return enemies
            .Where(e => e.IsAlive)
            .OrderBy(e => Vector3.Distance(
                attacker.transform.position,
                e.transform.position))
            .FirstOrDefault();
    }

    // ─── Strateji: En Az HP ───────────────────────────────────────────────────

    /// <summary>
    /// Canlı düşmanlar arasından CurrentHP değeri en düşük olanı döndürür.
    /// HP eşitliğinde ilk bulunan seçilir.
    /// </summary>
    private static Character LowestHpEnemy(List<Character> enemies)
    {
        return enemies
            .Where(e => e.IsAlive)
            .OrderBy(e => e.CurrentHP)
            .FirstOrDefault();
    }

    // ─── Strateji Stub'ları (Sprint 3 için ayrılmış) ──────────────────────────

    /// <summary>TODO Sprint 3: En yüksek ATK'lı (en tehlikeli) düşmanı hedef al.</summary>
    private static Character HighestThreatEnemy(List<Character> enemies)
    {
        // STUB — Sprint 3'te doldurulacak
        return enemies
            .Where(e => e.IsAlive)
            .OrderByDescending(e => e.ATK)
            .FirstOrDefault();
    }

    /// <summary>TODO Sprint 3: Rastgele düşman seç.</summary>
    private static Character RandomEnemy(List<Character> enemies)
    {
        // STUB — Sprint 3'te doldurulacak
        var alive = enemies.Where(e => e.IsAlive).ToList();
        if (alive.Count == 0) return null;
        return alive[Random.Range(0, alive.Count)];
    }

    // ─── Gizlenme Filtresi ────────────────────────────────────────────────────

    /// <summary>
    /// Saldıranın göremeyeceği gizlenmiş karakterleri listeden çıkarır.
    /// Detection varsa tüm düşmanlar görünürdür.
    /// </summary>
    private static List<Character> FilterVisible(Character attacker, List<Character> enemies)
    {
        if (attacker.HasDetection)
            return enemies.Where(e => e.IsAlive).ToList();

        return enemies
            .Where(e => e.IsAlive && !e.IsStealth)
            .ToList();
    }
}
