using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;
    [SerializeField] private healthbarScript healthBar;
    [SerializeField] private OgreScript ogre;
    public bool isDead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth == 0)
        {
            isDead = true;
        }
        if (isDead)
        {
            
        }
    }
    public void playerfireballAttack()
    {
        ogre.takeDamage(35);
    }
    public void playerHeal()
    {
        healHealth(10);
    }
    public void takeDamage (int damage)
    {
        if (currHealth >= damage)
        {
            currHealth -= damage;
            healthBar.setHealth(currHealth);
        }
        else
        {
            currHealth = 0;
            healthBar.setHealth(currHealth);
        }
    }
    public void healHealth (int healAmount)
    {
        if (currHealth + healAmount < maxHealth)
        {
            currHealth += healAmount;
        }
        else
        {
            currHealth = maxHealth;
        }
        healthBar.setHealth(currHealth);
    }
}
