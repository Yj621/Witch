using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player;
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        player = new Player(1, 100, 0);

    }
    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
