using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player;
    public SkillObjectPool skillObjectPool;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        player = new Player(
            level: 1,
            exp: 0,
            playerSpeed: 5f,
            dashSpeed: 8f,
            maxHp : 100
        );

    }
}
