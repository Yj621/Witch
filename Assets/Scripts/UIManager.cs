using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider HpSlider;
    public Slider ExpSlider;
    Player player;

    void Start()
    {
        player = GameManager.Instance.player;
        HpSlider.maxValue = player.Hp;
        ExpSlider.maxValue = player.maxExp;
    }
    
    void Update()
    {
        HpSlider.value = player.Hp;
        ExpSlider.maxValue = player.maxExp;
        ExpSlider.value = player.Exp;
    }
}
