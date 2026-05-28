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
        if(GameManager.Instance.State == GameState.SpawnHeroes && OccupiedUnit == null)
        {
            UnitManager.Instance.SpawnHeroes(this);
            GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
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
