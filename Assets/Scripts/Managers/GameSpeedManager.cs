using UnityEngine;
using System;

public class GameSpeedManager : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.1f;
    [SerializeField] private float maxSpeed = 15f;

    [Header("Debug Options")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private float debugUpdateInterval = 1f;

    [SerializeField] private float currentSpeed;
    private float distanceTraveled;
    private bool isActive = true;
    private float debugTimer;

    public static GameSpeedManager Instance { get; private set; }

    public float CurrentSpeed => currentSpeed;
    public float DistanceTraveled => distanceTraveled;
    public float InitialSpeed => initialSpeed;
    public float MaxSpeed => maxSpeed;
    public bool IsActive => isActive;

    public event Action<float> OnSpeedChanged;
    public event Action<float> OnDistanceChanged;

    void Awake()
    {
        SetupSingleton();
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (!isActive) return;

        UpdateSpeed();
        UpdateDistance();
        UpdateDebugDisplay();
    }

    void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Duplicate GameSpeedManager detected. Destroying this instance.");
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        currentSpeed = initialSpeed;
        distanceTraveled = 0f;
        isActive = true;
    }

    void UpdateSpeed()
    {
        if (currentSpeed < maxSpeed)
        {
            float previousSpeed = currentSpeed;
            currentSpeed += speedIncreaseRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            if (Mathf.Abs(currentSpeed - previousSpeed) > 0.01f)
            {
                OnSpeedChanged?.Invoke(currentSpeed);
            }
        }
    }

    void UpdateDistance()
    {
        float previousDistance = distanceTraveled;
        distanceTraveled += currentSpeed * Time.deltaTime;

        if (Mathf.Abs(distanceTraveled - previousDistance) > 0.1f)
        {
            OnDistanceChanged?.Invoke(distanceTraveled);
        }
    }

    void UpdateDebugDisplay()
    {
        if (!showDebugInfo) return;

        debugTimer += Time.deltaTime;
        if (debugTimer >= debugUpdateInterval)
        {
            debugTimer = 0f;
            Debug.Log($"[GameSpeed] Speed: {currentSpeed:F2} m/s | Distance: {distanceTraveled:F1} m | Progress: {GetSpeedProgress():F1}%");
        }
    }

    public void Pause()
    {
        isActive = false;
    }

    public void Resume()
    {
        isActive = true;
    }

    public void ResetSpeed()
    {
        currentSpeed = initialSpeed;
        distanceTraveled = 0f;
        OnSpeedChanged?.Invoke(currentSpeed);
        OnDistanceChanged?.Invoke(distanceTraveled);
    }

    public float GetSpeedProgress()
    {
        return Mathf.InverseLerp(initialSpeed, maxSpeed, currentSpeed) * 100f;
    }

    public float GetSpeedMultiplier()
    {
        return currentSpeed / initialSpeed;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void OnValidate()
    {
        if (initialSpeed <= 0f)
        {
            initialSpeed = 1f;
            Debug.LogWarning("Initial speed must be greater than 0. Reset to 1.");
        }

        if (maxSpeed < initialSpeed)
        {
            maxSpeed = initialSpeed * 2f;
            Debug.LogWarning("Max speed must be greater than or equal to initial speed.");
        }

        if (speedIncreaseRate < 0f)
        {
            speedIncreaseRate = 0.1f;
            Debug.LogWarning("Speed increase rate cannot be negative.");
        }
    }
}
