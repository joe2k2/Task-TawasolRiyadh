using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Transform poolContainer;

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

        poolContainer = new GameObject("Pool Container").transform;
        poolContainer.SetParent(transform);
    }

    public void CreatePool(string poolKey, GameObject prefab, int initialSize)
    {
        if (poolDictionary.ContainsKey(poolKey))
        {
            return;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();
        prefabDictionary[poolKey] = prefab;

        Transform poolParent = new GameObject($"Pool_{poolKey}").transform;
        poolParent.SetParent(poolContainer);

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, poolParent);
            obj.name = $"{prefab.name}_{i:D3}";
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        poolDictionary[poolKey] = objectPool;
    }

    public GameObject SpawnFromPool(string poolKey, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            return null;
        }

        GameObject objectToSpawn;

        if (poolDictionary[poolKey].Count > 0)
        {
            objectToSpawn = poolDictionary[poolKey].Dequeue();
        }
        else
        {
            GameObject prefab = prefabDictionary[poolKey];
            objectToSpawn = Instantiate(prefab);
            objectToSpawn.name = $"{prefab.name}_Dynamic";
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public void ReturnToPool(string poolKey, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);

        Transform poolParent = poolContainer.Find($"Pool_{poolKey}");
        if (poolParent != null)
        {
            obj.transform.SetParent(poolParent);
        }

        poolDictionary[poolKey].Enqueue(obj);
    }

    public void ClearPool(string poolKey)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
            return;
        }

        Queue<GameObject> pool = poolDictionary[poolKey];
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            Destroy(obj);
        }

        poolDictionary.Remove(poolKey);
        prefabDictionary.Remove(poolKey);
    }

    public void ClearAllPools()
    {
        foreach (var pool in poolDictionary.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }

        poolDictionary.Clear();
        prefabDictionary.Clear();
    }
}
