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
        GridManager gridManager = new GridManager();
        UnitManager unitManager = new UnitManager();
        GameManager gameManager = new GameManager();
        BaseUnit hero = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        Tile startTile = gridManager.GetTileAtPosition(new Vector2(0,0));
        startTile.SetUnit(hero);

        yield return null;
    }
}
