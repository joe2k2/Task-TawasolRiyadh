using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private bool spawnObstacles = true;
    [SerializeField][Range(0f, 1f)] private float obstacleSpawnChance = 0.6f;
    [SerializeField] private int minObstaclesPerTile = 1;
    [SerializeField] private int maxObstaclesPerTile = 2;

    [Header("Collectible Settings")]
    [SerializeField] private GameObject[] collectiblePrefabs;
    [SerializeField] private bool spawnCollectibles = true;
    [SerializeField][Range(0f, 1f)] private float collectibleSpawnChance = 0.7f;
    [SerializeField] private int minCollectiblesPerTile = 5;
    [SerializeField] private int maxCollectiblesPerTile = 10;

    [Header("Spawn Heights")]
    [SerializeField] private float obstacleHeight = 0.5f;
    [SerializeField] private float collectibleHeight = 1.5f;

    [Header("Jump Physics")]
    [SerializeField] private float playerJumpForce = 7f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float safetyMargin = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private float fixedMinSpacing;

    public static ItemSpawner Instance { get; private set; }

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
        CalculateFixedSpacing();
    }

    void CalculateFixedSpacing()
    {
        float jumpDuration = (2f * playerJumpForce) / gravity;

        float maxSpeed = GameSpeedManager.Instance != null
            ? GameSpeedManager.Instance.MaxSpeed
            : 15f;

        float distanceDuringJump = maxSpeed * jumpDuration;
        fixedMinSpacing = distanceDuringJump * safetyMargin;

        if (showDebugInfo)
        {
            Debug.Log($"[ItemSpawner] Fixed Min Spacing (based on max speed {maxSpeed} m/s): {fixedMinSpacing:F2}m");
            Debug.Log($"[ItemSpawner] Jump Duration: {jumpDuration:F2}s");
        }
    }

    public void SpawnItemsOnTile(Transform tile)
    {
        if (spawnObstacles && Random.value < obstacleSpawnChance)
        {
            SpawnObstacles(tile);
        }

        if (spawnCollectibles && Random.value < collectibleSpawnChance)
        {
            SpawnCollectibles(tile);
        }
    }

    void SpawnObstacles(Transform tile)
    {
        if (obstaclePrefabs.Length == 0) return;

        int count = Random.Range(minObstaclesPerTile, maxObstaclesPerTile + 1);

        float tileLength = 100f;
        float availableSpace = tileLength - (fixedMinSpacing * 2);

        if (count > 1)
        {
            float maxPossible = availableSpace / fixedMinSpacing;
            count = Mathf.Min(count, Mathf.FloorToInt(maxPossible));
        }

        if (count < 1) count = 1;

        if (showDebugInfo)
        {
            Debug.Log($"[ItemSpawner] Spawning {count} obstacles with spacing {fixedMinSpacing:F2}m");
        }

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Vector3 position = GetSafeObstaclePosition(tile, i, count);
            GameObject obstacle = Instantiate(prefab, position, Quaternion.identity, tile);
            obstacle.name = $"Obstacle_{i}";
        }
    }

    Vector3 GetSafeObstaclePosition(Transform tile, int index, int totalCount)
    {
        float tileLength = 100f;
        float startOffset = fixedMinSpacing;
        float endOffset = fixedMinSpacing;
        float usableLength = tileLength - startOffset - endOffset;

        float z;
        if (totalCount == 1)
        {
            z = tileLength / 2f;
        }
        else
        {
            float spacing = usableLength / (totalCount - 1);
            spacing = Mathf.Max(spacing, fixedMinSpacing);
            z = startOffset + (spacing * index);
        }

        Vector3 localPos = new Vector3(0f, obstacleHeight, z);
        return tile.TransformPoint(localPos);
    }

    void SpawnCollectibles(Transform tile)
    {
        if (collectiblePrefabs.Length == 0) return;

        int count = Random.Range(minCollectiblesPerTile, maxCollectiblesPerTile + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];
            Vector3 position = GetCollectiblePosition(tile, count, i);
            GameObject collectible = Instantiate(prefab, position, Quaternion.identity, tile);
            collectible.name = $"Collectible_{i}";
        }
    }

    Vector3 GetCollectiblePosition(Transform tile, int totalCount, int index)
    {
        float tileLength = 100f;
        float spacing = tileLength / (totalCount + 1);
        float z = spacing * (index + 1);

        Vector3 localPos = new Vector3(0f, collectibleHeight, z);
        return tile.TransformPoint(localPos);
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            CalculateFixedSpacing();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
