using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _showPhaseObject;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private Transform _WorldSpaceCanvas;
    [SerializeField] private GameObject _floatingDamagePrefab;
    [SerializeField] private Vector2 _floatingDamageIntialPos;
    [SerializeField] private TextMeshProUGUI _heroLevelUp;
    public GameObject _pauseMenu;
    void Awake()
    {
        Instance = this;
    }

    public void ShowGamePhase(GameState currentState)
    {
        string phaseName;
        if(currentState == GameState.SpawnHeroes)
        {
            phaseName = "Spawn Heroes Phase";
        }
        else if(currentState == GameState.MovementPhase)
        {
            phaseName = "Movement Phase";
        }
        else if(currentState == GameState.AttackPhase)
        {
            phaseName = "Attack Phase";
        }
        else if(currentState == GameState.Victory)
        {
            phaseName = "Victory";
            _victoryPanel.SetActive(true);
            ProgressManager.Instance.LevelComplete();
        }
        else if(currentState == GameState.Defeat)
        {
            phaseName = "Defeat";
            _defeatPanel.SetActive(true);
        }
        else
        {
            _showPhaseObject.SetActive(false);
            return;
        }

        _showPhaseObject.GetComponentInChildren<TextMeshProUGUI>().text = phaseName;
        _showPhaseObject.SetActive(true);
    }
    
    public void ShowPauseMenu(bool show)
    {
        _pauseMenu.SetActive(show);
    }
    public void ShowHeroLeveledUp(List<string> units, bool show)
    {
        _heroLevelUp.gameObject.SetActive(show);
        if (!show)
        {
            return;
        }
        string text = "";
        foreach (string unit in units)
        {
            text += unit + " leveled up\n";
        }
        text += "Go to the team editor to evolve into new classes";
        _heroLevelUp.text = text;
    }
    public void SpawnDamageIndicator(string message, Vector3 positon, bool blocked, bool heal)
    {
        Vector3 DmgObjPos = positon + (Vector3) _floatingDamageIntialPos;
        GameObject DmgObj = Instantiate(_floatingDamagePrefab, DmgObjPos , Quaternion.identity, _WorldSpaceCanvas);
        DamageIndicator DmgObjScript = DmgObj.GetComponent<DamageIndicator>();
        DmgObjScript.SetupMessage(message, blocked, heal);
    }
    void Update()
    {
        if (UnitManager.Instance.SelectedHero == null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ShowPauseMenu(!_pauseMenu.activeSelf);
        }
        if (GameManager.Instance.State == GameState.SpawnHeroes)
        {
            if (UnitManager.Instance.GetHeroesList().Count > 0)
            {
                EndTurnButton.Instance.ActivateEndTurnButton();
            }
            else
            {
                EndTurnButton.Instance.DeactivateEndTurnButton();
            }
        }
        if ((GameManager.Instance.State == GameState.AttackPhase || GameManager.Instance.State == GameState.MovementPhase) && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
           EndTurnButton.Instance.EndTurn();
        }
        if (UnitManager.Instance.SelectedHero != null && Keyboard.current.escapeKey.wasPressedThisFrame )
        {
            // if (UnitManager.Instance.SelectedHero != null && UnitManager.Instance.SelectedHero.OccupiedTile != null)
            // {
            //     UnitManager.Instance.SelectedHero.OccupiedTile.highlightSelect.SetActive(false);
            // }
            UnitManager.Instance.SetSelectedHero(null);
        }
    }
}
