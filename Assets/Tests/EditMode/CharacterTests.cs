using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Character ve TargetSelector için EditMode testleri.
/// Konumu: Assets/Tests/EditMode/CharacterTests.cs
/// Çalıştırmak için: Unity → Window → General → Test Runner → EditMode → Run All
/// </summary>
public class CharacterTests
{
    // ─── Yardımcı: CharacterData oluştur ─────────────────────────────────────

    private CharacterData MakeData(string name = "Test",
                                   int hp  = 200,
                                   int atk = 100,
                                   int def = 20,
                                   bool hasArmor   = false,
                                   int  armorHp    = 0,
                                   bool stealth    = false,
                                   bool detection  = false)
    {
        var d = ScriptableObject.CreateInstance<CharacterData>();
        d.cardName         = name;
        d.hp               = hp;
        d.atk              = atk;
        d.def              = def;
        d.hasArmor         = hasArmor;
        d.armorHp          = armorHp;
        d.hasStealth       = stealth;
        d.hasDetection     = detection;
        d.attackSpeedTurns = 1;
        return d;
    }

    private Character MakeCharacter(CharacterData data, int teamId = 0)
    {
        var go = new GameObject("TestCharacter");
        var c  = go.AddComponent<Character>();
        c.Initialize(data, teamId);
        return c;
    }

    // ─── Initialize Testleri ──────────────────────────────────────────────────

    [Test]
    public void Initialize_SetsHPCorrectly()
    {
        var c = MakeCharacter(MakeData(hp: 300));
        Assert.AreEqual(300, c.MaxHP);
        Assert.AreEqual(300, c.CurrentHP);
    }

    [Test]
    public void Initialize_SetsTeamId()
    {
        var c = MakeCharacter(MakeData(), teamId: 1);
        Assert.AreEqual(1, c.TeamId);
    }

    [Test]
    public void Initialize_IsAlive_True()
    {
        var c = MakeCharacter(MakeData());
        Assert.IsTrue(c.IsAlive);
    }

    // ─── TakeDamage — DEF hesabı ──────────────────────────────────────────────

    [Test]
    public void TakeDamage_ReducesHP_AfterDEF()
    {
        // DEF=20, ham hasar=100 → net hasar=80 → HP: 200-80=120
        var c = MakeCharacter(MakeData(hp: 200, def: 20));
        c.TakeDamage(100);
        Assert.AreEqual(120, c.CurrentHP);
    }

    [Test]
    public void TakeDamage_MinimumOneDamage_WhenATKLessThanDEF()
    {
        // DEF=50 > ATK=10 → net hasar minimum 1 → HP: 200-1=199
        var c = MakeCharacter(MakeData(hp: 200, def: 50));
        c.TakeDamage(10);
        Assert.AreEqual(199, c.CurrentHP);
    }

    [Test]
    public void TakeDamage_HPNeverBelowZero()
    {
        var c = MakeCharacter(MakeData(hp: 50, def: 0));
        c.TakeDamage(9999);
        Assert.AreEqual(0, c.CurrentHP);
    }

    [Test]
    public void TakeDamage_FiresOnDamageTakenEvent()
    {
        var c = MakeCharacter(MakeData(hp: 200, def: 0));
        int received = -1;
        c.OnDamageTaken += dmg => received = dmg;
        c.TakeDamage(60);
        Assert.AreEqual(60, received);
    }

    // ─── TakeDamage — Ölüm ───────────────────────────────────────────────────

    [Test]
    public void TakeDamage_KillsCharacter_WhenHPReachesZero()
    {
        var c = MakeCharacter(MakeData(hp: 100, def: 0));
        c.TakeDamage(100);
        Assert.IsFalse(c.IsAlive);
    }

    [Test]
    public void TakeDamage_FiresOnDiedEvent()
    {
        var c = MakeCharacter(MakeData(hp: 50, def: 0));
        Character diedCharacter = null;
        c.OnDied += who => diedCharacter = who;
        c.TakeDamage(999);
        Assert.IsNotNull(diedCharacter);
    }

    // ─── TakeDamage — Zırh ───────────────────────────────────────────────────

