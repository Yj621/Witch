using UnityEngine;
using UnityEngine.EventSystems;

public class LevelUpCard : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject LevelUpPanel;
    Animator ani;

    void Start()
    {
        ani = GetComponent<Animator>();
        LevelUpPanel.SetActive(false);
    }

    public void LevelUpPanelPop()
    {
        LevelUpPanel.SetActive(true);

    }

    public void OnSelect(BaseEventData eventData)
    {
        ani.SetTrigger("Select");
    }

    public void OnPressed()
    {
        LevelUpPanel.SetActive(false);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ani.SetTrigger("Idle");
    }
}
