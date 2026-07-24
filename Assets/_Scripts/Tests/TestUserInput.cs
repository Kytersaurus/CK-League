using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestUserInput
{
    Button endTurn;
    BaseHero warrior, archer, mage, cavalry;
    BaseEnemy[] enemyList;

    [UnityTest, Order(-1)]
    public IEnumerator InitTestSuite()
    {
        SceneManager.LoadScene("TestScene");
        yield return null;
        GameManager.Instance.EnemyDifficulty = 3;
        endTurn = GameObject.Find("EndTurnButton").GetComponent<Button>();
        warrior = GameObject.FindAnyObjectByType<BaseWarrior>();
        archer = GameObject.FindAnyObjectByType<BaseArcher>();
        mage = GameObject.FindAnyObjectByType<BaseMage>();
        cavalry = GameObject.FindAnyObjectByType<BaseCavalry>();
        enemyList = GameObject.FindObjectsByType<BaseEnemy>();
    }
    
    [UnityTearDown]
    public IEnumerator RunAfterEveryPlayModeTest()
    {
        
        Tile heroHomeTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,0));
        heroHomeTile.SetUnit(warrior);
        heroHomeTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,1));
        heroHomeTile.SetUnit(archer);
        heroHomeTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,2));
        heroHomeTile.SetUnit(mage);
        heroHomeTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,3));
        heroHomeTile.SetUnit(cavalry);
        Tile enemyHomeTile = GridManager.Instance.GetTileAtPosition(new Vector2(15,0));
        int y = 0;
        foreach(BaseEnemy enemy in enemyList)
        {
            enemyHomeTile.SetUnit(enemy);
            y++;
            enemyHomeTile = GridManager.Instance.GetTileAtPosition(new Vector2(15,y));
        }
        yield return null;
        
        Debug.Log("Asynchronous cleanup finished.");
    }

    [UnityTest]
    public IEnumerator TestHeroMovementInput()
    {
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,8));
        warriorTile.SetUnit(warrior);
        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        warriorTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        Assert.AreEqual(warrior,UnitManager.Instance.SelectedHero); //check if warrior is selected

        Tile adjacentTile = GridManager.Instance.GetTileAtPosition(new Vector2(1,8));
        adjacentTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(1,8), warrior.Position);

        Tile farTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,8));
        farTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(1,8), warrior.Position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestHeroAttackInput()
    {
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,8));
        warriorTile.SetUnit(warrior);
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(1,8)); //1 tile to the right
        BaseEnemy enemy = enemyList[0];
        enemyTile.SetUnit(enemy);
        GameManager.Instance.UpdateGameState(GameState.AttackPhase);
        warriorTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        Assert.AreEqual(warrior,UnitManager.Instance.SelectedHero); //check if warrior is selected


        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(1,8),enemy.Position); //check if moved towards warrior
        yield return null;
    }
}
