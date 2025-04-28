using UnityEngine;

public class SoundManager : MonoBehaviour
{
    
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
