using UnityEngine;

public class Mitigate : Attacks
{
    public float chance;
    public float perReduction;
    protected void NegateDmg(BaseHero unit)
    {
        if (Random.value < chance)
        {
            unit.immune = true;
        }
    }
    protected void ReducedDmg(BaseHero unit)
    {
        if (Random.value < chance)
        {
            unit.reducedDmg = perReduction;
        }
    }
    protected void ReduceOrNegate(BaseHero unit)
    {
        if (Random.value < chance)
        {
            unit.immune = true;
        } 
        else
        {
            unit.reducedDmg = perReduction;    
        }
    }
}
