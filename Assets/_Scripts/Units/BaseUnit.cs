using System;
using System.Collections.Generic;
using UnityEditor;
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
    public int moveRange;
    public Dictionary<Tile, Tile> PathDictionary = new Dictionary<Tile, Tile>();
    public Queue<Tile> Path = new Queue<Tile>();
    public Tile DestinationTile;
    public bool hasMoved;
    public bool immune;
    public float reducedDmg = 1;
    public BaseUnit attackedBy;
    public bool counterAtk;
    public int counterAtkDmg;
    public static event Action<BaseUnit> OnUnitDeath;
    public static event Action OnUnitAction;
    public static event Action<BaseUnit, int> OnDamageTaken;
    public Image AttackIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        CurrentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }
    public void TakeDamage (int damage)
    {
        int dmgTaken = CurrentHealth - damage < 0 ? CurrentHealth : damage;
        string val = $"{dmgTaken}";
        bool blocked = false;
        if (immune)
        {
            val = "0";
            blocked = true;
            damage = 0;
            immune = false;
        }
        else if (reducedDmg != 1)
        {
            blocked = true;
        }
        if(MenuManager.Instance != null)
        {
            MenuManager.Instance.SpawnDamageIndicator(val, transform.position, blocked, false);
        }
        CurrentHealth -= damage;
        OnDamageTaken?.Invoke(this, dmgTaken);
        if(CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnUnitDeath?.Invoke(this);
        }
        else
        {
            healthBar.SetHealth(CurrentHealth);
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
        int healthHealed = CurrentHealth + healAmount > maxHealth ? maxHealth - CurrentHealth : healAmount; 
        MenuManager.Instance.SpawnDamageIndicator($"{healthHealed}", transform.position, false, true);
        if (CurrentHealth + healAmount < maxHealth)
        {
            CurrentHealth += healAmount;
        }
        else
        {
            CurrentHealth = maxHealth;
        }
        healthBar.SetHealth(CurrentHealth);
    }

    public void SetTarget(BaseUnit targetUnit)
    {
        Target = targetUnit;
        OnUnitAction?.Invoke();
        //UnitManager.Instance.DeselectHero();
    }
    
    public void SetDestination(Tile tile)
    {
        DestinationTile = tile;
        GridManager.Instance.ShowUnitDest(this, true);
        Path.Clear();
        ConstructPath(tile);
        OnUnitAction?.Invoke();
        /*if(UnitManager.Instance.SelectedHero != null)
        {
            UnitManager.Instance.DeselectHero();
        }*/
    }

    public void ConstructPath(Tile destination)
    {
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