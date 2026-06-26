using UnityEngine;

public class Mitigate : Attacks
{
    public float chance;
    public float perReduction;
    public float counterAttackChance;
    public int counterAttackDmg;
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
            unit.reducedDmg = 1 - perReduction;
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
            unit.reducedDmg = 1 - perReduction;    
        }
    }
}
