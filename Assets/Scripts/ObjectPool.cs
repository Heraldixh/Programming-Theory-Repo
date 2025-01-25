using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
        public int maxSize;
        public bool expandable;
    }

    public static ObjectPool Instance { get; private set; }

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Transform poolContainer;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, Pool> poolSettings;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolSettings = new Dictionary<string, Pool>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                CreateNewPoolObject(pool, objectPool);
            }

            poolDictionary.Add(pool.tag, objectPool);
            poolSettings.Add(pool.tag, pool);
        }
    }

    private GameObject CreateNewPoolObject(Pool pool, Queue<GameObject> objectPool)
    {
        GameObject obj = Instantiate(pool.prefab, poolContainer != null ? poolContainer : transform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
        return obj;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        Pool settings = poolSettings[tag];

        if (pool.Count == 0 && settings.expandable && pool.Count < settings.maxSize)
        {
            return CreateNewPoolObject(settings, pool);
        }

        GameObject objectToSpawn = pool.Dequeue();

        if (objectToSpawn == null)
        {
            objectToSpawn = CreateNewPoolObject(settings, pool);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        pool.Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            obj.SetActive(false);
            obj.transform.SetParent(poolContainer != null ? poolContainer : transform);
        }
    }

    public void ClearPool(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Queue<GameObject> pool = poolDictionary[tag];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }
    }
}