using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestEnemyAI
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
    public IEnumerator TestFindClosestHero()
    {
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,8));
        warriorTile.SetUnit(warrior);
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(2,8)); //2 tiles to the right
        BaseEnemy enemy = enemyList[0];
        enemyTile.SetUnit(enemy);
        Tile mageTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,8));
        mageTile.SetUnit(mage);
        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(1,8),enemy.Position); //check if moved towards warrior
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPathFinding()
    {
        Tile heroTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,8));
        heroTile.SetUnit(warrior);
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(2,8)); //2 tiles to the right
        BaseEnemy enemy = enemyList[0];
        enemyTile.SetUnit(enemy);
        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(1,8),enemy.Position); //check if moved towards hero
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestRetreatRight()
    {
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,6));
        BaseEnemy enemy = enemyList[0];
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,7)); //top
        warriorTile.SetUnit(warrior);
        enemyTile.SetUnit(enemy);
        Tile cavalryTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,5)); //bottom
        cavalryTile.SetUnit(cavalry);
        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(9,6),enemy.Position); //check if retreat right
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestRetreatWhenFullySurrounded()
    {
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,6));
        BaseEnemy enemy = enemyList[0];
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,7)); //top
        warriorTile.SetUnit(warrior);
        enemyTile.SetUnit(enemy);
        Tile cavalryTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,5)); //bottom
        cavalryTile.SetUnit(cavalry);
        Tile mageTile = GridManager.Instance.GetTileAtPosition(new Vector2(7,6)); //left
        mageTile.SetUnit(mage);
        Tile archerTile = GridManager.Instance.GetTileAtPosition(new Vector2(9,6)); //right
        archerTile.SetUnit(archer);
        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(8,6),enemy.Position); //check if stays in place
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestSurround()
    {
        Tile heroTile = GridManager.Instance.GetTileAtPosition(new Vector2(7,6));
        heroTile.SetUnit(warrior);
        Tile enemy1Tile = GridManager.Instance.GetTileAtPosition(new Vector2(8,6));
        BaseEnemy enemy1 = enemyList[0];
        enemy1Tile.SetUnit(enemy1);
        Tile enemy2Tile = GridManager.Instance.GetTileAtPosition(new Vector2(7,7));
        BaseEnemy enemy2 = enemyList[1];
        enemy2Tile.SetUnit(enemy2);
        Tile enemy3Tile = GridManager.Instance.GetTileAtPosition(new Vector2(9,7));
        BaseEnemy enemy3 = enemyList[2];
        enemy3Tile.SetUnit(enemy3);
        GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(new Vector2(9,6),enemy3.Position);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestSmartTargetingForLowHealthUnit()
    {
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,6));
        BaseEnemy enemy = enemyList[0];
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,7)); //top
        warriorTile.SetUnit(warrior);
        enemyTile.SetUnit(enemy);
        Tile cavalryTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,5)); //bottom
        cavalryTile.SetUnit(cavalry);
        Tile mageTile = GridManager.Instance.GetTileAtPosition(new Vector2(7,6)); //left
        mageTile.SetUnit(mage);
        Tile archerTile = GridManager.Instance.GetTileAtPosition(new Vector2(9,6)); //right
        archerTile.SetUnit(archer);
        warrior.CurrentHealth = 100;
        mage.CurrentHealth = 80;
        archer.CurrentHealth = 100;
        cavalry.CurrentHealth = 100;
        GameManager.Instance.UpdateGameState(GameState.AttackPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(mage,enemy.Target); //check if mage is attacked
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestSmartTargetingForSurroundedUnit()
    {
        Tile enemyTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,6));
        BaseEnemy enemy = enemyList[0];
        enemyTile.SetUnit(enemy);
        Tile warriorTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,7)); //top
        warriorTile.SetUnit(warrior);
        Tile cavalryTile = GridManager.Instance.GetTileAtPosition(new Vector2(8,5)); //bottom
        cavalryTile.SetUnit(cavalry);
        Tile mageTile = GridManager.Instance.GetTileAtPosition(new Vector2(7,6)); //left
        mageTile.SetUnit(mage);
        Tile archerTile = GridManager.Instance.GetTileAtPosition(new Vector2(9,6)); //right
        archerTile.SetUnit(archer);
        Tile enemy1Tile = GridManager.Instance.GetTileAtPosition(new Vector2(6,6)); //left of mage
        BaseEnemy enemy1 = enemyList[1];
        enemy1Tile.SetUnit(enemy1);
        warrior.CurrentHealth = 100;
        mage.CurrentHealth = 110;
        archer.CurrentHealth = 100;
        cavalry.CurrentHealth = 100;
        GameManager.Instance.UpdateGameState(GameState.AttackPhase);
        endTurn.onClick.Invoke();
        Assert.AreEqual(mage,enemy.Target); //check if mage is attacked
        yield return null;
    }

}
