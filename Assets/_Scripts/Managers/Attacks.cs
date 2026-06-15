using System.Collections.Generic;
using UnityEngine;

public abstract class Attacks : ScriptableObject
{
    public string attackName;
    public int damage;
    public int aoeDamage;
    public int healAmount;
    public int range;
    public int aoeRange;
    public Sprite icon;
    public virtual void Execute(BaseUnit attacker, BaseUnit target)//Default attacking function for all attacks, overide if neccessary
    {
        target.TakeDamage(damage);
    }
}
public abstract class FireAttack : Attacks
{
    protected void FireAOEAttack(Tile origin, int range)
    {
        List<Tile> affectedTiles = GridManager.Instance.GetNeighbourTiles(origin);
        foreach(Tile tile in affectedTiles)
        {
            if (tile.OccupiedUnit == null)
            {
                continue;
            }
            tile.OccupiedUnit.TakeDamage(aoeDamage);
        }
    }
    //Add other helper functions for fire attacks here
}
[CreateAssetMenu(menuName = "Attacks/Basic Fireball")]
public class BasicFireballAttack : FireAttack
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        target.TakeDamage(damage);
        FireAOEAttack(target.OccupiedTile, aoeRange);
    }
}
[CreateAssetMenu(menuName = "Attacks/Basic Lightning")]
public class BasicLightningAttack : Attacks
{
   
}
[CreateAssetMenu(menuName = "Attacks/Basic Heal")]
public class BasicHeal : Attacks
{
    public override void Execute(BaseUnit attacker, BaseUnit target)
    {
        attacker.HealHealth(healAmount);
    }
}
[CreateAssetMenu(menuName = "Attacks/Basic slash")]
public class BasicSlashAttack : Attacks
{
    
}
