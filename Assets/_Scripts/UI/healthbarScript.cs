using UnityEngine;
using UnityEngine.UI;
public class healthbarScript : MonoBehaviour
{
    public Slider slider;
    
    public void SetMaxHealth (int health)
    {
        slider.maxValue = health;
        SetHealth(health);
    }
    public void SetHealth (int health)
    {
        slider.value = health;
    }
}
