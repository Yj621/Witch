using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerInput.Instance != null)
        {
            Destroy(PlayerInput.Instance.gameObject);
        }
        SoundManager.Instance.PlayBGM("ClearBGM");
        StartCoroutine(EndingCoroutine());
    }

    IEnumerator EndingCoroutine()
    {
        float timer = 0f;
        float autoSkipTime = 5f;

        while (timer < autoSkipTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("OpeningScene");
                yield break; // 씬 로딩했으니 코루틴 중단
            }

            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("OpeningScene");
    }

}
