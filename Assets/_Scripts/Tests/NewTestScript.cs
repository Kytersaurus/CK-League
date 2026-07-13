using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class NewTestScript
{
    
    [UnitySetUp]
    public IEnumerator Setup()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Tutorial");
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
