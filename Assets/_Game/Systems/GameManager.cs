using System;
using UnityEngine;

// 1. Oyun Durumları (GameState enum)
public enum GameState 
{
    MainMenu,
    Preparation,
    Battle,
    PostBattle
}

public class GameManager : MonoBehaviour 
{
    // 2. Singleton Instance Garantisi
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    // 3. OnStateChanged event (C# Action kullanıldı)
    public event Action<GameState> OnStateChanged;

    private void Awake() 
    {
        // KRİTİK: Zaten bir instance varsa, sahne geçişinde çifte binmemesi için yenisini hemen öldür.
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }

        // İlk defa oluşuyorsa Instance olarak ata ve yok edilmesini engelle
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Oyun durumunu değiştirmek için kullanılacak tek fonksiyon
    public void ChangeState(GameState newState) 
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameManager] Durum değişti: {newState}");
    }
}