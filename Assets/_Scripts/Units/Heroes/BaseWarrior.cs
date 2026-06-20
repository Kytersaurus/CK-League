using Unity.VisualScripting;
using UnityEngine;

public class BaseWarrior : BaseHero
{
    public void Start()
    {
        maxHealth += 100;
        AttackPower = 40;
        AttackSpeed = 3;
        AttackRange = 1;
        moveRange = 2;
    }
    
}
