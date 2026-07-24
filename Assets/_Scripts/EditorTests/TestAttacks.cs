using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestAttacks
{
    [UnityTest]
    public IEnumerator TestNormalAttacks()
    {
        BaseHero hero = (BaseHero)MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
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
    public IEnumerator TestBlock()
    {
        BaseHero hero = (BaseHero)MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        BaseUnit enemy = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Enemies/Ogre").UnitPrefab);
        yield return null;
        hero.CurrentHealth = 100;
        enemy.CurrentHealth = 100;
        Attacks attackUsed = (Attacks)ScriptableObject.CreateInstance("BasicSlashAttack");
        attackUsed.damage = 30;
        Block block = (Block)ScriptableObject.CreateInstance("Block");
        block.chance = 1.0f;
        block.Execute(hero, enemy);
        attackUsed.Execute(enemy, hero);
        Assert.AreEqual(100, hero.CurrentHealth);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestDodge()
    {
        BaseHero hero = (BaseHero)MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        BaseUnit enemy = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Enemies/Ogre").UnitPrefab);
        yield return null;
        hero.CurrentHealth = 100;
        enemy.CurrentHealth = 100;
        Attacks attackUsed = (Attacks)ScriptableObject.CreateInstance("BasicSlashAttack");
        attackUsed.damage = 30;
        Dodge dodge = (Dodge)ScriptableObject.CreateInstance("Dodge");
        dodge.chance = 1.0f;
        dodge.Execute(hero, enemy);
        attackUsed.Execute(enemy, hero);
        Assert.AreEqual(100, hero.CurrentHealth);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestHeal()
    {
        BaseHero hero = (BaseHero)MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        BaseUnit enemy = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Enemies/Ogre").UnitPrefab);
        yield return null;
        hero.CurrentHealth = 100;
        hero.maxHealth = 100;
        enemy.CurrentHealth = 100;
        Heals healUsed = (Heals)ScriptableObject.CreateInstance("BasicHeal");
        healUsed.healAmount = 30;
        healUsed.Execute(hero, enemy);
        Assert.AreEqual(100, hero.CurrentHealth);
        yield return null;
        hero.CurrentHealth = 10;
        healUsed.Execute(hero, enemy);
        Assert.AreEqual(40, hero.CurrentHealth);
        yield return null;
        hero.CurrentHealth = 90;
        healUsed.Execute(hero, enemy);
        Assert.AreEqual(100, hero.CurrentHealth);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestHealAlly()
    {
        BaseHero hero = (BaseHero)MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        BaseUnit enemy = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Enemies/Ogre").UnitPrefab);
        BaseHero mage = (BaseHero)MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Wizard").UnitPrefab);
        yield return null;
        hero.CurrentHealth = 100;
        hero.maxHealth = 100;
        enemy.CurrentHealth = 100;
        HealExternal healUsed = (HealExternal)ScriptableObject.CreateInstance("HealPoolSpell");
        healUsed.healAmount = 30;
        healUsed.Execute(mage, hero);
        Assert.AreEqual(100, hero.CurrentHealth);
        yield return null;
        hero.CurrentHealth = 10;
        healUsed.Execute(mage, hero);
        Assert.AreEqual(40, hero.CurrentHealth);
        yield return null;
        hero.CurrentHealth = 90;
        healUsed.Execute(mage, hero);
        Assert.AreEqual(100, hero.CurrentHealth);
        yield return null;
    }
}
