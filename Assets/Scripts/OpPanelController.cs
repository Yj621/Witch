using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpPanelController : MonoBehaviour
{
    public Image[] buttonpickimages;

    private int selectedIndex = 0;
   
    void Start()
    {
        for (int i = 0; i < buttonpickimages.Length; i++)
        {
            buttonpickimages[i].enabled = (i == 0);
        }
    }

    void Update()
    {
            HandleMenuInput();
            HandleMenuSelection();
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
                    
                    break;
                case 1:
                    
                    break;
                case 2:
                    
                    break;
            }
        }
    }
}
