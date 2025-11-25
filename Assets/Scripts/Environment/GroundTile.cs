using UnityEngine;

public class GroundTile : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform nextTileSpawnPos;

    [Header("Destruction Settings")]
    [SerializeField] private float destroyDelay = 1f;

    [Header("Pooling Settings")]
    [SerializeField] private bool usePooling = true;

    private string poolKey;

    private SpawnManager spawnManager;
    private bool hasTriggeredSpawn;
    private float currentSpeed;

    public Transform NextTileSpawnPos => nextTileSpawnPos;
    public bool HasTriggeredSpawn => hasTriggeredSpawn;

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        hasTriggeredSpawn = false;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        UpdateMovement();
    }

    void Initialize()
    {
        spawnManager = FindFirstObjectByType<SpawnManager>();
        hasTriggeredSpawn = false;
    }

    void UpdateMovement()
    {
        currentSpeed = GameSpeedManager.Instance != null
            ? GameSpeedManager.Instance.CurrentSpeed
            : 10f;

        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!hasTriggeredSpawn && spawnManager != null)
        {
            hasTriggeredSpawn = true;
            spawnManager.SpawnNextTile();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DestroyTile();
        }
    }

    void DestroyTile()
    {
        if (usePooling && PoolManager.Instance != null)
        {
            Invoke(nameof(ReturnToPool), destroyDelay);
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    void ReturnToPool()
    {
        string difficulty = DetermineDifficulty();
        poolKey = $"{difficulty}_{GetPrefabName()}";

        PoolManager.Instance.ReturnToPool(poolKey, gameObject);
    }

    string DetermineDifficulty()
    {
        if (name.Contains("Easy")) return "Easy";
        if (name.Contains("Medium")) return "Medium";
        if (name.Contains("Hard")) return "Hard";
        return "Easy";
    }

    string GetPrefabName()
    {
        string fullName = gameObject.name;
        int underscoreIndex = fullName.LastIndexOf('_');

        if (underscoreIndex >= 0)
        {
            return fullName.Substring(0, underscoreIndex);
        }

        return fullName;
    }

    public void ResetTile()
    {
        hasTriggeredSpawn = false;
    }

    void OnValidate()
    {
        if (nextTileSpawnPos == null)
        {
            Transform spawnPoint = transform.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                nextTileSpawnPos = spawnPoint;
            }
        }
    }
}
