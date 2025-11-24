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

    [Header("Organization")]
    [SerializeField] private bool organizeTilesInHierarchy = true;
    [SerializeField] private string tilesContainerName = "Tiles";

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private Transform lastTileSpawnPos;
    private Transform tilesContainer;
    private int totalTilesSpawned;

    public int TotalTilesSpawned => totalTilesSpawned;

    void Start()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        InitializeSpawner();
        SpawnInitialTiles();
    }

    bool ValidateSetup()
    {
        if (easyTiles == null || easyTiles.Length == 0)
        {
            Debug.LogError($"{gameObject.name}: No easy tile prefabs assigned!");
            return false;
        }

        if (mediumTiles == null || mediumTiles.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No medium tile prefabs assigned! Will use easy tiles.");
        }

        if (hardTiles == null || hardTiles.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No hard tile prefabs assigned! Will use medium tiles.");
        }

        GameObject[] allTiles = GetAllTilePrefabs();
        foreach (GameObject prefab in allTiles)
        {
            if (prefab == null) continue;

            GroundTile tileComponent = prefab.GetComponent<GroundTile>();
            if (tileComponent == null)
            {
                Debug.LogError($"{gameObject.name}: Tile Prefab '{prefab.name}' must have a GroundTile component!");
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

    void InitializeSpawner()
    {
        if (organizeTilesInHierarchy)
        {
            GameObject container = new GameObject(tilesContainerName);
            tilesContainer = container.transform;
            tilesContainer.SetParent(transform);
            tilesContainer.localPosition = Vector3.zero;
        }

        totalTilesSpawned = 0;
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
        else
        {
            Debug.LogError($"{gameObject.name}: Failed to spawn first tile!");
        }
    }

    public void SpawnNextTile()
    {
        if (lastTileSpawnPos == null)
        {
            Debug.LogWarning($"{gameObject.name}: Cannot spawn tile - lastTileSpawnPos is null!");
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

        GameObject tile = Instantiate(selectedPrefab, position, Quaternion.identity, tilesContainer);
        tile.name = $"{selectedPrefab.name}_{totalTilesSpawned:D3}";
        totalTilesSpawned++;

        if (showDebugInfo)
        {
            Debug.Log($"[SpawnManager] Spawned {selectedPrefab.name} | Difficulty: {GetCurrentDifficulty()} | Speed: {GetCurrentSpeed():F2} m/s");
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

    public void ClearAllTiles()
    {
        if (tilesContainer != null)
        {
            foreach (Transform child in tilesContainer)
            {
                Destroy(child.gameObject);
            }
        }

        totalTilesSpawned = 0;
        lastTileSpawnPos = null;
    }

    public void ResetSpawner()
    {
        ClearAllTiles();
        SpawnInitialTiles();
    }

    void OnValidate()
    {
        if (initialTileCount < 1)
        {
            initialTileCount = 1;
            Debug.LogWarning("Initial tile count must be at least 1.");
        }

        if (hardSpeedThreshold <= mediumSpeedThreshold)
        {
            hardSpeedThreshold = mediumSpeedThreshold + 1f;
            Debug.LogWarning("Hard speed threshold must be greater than medium speed threshold.");
        }
    }
}
