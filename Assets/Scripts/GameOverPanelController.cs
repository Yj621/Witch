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
        UINavigation();
        UISelection();
    }

    /// <summary>
    /// 위/아래 (W/S) 키로 메뉴 인덱스 조정 및 하이라이트 갱신
    /// </summary>
    private void UINavigation()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelection(-1);
        }
    }

    /// <summary>
    /// 스페이스(Enter) 키로 현재 선택 실행
    /// </summary>
    private void UISelection()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySFX("ButtonClickSfx");
            ExecuteSelected();
        }
    }

    /// <summary>
    /// 선택 인덱스 변경 및 하이라이트 업데이트, 효과음 재생
    /// </summary>
    /// <param name="direction">+1 = 아래, -1 = 위</param>
    private void MoveSelection(int direction)
    {
        selectedIndex = Mathf.Clamp(selectedIndex + direction, 0, highlightImages.Length - 1);
        SoundManager.Instance.PlaySFX("ButtonFlipSfx");
        UpdateHighlight();
    }

    /// <summary>
    /// 하이라이트 이미지를 현재 selectedIndex 기준으로 켜고 나머지는 끕니다.
    /// </summary>
    private void UpdateHighlight()
    {
        for (int i = 0; i < highlightImages.Length; i++)
        {
            highlightImages[i].enabled = (i == selectedIndex);
        }
    }

    /// <summary>
    /// selectedIndex에 따라 해당 동작을 실행합니다.
    /// </summary>
    private void ExecuteSelected()
    {
        switch (selectedIndex)
        {
            case 0:
                // 다시 시작
                GameManager.Instance.ReStartGame();
                break;
            case 1:
                // 타이틀로 돌아가기
                GameManager.Instance.GoTitle();
                break;
        }
    }
}