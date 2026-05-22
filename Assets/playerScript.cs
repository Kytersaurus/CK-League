using UnityEngine;
using UnityEngine.InputSystem;

public class playerScript : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currHealth;
    [SerializeField] private healthbarScript healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            healHealth(10);
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
