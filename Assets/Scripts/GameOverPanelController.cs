using UnityEngine;
using UnityEngine.UI;
public class GameOverPanelController : MonoBehaviour
{
    [Header("메뉴 버튼 하이라이트 이미지")]
    public Image[] highlightImages; // 두 개 이미지 할당

    private int selectedIndex = 0;

    void Start()
    {
        // 시작 시 첫번째(다시시작) 선택 표시
        UpdateHighlight();
    }

    void Update()
    {
        // 위/아래 키로 선택 이동
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = Mathf.Min(selectedIndex + 1, highlightImages.Length - 1);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
            UpdateHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = Mathf.Max(selectedIndex - 1, 0);
            SoundManager.Instance.PlaySFX("ButtonFlipSfx");
            UpdateHighlight();
        }

        // Enter 키로 실행
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySFX("ButtonClickSfx");
            ExecuteSelected();
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < highlightImages.Length; i++)
        {
            highlightImages[i].enabled = (i == selectedIndex);
        }
    }

    private void ExecuteSelected()
    {
        switch (selectedIndex)
        {
            case 0:
                // 다시시작
                GameManager.Instance.ReStartGame();
                break;
            case 1:
                // 타이틀로 돌아가기
                GameManager.Instance.GoTitle();
                //만약 '게임 종료'로 바꾸려면 위 줄을 아래로 바꿔주세요.
                // GameManager.Instance.Exit();
                break;
        }
    }
}
