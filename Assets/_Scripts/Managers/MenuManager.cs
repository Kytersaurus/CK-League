using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _showPhaseObject;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;
    public GameObject _pauseMenu;
    public bool popUpActive;

    void Awake()
    {
        Instance = this;
    }

    public void ShowGamePhase(GameState currentState)
    {
        string phaseName;
        if(currentState == GameState.MovementPhase)
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
    void Update()
    {
        if (UnitManager.Instance.SelectedHero == null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!_pauseMenu.activeSelf)
            {
                _pauseMenu.SetActive(true);
                popUpActive = true;
            } 
            else
            {
                _pauseMenu.SetActive(false);
                popUpActive = false;
            }
        }
        if (GameManager.Instance.State == GameState.AttackPhase || GameManager.Instance.State == GameState.MovementPhase)
        {
            if (!popUpActive && (UnitManager.Instance.AllAttacksSelected() || GameManager.Instance.State == GameState.MovementPhase))
            {
                EndTurnButton.Instance.ActivateEndTurnButton();
            }
            else
            {
                EndTurnButton.Instance.DeactivateEndTurnButton();
            }
        }
        if (UnitManager.Instance.SelectedHero != null && Keyboard.current.escapeKey.wasPressedThisFrame )
        {
            UnitManager.Instance.SetSelectedHero(null);
        }
    }
}
