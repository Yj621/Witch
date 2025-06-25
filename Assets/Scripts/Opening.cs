using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Opening : MonoBehaviour
{
    [System.Serializable]
    public class SoundOption
    {
        public Image[] volumeSteps; // 5개 이미지nt
        public int currentVolumeStep = 2; // 초기 2단계
    }

    public bool isSound = false;
    public Image[] SoundButtons;
    private int selectedSoundIndex = 0;
    public SoundOption[] SoundOptions;

    public GameObject OptionPanel;
    public GameObject HowToPanel;
    public GameObject ShopPanel;
    public Image[] buttonpickimages;

    private int selectedIndex = 0;
    private bool isOptionPanelOpen = false;

    public GameObject[] Lights;
    public GameObject ResultPanel;
    public TextMeshProUGUI ResultText;
    public TextMeshProUGUI CoinText;

    void Start()
    {
        // 메뉴 화살표 초기화
        for (int i = 0; i < buttonpickimages.Length; i++)
            buttonpickimages[i].enabled = (i == 0);

        // 옵션 버튼(볼륨 조절용) 숨기기
        for (int i = 0; i < SoundButtons.Length; i++)
            SoundButtons[i].enabled = false;

        // 사운드 매니저 초기값 설정 → UI와 매니저 동기화 순서 보장
        SoundManager.Instance.MasterSoundLevel = 2;
        SoundManager.Instance.BgmLevel = 2;
        SoundManager.Instance.SfxLevel = 2;

        // UI의 currentVolumeStep 을 매니저 값으로 초기화
        SoundOptions[0].currentVolumeStep = SoundManager.Instance.MasterSoundLevel;
        SoundOptions[1].currentVolumeStep = SoundManager.Instance.BgmLevel;
        SoundOptions[2].currentVolumeStep = SoundManager.Instance.SfxLevel;

        // UI 그리기
        foreach (var option in SoundOptions)
            UpdateVolUI(option);

        // 오프닝 BGM 재생
        SoundManager.Instance.PlayBGM("OpeningSceneBGM");

        UpdateUpgradeText();
        for(int i = 0; i < Lights.Length; i++)
        {
            Lights[i].SetActive(false);
        }
    }

    void Update()
    {
        if (!isOptionPanelOpen)
        {
            HandleMenuInput();
            HandleMenuSelection();
        }
        else
        {
            HandleSoundInput();
            SoundButtonControll();
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseOptionPanel();
        }
    }
    void ChangeSelection(int direction)
    {
        buttonpickimages[selectedIndex].enabled = false;

        selectedIndex += direction;
        if (selectedIndex < 0) selectedIndex = buttonpickimages.Length - 1;
        if (selectedIndex >= buttonpickimages.Length) selectedIndex = 0;
        buttonpickimages[selectedIndex].enabled = true;
    }

    void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
            ChangeSelection(1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
            ChangeSelection(-1);
        }
    }

    void HandleSoundInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (selectedSoundIndex < 0)
            {
                selectedSoundIndex = 0;
                SoundButtons[selectedSoundIndex].enabled = true;
            }
            else if (selectedSoundIndex < SoundButtons.Length - 1)
            {
                SoundButtons[selectedSoundIndex].enabled = false;
                selectedSoundIndex++;
                SoundButtons[selectedSoundIndex].enabled = true;
            }
            Debug.Log("Sound = " + selectedSoundIndex);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (selectedSoundIndex > 0)
            {
                SoundButtons[selectedSoundIndex].enabled = false;
                selectedSoundIndex--;
                SoundButtons[selectedSoundIndex].enabled = true;
            }
            Debug.Log("Sound = " + selectedSoundIndex);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }

    }

    void ChangeSoundSelection(int direction)
    {
        SoundButtons[selectedSoundIndex].enabled = false;

        selectedSoundIndex += direction;
        selectedSoundIndex = Mathf.Clamp(selectedSoundIndex, 0, SoundButtons.Length - 1);

        SoundButtons[selectedSoundIndex].enabled = true;
    }

    void SoundButtonControll()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            IncreaseVol(selectedSoundIndex);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            DecreaseVol(selectedSoundIndex);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
    }
    void UpdateVolUI(SoundOption option)
    {
        for (int i = 0; i < option.volumeSteps.Length; i++)
            option.volumeSteps[i].enabled = (i <= option.currentVolumeStep);
    }

    void IncreaseVol(int index)
    {
        var opt = SoundOptions[index];
        if (opt.currentVolumeStep < opt.volumeSteps.Length - 1)
        {
            opt.currentVolumeStep++;
            UpdateVolUI(opt);
            SoundManager.Instance.ChangeVol(index, +1);
        }
    }

    void DecreaseVol(int index)
    {
        var opt = SoundOptions[index];
        if (opt.currentVolumeStep > 0)
        {
            opt.currentVolumeStep--;
            UpdateVolUI(opt);
            SoundManager.Instance.ChangeVol(index, -1);
        }
    }

    void HandleMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySFX("ButtonClickSfx");
            switch (selectedIndex)
            {
                case 0:
                    StartGame();
                    break;
                case 1:
                    OpenOptionPanel();
                    break;
                case 2:
                    OpenHowToPanel();
                    break;
                case 3:
                    GameEnd();
                    break;
                case 4:
                    OpenShopPanel();
                    break;
            }
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("StoryScene");
    }

    void OpenOptionPanel()
    {
        OptionPanel.SetActive(true);
        isOptionPanelOpen = true;
        selectedSoundIndex = 0; // 첫 번째 버튼 선택
        for (int i = 0; i < SoundButtons.Length; i++)
            SoundButtons[i].enabled = (i == selectedSoundIndex);
    }

    void OpenHowToPanel()
    {
        isOptionPanelOpen = true;
        HowToPanel.SetActive(true);
    }

    void OpenShopPanel()
    {
        isOptionPanelOpen = true;
        ShopPanel.SetActive(true);
    }

    void CloseOptionPanel()
    {
        SoundManager.Instance.PlaySFX("ButtonClickSfx");
        OptionPanel.SetActive(false);
        HowToPanel.SetActive(false);
        ShopPanel.SetActive(false);
        isOptionPanelOpen = false;
    }

    void GameEnd()
    {
        Application.Quit();
    }


    public void Revive()
    {
        if (PlayerPrefs.GetInt("Revive") == 1)
        {
            Debug.Log("Already Revived");
            return;
        }
        else
        {
            Coin.coin -= 5;
            PlayerPrefs.SetInt("Revive", 1);
        }
    }

    public void ActiveSkillUpgrade(string Case)
    {
        switch (Case)
        {
            case "IceLrn":
                {
                    if (PlayerPrefs.GetInt("IceLrn") == 1)
                    {
                        StartCoroutine(ResultPanelPopup("이미 배웠습니다", false));
                    }
                    else if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        PlayerPrefs.SetInt("IceLrn", 1);
                        StartCoroutine(ResultPanelPopup("아이스필러 습득 완료!", true));
                        Lights[0].SetActive(true);
                    }
                    break;
                }
            case "InfLrn":
                {
                    if (PlayerPrefs.GetInt("InfLrn") == 1)
                    {
                        StartCoroutine(ResultPanelPopup("이미 배웠습니다", false));
                    }
                    else if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        PlayerPrefs.SetInt("InfLrn", 1);
                        StartCoroutine(ResultPanelPopup("인페르노 습득 완료!", true));
                        Lights[1].SetActive(true);
                    }
                    break;
                }
            case "BlackLrn":
                {
                    if (PlayerPrefs.GetInt("BlackLrn") == 1)
                    {
                        StartCoroutine(ResultPanelPopup("이미 배웠습니다", false));
                    }
                    else if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        PlayerPrefs.SetInt("BlackLrn", 1);
                        StartCoroutine(ResultPanelPopup("블랙홀 습득 완료!", true));
                        Lights[2].SetActive(true);
                    }
                    break;
                }

            case "IceUp":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("IceUp") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("아이스필러는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("IceUp", 0);
                        PlayerPrefs.SetInt("IceUp", current + 1);

                        StartCoroutine(ResultPanelPopup("아이스필러 레벨업!", true));
                    }
                    break;
                }
            case "InfUp":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("InfUp") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("인페르노는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("InfUp", 0);
                        PlayerPrefs.SetInt("InfUp", current + 1);

                        StartCoroutine(ResultPanelPopup("인페르노 레벨업!", true));
                    }
                    break;
                }
            case "BlackUp":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("BlackUp") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("블랙홀은 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("BlackUp", 0);
                        PlayerPrefs.SetInt("BlackUp", current + 1);

                        StartCoroutine(ResultPanelPopup("블랙홀 레벨업!", true));
                    }
                    break;
                }
        }
    }

    public void DefaultSkillUpgrade(string Case)
    {
        switch (Case)
        {
            case "FSCool":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("FSCool") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("슬래시 쿨타임은 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("FSCool", 0);
                        PlayerPrefs.SetInt("FSCool", current + 1);
                        StartCoroutine(ResultPanelPopup("파이어슬래시 쿨타임 감소!", true));
                    }
                    break;
                }
            case "FSDmg":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("FSDmg") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("슬래시 데미지는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("FSDmg", 0);
                        PlayerPrefs.SetInt("FSDmg", current + 1);
                        StartCoroutine(ResultPanelPopup("파이어슬래시 데미지 증가!", true));
                    }
                    break;
                }

            case "TDCool":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("TDCool") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("번개 쿨타임은 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("TDCool", 0);
                        PlayerPrefs.SetInt("TDCool", current + 1);
                        StartCoroutine(ResultPanelPopup("번개 쿨타임 감소!", true));
                    }
                    break;
                }
            case "TDDmg":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("TDDmg") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("번개 데미지는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("TDDmg", 0);
                        PlayerPrefs.SetInt("TDDmg", current + 1);
                        StartCoroutine(ResultPanelPopup("번개 데미지 증가!", true));
                    }
                    break;
                }
        }
    }

    public void PassiveSkillUpgrade(string Case)
    {
        switch (Case)
        {
            case "DashCool":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("DashCool") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("대쉬 쿨타임은 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("DashCool", 0);
                        PlayerPrefs.SetInt("DashCool", current + 1);
                        StartCoroutine(ResultPanelPopup("대쉬 쿨타임 레벨 증가!", true));
                    }
                    break;
                }
            case "DashSpeed":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("DashSpeed") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("대쉬 속도는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("DashSpeed", 0);
                        PlayerPrefs.SetInt("DashSpeed", current + 1);
                        StartCoroutine(ResultPanelPopup("대쉬 속도 레벨 증가!", true));
                    }
                    break;
                }
            case "ExpUp":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("ExpUp") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("경험치 증가는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("ExpUp", 0);
                        PlayerPrefs.SetInt("ExpUp", current + 1);
                        StartCoroutine(ResultPanelPopup("경험치 증가 레벨 증가!", true));
                    }
                    break;
                }
            case "HpUp":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("HpUp") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("체력 증가는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("HpUp", 0);
                        PlayerPrefs.SetInt("HpUp", current + 1);
                        StartCoroutine(ResultPanelPopup("최대 체력 증가!", true));
                    }
                    break;
                }
            case "MoveSpeedUp":
                {
                    if (Coin.coin < 3)
                    {
                        StartCoroutine(ResultPanelPopup("코인이 부족합니다", false));
                    }
                    else if (PlayerPrefs.GetInt("MoveSpeedUp") >= 5)
                    {
                        StartCoroutine(ResultPanelPopup("이동속도는 최대 레벨입니다", false));
                    }
                    else
                    {
                        Coin.coin -= 3;
                        CoinText.text = Coin.coin.ToString();
                        int current = PlayerPrefs.GetInt("MoveSpeedUp", 0);
                        PlayerPrefs.SetInt("MoveSpeedUp", current + 1);
                        StartCoroutine(ResultPanelPopup("이동속도 레벨 증가!", true));
                    }
                    break;
                }
        }
    }

    private IEnumerator ResultPanelPopup(string message, bool status)
    {
        ResultPanel.SetActive(true);
        ResultText.text = message;
        ResultText.color = status ? Color.green : Color.red;
        yield return new WaitForSeconds(2f);
        ResultPanel.SetActive(false);
    }

    private void UpdateUpgradeText()
    {
        CoinText.text = Coin.coin.ToString();
    }
}
