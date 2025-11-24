using UnityEngine;

public class GroundTile : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform nextTileSpawnPos;

    [Header("Destruction Settings")]
    [SerializeField] private float destroyDelay = 1f;

    private SpawnManager spawnManager;
    private bool hasTriggeredSpawn;
    private float currentSpeed;

    public Transform NextTileSpawnPos => nextTileSpawnPos;
    public bool HasTriggeredSpawn => hasTriggeredSpawn;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdateMovement();
    }

    void Initialize()
    {
        spawnManager = FindFirstObjectByType<SpawnManager>();

        if (spawnManager == null)
        {
            Debug.LogError($"{gameObject.name}: SpawnManager not found in scene!");
        }

        if (nextTileSpawnPos == null)
        {
            Debug.LogWarning($"{gameObject.name}: nextTileSpawnPos is not assigned!");
        }

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
        Destroy(gameObject, destroyDelay);
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
