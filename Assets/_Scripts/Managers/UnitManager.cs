using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Clrain.Collections;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units, _heroes;
    private List<BaseUnit> _remainingHeroes = new List<BaseUnit>();
    private List<BaseUnit> _remainingEnemies = new List<BaseUnit>();
    private List<BaseUnit> _remainingUnits = new List<BaseUnit>();

    public BaseHero SelectedHero;
    private PriorityQueue<BaseUnit, int> _actionQueue = new PriorityQueue<BaseUnit, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    public List<Tile> ReachableTiles {get; private set;} = new List<Tile>();
    private GameObject _attackBar;
    public bool IsAttackBarActive => _attackBar != null;
    [SerializeField] private Canvas _canvas;
    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        _heroes = _units.Where(u=>u.Faction == Faction.Hero).ToList();
    }

    public void SpawnEnemies()
    {
        foreach(ScriptableUnit unit in _units)
        {
            if(unit.Faction == Faction.Enemy)
            {
                var spawnedEnemy = Instantiate(unit.UnitPrefab);
                var spawnTile = GridManager.Instance.GetEnemySpawnTile();
                spawnTile.SetUnit(spawnedEnemy);
                _remainingEnemies.Add(spawnedEnemy);
                _remainingUnits.Add(spawnedEnemy);
            }
        }

        GameManager.Instance.UpdateGameState(GameState.SpawnHeroes);
    }

    public void SpawnHeroes(Tile spawnTile)
    {
        var spawnedHero = Instantiate(_heroes[0].UnitPrefab);
        spawnTile.SetUnit(spawnedHero);
        _remainingHeroes.Add(spawnedHero);
        _remainingUnits.Add(spawnedHero);
    }

    public void SetSelectedHero(BaseHero hero)
    {
        SelectedHero = hero;

        if (hero == null)
        {
            if (_attackBar != null)
            {
                Destroy(_attackBar);
            }
            return;
        }
        if (GameManager.Instance.State == GameState.AttackPhase)
        {
            _attackBar = Instantiate(hero.attackToolBar, _canvas.transform);
            RectTransform rect = _attackBar.GetComponent<RectTransform>();
            int yTransform = SelectedHero.OccupiedTile.gridPos.y < 3 ? 145 : -225; 
            rect.anchoredPosition = new Vector2(0, yTransform);
        }   
        if (GameManager.Instance.State == GameState.MovementPhase)
        {
            ReachableTiles = GridManager.Instance.GetReachableTiles(SelectedHero.OccupiedTile, SelectedHero.moveRange);    
            foreach (Tile tile in ReachableTiles)
            {
                tile._highlight.SetActive(true);
            }    
        }
        
        if(GameManager.Instance.State == GameState.MovementPhase)
        {
            ReachableTiles = GridManager.Instance.GetReachableTiles(SelectedHero.OccupiedTile, SelectedHero.moveRange);    
            foreach (Tile tile in ReachableTiles)
            {
                tile._highlight.SetActive(true);
            }
        }
        else if(GameManager.Instance.State == GameState.AttackPhase)
        {
            var targetList = hero.TargetsList;
            foreach(BaseUnit enemy in targetList)
            {
                enemy.OccupiedTile._highlight.SetActive(true);
            }
        }
    }

    public void DeselectHero()
    {
        foreach (Tile tile in ReachableTiles)
        {
            tile._highlight.SetActive(false);
        }
        ReachableTiles.Clear();
        var targetList = SelectedHero.TargetsList;
        foreach(BaseUnit enemy in targetList)
        {
            enemy.OccupiedTile._highlight.SetActive(false);
        }
        SetSelectedHero(null);
    }

    public void KillUnit(BaseUnit unit)
    {
        if(unit.Faction == Faction.Hero)
        {
            _remainingHeroes.Remove(unit);
            /*if(_remainingHeroes.Count == 0)
                GameManager.Instance.UpdateGameState(GameState.Defeat);
            else
                GameManager.Instance.UpdateGameState(GameState.MovementPhase);*/
        }
        else
        {
            _remainingEnemies.Remove(unit);
            /*if(_remainingEnemies.Count == 0)
                GameManager.Instance.UpdateGameState(GameState.Victory);
            else
                GameManager.Instance.UpdateGameState(GameState.MovementPhase);*/
        }
        _remainingUnits.Remove(unit);
        unit.Alive = false;
        Destroy(unit.gameObject);
    }

    public bool IsVictory()
    {
        return _remainingEnemies.Count == 0;
    }

    public bool IsDefeat()
    {
        return _remainingHeroes.Count == 0;
    }

    public bool InAttackRange(BaseUnit attackingUnit, BaseUnit defendingUnit)
    {
        var horizontalDistance = Mathf.Abs(attackingUnit.transform.position.x - defendingUnit.transform.position.x);
        var verticalDistance = Mathf.Abs(attackingUnit.transform.position.y - defendingUnit.transform.position.y);
        return attackingUnit.AttackRange >= (horizontalDistance + verticalDistance);
    }

    private List<BaseUnit> FindAllAttackTargets(BaseUnit unit)
    {
        var targetsList = new List<BaseUnit>();
        if(unit.Faction == Faction.Hero){
            foreach(BaseUnit enemy in _remainingEnemies)
            {
                if(InAttackRange(unit, enemy))
                    targetsList.Add(enemy);
            }
        }
        else
        {
            foreach(BaseUnit hero in _remainingHeroes)
            {
                if(InAttackRange(unit, hero))
                    targetsList.Add(hero);
            }
            
        }
        return targetsList;
    }

    public void UpdateAllTargetLists()
    {
        foreach(BaseUnit unit in _remainingUnits)
        {
            unit.TargetsList.Clear();
            unit.TargetsList = FindAllAttackTargets(unit);
        }
    }

    public bool SkipAttackPhase()
    {
        foreach(BaseUnit hero in _remainingHeroes)
        {
            foreach(BaseUnit enemy in _remainingEnemies)
            {
                if(InAttackRange(hero, enemy) || InAttackRange(enemy, hero))
                    return false;
            }
        }
        return true;
    }

    public void SetEnemyAttacks()
    {
        foreach(BaseUnit enemy in _remainingEnemies)
        {
            enemy.Action = AttackPhaseAction.Attack;
            if(enemy.TargetsList.Count > 0)
            {
                enemy.Target = enemy.TargetsList.OrderBy(o=>Random.value).First();
            }
        }
    }

    public void ExecuteAllActions()
    {
        foreach(BaseUnit unit in _remainingUnits)
        {
            if(unit.Action == AttackPhaseAction.Attack && unit.Target != null)
            {
                _actionQueue.Enqueue(unit, unit.AttackSpeed);
            }
        }

        while(_actionQueue.Count > 0)
        {
            var unit = _actionQueue.Dequeue();
            if (unit.Alive)
            {
                unit.SelectedAttack.Execute(unit, unit.Target);
                unit.SelectedAttack = null;
            }
        }
    }

    public void ResetAllTargets()
    {
        foreach(BaseUnit unit in _remainingUnits)
        {
            unit.Target = null;
        }
    }

    /*private T GetUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u=>u.Faction == faction).OrderBy(o=>Random.value).First().UnitPrefab;
    }*/
    public List<BaseUnit> GetRemainingHeroes()
    {
        return _remainingHeroes;
    }
}
