using UnityEngine;
using UnityEngine.UI;

public class OpPanelController : MonoBehaviour
{
    [System.Serializable]
    public class SoundOption
    {
        public Image[] volumeSteps; // 5개 이미지nt
        public int currentVolumeStep = 2; // 초기 2단계
    }


    public Image[] TopButtons;
    public Image[] OptionButtons;
    public Image[] SoundButtons;
    public GameObject[] TopPanels;

    private bool isOption;
    private bool isSound;
    private bool isTopSelectMode = true;

    private int selectedOpIndex = 0;
    private int selectedSoundIndex = 0;
    private int selectedTopIndex = 0;
    public SoundOption[] SoundOptions;

    void Start()
    {
        TopButtons[0].enabled = true;
        TopButtons[1].enabled = false;
        isOption = true;
        isSound = false;

        // 시작할 때 세부 항목은 모두 꺼둔다
        for (int i = 0; i < SoundButtons.Length; i++)
            SoundButtons[i].enabled = false;

        for (int i = 0; i < OptionButtons.Length; i++)
            OptionButtons[i].enabled = false;

        SoundOptions[0].currentVolumeStep = SoundManager.Instance.MasterSoundLevel;
        SoundOptions[1].currentVolumeStep = SoundManager.Instance.BgmLevel;
        SoundOptions[2].currentVolumeStep = SoundManager.Instance.SfxLevel;

        foreach (var option in SoundOptions)
        {
            UpdateVolUI(option);
        }

        selectedOpIndex = -1;
        selectedSoundIndex = -1;
    }

    void Update()
    {
        HandleModeSwitch(); // 모드 전환 먼저
        if (isTopSelectMode)
        {
            HandleTopMenu();
        }
        else
        {
            HandleMenuInput();
        }
        SoundButtonControll();
        HandleMenuSelection();
    }

    void HandleTopMenu()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ChangeTopSelection(-1);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ChangeTopSelection(1);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
        Debug.Log("Top = " + selectedTopIndex);
    }

    void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (isOption)
            {
                if (selectedOpIndex < 0)
                {
                    selectedOpIndex = 0;
                    OptionButtons[selectedOpIndex].enabled = true;
                }
                else if (selectedOpIndex < OptionButtons.Length - 1)
                {
                    OptionButtons[selectedOpIndex].enabled = false;
                    selectedOpIndex++;
                    OptionButtons[selectedOpIndex].enabled = true;
                }
                Debug.Log("Op = " + selectedOpIndex);
            }
            else if (isSound)
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
            }
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (isOption)
            {
                if (selectedOpIndex > 0)
                {
                    OptionButtons[selectedOpIndex].enabled = false;
                    selectedOpIndex--;
                    OptionButtons[selectedOpIndex].enabled = true;
                }
                Debug.Log("Op = " + selectedOpIndex);
            }
            else if (isSound)
            {
                if (selectedSoundIndex > 0)
                {
                    SoundButtons[selectedSoundIndex].enabled = false;
                    selectedSoundIndex--;
                    SoundButtons[selectedSoundIndex].enabled = true;
                }
                Debug.Log("Sound = " + selectedSoundIndex);
            }
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        }
    }



    void HandleModeSwitch()
    {
        if (isTopSelectMode)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                SoundManager.Instance.PlaySFX("ButtonFlipSfx");
                TopButtons[selectedTopIndex].enabled = false;
                isTopSelectMode = false;

                if (isOption)
                {
                    selectedOpIndex = -1;
                    for (int i = 0; i < OptionButtons.Length; i++)
                        OptionButtons[i].enabled = false;
                }
                else if (isSound)
                {
                    selectedSoundIndex = -1;
                    for (int i = 0; i < SoundButtons.Length; i++)
                        SoundButtons[i].enabled = false;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                SoundManager.Instance.PlaySFX("ButtonFlipSfx");
                bool isAtTopOfList = (isOption && selectedOpIndex == 0) || (isSound && selectedSoundIndex == 0);

                if (isAtTopOfList)
                {
                    TopButtons[selectedTopIndex].enabled = true;
                    isTopSelectMode = true;

                    ClearDetailSelection();
                }
            }
        }
    }


    void ChangeTopSelection(int direction)
    {
        TopButtons[selectedTopIndex].enabled = false;
        TopPanels[selectedTopIndex].SetActive(false);

        selectedTopIndex += direction;

        // Top은 한바퀴 돌게
        if (selectedTopIndex < 0) selectedTopIndex = TopButtons.Length - 1;
        if (selectedTopIndex >= TopButtons.Length) selectedTopIndex = 0;

        TopButtons[selectedTopIndex].enabled = true;
        TopPanels[selectedTopIndex].SetActive(true);

        if (selectedTopIndex == 0)
        {
            isOption = true;
            isSound = false;
        }
        else if (selectedTopIndex == 1)
        {
            isOption = false;
            isSound = true;

            foreach (var option in SoundOptions)
            {
                UpdateVolUI(option);
            }
        }
    }

    void ChangeOptionSelection(int direction)
    {
        OptionButtons[selectedOpIndex].enabled = false;

        selectedOpIndex += direction;
        selectedOpIndex = Mathf.Clamp(selectedOpIndex, 0, OptionButtons.Length - 1);

        OptionButtons[selectedOpIndex].enabled = true;
        Debug.Log(selectedOpIndex);
    }

    void ChangeSoundSelection(int direction)
    {
        SoundButtons[selectedSoundIndex].enabled = false;

        selectedSoundIndex += direction;
        selectedSoundIndex = Mathf.Clamp(selectedSoundIndex, 0, SoundButtons.Length - 1);

        SoundButtons[selectedSoundIndex].enabled = true;

       
    }

    void ClearDetailSelection()
    {
        // 세부 항목 모두 끄기
        for (int i = 0; i < OptionButtons.Length; i++)
            OptionButtons[i].enabled = false;
        for (int i = 0; i < SoundButtons.Length; i++)
            SoundButtons[i].enabled = false;
    }

    void SoundButtonControll()
    {
        if(!isTopSelectMode && isSound)
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
    }

    void IncreaseVol(int index)
    {
        var option = SoundOptions[index];
        if(option.currentVolumeStep < option.volumeSteps.Length - 1)
        {
            option.currentVolumeStep++;
            UpdateVolUI(option);
            SoundManager.Instance.ChangeVol(index, 1);
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
        for(int i = 0; i < option.volumeSteps.Length; i++)
        {
            option.volumeSteps[i].enabled = (i <= option.currentVolumeStep);
        }
    }

    void HandleMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.Instance.PlaySFX("ButtonClickSfx");
            switch (selectedOpIndex)
            {
                case 0:
                    GameManager.Instance.ReStartGame();
                    break;
                case 1:
                    GameManager.Instance.GoTitle();
                    break;
                case 2:
                    GameManager.Instance.Exit();
                    break;
            }
        }
    }


}
