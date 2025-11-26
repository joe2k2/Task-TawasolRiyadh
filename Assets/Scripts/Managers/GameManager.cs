using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera playerCamera;

    public int maxLives = 3;
    public int currentLives;
    public int score;

    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;

    public static Action OnPlayerHitObstacle;
    public static Action<int> OnScoreChanged;
    public static Action<int> OnLivesChanged;
    public static Action OnGameOver;

    void Awake()
    {
        Instance = this;
        #if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        #endif

    }

    void Start()
    {
        currentLives = maxLives;
        score = 0;
        isGameOver = false;

        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateLives(currentLives);
    }

    public void PlayerHitObstacle()
    {
        if (isGameOver) return;

        ReduceLife(1);
        OnPlayerHitObstacle?.Invoke();
    }

    public void AddScore(int value)
    {
        if (isGameOver) return;

        score += value;

        UIManager.Instance.UpdateScore(score);

        OnScoreChanged?.Invoke(score);
    }

    void ReduceLife(int amount)
    {
        currentLives -= amount;

        UIManager.Instance.UpdateLives(currentLives);

        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            currentLives = 0;
            GameOver();
        }
    }

    void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        GameSpeedManager.Instance.Pause();
        UIManager.Instance.ShowGameOver(score);

        OnGameOver?.Invoke();
    }
}
