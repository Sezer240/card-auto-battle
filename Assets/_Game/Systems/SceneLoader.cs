using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        // GameManager'daki state değişimlerini dinlemeye başla
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
                // Savaş veya hazırlık durumunda Battle sahnesini yükle
                StartCoroutine(LoadSceneAsyncRoutine("Battle"));
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