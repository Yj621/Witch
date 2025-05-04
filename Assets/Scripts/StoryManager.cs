using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI Dialouge;
    public GameObject TextUI;

    public float TypeSpeed = 0.05f;

    public List<string> dialouges = new List<string>();

    private int currentDialougeIndex = 0;
    private Coroutine TypeCoroutine;
    private bool isTyping = false;
    private string currentLine = "";
    public bool FinishDialouge = false;

    private void Start()
    {
        //TextUI.SetActive(false);
    }

    public void StartDialouge()
    {
        currentDialougeIndex = 0;
        TextUI.SetActive(true);
        ShowNextDialouge();
    }

    public void ShowNextDialouge()
    {
        if (isTyping) return;
        if(currentDialougeIndex < dialouges.Count)
        {
            currentLine = dialouges[currentDialougeIndex];
            TypeCoroutine = StartCoroutine(TypeDialouge(currentLine));
            currentDialougeIndex++;
        }
        else
        {
            EndDialouge();
        }
    }

    IEnumerator TypeDialouge(string line)
    {
        isTyping = true;
        Dialouge.text = "";
        foreach(char letter in line.ToCharArray())
        {
            Dialouge.text += letter;
            yield return new WaitForSeconds(TypeSpeed);
        }
        isTyping = false;
    }

    public void EndDialouge()
    {
        TextUI.SetActive(false);
        FinishDialouge = true;
    }

    private void Update()
    {
        if(TextUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(TypeCoroutine);
                Dialouge.text = currentLine;
                isTyping = false;
            }
            else
            {
                ShowNextDialouge();
            }
        }
    }
}
