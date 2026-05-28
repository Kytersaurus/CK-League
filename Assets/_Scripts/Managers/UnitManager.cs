using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private BaseUnit Enemy, Hero;
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;

    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnEnemies()
    {
        var spawnedEnemy = Instantiate(Enemy);
        var spawnTile = GridManager.Instance.GetEnemySpawnTile();
        spawnTile.SetUnit(spawnedEnemy);

        GameManager.Instance.UpdateGameState(GameState.SpawnHeroes);
    }

    public void SpawnHeroes(Tile spawnTile)
    {
        var spawnedHero = Instantiate(Hero);
        spawnTile.SetUnit(spawnedHero);

        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
    }

    /*private T GetUnit<T>(Faction faction) where T : BaseUnit
    {
        //return (T)_units.Where
    }*/
}
