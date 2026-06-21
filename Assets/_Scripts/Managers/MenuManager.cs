using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _showPhaseObject;
    public GameObject _pauseMenu;

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
        }
        else if(currentState == GameState.Defeat)
        {
            phaseName = "Defeat";
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
        if (!UnitManager.Instance.IsAttackBarActive && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!_pauseMenu.activeSelf)
            {
                _pauseMenu.SetActive(true);    
            } 
            else
            {
                _pauseMenu.SetActive(false);
            }
        }
    }
}
