using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Image[] buttonpickimages;

    private int selectedIndex = 0;
    private bool isOptionPanelOpen = false;

    void Start()
    {
        for (int i = 0; i < buttonpickimages.Length; i++)
        {
            buttonpickimages[i].enabled = (i == 0);
        }

        for (int i = 0; i < SoundButtons.Length; i++)
            SoundButtons[i].enabled = false;

        SoundOptions[0].currentVolumeStep = SoundManager.Instance.MasterSoundLevel;
        SoundOptions[1].currentVolumeStep = SoundManager.Instance.BgmLevel;
        SoundOptions[2].currentVolumeStep = SoundManager.Instance.SfxLevel;

        foreach (var option in SoundOptions)
        {
            UpdateVolUI(option);
        }


        SoundManager.Instance.MasterSoundLevel = 2;
        SoundManager.Instance.BgmLevel = 2;
        SoundManager.Instance.SfxLevel = 2;
        
        SoundManager.Instance.PlayBGM("OpeningSceneBGM");
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
            {
                CloseOptionPanel();
            }
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
                Debug.Log("A");
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                DecreaseVol(selectedSoundIndex);
                Debug.Log("A");
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
    }

    void IncreaseVol(int index)
    {
        var option = SoundOptions[index];
        if (option.currentVolumeStep < option.volumeSteps.Length - 1)
        {
            option.currentVolumeStep++;
            UpdateVolUI(option);
            SoundManager.Instance.ChangeVol(index, +1);
            Debug.Log(index + "Sound Lev = " + option.currentVolumeStep);
        }
    }

    void DecreaseVol(int index)
    {
        var option = SoundOptions[index];
        if (option.currentVolumeStep > 0)
        {
            option.currentVolumeStep--;
            UpdateVolUI(option);
            SoundManager.Instance.ChangeVol(index, -1);
            Debug.Log(index + "Sound Lev = " + option.currentVolumeStep);
        }
    }

    void UpdateVolUI(SoundOption option)
    {
        for (int i = 0; i < option.volumeSteps.Length; i++)
        {
            option.volumeSteps[i].enabled = (i <= option.currentVolumeStep);
        }
    }

    void HandleMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return))
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

    void CloseOptionPanel()
    {
        SoundManager.Instance.PlaySFX("ButtonClickSfx");
        OptionPanel.SetActive(false);
        HowToPanel.SetActive(false);
        isOptionPanelOpen = false;
    }

    void GameEnd()
    {
        Application.Quit();
    }
}