    [Test]
    public void TakeDamage_ArmorAbsorbsFirst()
    {
        // Zırh=50, net hasar=80 → zırh 50 absorbe eder, 30 HP'ye geçer
        var c = MakeCharacter(MakeData(hp: 200, def: 0, hasArmor: true, armorHp: 50));
        c.TakeDamage(80);
        Assert.AreEqual(170, c.CurrentHP);   // 200 - 30 = 170
        Assert.AreEqual(0,   c.CurrentArmorHP);
        Assert.IsFalse(c.HasArmor);
    }

    [Test]
    public void TakeDamage_ArmorFullyBlocksDamage_WhenArmorHighEnough()
    {
        // Zırh=200, net hasar=50 → tüm hasar zırha → HP değişmez
        var c = MakeCharacter(MakeData(hp: 200, def: 0, hasArmor: true, armorHp: 200));
        c.TakeDamage(50);
        Assert.AreEqual(200, c.CurrentHP);
        Assert.AreEqual(150, c.CurrentArmorHP);
    }

    // ─── Heal Testleri ────────────────────────────────────────────────────────

    [Test]
    public void Heal_IncreasesHP()
    {
        var c = MakeCharacter(MakeData(hp: 200, def: 0));
        c.TakeDamage(100);       // HP: 100
        c.Heal(40);
        Assert.AreEqual(140, c.CurrentHP);
    }

    [Test]
    public void Heal_NeverExceedsMaxHP()
    {
        var c = MakeCharacter(MakeData(hp: 200, def: 0));
        c.TakeDamage(10);        // HP: 190
        c.Heal(9999);
        Assert.AreEqual(200, c.CurrentHP);
    }

    [Test]
    public void Heal_FiresOnHealedEvent()
    {
        var c = MakeCharacter(MakeData(hp: 200, def: 0));
        c.TakeDamage(60);
        int healAmount = -1;
        c.OnHealed += h => healAmount = h;
        c.Heal(30);
        Assert.AreEqual(30, healAmount);
    }

    // ─── TargetSelector Testleri ──────────────────────────────────────────────

    [Test]
    public void SelectTarget_ReturnsNull_WhenNoEnemies()
    {
        var attacker = MakeCharacter(MakeData());
        var result   = TargetSelector.SelectTarget(attacker, new List<Character>());
        Assert.IsNull(result);
    }

    [Test]
    public void SelectTarget_ReturnsNull_WhenAllEnemiesDead()
    {
        var attacker = MakeCharacter(MakeData());
        var dead     = MakeCharacter(MakeData(hp: 10, def: 0), teamId: 1);
        dead.TakeDamage(9999);

        var result = TargetSelector.SelectTarget(attacker, new List<Character> { dead });
        Assert.IsNull(result);
    }

    [Test]
    public void SelectTarget_ReturnsClosestEnemy()
    {
        var attacker = MakeCharacter(MakeData());
        attacker.transform.position = Vector3.zero;

        var near = MakeCharacter(MakeData(name: "Yakın"), teamId: 1);
        near.transform.position = new Vector3(1f, 0, 0);

        var far = MakeCharacter(MakeData(name: "Uzak"), teamId: 1);
        far.transform.position = new Vector3(10f, 0, 0);

        var result = TargetSelector.SelectTarget(attacker, new List<Character> { far, near });
        Assert.AreEqual("Yakın", result.CharacterName);
    }

    [Test]
    public void SelectTarget_IgnoresStealthEnemy_WhenNoDetection()
    {
        var attacker = MakeCharacter(MakeData(detection: false));
        var stealthy = MakeCharacter(MakeData(name: "Gizli", stealth: true), teamId: 1);

        var result = TargetSelector.SelectTarget(attacker, new List<Character> { stealthy });
        Assert.IsNull(result);
    }

    [Test]
    public void SelectTarget_CanSeeStealthEnemy_WhenDetectionEnabled()
    {
        var attacker = MakeCharacter(MakeData(detection: true));
        var stealthy = MakeCharacter(MakeData(name: "Gizli", stealth: true), teamId: 1);

        var result = TargetSelector.SelectTarget(attacker, new List<Character> { stealthy });
        Assert.IsNotNull(result);
        Assert.AreEqual("Gizli", result.CharacterName);
    }

    // ─── Teardown ─────────────────────────────────────────────────────────────

    [TearDown]
    public void Cleanup()
    {
        foreach (var go in Object.FindObjectsOfType<GameObject>())
            Object.DestroyImmediate(go);
    }
}
