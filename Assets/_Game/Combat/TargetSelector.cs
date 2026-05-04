using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Bir karakterin saldıracağı hedefi seçen sistem.
/// Sprint 2'de genişletilecek — şu an "en yakın düşman" mantığı aktif.
/// Konumu: Assets/_Game/Combat/TargetSelector.cs
/// </summary>
public static class TargetSelector
{
    // ─── Genel API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Saldıranın mevcut stratejisine göre hedef döndürür.
    /// Hiç uygun hedef yoksa null döner.
    /// </summary>
    public static Character SelectTarget(Character attacker, List<Character> enemies)
    {
        if (attacker == null || enemies == null || enemies.Count == 0)
            return null;

        // Gizlenmiş düşmanları filtrele (attacker Detection yoksa göremez)
        var visibleEnemies = FilterVisible(attacker, enemies);
        if (visibleEnemies.Count == 0) return null;

        // TODO Sprint 2: strateji seçimi burada genişleyecek (Priority enum)
        return NearestEnemy(attacker, visibleEnemies);
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

    // ─── Strateji Stub'ları (Sprint 2–3 için ayrılmış) ───────────────────────

    /// <summary>TODO: En düşük HP'li düşmanı hedef al.</summary>
    private static Character LowestHpEnemy(List<Character> enemies)
    {
        // STUB — ileride doldurulacak
        return enemies.OrderBy(e => e.CurrentHP).FirstOrDefault();
    }

    /// <summary>TODO: En yüksek ATK'lı (en tehlikeli) düşmanı hedef al.</summary>
    private static Character HighestThreatEnemy(List<Character> enemies)
    {
        // STUB — ileride doldurulacak
        return enemies.OrderByDescending(e => e.ATK).FirstOrDefault();
    }

    /// <summary>TODO: Rastgele düşman seç (karıştırılmış düşman listeleri için).</summary>
    private static Character RandomEnemy(List<Character> enemies)
    {
        // STUB — ileride doldurulacak
        if (enemies.Count == 0) return null;
        return enemies[Random.Range(0, enemies.Count)];
    }

    // ─── Gizlenme Filtresi ────────────────────────────────────────────────────

    /// <summary>
    /// Saldıranın göremeyeceği gizlenmiş karakterleri listeden çıkarır.
    /// Detection varsa tüm düşmanlar görünürdür.
    /// </summary>
    private static List<Character> FilterVisible(Character attacker, List<Character> enemies)
    {
        if (attacker.HasDetection) return enemies.Where(e => e.IsAlive).ToList();

        return enemies
            .Where(e => e.IsAlive && !e.IsStealth)
            .ToList();
    }
}
