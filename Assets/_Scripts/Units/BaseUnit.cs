using UnityEngine;
using UnityEngine.UI;
public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public int maxHealth = 100;
    public int CurrentHealth;
   
    public healthbarScript healthBar;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        maxHealth = 100;
        CurrentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }
    
    public void takeDamage (int damage)
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
