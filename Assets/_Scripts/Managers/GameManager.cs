using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.GenerateGrid);
    }
    public void UpdateGameState(GameState newState)
    {
        State = newState;
        MenuManager.Instance.ShowGamePhase(newState);

        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.SpawnHeroes:
                
                break;
            case GameState.MovementPhase:
            
                break;
            case GameState.AttackPhase:
                UnitManager.Instance.UpdateAllTargetLists();
                UnitManager.Instance.ResetAllTargets();
                UnitManager.Instance.SetEnemyAttacks();
                EndTurnButton.Instance.ActivateEndTurnButton();
                break;
            case GameState.Victory:
                //HandleVictory();
                break;
            case GameState.Defeat:
                //HandleDefeat();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);

    }
}

public enum GameState
{
    GenerateGrid,
    SpawnEnemies,
    SpawnHeroes,
    MovementPhase,
    AttackPhase,
    Victory,
    Defeat
}