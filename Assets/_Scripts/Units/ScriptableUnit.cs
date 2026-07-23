using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
    public int spawnX, spawnY;

    public ScriptableUnit EvolvePathA;
    public ScriptableUnit EvolvePathB;
    public int EvolveLevel;
}


public enum Faction
{
    Hero = 0,
    Enemy = 1
}