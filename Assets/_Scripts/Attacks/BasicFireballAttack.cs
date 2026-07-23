using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/BasicFireball")]
public class BasicFireballAttack : FireAttack
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        target.TakeDamage(CalculateDmg(target.reducedDmg ,damage));
        target.reducedDmg = 1;
        FireAOEAttack(target.OccupiedTile, aoeRange);
    }
}
