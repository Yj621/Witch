using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening : MonoBehaviour
{
    public GameObject OptionPanel;
    public Image[] buttonpickimages;

    private int selectedIndex = 0;
    private bool isOptionPanelOpen = false;

    void Start()
    {
        for (int i = 0; i < buttonpickimages.Length; i++)
        {
            buttonpickimages[i].enabled = (i == 0);
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseOptionPanel();
            }
        }
    }

    void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeSelection(-1);
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

    void HandleMenuSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (selectedIndex)
            {
                case 0:
                    StartGame();
                    break;
                case 1:
                    OpenOptionPanel();
                    break;
                case 2:
                    GameEnd();
                    break;
            }
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("MapScene");
    }

    void OpenOptionPanel()
    {
        OptionPanel.SetActive(true);
        isOptionPanelOpen = true;
    }

    void CloseOptionPanel()
    {
        OptionPanel.SetActive(false);
        isOptionPanelOpen = false;
    }

    void GameEnd()
    {
        Application.Quit();
    }
}
