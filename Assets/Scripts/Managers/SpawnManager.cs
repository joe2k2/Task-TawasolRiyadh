using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Difficulty Progression (Speed-Based)")]
    [SerializeField] private GameObject[] easyTiles;
    [SerializeField] private GameObject[] mediumTiles;
    [SerializeField] private GameObject[] hardTiles;
    [SerializeField] private float mediumSpeedThreshold = 8f;
    [SerializeField] private float hardSpeedThreshold = 12f;

    [Header("Spawn Settings")]
    [SerializeField] private int initialTileCount = 3;
    [SerializeField] private int poolSizePerTile = 5;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private Transform lastTileSpawnPos;
    private int totalTilesSpawned;

    public int TotalTilesSpawned => totalTilesSpawned;

    void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        InitializePools();
        SpawnInitialTiles();
    }

    void InitializePools()
    {
        if (PoolManager.Instance == null)
        {
            enabled = false;
            return;
        }

        CreatePoolsForTiles(easyTiles, "Easy");
        CreatePoolsForTiles(mediumTiles, "Medium");
        CreatePoolsForTiles(hardTiles, "Hard");

        totalTilesSpawned = 0;
    }

    void CreatePoolsForTiles(GameObject[] tiles, string difficulty)
    {
        if (tiles == null || tiles.Length == 0) return;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null) continue;

            string poolKey = GetPoolKey(tiles[i].name, difficulty);
            PoolManager.Instance.CreatePool(poolKey, tiles[i], poolSizePerTile);
        }
    }

    string GetPoolKey(string tileName, string difficulty)
    {
        return $"{difficulty}_{tileName}";
    }

    bool ValidateSetup()
    {
        if (easyTiles == null || easyTiles.Length == 0)
        {
            return false;
        }

        GameObject[] allTiles = GetAllTilePrefabs();
        foreach (GameObject prefab in allTiles)
        {
            if (prefab == null) continue;

            GroundTile tileComponent = prefab.GetComponent<GroundTile>();
            if (tileComponent == null)
            {
                return false;
            }

            if (tileComponent.NextTileSpawnPos == null)
            {
                Debug.LogError($"{gameObject.name}: Tile Prefab '{prefab.name}' NextTileSpawnPos is not assigned!");
                return false;
            }
        }

        return true;
    }

    GameObject[] GetAllTilePrefabs()
    {
        System.Collections.Generic.List<GameObject> allTiles = new System.Collections.Generic.List<GameObject>();

        if (easyTiles != null) allTiles.AddRange(easyTiles);
        if (mediumTiles != null) allTiles.AddRange(mediumTiles);
        if (hardTiles != null) allTiles.AddRange(hardTiles);

        return allTiles.ToArray();
    }

    void SpawnInitialTiles()
    {
        GameObject firstTile = SpawnTileAt(Vector3.zero);

        if (firstTile != null)
        {
            GroundTile groundTile = firstTile.GetComponent<GroundTile>();
            lastTileSpawnPos = groundTile.NextTileSpawnPos;

            for (int i = 1; i < initialTileCount; i++)
            {
                SpawnNextTile();
            }
        }
    }

    public void SpawnNextTile()
    {
        if (lastTileSpawnPos == null)
        {
            return;
        }

        GameObject newTile = SpawnTileAt(lastTileSpawnPos.position);

        if (newTile != null)
        {
            GroundTile groundTile = newTile.GetComponent<GroundTile>();
            if (groundTile != null)
            {
                lastTileSpawnPos = groundTile.NextTileSpawnPos;
            }
        }
    }

    GameObject SpawnTileAt(Vector3 position)
    {
        GameObject selectedPrefab = GetRandomTilePrefab();
        string difficulty = GetCurrentDifficulty();
        string poolKey = GetPoolKey(selectedPrefab.name, difficulty);

        GameObject tile = PoolManager.Instance.SpawnFromPool(poolKey, position, Quaternion.identity);

        if (tile == null)
        {
            return null;
        }

        totalTilesSpawned++;

        if (showDebugInfo)
        {
            Debug.Log($"[SpawnManager] Spawned {tile.name} from pool '{poolKey}' | Difficulty: {difficulty} | Speed: {GetCurrentSpeed():F2} m/s");
        }

        return tile;
    }

    GameObject GetRandomTilePrefab()
    {
        float currentSpeed = GetCurrentSpeed();

        if (currentSpeed < mediumSpeedThreshold)
        {
            if (easyTiles.Length == 0) return GetFallbackTile();
            return easyTiles[Random.Range(0, easyTiles.Length)];
        }
        else if (currentSpeed < hardSpeedThreshold)
        {
            if (mediumTiles.Length == 0) return easyTiles[Random.Range(0, easyTiles.Length)];
            return mediumTiles[Random.Range(0, mediumTiles.Length)];
        }
        else
        {
            if (hardTiles.Length == 0)
            {
                if (mediumTiles.Length == 0) return easyTiles[Random.Range(0, easyTiles.Length)];
                return mediumTiles[Random.Range(0, mediumTiles.Length)];
            }
            return hardTiles[Random.Range(0, hardTiles.Length)];
        }
    }

    float GetCurrentSpeed()
    {
        if (GameSpeedManager.Instance != null)
        {
            return GameSpeedManager.Instance.CurrentSpeed;
        }
        return 5f;
    }

    GameObject GetFallbackTile()
    {
        if (easyTiles.Length > 0) return easyTiles[0];
        if (mediumTiles.Length > 0) return mediumTiles[0];
        if (hardTiles.Length > 0) return hardTiles[0];
        return null;
    }

    public string GetCurrentDifficulty()
    {
        float currentSpeed = GetCurrentSpeed();

        if (currentSpeed < mediumSpeedThreshold) return "Easy";
        if (currentSpeed < hardSpeedThreshold) return "Medium";
        return "Hard";
    }

    void OnValidate()
    {
        if (initialTileCount < 1)
        {
            initialTileCount = 1;
        }

        if (hardSpeedThreshold <= mediumSpeedThreshold)
        {
            hardSpeedThreshold = mediumSpeedThreshold + 1f;
        }
    }
}
