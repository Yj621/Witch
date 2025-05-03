using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI dialouge;
    public GameObject TextUI;

    public float TypeSpeed = 0.05f;

    public List<string> dialouges = new List<string>();

    private int currentDialougeIndex = 0;
    private Coroutine TypeCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        TextUI.SetActive(false);
    }

    public void StartDialouge()
    {
        currentDialougeIndex = 0;
        TextUI.SetActive(true);
        //ShowNextDialouge();
    }
}
