using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    // list of pools you can define in the inspector
    [SerializeField] List<Pool> pools;

    // dictionary to store the pool queues for each object type
    Dictionary<string, Queue<GameObject>> poolDictionary;

    #region Singleton

    void Awake()
    {
        #region singleton
        // Make sure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // destroy duplicate
            return;
        }

        instance = this;
        #endregion
    }
    #endregion

    void Start()
    {
        // Initialize the dictionary and populate each pool
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectQueue);
        }
    }

    // Method to get an object from a specific pool
    public GameObject GetObject(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Queue<GameObject> objectQueue = poolDictionary[tag];

            if (objectQueue.Count > 0)
            {
                GameObject obj = objectQueue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }

    // Method to return an object to the pool
    public void ReturnObject(string tag, GameObject obj)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            obj.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }
    }
}