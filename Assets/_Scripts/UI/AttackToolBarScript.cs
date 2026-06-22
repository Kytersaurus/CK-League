using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackToolBarScript : MonoBehaviour
{
    private BaseUnit _selectedUnit;
    [SerializeField] private Image _profileIcon;
    [SerializeField] private List<Toggle> _attackButtons;
    [SerializeField] private List<Image> _attackButtonIcons;
    [SerializeField] private TextMeshProUGUI _unitDescription;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _label;
    void Awake()
    {
        Refresh();
    }
    void Update()
    {
        _slider.value = _selectedUnit.CurrentHealth;
        _label.text = $"{_selectedUnit.CurrentHealth} / {_selectedUnit.maxHealth}";
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnitManager.Instance.SetSelectedHero(null);
        }
    }
    public void Refresh()
    {
        _selectedUnit = UnitManager.Instance.SelectedHero;
        if (_selectedUnit == null)
        {
            return;
        }
         _slider.maxValue = _selectedUnit.maxHealth;
        _profileIcon.sprite = _selectedUnit.UnitIcon;
        _unitDescription.text = _selectedUnit.UnitDescription;
        var attacks = _selectedUnit.moveSet;
        for (int i = 0; i < attacks.Count && i <_attackButtons.Count; i++)
        {
            _attackButtonIcons[i].sprite = attacks[i].icon;
            var buttonScript = _attackButtons[i].GetComponent<AttackButtonScript>();
            buttonScript.attack = attacks[i];
        } 
    }
}
