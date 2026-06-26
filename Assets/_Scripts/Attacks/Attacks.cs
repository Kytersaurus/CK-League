using UnityEngine;

public abstract class Attacks : ScriptableObject
{
    public string attackName;
    public int damage;
    public Sprite icon;
    public string attackDesc;
    public virtual void Execute(BaseUnit attacker, BaseUnit target)//Default attacking function for all attacks, overide if neccessary
    {
        if (target.immune)
        {
            target.immune = false;
            return;
        }
        target.TakeDamage(CalculateDmg(target.reducedDmg, damage));
    }
    public int CalculateDmg(float reduction, int dmg)
    {
        return (int)(reduction * dmg);
    }
}