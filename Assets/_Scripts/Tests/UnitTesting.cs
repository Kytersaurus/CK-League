using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTesting
{
    [UnityTest]
    public IEnumerator TestAttacks()
    {
        BaseUnit hero = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        BaseUnit enemy = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Enemies/Ogre").UnitPrefab);
        yield return null;
        hero.CurrentHealth = 100;
        enemy.CurrentHealth = 100;
        Attacks attackUsed = (Attacks)ScriptableObject.CreateInstance("BasicSlashAttack");
        attackUsed.damage = 30;
        enemy.immune = false;
        attackUsed.Execute(hero, enemy);
        Assert.AreEqual(70, enemy.CurrentHealth);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMovementOnEmptyTileInRange()
    {

        //WIP
        
        GameObject gridManagerObject = new GameObject("GridManager");
        GridManager gridManager = gridManagerObject.AddComponent<GridManager>();
        yield return null;
        GameObject unitManagerObject = new GameObject("UnitManager");
        UnitManager unitManager = unitManagerObject.AddComponent<UnitManager>();
        //GameObject gameManagerObject = new GameObject("GameManager");
        //GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
        BaseUnit hero = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        gridManager.GenerateGrid();
        Tile startTile = gridManager.GetTileAtPosition(new Vector2(0,0));
        //startTile.SetUnit(hero);

        yield return null;
    }
}
