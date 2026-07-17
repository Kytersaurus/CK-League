using UnityEngine;
using UnityEngine.EventSystems;

public enum TileType
{
    Grass,
    Water,
    Marsh,
    Mountain,
    Tree
}

public enum TileVariant
{
    Body1,
    Body2,
    EdgeTL,
    EdgeTR,
    EdgeBL,
    EdgeBR,
    Pillar,
    PillarBase
}

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _baseRenderer;
    public GameObject highlight;
    public GameObject highlightSelect;
    public GameObject highlightError;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private int _movementCost;
    [SerializeField] protected TileType _tileType;
    [SerializeField] protected TileVariant _tileVariant;
    public GameObject Objective;
    public Vector2 GridPos {get; private set;}
    public TileType TileType => _tileType;
    public TileVariant TileVariant => _tileVariant;
    public BaseUnit OccupiedUnit;
    public bool Walkable => OccupiedUnit == null && _isWalkable == true && Objective.activeSelf == false;
    public int MoveCost => _movementCost;
    public bool IsOffset {get; private set;}
    public virtual void Init(int x, int y)
    {
        GridPos = new Vector2(x, y);
        IsOffset = (x + y) % 2 == 1; 
        _baseRenderer.color = IsOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {   
        if (MenuManager.Instance._pauseMenu.activeSelf)
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        #if UNITY_EDITOR
        if (LevelEditor.Instance != null && LevelEditor.Instance.EditingMode)
        {
            highlightSelect.SetActive(true);
            return;
        }
        #endif

        if (GameManager.Instance == null)
        {
            return;
        }
        
        if (GameManager.Instance.State == GameState.SpawnHeroes)
        {
            if (UnitManager.Instance.UnitToSpawn != null && Walkable && GridManager.Instance.SpawnTiles.Contains(this))
            {
                highlightSelect.SetActive(true);
            }
            if (OccupiedUnit != null && UnitManager.Instance.SelectedHero == null && OccupiedUnit.Faction == Faction.Hero && GridManager.Instance.SpawnTiles.Contains(this))
            {
                highlightSelect.SetActive(true);
            }
            else if (UnitManager.Instance.SelectedHero != null && GridManager.Instance.SpawnTiles.Contains(this))
            {
                highlightSelect.SetActive(true);
            }
            else if (!GridManager.Instance.SpawnTiles.Contains(this))
            {
                highlightError.SetActive(true);    
            }
        }
        if (GameManager.Instance.State == GameState.MovementPhase)
        {
            if (OccupiedUnit != null && UnitManager.Instance.SelectedHero == null && OccupiedUnit.Faction == Faction.Hero)
            {
                highlightSelect.SetActive(true);
            }
            else if (UnitManager.Instance.SelectedHero != null && Walkable && UnitManager.Instance.ReachableTiles.Contains(this))
            {
                highlightSelect.SetActive(true);
            }
            else
            {
                highlightError.SetActive(true);    
            }
        }
        else if (GameManager.Instance.State == GameState.AttackPhase)
        {
            if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero && UnitManager.Instance.SelectedHero == null && OccupiedUnit.TargetsList.Count != 0)
            {
                highlightSelect.SetActive(true);
                return;
            }
            else if (OccupiedUnit != null && UnitManager.Instance.SelectedHero != null && OccupiedUnit.Faction == Faction.Enemy && UnitManager.Instance.SelectedHero.SelectedAttack != null && !(UnitManager.Instance.SelectedHero.SelectedAttack is Heals || UnitManager.Instance.SelectedHero.SelectedAttack is Mitigate))
            {
                highlightSelect.SetActive(true);
            }
            else
            {
                highlightError.SetActive(true);
            }
            
        }
        else
        {
            return;
        }
    }
    void OnMouseExit()
    {
        if (highlightSelect.activeSelf)
        {
            highlightSelect.SetActive(false);
        }
        else if (highlightError.activeSelf)
        {
            highlightError.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        #if UNITY_EDITOR
        //For level editor
        if (LevelEditor.Instance != null && LevelEditor.Instance.EditingMode)
        {
            var pos = new Vector2(transform.position.x, transform.position.y);
            GridManager.Instance.PlaceTile(pos, LevelEditor.Instance.selectedType, LevelEditor.Instance.selectedVariant);
            return;
        }
        #endif
        
        if(GameManager.Instance.State == GameState.SpawnHeroes && Walkable && GridManager.Instance.SpawnTiles.Contains(this))
        {
            if (UnitManager.Instance.UnitToSpawn != null)
            {
                UnitManager.Instance.SpawnHero((int)GridPos.x, (int)GridPos.y);
            }
            if (UnitManager.Instance.UnitToSpawn == null && UnitManager.Instance.SelectedHero == null)
            {
                UnitManager.Instance.SelectedHero = (BaseHero)OccupiedUnit;
            }
            if (UnitManager.Instance.UnitToSpawn == null && UnitManager.Instance.SelectedHero != null)
            {
                SetUnit(UnitManager.Instance.SelectedHero);
            }
        }
        //Movement Phase
        else if(GameManager.Instance.State == GameState.MovementPhase)
        {
            if(UnitManager.Instance.SelectedHero == null && OccupiedUnit is BaseHero hero)
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
            }
            else if(UnitManager.Instance.SelectedHero != null && OccupiedUnit == null && UnitManager.Instance.ReachableTiles.Contains(this))
            {
                UnitManager.Instance.SelectedHero.hasMoved = true;
                GridManager.Instance.ShowUnitDest(UnitManager.Instance.SelectedHero, false);
                UnitManager.Instance.SelectedHero.SetDestination(this);
                //UnitManager.Instance.SelectedHero.ConstructPath(this);
                if (UnitManager.Instance.AllMovementsSelected())
                {
                    EndTurnButton.Instance.ActivateEndTurnButton();
                }
                /*SetUnit(UnitManager.Instance.SelectedHero);
                UnitManager.Instance.DeselectHero();
                if(!UnitManager.Instance.SkipAttackPhase())
                    GameManager.Instance.UpdateGameState(GameState.AttackPhase);*/
            }
        }
        //Attack Phase
        else if(GameManager.Instance.State == GameState.AttackPhase && OccupiedUnit != null)
        {
            if(OccupiedUnit.Faction == Faction.Hero && OccupiedUnit.TargetsList.Count != 0)
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
            }
            else if(UnitManager.Instance.SelectedHero != null && UnitManager.Instance.InAttackRange(UnitManager.Instance.SelectedHero, OccupiedUnit) && UnitManager.Instance.SelectedHero.SelectedAttack != null)
            {
                UnitManager.Instance.SelectedHero.Action = AttackPhaseAction.Attack;
                if (UnitManager.Instance.SelectedHero.SelectedAttack is Heals || UnitManager.Instance.SelectedHero.SelectedAttack is Mitigate)
                {
                    return;   
                }
                else
                {
                    UnitManager.Instance.SelectedHero.SetTarget(OccupiedUnit);  
                }
                if (UnitManager.Instance.AllAttacksSelected())
                {
                    EndTurnButton.Instance.ActivateEndTurnButton();
                }
            }
        }
    }

    public void SetUnit(BaseUnit unit)
    {
        if(unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
        GridManager.Instance.ShowUnitDest(unit, false);
    }
}
