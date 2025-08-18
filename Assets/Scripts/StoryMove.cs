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
        SoundManager.Instance.PlayBGM("StorySceneBGM");
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


        // "StoryDone" 키의 값을 1로 설정하여 스토리 완료를 기록
        Debug.Log("스토리가 종료되어 StoryDone 값을 1로 저장합니다.");
        PlayerPrefs.SetInt("StoryDone", 1);
        PlayerPrefs.Save(); // 변경사항을 즉시 저장

        // 다음 씬(실제 플레이 씬)으로 이동
        SceneManager.LoadScene("MapScene");
    }
}
