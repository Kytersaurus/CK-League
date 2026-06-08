using UnityEditor.Experimental.GraphView;
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
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _highlightError;
    [SerializeField] private bool _isWalkable;
    [SerializeField] private int _movementCost;
    [SerializeField] protected TileType _tileType;
    [SerializeField] protected TileVariant _tileVariant;
    public Vector2 gridPos {get; private set;}
    
    public TileType TileType => _tileType;
    public TileVariant TileVariant => _tileVariant;
    public BaseUnit OccupiedUnit;
    public bool Walkable => OccupiedUnit == null && _isWalkable == true;
    public int moveCost => _movementCost;
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
                _highlight.SetActive(true);
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
                _highlight.SetActive(true);
            }
            else if ((Walkable || (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero)) && UnitManager.Instance.SelectedHero != null)
            {
                _highlight.SetActive(true);    
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
                _highlight.SetActive(true);    
            }
            else if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Hero)
            {
                _highlight.SetActive(true);    
            }
            else
            {
                _highlightError.SetActive(true);
            }
        }
    }
    void OnMouseExit()
    {
        if(_highlight.activeSelf)
        {
            _highlight.SetActive(false);
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
        else if(GameManager.Instance.State == GameState.MovementPhase)
        {
            if(UnitManager.Instance.SelectedHero == null && OccupiedUnit is BaseHero hero)
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
            }
            else if(UnitManager.Instance.SelectedHero != null && OccupiedUnit == null)
            {
                SetUnit(UnitManager.Instance.SelectedHero);
                UnitManager.Instance.SetSelectedHero((BaseHero)null);
                GameManager.Instance.UpdateGameState(GameState.AttackPhase);
            }
        }
        else if(GameManager.Instance.State == GameState.AttackPhase && OccupiedUnit != null)
        {
            if(OccupiedUnit.Faction == Faction.Hero)
            {
                UnitManager.Instance.SelectedHero = (BaseHero)OccupiedUnit;
            }
            else if(UnitManager.Instance.SelectedHero != null)
            {
                UnitManager.Instance.TakeDamage(OccupiedUnit);
                if(OccupiedUnit.CurrentHealth <= 0)
                {
                    Destroy(OccupiedUnit.gameObject);
                    GameManager.Instance.UpdateGameState(GameState.Victory);
                }
                else
                {
                    UnitManager.Instance.SetSelectedHero((BaseHero)null);
                    GameManager.Instance.UpdateGameState(GameState.MovementPhase);
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
