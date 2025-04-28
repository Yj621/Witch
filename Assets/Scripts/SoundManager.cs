using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isSound1 = false;
    public bool isSound2 = false;
    public bool isSound3 = false;
    public static SoundManager Instance { get; private set; }

   
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
