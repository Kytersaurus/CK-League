using Unity.VisualScripting;
using UnityEngine;

public class BaseArcher : BaseHero
{
    public void Start()
    {
        maxHealth -= 30;
        AttackPower = 60;
        AttackSpeed = 3;
        AttackRange = 3;
        moveRange = 2;
    }
    
}
