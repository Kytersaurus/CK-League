using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackToolBarScript : MonoBehaviour
{
    private BaseUnit _selectedUnit;
    [SerializeField] private Image _profileIcon;
    [SerializeField] private List<Button> _attackButtons;
    [SerializeField] private TextMeshProUGUI _unitDescription;
    void Awake()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit = UnitManager.Instance.SelectedHero;
            _profileIcon.sprite = _selectedUnit.UnitIcon;
            _unitDescription.text = _selectedUnit.UnitDescription;
            var attacks = _selectedUnit.moveSet;
            for (int i = 0; i < 4; i++)
            {
                _attackButtons[i].image.sprite = attacks[i].icon;
                var buttonScript = _attackButtons[i].GetComponent<AttackButtonScript>();
                buttonScript.attack = attacks[i];
            }    
        }
    }
}
