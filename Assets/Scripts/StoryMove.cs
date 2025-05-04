using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryMove : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public StoryManager story;
    [SerializeField]
    public Image FadePanel;

    void Start()
    {
        story.StartDialouge();
    }
    private void Update()
    {
        if(story.FinishDialouge) StartCoroutine(PlayerSpriteMove());
    }

    IEnumerator PlayerSpriteMove()
    {
        player.GetComponent<Animator>().SetTrigger("Walk");

        Vector3 targetPos = player.transform.position + Vector3.right * 5f; 
        float duration = 4f;
        float elapsed = 0f;

        Vector3 startPos = player.transform.position;
        Color color = FadePanel.color;
        FadePanel.color = color;
        color.a = 0f;

        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            FadePanel.color = color;
            yield return null;
        }

        player.transform.position = targetPos;
        
        SceneManager.LoadScene("MapScene");
    }
}
