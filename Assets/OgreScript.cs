using System;
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
    void Start()
    {
        currHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            takeDamage(20);
        }
        if (timer < attackRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            target.takeDamage(20);
            timer = 0;
        }
    }
    public void takeDamage (int damage)
    {
        if (currHealth >= damage)
        {
            currHealth -= damage;
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
