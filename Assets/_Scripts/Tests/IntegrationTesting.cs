using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntegrationTesting
{
    Button endTurn;
    BaseEnemy enemy;
    BaseHero hero;

    [UnityTest, Order(0)]
    public IEnumerator InitTestSuite()
    {
        SceneManager.LoadScene("Tutorial");
        yield return null;
        endTurn = GameObject.Find("EndTurnButton").GetComponent<Button>();
        enemy = GameObject.FindAnyObjectByType<BaseEnemy>();
        hero = GameObject.FindAnyObjectByType<BaseHero>();
    }

    [UnityTest, Order(1)]
    public IEnumerator TestStartingPhase()
    {
        Assert.AreEqual(GameState.MovementPhase, GameManager.Instance.State);
        yield return null;
    }

    [UnityTest, Order(2)]
    public IEnumerator TestNoValidAttackTarget()
    {
        endTurn.onClick.Invoke();
        Assert.AreEqual(GameState.MovementPhase, GameManager.Instance.State);
        yield return null;
    }

    [UnityTest, Order(3)]
    public IEnumerator TestHaveValidAttackTarget()
    {
        Tile enemyTile = enemy.OccupiedTile;
        Tile enemyNeighbourTile = GridManager.Instance.GetNeighbourTiles(enemyTile)[0];
        enemyNeighbourTile.SetUnit(hero);
        endTurn.onClick.Invoke();
        Assert.AreEqual(GameState.AttackPhase, GameManager.Instance.State);
        yield return null;
    }

    [UnityTest, Order(4)]
    public IEnumerator TestPhaseAfterAttack()
    {
        endTurn.onClick.Invoke();
        Assert.AreEqual(GameState.MovementPhase, GameManager.Instance.State);
        yield return null;
    }

    [UnityTest, Order(5)]
    public IEnumerator TestKillUnit()
    {
        //Go back to attack phase
        endTurn.onClick.Invoke();

        hero.CurrentHealth = 100;
        enemy.CurrentHealth = 100;
        Attacks attackUsed = (Attacks)ScriptableObject.CreateInstance("BasicSlashAttack");
        attackUsed.damage = 100;
        enemy.immune = false;
        hero.Target = enemy;
        hero.SelectedAttack = attackUsed;
        endTurn.onClick.Invoke();
        yield return null;
        Assert.IsTrue(enemy == null);
        yield return null;
    }

    [UnityTest, Order(6)]
    public IEnumerator TestVictory()
    {
        Assert.AreEqual(GameState.Victory, GameManager.Instance.State);
        yield return null;
    }

}
