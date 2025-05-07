using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player;
    public SkillObjectPool skillObjectPool;
    public AutoSkillPool autoSkillPool;
    public float currentClean;
    // 기존 데이터 클래스 Player 외에
    public Transform PlayerTransform { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 씬에 로드된 Player 오브젝트 찾아서 Transform 저장
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            PlayerTransform = playerGO.transform;
        else
            Debug.LogError("GameManager: Player 태그가 붙은 오브젝트를 찾을 수 없음");

        player = new Player(
            exp: 0,
            level: 1,
            playerSpeed: 5f,
            dashSpeed: 8f,
            maxHp : 100
        );
    }

    // 씬 전환 후 PlayerTransform이 리셋될 경우를 대비해
    private void OnEnable()
        => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable()
        => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MapScene")
        {
            var playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
                PlayerTransform = playerGO.transform;
        }
    }

    public void Exit()
    {
        Application.Quit(); 
    }

    public void ReStartGame()
    {
        SceneManager.LoadScene("MapScene");
        Time.timeScale = 1f;
    }

    public void GoTitle()
    {
        SceneManager.LoadScene("OpeningScene");
    }
}
