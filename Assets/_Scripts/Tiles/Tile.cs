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
    public Vector2 GridPos {get; private set;}
    public TileType TileType => _tileType;
    public TileVariant TileVariant => _tileVariant;
    public BaseUnit OccupiedUnit;
    public bool Walkable => OccupiedUnit == null && _isWalkable == true;
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
        if (MenuManager.Instance != null && MenuManager.Instance.popUpActive)
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
            if (Walkable || (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero))
            {
                highlightSelect.SetActive(true);
            }
            else
            {
                highlightError.SetActive(true);
            }
        }
        else if (GameManager.Instance.State == GameState.MovementPhase)
        {
            if (OccupiedUnit != null && UnitManager.Instance.SelectedHero == null && OccupiedUnit.Faction == Faction.Hero)
            {
                highlightSelect.SetActive(true);
            }
            else if (UnitManager.Instance.SelectedHero != null && UnitManager.Instance.ReachableTiles.Contains(this))
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
            if (OccupiedUnit != null && UnitManager.Instance.SelectedHero != null && OccupiedUnit.Faction == Faction.Enemy)
            {
                highlightSelect.SetActive(true);
            }
            else if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero && UnitManager.Instance.SelectedHero == null)
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
        if (MenuManager.Instance != null && MenuManager.Instance.popUpActive)
        {
            return;
        }
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
        
        if(GameManager.Instance.State == GameState.SpawnHeroes && (Walkable || OccupiedUnit.Faction == Faction.Hero))
        {
            if(UnitManager.Instance.SelectedHero != null)
            {
                if(OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero)
                {
                    UnitManager.Instance.SelectedHero.OccupiedTile.SetUnit(OccupiedUnit);
                }
                SetUnit(UnitManager.Instance.SelectedHero);
                UnitManager.Instance.DeselectHero();
            }
            else if(UnitManager.Instance.SelectedHero == null && OccupiedUnit != null)
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
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
            if(OccupiedUnit.Faction == Faction.Hero)
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
            }
            else if(UnitManager.Instance.SelectedHero != null && UnitManager.Instance.InAttackRange(UnitManager.Instance.SelectedHero, OccupiedUnit) && UnitManager.Instance.SelectedHero.SelectedAttack != null)
            {
                UnitManager.Instance.SelectedHero.Action = AttackPhaseAction.Attack;
                UnitManager.Instance.SelectedHero.SetTarget(OccupiedUnit);
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
    }
}
