using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    [SerializeField] private GameObject _attackBar;
    [SerializeField] private List<GameObject> _tooltips;
    public bool flipped;
    void Awake()
    {
        Refresh();
    }
    void Update()
    {
        _slider.value = _selectedUnit.CurrentHealth;
        _label.text = $"{_selectedUnit.CurrentHealth} / {_selectedUnit.maxHealth}";
    }
    public void Refresh()
    {
        _selectedUnit = UnitManager.Instance.SelectedHero;
        if (_selectedUnit == null)
        {
            return;
        }
        RectTransform _rect = _attackBar.GetComponent<RectTransform>();
        _rect.anchoredPosition = new Vector2(0, flipped ? 145 : -225);

        foreach (var tooltip in _tooltips)
        {
            RectTransform toolTipRect = tooltip.GetComponent<RectTransform>();
            toolTipRect.anchoredPosition = new Vector2(toolTipRect.anchoredPosition.x, flipped ? -toolTipRect.anchoredPosition.y : toolTipRect.anchoredPosition.y);
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
