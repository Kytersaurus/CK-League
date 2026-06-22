using Unity.VisualScripting;
using UnityEngine;

public class BaseMage : BaseHero
{
    void Start()
    {
        maxHealth = 50;
        AttackPower = 70;
        AttackSpeed = 3;
        AttackRange = 3;
        moveRange = 1;
    }

}
