using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EndingCoroutine());
    }

    IEnumerator EndingCoroutine()
    {
        yield return new WaitForSeconds(2f);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("OpeningScene");
        }

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("OpeningScene");
    }
}
