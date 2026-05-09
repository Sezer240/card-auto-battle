#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Battle sahnesi Inspector bağlantılarını otomatik olarak kurar.
///
/// Kullanım:
///   Unity menüsünden → Card Battler → Setup Battle Scene
///
/// Ne yapar:
///   • BattleManager.playerSpawnPoints  → sahnedeki PlayerSpawnPoints alt objelerini bağlar
///   • BattleManager.enemySpawnPoints   → sahnedeki EnemySpawnPoints alt objelerini bağlar
///   • BattleManager.characterPrefab    → Assets/_Game/Characters/Prefabs/ altındaki prefabı bağlar
///   • BattleStarter → handUI, battleManager, aiOpponent referanslarını bağlar
///
/// Gerekli sahne hiyerarşisi (manuel oluşturulacak, sonra bu script bağlantı kurar):
/// ┌ BattleManager       (BattleManager + BattleStarter bileşenleri)
/// ├ EnemyAI             (AIOpponent bileşeni — zaten mevcut)
/// ├ Hand                (HandUI bileşeni — zaten mevcut)
/// ├ PlayerSpawnPoints/
/// │  ├ Slot0
/// │  ├ Slot1
/// │  └ Slot2
/// └ EnemySpawnPoints/
///    ├ Slot0
///    ├ Slot1
///    └ Slot2
///
/// Konum: Assets/_Game/Combat/Editor/BattleSceneSetup.cs
/// </summary>
public static class BattleSceneSetup
{
    private const string BATTLE_SCENE_PATH = "Assets/Scenes/Battle/Battle.unity";
    private const string CHARACTER_PREFAB_SEARCH = "t:Prefab Character";

    [MenuItem("Card Battler/Setup Battle Scene")]
    public static void SetupBattleScene()
    {
        // Battle sahnesi açık mı kontrol et
        Scene active = SceneManager.GetActiveScene();
        if (!active.path.Contains("Battle"))
        {
            bool open = EditorUtility.DisplayDialog(
                "Battle Sahnesi Gerekli",
                $"Lütfen önce Battle sahnesini açın:\n{BATTLE_SCENE_PATH}\n\nŞimdi açılsın mı?",
                "Evet, Aç", "İptal");

            if (!open) return;

            EditorSceneManager.OpenScene(BATTLE_SCENE_PATH);
            active = SceneManager.GetActiveScene();
        }

        int fixCount = 0;

        // ── BattleManager ────────────────────────────────────────────────────
        BattleManager bm = Object.FindObjectOfType<BattleManager>();
        if (bm == null)
        {
            Debug.LogError("[BattleSceneSetup] Sahnede BattleManager bulunamadı. " +
                           "Lütfen bir GameObject'e BattleManager bileşenini ekleyin.");
        }
        else
        {
            fixCount += WireSpawnPoints(bm);
            fixCount += WireCharacterPrefab(bm);
        }

        // ── BattleStarter ────────────────────────────────────────────────────
        BattleStarter starter = Object.FindObjectOfType<BattleStarter>();
        if (starter == null)
        {
            Debug.LogWarning("[BattleSceneSetup] Sahnede BattleStarter bulunamadı. " +
                             "BattleManager GameObject'ine BattleStarter bileşeni ekleniyor...");
            if (bm != null)
            {
                starter = bm.gameObject.AddComponent<BattleStarter>();
                fixCount++;
            }
        }

        if (starter != null)
            fixCount += WireBattleStarter(starter, bm);

        // ── Kaydet ───────────────────────────────────────────────────────────
        if (fixCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(active);
            EditorSceneManager.SaveScene(active);
            Debug.Log($"[BattleSceneSetup] ✅ {fixCount} bağlantı kuruldu ve sahne kaydedildi.");
        }
        else
        {
            Debug.Log("[BattleSceneSetup] ℹ️ Tüm bağlantılar zaten kurulu.");
        }
    }

    // ─── Spawn Noktaları ──────────────────────────────────────────────────────

    private static int WireSpawnPoints(BattleManager bm)
    {
        int fixes = 0;
        var serialized = new SerializedObject(bm);

        // playerSpawnPoints
        var playerProp = serialized.FindProperty("playerSpawnPoints");
        Transform playerRoot = GameObject.Find("PlayerSpawnPoints")?.transform;
        if (playerRoot != null && playerProp != null)
        {
            var slots = GetChildTransforms(playerRoot);
            if (slots.Length > 0)
            {
                playerProp.ClearArray();
                for (int i = 0; i < slots.Length; i++)
                {
                    playerProp.InsertArrayElementAtIndex(i);
                    playerProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
                }
                fixes++;
                Debug.Log($"[BattleSceneSetup] playerSpawnPoints: {slots.Length} slot bağlandı.");
            }
        }
        else if (playerRoot == null)
        {
            Debug.LogWarning("[BattleSceneSetup] 'PlayerSpawnPoints' GameObject bulunamadı. " +
                             "Hiyerarşide oluşturun: PlayerSpawnPoints > Slot0, Slot1, Slot2");
        }

        // enemySpawnPoints
        var enemyProp = serialized.FindProperty("enemySpawnPoints");
        Transform enemyRoot = GameObject.Find("EnemySpawnPoints")?.transform;
        if (enemyRoot != null && enemyProp != null)
        {
            var slots = GetChildTransforms(enemyRoot);
            if (slots.Length > 0)
            {
                enemyProp.ClearArray();
                for (int i = 0; i < slots.Length; i++)
                {
                    enemyProp.InsertArrayElementAtIndex(i);
                    enemyProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
                }
                fixes++;
                Debug.Log($"[BattleSceneSetup] enemySpawnPoints: {slots.Length} slot bağlandı.");
            }
        }
        else if (enemyRoot == null)
        {
            Debug.LogWarning("[BattleSceneSetup] 'EnemySpawnPoints' GameObject bulunamadı. " +
                             "Hiyerarşide oluşturun: EnemySpawnPoints > Slot0, Slot1, Slot2");
        }

        serialized.ApplyModifiedProperties();
        return fixes;
    }

