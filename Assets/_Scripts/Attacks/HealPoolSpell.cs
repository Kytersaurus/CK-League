using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/HealPoolSpell")]
public class HealPoolSpell : HealExternal
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        target.HealHealth(healAmount);
    }
}
