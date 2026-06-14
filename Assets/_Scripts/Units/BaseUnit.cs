using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public int maxHealth = 100;
    public int CurrentHealth;
    public int AttackRange, AttackPower, AttackSpeed;
    public AttackPhaseAction Action = AttackPhaseAction.Attack;
    public bool Alive = true;
    //public List<BaseUnit> ValidTargets;
   
    public healthbarScript healthBar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        maxHealth = 100;
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
            GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
        UnitManager.Instance.DeselectHero();
    }

    public void Attack(BaseUnit defendingUnit)
    {
        float counterAttackChance = 0.5f;
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
        }
    }

    public void healHealth (int healAmount)
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

public enum AttackPhaseAction
{
    Attack,
    Block,
    Dodge
}