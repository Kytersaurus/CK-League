using Unity.VisualScripting;
using UnityEngine;

public class BaseCavalry : BaseHero
{
    public void Start()
    {
        maxHealth += 0;
        AttackPower = 50;
        AttackSpeed = 3;
        AttackRange = 1;
        moveRange = 4;
    }
    
}
