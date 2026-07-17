using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Attacks attack;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private TextMeshProUGUI _attackDescription;
    [SerializeField] private TextMeshProUGUI _attackVal;
    [SerializeField] private Image _selectedBox;
    void Awake()
    {
        _toggle.onValueChanged.AddListener(OnPress);
        _attackDescription.text = $"{attack.attackName} : {attack.attackDesc}";
        string val;
        if (attack is Heals healattack)
        {
            val = $"Heal Amount: {healattack.healAmount}";
        } 
        else
        {
            val = $"Damage Amount: {attack.damage}";    
        }
        _attackVal.text = val;
    }
    void OnPress(bool pressed)
    {
        _selectedBox.gameObject.SetActive(pressed);
        if (pressed)
        {
            UnitManager.Instance.SelectedHero.SelectedAttack = attack;
            List<BaseUnit> targets = UnitManager.Instance.SelectedHero.TargetsList;
            bool targetsActive = !(attack is Heals || attack is Mitigate);
            foreach (BaseUnit unit in targets)
            {
                unit.OccupiedTile.highlight.SetActive(targetsActive);
            }
    }
        else
        {
            UnitManager.Instance.SelectedHero.SelectedAttack = null;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipPanel.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipPanel.SetActive(false);
    }
}
