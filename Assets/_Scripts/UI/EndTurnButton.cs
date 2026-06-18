using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    public void EndTurn()
    {
        //Debug.Log("Execute commands");
        if(GameManager.Instance.State == GameState.AttackPhase)
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
    }
}
