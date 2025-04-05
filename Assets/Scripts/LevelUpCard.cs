using UnityEngine;
using UnityEngine.EventSystems;

public class LevelUpCard : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject LevelUpPanel;
    Animator ani;
    Player player;

    void Start()
    {
        player = GameManager.Instance.player;
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

    public void OnLevelUp()
    {
        player.LevelUp();
    }

}
