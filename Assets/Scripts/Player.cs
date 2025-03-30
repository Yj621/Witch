using UnityEngine;

public class Player
{
    public int level;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    public int hp;
    public int Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                Die();
            }
        }
    }
    public int exp;
    public int Exp
    {
        get { return exp; }
        set { exp = value; }
    }

    public int maxExp;
    public Player(int level, int hp, int exp)
    {
        Level = level;
        Hp = hp;
        Exp = exp;
        maxExp = NextLevelExp(Level);
    }

    public void Hurt(int damage)
    {
        Hp -= damage;
    }
    public void Die()
    {
        Debug.Log("주금");
    }

    public void Heal(int amount)
    {
        Hp += amount;
    }

    public void GetExperience(int getExp)
    {
        Exp += getExp;
        while (Exp >= maxExp)
        {
            Exp -= maxExp;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        Level++;
        Hp += 10;
        maxExp = NextLevelExp(Level);
        Debug.Log($"레벨 업! 현재 레벨: {Level}, 다음 레벨 경험치: {maxExp}");
    }

    //다음 레벨의 경험치 계산
    private int NextLevelExp(int level)
    {
        return 100 + (level - 1) * 50;
    }
}
