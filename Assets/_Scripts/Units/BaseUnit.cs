using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public int maxHealth = 100;
    public int CurrentHealth;
    public int AttackRange;
    public healthbarScript healthBar;
    public List<Attacks> moveSet = new List<Attacks>();
    public Sprite UnitIcon;
    public Attacks SelectedAttack;
    public string UnitDescription;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        maxHealth = 100;
        CurrentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }
    
    public void TakeDamage (int damage)
    {
        if (CurrentHealth >= damage)
        {
            CurrentHealth -= damage;
            healthBar.setHealth(CurrentHealth);
        }
        else
        {
            CurrentHealth = 0;
            healthBar.setHealth(CurrentHealth);
        }
    }
    public void HealHealth (int healAmount)
    {
        if (CurrentHealth + healAmount < maxHealth)
        {
            CurrentHealth += healAmount;
        }
        else
        {
            CurrentHealth = maxHealth;
        }
        healthBar.setHealth(CurrentHealth);
    }
    
}
