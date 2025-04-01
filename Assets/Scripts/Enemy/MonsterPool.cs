using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    public enum MonsterType
    {
        Pumpkin,
        Ghost,
        Spider,
        Skull
    }

    [System.Serializable]
    public class PoolEntry
    {
        public MonsterType type;
        public GameObject prefab;
        public int initialSize;
    }

    public PoolEntry[] entries;

    private Dictionary<MonsterType, Queue<GameObject>> pools = new();

    public static MonsterPool Instance;

    void Awake()
    {
        Instance = this;

        foreach (var entry in entries)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < entry.initialSize; i++)
            {
                GameObject obj = Instantiate(entry.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            pools[entry.type] = queue;
        }
    }

    public GameObject Get(MonsterType type)
    {
        if (!pools.ContainsKey(type))
        {
            Debug.LogWarning($"풀 없음: {type}");
            return null;
        }

        var pool = pools[type];
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            var prefab = entries.First(e => e.type == type).prefab;
            return Instantiate(prefab);
        }
    }

    public void Return(MonsterType type, GameObject obj)
    {
        obj.SetActive(false);
        pools[type].Enqueue(obj);
    }
}
