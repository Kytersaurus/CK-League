using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    //[SerializeField] private BaseUnit Enemy, Hero;
    public static UnitManager Instance;

    private List<ScriptableUnit> _units, _heroes, _enemies;
    private List<BaseUnit> _heroesLeft = new List<BaseUnit>();
    private List<BaseUnit> _enemiesLeft = new List<BaseUnit>();

    public BaseHero SelectedHero;
    public List<Tile> ReachableTiles {get; private set;} = new List<Tile>();
    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        _heroes = _units.Where(u=>u.Faction == Faction.Hero).ToList();
        _enemies = _units.Where(u=>u.Faction == Faction.Enemy).ToList();
    }

    public void SpawnEnemies()
    {
        foreach(ScriptableUnit enemy in _enemies)
        {
            var spawnedEnemy = Instantiate(enemy.UnitPrefab);
            var spawnTile = GridManager.Instance.GetEnemySpawnTile();
            spawnTile.SetUnit(spawnedEnemy);
            _enemiesLeft.Add(spawnedEnemy);
        }

        GameManager.Instance.UpdateGameState(GameState.SpawnHeroes);
    }

    public void SpawnHeroes(Tile spawnTile)
    {
        var spawnedHero = Instantiate(_heroes[0].UnitPrefab);
        spawnTile.SetUnit(spawnedHero);
        _heroesLeft.Add(spawnedHero);
    }

    public void SetSelectedHero(BaseHero hero)
    {
        foreach (Tile tile in ReachableTiles)
        {
            tile._highlight.SetActive(false);
        }
        ReachableTiles.Clear();

        SelectedHero = hero;

        if (hero == null)
        {
            return;
        }

        ReachableTiles = GridManager.Instance.GetReachableTiles(SelectedHero.OccupiedTile, SelectedHero.moveRange);    
        foreach (Tile tile in ReachableTiles)
        {
            tile._highlight.SetActive(true);
        }
    }

    public void KillUnit(BaseUnit unit)
    {
        if(unit.Faction == Faction.Hero)
        {
            _heroesLeft.Remove(unit);
            if(_heroesLeft.Count == 0)
                GameManager.Instance.UpdateGameState(GameState.Defeat);
            else
                GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
        else
        {
            _enemiesLeft.Remove(unit);
            if(_enemiesLeft.Count == 0)
                GameManager.Instance.UpdateGameState(GameState.Victory);
            else
                GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
        Destroy(unit.gameObject);
    }

    public bool InAttackRange(BaseUnit attackingUnit, BaseUnit defendingUnit)
    {
        var horizontalDistance = Mathf.Abs(attackingUnit.transform.position.x - defendingUnit.transform.position.x);
        var verticalDistance = Mathf.Abs(attackingUnit.transform.position.y - defendingUnit.transform.position.y);
        return attackingUnit.AttackRange >= (horizontalDistance + verticalDistance);
    }

    public bool SkipAttackPhase()
    {
        foreach(BaseUnit hero in _heroesLeft)
        {
            foreach(BaseUnit enemy in _enemiesLeft)
            {
                if(InAttackRange(hero, enemy) || InAttackRange(enemy, hero))
                    return false;
            }
        }
        return true;
    }

    /*private T GetUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u=>u.Faction == faction).OrderBy(o=>Random.value).First().UnitPrefab;
    }*/
}
