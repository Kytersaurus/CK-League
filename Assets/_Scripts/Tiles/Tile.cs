using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable = true;

    public BaseUnit OccupiedUnit;
    public bool Walkable => OccupiedUnit == null && _isWalkable == true;
    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    void OnMouseDown()
    {
        if(GameManager.Instance.State == GameState.SpawnHeroes && OccupiedUnit == null)
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
