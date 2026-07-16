using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackToolBarScript : MonoBehaviour
{
    public static AttackToolBarScript Instance;
    private BaseUnit _selectedUnit;
    [SerializeField] private Image _profileIcon;
    [SerializeField] private List<GameObject> _attackButtonsObj;
    [SerializeField] private List<Toggle> _attackButtons;
    [SerializeField] private List<Image> _attackButtonIcons;
    [SerializeField] private TextMeshProUGUI _unitDescription;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private GameObject _attackBar;
    void Awake()
    {
        Instance = this;
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
        _rect.anchoredPosition = new Vector2(0, -560);

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
    public void SetButtonsActive(bool state)
    {
        for (int i = 1; i < 4; i++)
        {
            _attackButtonsObj[i].SetActive(state);
        }
    }
}
