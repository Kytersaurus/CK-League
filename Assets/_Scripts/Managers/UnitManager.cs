using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Clrain.Collections;
using TMPro;
using Unity.Plastic.Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    [SerializeField] private List<ScriptableUnit> _units;
    private List<BaseUnit> _remainingHeroes = new List<BaseUnit>();
    private List<BaseUnit> _remainingEnemies = new List<BaseUnit>();
    private List<BaseUnit> _remainingUnits = new List<BaseUnit>();
    private List<BaseHero> _allUnitsUsedInStage = new List<BaseHero>();

    public BaseHero SelectedHero;
    private PriorityQueue<BaseUnit, int> _movementQueue = new PriorityQueue<BaseUnit, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    private PriorityQueue<BaseUnit, int> _actionQueue = new PriorityQueue<BaseUnit, int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    public List<Tile> ReachableTiles {get; private set;} = new List<Tile>();
    private GameObject _attackBar;
    public bool IsAttackBarActive => _attackBar != null;
    [SerializeField] private Canvas _canvas;
    public bool SpecificHeroSpawn, SpecificEnemySpawn;
    public UnitSaveData UnitToSpawn;
    [SerializeField] Toggle _unitSpawnToggle;
    [SerializeField] Transform _spawnPanel;
    [SerializeField] ToggleGroup _unitSpawnToggleGroup;
    [SerializeField] GameObject _spawnPanelObj, _teamSelectToggles;
    private bool _teamIsSelected = false;
    private List<Toggle> _unitSpawnToggles = new List<Toggle>();
    public List<UnitSaveData> _selectedTeamData;
    public Vector2 SpawnBox = new Vector2();
    
    public static event Action<BaseHero> OnExperienceAdded;
    void Awake()
    {
        Instance = this;
        
        // _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        // _heroes = _units.Where(u=>u.Faction == Faction.Hero).ToList();
        
    }

    private void OnEnable()
    {
        BaseUnit.OnUnitDeath += KillUnit;
        BaseUnit.OnUnitAction += DeselectHero;
        BaseUnit.OnDamageTaken += AddExperienceFromDamage;
    }
    private void OnDisable()
    {
        BaseUnit.OnUnitDeath -= KillUnit;
        BaseUnit.OnUnitAction -= DeselectHero;
        BaseUnit.OnDamageTaken -= AddExperienceFromDamage;
    }
    public List<BaseUnit> GetHeroesList()
    {
        return _remainingHeroes;
    }
    public void SpawnEnemies()
    {
        foreach(ScriptableUnit unit in _units)
        {
            if(unit.Faction == Faction.Enemy)
            {
                var spawnedEnemy = Instantiate(unit.UnitPrefab);
                Tile spawnTile;
                if (SpecificEnemySpawn)
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
        foreach (ScriptableUnit unit in _units)
        {
            if (unit.Faction == Faction.Hero)
            {
                var spawnedHero = Instantiate(unit.UnitPrefab);
                Tile spawnTile = GridManager.Instance.GetSpecificSpawnTile(unit.spawnX, unit.spawnY, true);
                spawnTile.SetUnit(spawnedHero);
                _remainingHeroes.Add(spawnedHero);
                _remainingUnits.Add(spawnedHero);
            }
        }
    }
    public void SpawnPanelActive(bool isActive)
    {
        _spawnPanelObj.SetActive(isActive);
        if (isActive)
        {
            GridManager.Instance.SpawnCamPosition();
        }
        else
        {
            GridManager.Instance.DefaultCamPosition();
        }
    }
    public void SelectTeam()
    {
        if (_selectedTeamData == null || _selectedTeamData.Count == 0)
        {
            Debug.Log("Selected team has no units!");
            return;
        }
        _teamIsSelected = true;
        _teamSelectToggles.SetActive(false);
        RefreshTeam();
    }
    public void LoadTeam(int slot)
    {
        TeamManager.Instance.ActiveTeamSlot = slot;
        _selectedTeamData = TeamManager.Instance.LoadTeamData();
        RefreshTeam();
    }
    public void RefreshTeam()
    {
        if (_unitSpawnToggles == null)
        {
            _unitSpawnToggles = new List<Toggle>();
        }
        foreach (Toggle toggle in _unitSpawnToggles)
        {
            Destroy(toggle.gameObject);
        }
        _unitSpawnToggles.Clear();

        if (_selectedTeamData == null || _selectedTeamData.Count == 0)
        {
            return;
        }

        int x = 0, y = 340;
        foreach (UnitSaveData unit in _selectedTeamData)
        {

            Toggle unitSelect = Instantiate(_unitSpawnToggle, _spawnPanel);
            RectTransform rect = unitSelect.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);
            unitSelect.group = _unitSpawnToggleGroup;
            ScriptableUnit scriptUnit = TeamManager.Instance.AllUnitPrefabs.FirstOrDefault(u => u.name == unit.unitName);
            if (scriptUnit == null)
            {
                Debug.LogError($"{unit.guid}'s unitname does not match any prefabs");
            }
            ToggleSelectSpawn toggleScript = unitSelect.GetComponent<ToggleSelectSpawn>();
            toggleScript.SetExistingUnit(unit);

            TMP_Text unitName = unitSelect.GetComponentInChildren<TMP_Text>();
            unitName.text = unit.unitName;

            BaseHero hero = scriptUnit.UnitPrefab as BaseHero;
            Image unitSprite = unitSelect.GetComponentInChildren<Image>();
            unitSprite.sprite = hero.UnitIcon;

            if (!_teamIsSelected)
            {
                unitSelect.interactable = false;
            }
            _unitSpawnToggles.Add(unitSelect);
            y -= 120;
        }
    }
    public void SpawnHero(int x, int y)
    {
        UnitSaveData data = UnitToSpawn;
        ScriptableUnit unit = TeamManager.Instance.AllUnitPrefabs.FirstOrDefault(u => u.name == data.unitName);


        BaseHero spawnedHero = Instantiate(unit.UnitPrefab) as BaseHero;
        spawnedHero.guid = data.guid;
        spawnedHero.unitName = data.unitName;
        spawnedHero.className = data.className;
        spawnedHero.level = data.level;
        spawnedHero.experience = data.experience;

        var attacks = TeamManager.Instance.AllAttacks;
        spawnedHero.moveSet = data.attackNames
            .Select(name => attacks.FirstOrDefault(a => a.attackName == name))
            .Where(a => a != null)
            .ToList();

        Tile spawnTile = GridManager.Instance.GetSpecificSpawnTile(x, y, true);;
        
        spawnTile.SetUnit(spawnedHero);
        _remainingHeroes.Add(spawnedHero);
        _remainingUnits.Add(spawnedHero);
        _selectedTeamData.Remove(UnitToSpawn);
        UnitToSpawn = null;
        RefreshTeam();
        GridManager.Instance.HighlightSpawnTiles(false);
    }
    public void SaveUnitProgress()
    {
        if (_allUnitsUsedInStage == null || _allUnitsUsedInStage.Count == 0)
        {
            return;
        }
        foreach (BaseHero hero in _allUnitsUsedInStage)
        {
            TeamManager.Instance.UpdateUnitData(hero);
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
        SelectedHero.OccupiedTile.highlightSelect.SetActive(true);
        if (GameManager.Instance.State == GameState.AttackPhase)
        {
            _attackBar = Instantiate(hero.attackToolBar, _canvas.transform);
            var attackBarScript = _attackBar.GetComponent<AttackToolBarScript>();
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
        if(SelectedHero == null)
        {
            return;
        }
        SelectedHero.OccupiedTile.highlightSelect.SetActive(false);
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
        }
        else
        {
            _remainingEnemies.Remove(unit);
            AddExperienceFromKill(unit);
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
            if (unit.TargetsList.Count != 0 && unit is BaseHero)
            {
                unit.AttackIndicator.gameObject.SetActive(true);
            }
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
        int difficulty = GameManager.Instance.EnemyDifficulty;
        Dictionary<BaseUnit, int> potentialAttacksReceived = new Dictionary<BaseUnit, int>();
        foreach(BaseUnit hero in _remainingHeroes)
        {
            potentialAttacksReceived[hero] = 0;
            foreach(BaseUnit enemy in _remainingEnemies)
            {
                if(InAttackRange(enemy, hero))
                {
                    potentialAttacksReceived[hero]++;
                }
            }
        }
        foreach(BaseUnit enemy in _remainingEnemies)
        {
            enemy.Action = AttackPhaseAction.Attack;
            if(difficulty == 1)
            {
                enemy.SelectedAttack = enemy.moveSet.OrderBy(o=>UnityEngine.Random.value).First();
                if(enemy.TargetsList.Count > 0)
                {
                    enemy.Target = enemy.TargetsList.OrderBy(o=>UnityEngine.Random.value).First();
                }                
            }
            else if(difficulty == 2)
            {
                enemy.SelectedAttack = enemy.moveSet.OrderBy(o=>o.damage).First();
                if(enemy.TargetsList.Count > 0)
                {
                    enemy.Target = enemy.TargetsList.OrderByDescending(o=>o.CurrentHealth).First();
                }                
            }
            else if(difficulty >= 3)
            {
                enemy.SelectedAttack = enemy.moveSet.OrderBy(o=>o.damage).First();
                if(enemy.TargetsList.Count > 0)
                {
                    int greatestPotentialAttacks = 0;
                    foreach(BaseUnit hero in enemy.TargetsList)
                    {
                        if(potentialAttacksReceived[hero] >= greatestPotentialAttacks)
                        {
                            enemy.Target = hero;
                            greatestPotentialAttacks = potentialAttacksReceived[hero];
                        }
                    }
                }
            }
        }
    }

    public void ExecuteAllActions()
    {
        foreach(BaseUnit unit in _remainingUnits)
        {
            if(unit.Action == AttackPhaseAction.Attack && unit.SelectedAttack != null)
            {
                if (unit.SelectedAttack is Heals || unit.SelectedAttack is Mitigate || unit.Target != null)
                {
                    _actionQueue.Enqueue(unit, unit.AttackSpeed);
                }
            }
        }

        while(_actionQueue.Count > 0)
        {
            var unit = _actionQueue.Dequeue();
            if (unit.Alive && unit.SelectedAttack != null)
            {
                if (unit.Target != null)
                {
                    unit.Target.attackedBy = unit;
                }
                unit.SelectedAttack.Execute(unit, unit.Target);
                unit.SelectedAttack = null;
            }
            unit.AttackIndicator.gameObject.SetActive(false);
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
            if(unit.SelectedAttack == null)
            {
                return false;
            }
            if (!(unit.SelectedAttack is Heals) || !(unit.SelectedAttack is Mitigate) && unit.Target == null)
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

    /*public BaseUnit FindClosestTarget(BaseUnit unit)
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
    }*/

    public void SetEnemyMovement()
    {
        foreach(BaseUnit enemy in _remainingEnemies)
        {
            //BaseHero target = (BaseHero)FindClosestTarget(enemy);
            Tile closestTileToTarget = GridManager.Instance.GetEnemyPath(enemy, _remainingHeroes);
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

    public void AddExperienceFromDamage(BaseUnit attackedUnit, int damage)
    {
        if(attackedUnit.attackedBy.Faction == Faction.Hero)
        {
            var multiplier = 1.0f;
            var hero = (BaseHero)attackedUnit.attackedBy;
            hero.experience += (int)(damage*multiplier);
            OnExperienceAdded?.Invoke(hero);
        }
    }
    public void AddExperienceFromKill(BaseUnit attackedUnit)
    {
        if(attackedUnit.attackedBy.Faction == Faction.Hero)
        {
            var experience = 10;
            var multiplier = 1.0f;
            var hero = (BaseHero)attackedUnit.attackedBy;
            hero.experience += (int)(experience*multiplier);
            OnExperienceAdded?.Invoke(hero);
        }
    }
}
