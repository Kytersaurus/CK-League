using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUpdateScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _label;
    void Awake()
    {
        _slider.maxValue = UnitManager.Instance.SelectedHero.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        _slider.maxValue = UnitManager.Instance.SelectedHero.CurrentHealth;
        _label.text = $"{UnitManager.Instance.SelectedHero.CurrentHealth} / {UnitManager.Instance.SelectedHero.maxHealth}";
    }
}
