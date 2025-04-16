using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Opening : MonoBehaviour
{
    public GameObject OptionPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void OpenOptionPanel()
    {
        OptionPanel.SetActive(true);
    }
    void CloseOptionPanel()
    {
        OptionPanel.SetActive(false);
    }

    void GameEnd()
    {
        Application.Quit();
    }
}