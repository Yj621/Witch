using System.Collections.Generic;
using UnityEngine;

public class SkillObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private int initalObjectNumber = 20;

    List<GameObject> objs;
    void Start()
    {
        objs = new List<GameObject>();
        for(int i = 0; i<initalObjectNumber;  i++)
        {
            GameObject go = Instantiate(firePrefab, transform);
            go.SetActive(false);
            objs.Add(go);
        }
    }

    public GameObject GetObject()
    {
        foreach(GameObject go in objs)
        {
            if(!go.activeSelf)
            {
                go.SetActive(true);
                return go;
            }
        }
        GameObject obj = Instantiate(firePrefab, transform);
        objs.Add(obj);
        return obj;
    }
}
