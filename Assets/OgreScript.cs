using System;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
public class OgreScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;
    [SerializeField] private healthbarScript healthBar;
    [SerializeField] private float attackRate = 10;
    private float timer = 0;
    [SerializeField] private playerScript target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool isDead;
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
    }
    public void ogreAttack()
    {
        target.takeDamage(20);
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
    private void deathAnimate()
    {
        
    }
}
