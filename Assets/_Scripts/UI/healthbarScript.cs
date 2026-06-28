using UnityEngine;
using UnityEngine.UI;
public class healthbarScript : MonoBehaviour
{
    public Slider slider;
    
    public void setMaxHealth (int health)
    {
        slider.maxValue = health;
        setHealth(health);
    }
    public void setHealth (int health)
    {
        slider.value = health;
    }
}
