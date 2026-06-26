using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/ChargeAttack")]
public class ChargeAttack : Attacks
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        if (attacker.hasMoved)
        {
            target.TakeDamage(damage);
        }
    }
}
