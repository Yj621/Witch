using UnityEngine;
using UnityEngine.UI;

public class OpPanelController : MonoBehaviour
{
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

    [System.Serializable]
    public class SoundOption
    {
        public Image[] volumeSteps; // 5개 이미지
        public int currentVolumeStep = 2; // 초기 2단계
    }

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
    }

    void HandleTopMenu()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeTopSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeTopSelection(1);
        }
    }

    void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log(selectedOpIndex);
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
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isOption)
            {
                if (selectedOpIndex > 0)
                {
                    OptionButtons[selectedOpIndex].enabled = false;
                    selectedOpIndex--;
                    OptionButtons[selectedOpIndex].enabled = true;
                }
            }
            else if (isSound)
            {
                if (selectedSoundIndex > 0)
                {
                    SoundButtons[selectedSoundIndex].enabled = false;
                    selectedSoundIndex--;
                    SoundButtons[selectedSoundIndex].enabled = true;
                }
            }
        }
    }



    void HandleModeSwitch()
    {
        if (isTopSelectMode)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                TopButtons[selectedTopIndex].enabled = false;
                isTopSelectMode = false;

                if (isOption)
                {
                    selectedOpIndex = -1; // << 이거 꼭 해줘야 해
                    for (int i = 0; i < OptionButtons.Length; i++)
                        OptionButtons[i].enabled = false;
                }
                else if (isSound)
                {
                    selectedSoundIndex = -1; // << 사운드 쪽도 똑같이
                    for (int i = 0; i < SoundButtons.Length; i++)
                        SoundButtons[i].enabled = false;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
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

    void SoundButtonControll(int index, int direction)
    {

    }
}
