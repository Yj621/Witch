using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanelController : MonoBehaviour
{
    public static UpgradePanelController Instance;

    private List<GameObject> cardObjects = new List<GameObject>();
    private int selectedIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterCard(GameObject cardObj)
    {
        cardObjects.Add(cardObj);
    }

    void OnEnable()
    {
        selectedIndex = 0;
        HighlightCard(selectedIndex);
    }

    void Update()
    {
        if (!gameObject.activeSelf || cardObjects.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedIndex = (selectedIndex + cardObjects.Count - 1) % cardObjects.Count;
            HighlightCard(selectedIndex);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            selectedIndex = (selectedIndex + 1) % cardObjects.Count;
            HighlightCard(selectedIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            cardObjects[selectedIndex].GetComponent<Button>().onClick.Invoke();
        }
    }

    void HighlightCard(int index)
    {
        for (int i = 0; i < cardObjects.Count; i++)
        {
            var animator = cardObjects[i].GetComponent<Animator>();
            if (animator == null) continue;

            if (i == index)
                animator.SetTrigger("Select");
            else
                animator.SetTrigger("Idle");
        }

        // 포커스 이동 (이벤트 시스템 연동도 하고 싶으면)
        EventSystem.current.SetSelectedGameObject(cardObjects[index]);
    }

    public void ClearCards()
    {
        cardObjects.Clear();
    }
}
