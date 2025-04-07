using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player;
    public SkillObjectPool skillObjectPool;
    public float currentClean;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        player = new Player(
            exp: 0,
            level: 1,
            playerSpeed: 5f,
            dashSpeed: 8f,
            maxHp : 100
        );
    }
}
