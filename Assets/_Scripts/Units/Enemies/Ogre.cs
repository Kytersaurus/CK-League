using UnityEngine;

public class Ogre : BaseEnemy
{
    void Awake()
    {
        maxHealth = 100;
        InitializeHealth(maxHealth);
    }
}
