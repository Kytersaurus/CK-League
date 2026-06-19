using UnityEngine;

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
    public GameObject _highlight;
    public GameObject _highlightSelect;
    public GameObject _highlightError;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private int _movementCost;
    [SerializeField] protected TileType _tileType;
    [SerializeField] protected TileVariant _tileVariant;
    public Vector2 gridPos {get; private set;}
    public TileType TileType => _tileType;
    public TileVariant TileVariant => _tileVariant;
    public BaseUnit OccupiedUnit;
    public bool Walkable => OccupiedUnit == null && _isWalkable == true;
    public int MoveCost => _movementCost;
    public virtual void Init(int x, int y)
    {
        gridPos = new Vector2(x, y);
        _baseRenderer.color = (x + y) % 2 == 1 ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {   
        if (GameManager.Instance.State == GameState.SpawnHeroes)
        {
            if (Walkable)
            {
                _highlightSelect.SetActive(true);
            }
            else
            {
                _highlightError.SetActive(true);
            }
        }
        else if (GameManager.Instance.State == GameState.MovementPhase)
        {
            if (OccupiedUnit != null && UnitManager.Instance.SelectedHero == null && OccupiedUnit.Faction == Faction.Hero)
            {
                _highlightSelect.SetActive(true);
            }
            else if (UnitManager.Instance.SelectedHero != null && UnitManager.Instance.ReachableTiles.Contains(this))
            {
                _highlightSelect.SetActive(true);
            }
            else
            {
                _highlightError.SetActive(true);    
            }
        }
        else if (GameManager.Instance.State == GameState.AttackPhase)
        {
            if (OccupiedUnit != null && UnitManager.Instance.SelectedHero != null && OccupiedUnit.Faction == Faction.Enemy)
            {
                _highlightSelect.SetActive(true);    
            }
            else if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero && UnitManager.Instance.SelectedHero == null)
            {
                _highlightSelect.SetActive(true);    
            }
            else
            {
                _highlightError.SetActive(true);
            }
        }
        else
        {
            return;
        }
    }
    void OnMouseExit()
    {
        if (_highlightSelect.activeSelf)
        {
            _highlightSelect.SetActive(false);
        }
        else if (_highlightError.activeSelf)
        {
            _highlightError.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if(GameManager.Instance.State == GameState.SpawnHeroes && OccupiedUnit == null && Walkable)
        {
            UnitManager.Instance.SpawnHeroes(this);
            GameManager.Instance.UpdateGameState(GameState.MovementPhase);
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
                SetUnit(UnitManager.Instance.SelectedHero);
                UnitManager.Instance.SetSelectedHero(null);
                if(!UnitManager.Instance.SkipAttackPhase())
                    GameManager.Instance.UpdateGameState(GameState.AttackPhase);
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
                UnitManager.Instance.SelectedHero.SelectedAttack.Execute(UnitManager.Instance.SelectedHero, OccupiedUnit);
                UnitManager.Instance.SelectedHero.SelectedAttack = null;
                if(OccupiedUnit.CurrentHealth <= 0)
                {
                    UnitManager.Instance.KillUnit(OccupiedUnit);
                    //Destroy(OccupiedUnit.gameObject);
                }
                else
                {
                    GameManager.Instance.UpdateGameState(GameState.MovementPhase);
                }
                UnitManager.Instance.SetSelectedHero(null);
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
