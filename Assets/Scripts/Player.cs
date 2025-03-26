using UnityEngine;

public class Player
{
    public int Hp {get; private set;}

    public Player(int hp)
    {
        Hp = hp;
    }

    public void Hurt(int damage)
    {
        Hp -= damage;
    }

    public void Heal(int amount)
    {
        Hp+=amount;
    }
}
