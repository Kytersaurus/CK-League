using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public int MaxHealth;
    public int CurrentHealth;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }
}
