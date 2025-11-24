using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private float distanceMultiplier = 10f;
    [SerializeField] private int collectiblePoints = 50;

    private int currentScore;
    private int collectiblesCollected;

    public static ScoreManager Instance { get; private set; }

    public int CurrentScore => currentScore;
    public event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (GameSpeedManager.Instance != null)
        {
            GameSpeedManager.Instance.OnDistanceChanged += UpdateDistanceScore;
        }
    }

    void UpdateDistanceScore(float distance)
    {
        int newScore = Mathf.FloorToInt(distance * distanceMultiplier);
        if (newScore != currentScore)
        {
            currentScore = newScore + (collectiblesCollected * collectiblePoints);
            OnScoreChanged?.Invoke(currentScore);
        }
    }

    public void AddCollectible()
    {
        collectiblesCollected++;
        currentScore += collectiblePoints;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        collectiblesCollected = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    void OnDestroy()
    {
        if (GameSpeedManager.Instance != null)
        {
            GameSpeedManager.Instance.OnDistanceChanged -= UpdateDistanceScore;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}
