using System.Collections.Generic;
using UnityEngine;

public abstract class Attacks : ScriptableObject
{
    public string attackName;
    public int damage;
    public int range;
    public Sprite icon;
    public string attackDesc;
    public virtual void Execute(BaseUnit attacker, BaseUnit target)//Default attacking function for all attacks, overide if neccessary
    {
        target.TakeDamage(damage);
    }
}