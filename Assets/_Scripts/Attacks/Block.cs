using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Block")]
public class Block : Mitigate
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        ReduceOrNegate((BaseHero)attacker);
    }
}
