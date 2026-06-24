using UnityEngine;
using System.Collections.Generic;

public abstract class FireAttack : Attacks
{
    public int aoeDamage;
    public int aoeRange;
    protected void FireAOEAttack(Tile origin, int range)
    {
        List<Tile> affectedTiles = GridManager.Instance.GetTileInAOERAnge(origin, range);
        foreach(Tile tile in affectedTiles)
        {
            if (tile.OccupiedUnit == null)
            {
                continue;
            }
            tile.OccupiedUnit.TakeDamage(aoeDamage);
        }
    }
}
