using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    
    public AttackPhaseAction Action = AttackPhaseAction.Attack;
    public bool Alive = true;
    public List<BaseUnit> TargetsList = new List<BaseUnit>();
    public BaseUnit Target;
    public healthbarScript healthBar;
    public List<Attacks> moveSet = new List<Attacks>();
    public Sprite UnitIcon;
    public Attacks SelectedAttack;
    public string UnitDescription;
    public int maxHealth;
    public int CurrentHealth, AttackRange, AttackPower, AttackSpeed;
    public Dictionary<Tile, Tile> PathDictionary = new Dictionary<Tile, Tile>();
    public Queue<Tile> Path = new Queue<Tile>();
    public Tile DestinationTile;
    public Tile PreviousTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        CurrentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }
    public void TakeDamage (int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            UnitManager.Instance.KillUnit(this);
        }
        else
        {
            healthBar.setHealth(CurrentHealth);
        }
    }

    public void Attack(BaseUnit defendingUnit)
    {
        defendingUnit.TakeDamage(AttackPower);
        /*float counterAttackChance = 0.5f;
        if(defendingUnit.Action == AttackPhaseAction.Attack)
        {
            if(AttackSpeed >= defendingUnit.AttackSpeed)
            {
                defendingUnit.TakeDamage(AttackPower);
                if(defendingUnit.Alive && UnitManager.Instance.InAttackRange(defendingUnit, this) && Random.value < counterAttackChance)
                {
                    this.TakeDamage(defendingUnit.AttackPower);
                }
            }
            else
            {
                this.TakeDamage(defendingUnit.AttackPower);
                if(this.Alive && UnitManager.Instance.InAttackRange(this, defendingUnit) && Random.value < counterAttackChance)
                {
                    defendingUnit.TakeDamage(AttackPower);
                }
            }
        }*/
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

    public void SetTarget(BaseUnit targetUnit)
    {
        Target = targetUnit;
        UnitManager.Instance.DeselectHero();
    }
    
    public void SetDestination(Tile tile)
    {
        DestinationTile = tile;
        ConstructPath(tile);
        UnitManager.Instance.DeselectHero();
    }

    public void ConstructPath(Tile destination)
    {
        Path.Clear();
        if(destination == OccupiedTile)
        {
            return;
        }
        ConstructPath(PathDictionary[destination]);
        Path.Enqueue(destination);
    }
}

public enum AttackPhaseAction
{
    Attack,
    Block,
    Dodge
}