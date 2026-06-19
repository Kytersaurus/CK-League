using UnityEngine;
[CreateAssetMenu(menuName = "Attacks/BasicHeal")]
public class BasicHeal : Heals
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        attacker.HealHealth(healAmount);
    }
}