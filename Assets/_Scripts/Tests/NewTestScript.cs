using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnitySetUp]
    public IEnumerator Setup()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level 1");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    [UnityTest]
    public IEnumerator TestVictoryCondition()
    {
        GameManager.Instance.UpdateGameState(GameState.AttackPhase);
        BaseEnemy[] enemies = GameObject.FindObjectsByType<BaseEnemy>();
        Button endTurn = GameObject.Find("EndTurnButton").GetComponent<Button>();
        foreach(BaseEnemy enemy in enemies)
        {
            UnitManager.Instance.KillUnit(enemy);
        }
        endTurn.onClick.Invoke();
        Assert.AreEqual(GameState.Victory, GameManager.Instance.State);
        yield return null;
    }
    [UnityTest]
    public IEnumerator TestDefeatCondition()
    {
        GameManager.Instance.UpdateGameState(GameState.AttackPhase);
        BaseHero[] heroes = GameObject.FindObjectsByType<BaseHero>();
        Button endTurn = GameObject.Find("EndTurnButton").GetComponent<Button>();
        foreach(BaseHero hero in heroes)
        {
            UnitManager.Instance.KillUnit(hero);
        }
        endTurn.onClick.Invoke();
        Assert.AreEqual(GameState.Defeat, GameManager.Instance.State);
        yield return null;
    }
}
