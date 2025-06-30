using TMPro;
using UnityEngine;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI IceDmgLv;
    [SerializeField] TextMeshProUGUI IceCoolLv;
    [SerializeField] TextMeshProUGUI IceRangeLv;
    [SerializeField] TextMeshProUGUI InfDmgLv;
    [SerializeField] TextMeshProUGUI InfCoolLv;
    [SerializeField] TextMeshProUGUI InfRangeLv;
    [SerializeField] TextMeshProUGUI BlackDmgLv;
    [SerializeField] TextMeshProUGUI BlackCoolLv;
    [SerializeField] TextMeshProUGUI BlackRangeLv;
    [SerializeField] TextMeshProUGUI FSDmgLv;
    [SerializeField] TextMeshProUGUI FSCoolLv;
    [SerializeField] TextMeshProUGUI FSRangeLv;
    [SerializeField] TextMeshProUGUI TDDmgLv;
    [SerializeField] TextMeshProUGUI TDCoolLv;
    [SerializeField] TextMeshProUGUI TDRangeLv;

    [SerializeField] TextMeshProUGUI DashCool;
    [SerializeField] TextMeshProUGUI DashSpeed;
    [SerializeField] TextMeshProUGUI ExpUp;
    [SerializeField] TextMeshProUGUI HpUp;
    [SerializeField] TextMeshProUGUI MoveSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IceDmgLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.IPSkillDamage).ToString();
        IceCoolLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.IPSkillCooldown).ToString();
        IceRangeLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.IPSkillRange).ToString();
        InfDmgLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.IFSkillDamage).ToString();
        InfCoolLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.IFSkillCooldown).ToString();
        InfRangeLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.IFSkillRange).ToString();
        BlackDmgLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.BHSkillDamage).ToString();
        BlackCoolLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.BHSkillCooldown).ToString();
        BlackRangeLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.BHSkillRange).ToString();
        FSDmgLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.FSSkillDamage).ToString();
        FSCoolLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.FSSkillCooldown).ToString();
        FSRangeLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.FSSkillRange).ToString();
        TDDmgLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.TDSkillDamage).ToString();
        TDCoolLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.TDSkillCooldown).ToString();
        TDRangeLv.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.TDSkillRange).ToString();
        DashCool.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.DashCooldown).ToString();
        DashSpeed.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.DashSpeed).ToString();
        ExpUp.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.ExpIncrease).ToString();
        HpUp.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.MaxHPIncrease).ToString();
        MoveSpeed.text = "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.CharacterMoveSpeed).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
