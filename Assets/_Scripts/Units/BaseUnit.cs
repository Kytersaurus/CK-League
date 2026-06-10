using UnityEngine;
using UnityEngine.UI;
public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public int maxHealth = 100;
    public int CurrentHealth;
    public int AttackRange, AttackPower;
   
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
            UnitManager.Instance.SetSelectedHero(null);
            GameManager.Instance.UpdateGameState(GameState.MovementPhase);
        }
    }

    public void Attack(BaseUnit defendingUnit)
    {
        defendingUnit.TakeDamage(AttackPower);
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
