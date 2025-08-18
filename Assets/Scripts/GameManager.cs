using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player;
    public SkillObjectPool skillObjectPool;
    public AutoSkillPool autoSkillPool;
    public float currentClean;
    public float maxClean = 500;
    private bool isClearCalled = false;
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

    public void Start()
    {
        UpgradeManager.Instance.SetLevel(UpgradeType.IPSkillDamage, PlayerPrefs.GetInt("IceUp", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.IFSkillDamage, PlayerPrefs.GetInt("InfUp", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.BHSkillDamage, PlayerPrefs.GetInt("BlackUp", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.FSSkillDamage, PlayerPrefs.GetInt("FSDmg", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.TDSkillDamage, PlayerPrefs.GetInt("TDDmg", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.FSSkillCooldown, PlayerPrefs.GetInt("FSCool", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.TDSkillCooldown, PlayerPrefs.GetInt("TDCool", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.DashSpeed, PlayerPrefs.GetInt("DashSpeed", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.DashCooldown, PlayerPrefs.GetInt("DashCool", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.ExpIncrease, PlayerPrefs.GetInt("ExpUp", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.MaxHPIncrease, PlayerPrefs.GetInt("HpUp", 1));
        UpgradeManager.Instance.SetLevel(UpgradeType.CharacterMoveSpeed, PlayerPrefs.GetInt("MoveSpeedUp", 1));
    }


    private void Update()
    {
        if(currentClean >= maxClean)
        {
            if (isClearCalled) return;

            isClearCalled = true;
            SceneManager.LoadScene("EndingScene");
        }
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
        Destroy(PlayerInput.Instance.gameObject);
        Destroy(GameManager.Instance.gameObject);
        Destroy(SkillManager.Instance.gameObject);
        Destroy(UpgradeManager.Instance.gameObject);
        Destroy(DamageManager.Instance.gameObject);
        Destroy(PlayerSkill.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapScene");
        //SkillManager.Instance.ReStartInit();
    }

    public void GoTitle()
    {
        Destroy(PlayerInput.Instance.gameObject);
        Destroy(GameManager.Instance.gameObject);
        Destroy(SkillManager.Instance.gameObject);
        Destroy(UpgradeManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        Destroy(DamageManager.Instance.gameObject);
        Destroy(PlayerSkill.Instance.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadScene("OpeningScene");
    }

    public void Clear()
    {
        
        SceneManager.LoadScene("EndingScene");
    }
}
