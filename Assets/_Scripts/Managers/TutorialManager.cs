using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject _panel1, _panel2, _panel3, _panel4, _panel5, _panel6;
    private bool _setup, _passagedCrossed, _toolBarTut, _attackTut, _targetTut;
    private BaseUnit _hero, _enemy;
    private Tile _obj1;
    void Start()
    {
        _panel1.SetActive(true);
        _setup = false;
        _passagedCrossed = false;
        _toolBarTut = false;
        _targetTut = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_setup)
        {
            _hero = UnitManager.Instance.GetRemainingHeroes()[0];
            _obj1 = GridManager.Instance.GetTileAtPosition(new Vector2(6, 5));
            _obj1.highlight.SetActive(true);
            _setup = true;
        }
        else if (_panel1.activeSelf && _hero.OccupiedTile.GridPos.x >= 6 && _hero.OccupiedTile.GridPos.y <= 6)
        {
            _panel1.SetActive(false);
            _obj1.highlight.SetActive(false);
            _panel2.SetActive(true);
        }
        else if (!_passagedCrossed && _hero.OccupiedTile.GridPos.x >= 9)
        {
            _panel2.SetActive(false);
            _panel3.SetActive(true);
            _passagedCrossed = true;
        }
        else if (!_toolBarTut && UnitManager.Instance.IsAttackBarActive)
        {
            _panel3.SetActive(false);
            _panel4.SetActive(true);
            _toolBarTut = true;
        }
        else if (!_attackTut && _hero.SelectedAttack != null)
        {
            _panel4.SetActive(false);
            _panel5.SetActive(true);
            _attackTut = true;
        }
        else if (_attackTut && !_targetTut && GameManager.Instance.State == GameState.MovementPhase)
        {
            _panel5.SetActive(false);
            _panel6.SetActive(true);
            _targetTut = true;
        }
        if (UnitManager.Instance.IsAttackBarActive)
        {
            AttackToolBarScript.Instance.SetButtonsActive(false);
        }
    }
}
