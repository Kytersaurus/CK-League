using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/LifeSlashAttack")]
public class LifeSlashAttack : MeleeAttack
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        if (attacker.CurrentHealth == 0)
        {
            return;
        }
        float healthToMaxHealthRatio = (float)attacker.CurrentHealth / attacker.maxHealth;
        int damageScaledToHealth = Mathf.RoundToInt(damage/healthToMaxHealthRatio);
        target.TakeDamage(CalculateDmg(target.reducedDmg, damageScaledToHealth));
        target.reducedDmg = 1;
    }
}
