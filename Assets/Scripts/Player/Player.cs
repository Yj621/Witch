using UnityEngine;

public class Player
{
    public int level;
    public int hp;
    public int exp;
    public int maxHp;
    public int maxExp;

    public float playerSpeed; 
    public float dashSpeed;
    public Skill skill;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }

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
            if( hp > maxHp)
            {
                hp = maxHp;
            }
        }
    }

    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp=value;}
    }

    public int Exp
    {
        get { return exp; }
        set { exp = value; }
    }

    public Player(int level, int exp, float playerSpeed, float dashSpeed, int maxHp)
    {
        skill = new Skill();
        Level = level;
        Exp = exp;
        maxExp = NextLevelExp(Level);

        this.maxHp = maxHp;
        if(hp>maxHp)
        {
            hp = maxHp;
        }
        this.playerSpeed = playerSpeed;
        this.dashSpeed = dashSpeed;
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
        if(Hp > maxHp)
        {
            Hp = maxHp;
        }
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
        //타임 스또뿌
        //Time.timeScale = 0f;
        Level++;
        Hp += 10;
        maxExp = NextLevelExp(Level);
        UIManager.Instance.LevelUpPanelPop();
        Debug.Log($"레벨 업! 현재 레벨: {Level}, 다음 레벨 경험치: {maxExp}");
    }

    //다음 레벨의 경험치 계산
    private int NextLevelExp(int level)
    {
        return 100 + (level - 1) * 50;
    }
}
