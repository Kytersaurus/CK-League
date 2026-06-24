using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Dodge")]
public class Dodge : Mitigate
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        ReduceOrNegate((BaseHero)attacker);
    }
}
