using UnityEngine;
using System.Collections;

public class IcePillar : MonoBehaviour
{
    public SkillData skillData;

    private void OnEnable()
    {
        //  SkillManager에 있는 최신 클론으로 덮어쓰기
        skillData = SkillManager.Instance.GetRuntimeSkillData("IcePillar");


        StartCoroutine(DisableAfterTime());
    }
    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(skillData.lifetime);
    }

    public void SkillEnd()
    {
        gameObject.SetActive(false);

        AutoSKillPool.Instance.ReturnSkillObject(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMove>().EnemyHurt(skillData.damage);
            Debug.Log($"[IcePillar] {other.name}에게 {skillData.damage} 데미지");
        }
    }
}
