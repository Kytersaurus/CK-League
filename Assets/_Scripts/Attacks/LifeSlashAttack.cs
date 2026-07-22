using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/LifeSlashAttack")]
public class LifeSlashAttack : MeleeAttack
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        target.TakeDamage(CalculateDmg(target.reducedDmg, damage / (attacker.CurrentHealth / attacker.maxHealth)));
        target.reducedDmg = 1;
    }
}
