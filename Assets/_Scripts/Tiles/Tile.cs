using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    public BaseUnit OccupiedUnit;
    public bool Walkable => OccupiedUnit == null;
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
        //Select Unit Spawn Location
        if(GameManager.Instance.State == GameState.SpawnHeroes && OccupiedUnit == null)
        {
            UnitManager.Instance.SpawnHeroes(this);
            GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
        //Movement Phase
        else if(GameManager.Instance.State == GameState.MovementPhase)
        {
            if(UnitManager.Instance.SelectedHero == null && OccupiedUnit != null)
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
        //Attack Phase
        else if(GameManager.Instance.State == GameState.AttackPhase && OccupiedUnit != null)
        {
            if(OccupiedUnit.Faction == Faction.Hero)
            {
                UnitManager.Instance.SelectedHero = (BaseHero)OccupiedUnit;
            }
            else if(UnitManager.Instance.SelectedHero != null && InAttackRange(UnitManager.Instance.SelectedHero))
            {
                OccupiedUnit.TakeDamage(20);
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

    public bool InAttackRange(BaseUnit unit)
    {
        var horizontalDistance = Mathf.Abs(unit.transform.position.x - transform.position.x);
        var verticalDistance = Mathf.Abs(unit.transform.position.y - transform.position.y);
        return unit.AttackRange >= (horizontalDistance + verticalDistance);
    }
}
