using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    public static EndTurnButton Instance;

    [SerializeField] private Button _endTurnButton;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _endTurnButton.gameObject.SetActive(true);
        DeactivateEndTurnButton();
        _endTurnButton.onClick.AddListener(EndTurn);
    }

    private void OnDestroy()
    {
        _endTurnButton.onClick.RemoveListener(EndTurn);
    }

    public void EndTurn()
    {
        if(GameManager.Instance.State == GameState.SpawnHeroes)
        {
            GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
        else if(GameManager.Instance.State == GameState.AttackPhase)
        {
            UnitManager.Instance.ExecuteAllActions();
            if (UnitManager.Instance.IsVictory())
            {
                GameManager.Instance.UpdateGameState(GameState.Victory);
            }
            else if (UnitManager.Instance.IsDefeat())
            {
                GameManager.Instance.UpdateGameState(GameState.Defeat);
            }
            else
            {
                GameManager.Instance.UpdateGameState(GameState.MovementPhase);
            }
        }
        else if(GameManager.Instance.State == GameState.MovementPhase)
        {
            UnitManager.Instance.ExecuteAllMovements();
            if (!UnitManager.Instance.SkipAttackPhase())
            {
                GameManager.Instance.UpdateGameState(GameState.AttackPhase);
            }
        }

        DeactivateEndTurnButton();
    }

    
    public void DeactivateEndTurnButton()
    {
        _endTurnButton.interactable = false;
    }

    public void ActivateEndTurnButton()
    {
        _endTurnButton.interactable = true;
    }
}
