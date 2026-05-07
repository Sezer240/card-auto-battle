using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
{
    if (GameManager.Instance == null)
    {
        Debug.LogError("[SceneLoader] GameManager.Instance bulunamadı! Init sahnesinden mi başlatıldı?");
        return;
    }
    GameManager.Instance.OnStateChanged += HandleStateChange;
}

    private void OnDestroy()
    {
        // Obje silinirse dinlemeyi bırak (Hata vermemesi için şart)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChange;
        }
    }

    private void HandleStateChange(GameState newState)
{
    switch (newState)
    {
        case GameState.MainMenu:
            StartCoroutine(LoadSceneAsyncRoutine("MainMenu"));
            break;
        case GameState.Preparation:
        case GameState.Battle:
            StartCoroutine(LoadSceneAsyncRoutine("Battle"));
            break;
        case GameState.PostBattle:
            // Sprint 3'te implement edilecek (skor ekranı, ödül UI)
            Debug.Log("[SceneLoader] PostBattle state'i henüz işlenmedi.");
            break;
        default:
            Debug.LogWarning($"[SceneLoader] Tanınmayan state: {newState}");
            break;
    }
}

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        Debug.Log($"[SceneLoader] {sceneName} arka planda yükleniyor...");
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        while (!operation.isDone)
        {
            yield return null;
        }

        Debug.Log($"[SceneLoader] {sceneName} başarıyla yüklendi!");
    }
}