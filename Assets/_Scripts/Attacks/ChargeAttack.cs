using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/ChargeAttack")]
public class ChargeAttack : Attacks
{
    protected bool CheckMovement(BaseUnit unit)
    {
        if (unit.PreviousTile == null)
        {
            return false;
        }
        else
        {
            return unit.PreviousTile == unit.OccupiedTile;
        }
    }
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        if (CheckMovement(attacker))
        {
            target.TakeDamage(damage);
        }
    }
}