    // ─── Character Prefab ─────────────────────────────────────────────────────

    private static int WireCharacterPrefab(BattleManager bm)
    {
        var serialized = new SerializedObject(bm);
        var prop = serialized.FindProperty("characterPrefab");

        if (prop == null || prop.objectReferenceValue != null) return 0;

        // Proje içinde Character prefabını ara
        string[] guids = AssetDatabase.FindAssets(CHARACTER_PREFAB_SEARCH);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && prefab.GetComponent<Character>() != null)
            {
                prop.objectReferenceValue = prefab;
                serialized.ApplyModifiedProperties();
                Debug.Log($"[BattleSceneSetup] characterPrefab: '{prefab.name}' bağlandı ({path}).");
                return 1;
            }
        }

        Debug.LogWarning("[BattleSceneSetup] Character bileşeni olan prefab bulunamadı. " +
                         "Assets/_Game/Characters/Prefabs/ altına bir prefab ekleyin.");
        return 0;
    }

    // ─── BattleStarter ────────────────────────────────────────────────────────

    private static int WireBattleStarter(BattleStarter starter, BattleManager bm)
    {
        int fixes = 0;
        var serialized = new SerializedObject(starter);

        var bmProp = serialized.FindProperty("battleManager");
        if (bmProp != null && bmProp.objectReferenceValue == null && bm != null)
        {
            bmProp.objectReferenceValue = bm;
            fixes++;
            Debug.Log("[BattleSceneSetup] BattleStarter.battleManager bağlandı.");
        }

        var handProp = serialized.FindProperty("handUI");
        if (handProp != null && handProp.objectReferenceValue == null)
        {
            HandUI hand = Object.FindObjectOfType<HandUI>();
            if (hand != null)
            {
                handProp.objectReferenceValue = hand;
                fixes++;
                Debug.Log("[BattleSceneSetup] BattleStarter.handUI bağlandı.");
            }
        }

        var aiProp = serialized.FindProperty("aiOpponent");
        if (aiProp != null && aiProp.objectReferenceValue == null)
        {
            AIOpponent ai = Object.FindObjectOfType<AIOpponent>();
            if (ai != null)
            {
                aiProp.objectReferenceValue = ai;
                fixes++;
                Debug.Log("[BattleSceneSetup] BattleStarter.aiOpponent bağlandı.");
            }
        }

        serialized.ApplyModifiedProperties();
        return fixes;
    }

    // ─── Yardımcılar ──────────────────────────────────────────────────────────

    private static Transform[] GetChildTransforms(Transform parent)
    {
        var result = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
            result[i] = parent.GetChild(i);
        return result;
    }

    /// <summary>
    /// Gerekli spawn noktalarını otomatik olarak sahnede oluşturur.
    /// Yoktan kurulum yaparken kullanın.
    /// </summary>
    [MenuItem("Card Battler/Create Spawn Points")]
    public static void CreateSpawnPoints()
    {
        CreateSpawnGroup("PlayerSpawnPoints", 3,
            new Vector3[] { new(-3, 0, 0), new(-1.5f, 0, 0), new(0, 0, 0) });

        CreateSpawnGroup("EnemySpawnPoints", 3,
            new Vector3[] { new(0, 0, 3), new(1.5f, 0, 3), new(3, 0, 3) });

        Scene active = SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(active);
        Debug.Log("[BattleSceneSetup] Spawn noktaları oluşturuldu. " +
                  "Pozisyonları sahne görünümünden düzenleyebilirsiniz.");
    }

    private static void CreateSpawnGroup(string groupName, int count, Vector3[] positions)
    {
        // Zaten varsa yeniden oluşturma
        if (GameObject.Find(groupName) != null)
        {
            Debug.Log($"[BattleSceneSetup] '{groupName}' zaten mevcut, atlandı.");
            return;
        }

        var root = new GameObject(groupName);
        for (int i = 0; i < count; i++)
        {
            var slot = new GameObject($"Slot{i}");
            slot.transform.SetParent(root.transform);
            slot.transform.position = i < positions.Length ? positions[i] : Vector3.zero;
        }
    }
}
#endif
