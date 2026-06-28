using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Clrain.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    [SerializeField] private List<ScriptableUnit> _units;
    private List<BaseUnit> _remainingHeroes = new List<BaseUnit>();
    private List<BaseUnit> _remainingEnemies = new List<BaseUnit>();
    private List<BaseUnit> _remainingUnits = new List<BaseUnit>();

    public BaseHero SelectedHero;
    private PriorityQueue<BaseUnit, int> _movementQueue = new PriorityQueue<BaseUnit, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    private PriorityQueue<BaseUnit, int> _actionQueue = new PriorityQueue<BaseUnit, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    public List<Tile> ReachableTiles {get; private set;} = new List<Tile>();
    private GameObject _attackBar;
    public bool IsAttackBarActive => _attackBar != null;
    [SerializeField] private Canvas _canvas;
    public bool SpecificSpawn;
    void Awake()
    {
        Instance = this;
        
        // _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        // _heroes = _units.Where(u=>u.Faction == Faction.Hero).ToList();
        
    }

    private void OnEnable()
    {
        BaseUnit.OnUnitDeath += KillUnit;
    }
    private void OnDisable()
    {
        BaseUnit.OnUnitDeath -= KillUnit;
    }
    public void SpawnEnemies()
    {
        foreach(ScriptableUnit unit in _units)
        {
            if(unit.Faction == Faction.Enemy)
            {
                var spawnedEnemy = Instantiate(unit.UnitPrefab);
                Tile spawnTile;
                if (SpecificSpawn)
                {
                    spawnTile = GridManager.Instance.GetSpecificSpawnTile(unit.spawnX, unit.spawnY, false);
                }
                else
                {
                    spawnTile = GridManager.Instance.GetEnemySpawnTile();;
                }
                spawnTile.SetUnit(spawnedEnemy);
                _remainingEnemies.Add(spawnedEnemy);
                _remainingUnits.Add(spawnedEnemy);
            }
        }

        GameManager.Instance.UpdateGameState(GameState.SpawnHeroes);
    }

    public void SpawnHeroes()
    {
        foreach(ScriptableUnit unit in _units)
        {
            if(unit.Faction == Faction.Hero)
            {
                var spawnedHero = Instantiate(unit.UnitPrefab);
                Tile spawnTile;
                if (SpecificSpawn)
                {
                    spawnTile = GridManager.Instance.GetSpecificSpawnTile(unit.spawnX, unit.spawnY, true);
                }
                else
                {
                    spawnTile = GridManager.Instance.GetHeroSpawnTile();
                }
                spawnTile.SetUnit(spawnedHero);
                _remainingHeroes.Add(spawnedHero);
                _remainingUnits.Add(spawnedHero);
            }
        }
    }

    public void SetSelectedHero(BaseHero hero)
    {
        if (SelectedHero != null)
        {
            DeselectHero();
        }
        SelectedHero = hero;
        if (_attackBar != null)
        {
            Destroy(_attackBar);
        }
        if (hero == null)
        {
            return;
        }
        if (GameManager.Instance.State == GameState.AttackPhase)
        {
            _attackBar = Instantiate(hero.attackToolBar, _canvas.transform);
            var attackBarScript = _attackBar.GetComponent<AttackToolBarScript>();
            attackBarScript.flipped = hero.OccupiedTile.GridPos.y < 3;
            attackBarScript.Refresh();
        }   
        /*if (GameManager.Instance.State == GameState.MovementPhase)
        {
            ReachableTiles = GridManager.Instance.GetReachableTiles(SelectedHero.OccupiedTile, SelectedHero.moveRange);    
            foreach (Tile tile in ReachableTiles)
            {
                tile.highlight.SetActive(true);
            }    
        }*/
        
        if(GameManager.Instance.State == GameState.MovementPhase)
        {
            ReachableTiles = GridManager.Instance.GetReachableTiles(SelectedHero, SelectedHero.moveRange);
            foreach (Tile tile in ReachableTiles)
            {
                tile.highlight.SetActive(true);
            }
        }
        else if(GameManager.Instance.State == GameState.AttackPhase)
        {
            var targetList = hero.TargetsList;
            foreach(BaseUnit enemy in targetList)
            {
                enemy.OccupiedTile.highlight.SetActive(true);
            }
        }
    }

    public void DeselectHero()
    {
        foreach (Tile tile in ReachableTiles)
        {
            tile.highlight.SetActive(false);
        }
        ReachableTiles.Clear();
        var targetList = SelectedHero.TargetsList;
        foreach(BaseUnit enemy in targetList)
        {
            enemy.OccupiedTile.highlight.SetActive(false);
        }
        SelectedHero = null;
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
            enemy.SelectedAttack = enemy.moveSet.OrderBy(o=>Random.value).First();
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
            if (unit.Alive && unit.SelectedAttack != null)
            {
                unit.SelectedAttack.Execute(unit, unit.Target);
                unit.Target.attackedBy = unit;
                unit.SelectedAttack = null;
            }
        }
        foreach (BaseUnit unit in _remainingUnits)
        {
            if (unit.counterAtk)
            {
                unit.attackedBy.TakeDamage(unit.counterAtkDmg);
                unit.counterAtk = false;
                unit.attackedBy = null;
            }
        }
        SetSelectedHero(null);
    }

    public bool AllAttacksSelected()
    {
        foreach(BaseUnit unit in _remainingUnits)
        {
            if(unit.TargetsList.Count != 0 && (unit.Target == null || unit.SelectedAttack == null))
            {
                return false;
            }
        }
        return true;
    }

    public void ResetAllTargets()
    {
        foreach(BaseUnit unit in _remainingUnits)
        {
            unit.Target = null;
        }
    }
    /*public void FillMovementQueue()
    {
        var sortedRemainingUnits = _remainingUnits.OrderBy(n=>n.moveRange).ToList();
        foreach(BaseUnit unit in sortedRemainingUnits)
        {
            int tempMoveRange = unit.moveRange;
            while(tempMoveRange > 0)
            {
                _movementQueue.Enqueue(unit, tempMoveRange);
                tempMoveRange--;
            }
        }
    }*/

    public BaseUnit FindClosestTarget(BaseUnit unit)
    {
        BaseUnit target = null;
        var shortestDistance = float.MaxValue;
        if(unit.Faction == Faction.Enemy)
        {
            foreach(BaseHero hero in _remainingHeroes)
            {
                var distance = Vector2.Distance(unit.OccupiedTile.transform.position, hero.OccupiedTile.transform.position);
                if(target == null || distance < shortestDistance)
                {
                    target = hero;
                    shortestDistance = distance;
                }
            }
        }
        return target;
    }

    public void SetEnemyMovement()
    {
        foreach(BaseUnit enemy in _remainingEnemies)
        {
            BaseHero target = (BaseHero)FindClosestTarget(enemy);
            Tile closestTileToTarget = GridManager.Instance.GetEnemyPath(enemy, target);
            ReachableTiles = GridManager.Instance.GetReachableTiles(enemy, enemy.moveRange);
            /*float closestDistance = Vector2.Distance(enemy.OccupiedTile.transform.position, target.OccupiedTile.transform.position);
            ReachableTiles = GridManager.Instance.GetReachableTiles(enemy, enemy.moveRange);
            foreach(Tile tile in ReachableTiles)
            {
                var distance = Vector2.Distance(tile.transform.position, target.OccupiedTile.transform.position);
                if(distance < closestDistance)
                {
                    closestTileToTarget = tile;
                    closestDistance = distance;
                }
            }*/
            enemy.SetDestination(closestTileToTarget);
        }
    }

    public void ExecuteAllMovements()
    {
        var unitList = _remainingUnits.OrderByDescending(n=>n.moveRange).ToList();
        foreach(BaseUnit unit in unitList)
        {
            if(unit.DestinationTile != null)
            {
                Tile nextTile;
                while(unit.Path.Count != 0)
                {
                    nextTile = unit.Path.Dequeue();
                    if(nextTile.OccupiedUnit == null)
                    {
                        nextTile.SetUnit(unit);
                    }
                    else
                    {
                        unit.Path.Clear();
                    }
                }
            }
        }
        SetSelectedHero(null);
    }

    public bool AllMovementsSelected()
    {
        foreach(BaseUnit unit in _remainingHeroes)
        {
            if(unit.DestinationTile == null)
            {
                return false;
            }
        }
        return true;
    }

    /*private T GetUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u=>u.Faction == faction).OrderBy(o=>Random.value).First().UnitPrefab;
    }*/
    public List<BaseUnit> GetRemainingHeroes()
    {
        return _remainingHeroes;
    }
    
    public void ResetMovedState()
    {
        foreach (BaseUnit unit in _remainingUnits)
        {
            unit.hasMoved = false;
        }
    }
}
