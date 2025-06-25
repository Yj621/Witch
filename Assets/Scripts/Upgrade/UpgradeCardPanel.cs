using UnityEngine;

public class UpgradeCardPanel : MonoBehaviour
{
    public UpgradeCard[] upgradeCards; // 에디터에서 3개 할당
    private int selectedIndex = 0;

    void Start()
    {
        UpdateHighlight();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.D)
            {
                selectedIndex = Mathf.Min(selectedIndex + 1, upgradeCards.Length - 1);
                UpdateHighlight();
            }
            else if (e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.A)
            {
                selectedIndex = Mathf.Max(selectedIndex - 1, 0);
                UpdateHighlight();
            }
            else if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Space)
            {
                upgradeCards[selectedIndex].OnSelect();
            }
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            upgradeCards[i].SetHighlight(i == selectedIndex);
        }
    }
}