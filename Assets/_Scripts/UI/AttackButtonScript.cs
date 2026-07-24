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
        if (UnitManager.Instance.SelectedHero == null)
        {
            return;
        }
        _selectedBox.gameObject.SetActive(pressed);
        if (pressed)
        {
            UnitManager.Instance.SelectedHero.SelectedAttack = attack;
            List<BaseUnit> targets = UnitManager.Instance.SelectedHero.TargetsList;
            bool targetsActive = !(attack is Heals || attack is Mitigate);
            bool healOthers = attack is HealPoolSpell;
            if (!targetsActive || healOthers)
            {
                UnitManager.Instance.SelectedHero.Target = null;
            }
            //UnitManager.Instance.SelectedHero.OccupiedTile.highlightSelect.SetActive(targetsActive);
            foreach (BaseUnit unit in targets)
            {
                if (unit == null || unit.OccupiedTile == null)
                {
                    continue;
                }
                if (unit is BaseEnemy)
                {
                    unit.OccupiedTile.highlight.SetActive(targetsActive && !healOthers);    
                }
                if (unit is BaseHero)
                {
                    unit.OccupiedTile.highlight.SetActive(healOthers);
                }
            }
        }
        else
        {
            UnitManager.Instance.SelectedHero.SelectedAttack = null;
            if (UnitManager.Instance.SelectedHero.TargetsList != null || UnitManager.Instance.SelectedHero.TargetsList.Count > 0)
            {
                foreach(BaseUnit unit in UnitManager.Instance.SelectedHero.TargetsList)
                {
                    if (unit != null && unit.OccupiedTile != null)
                    {
                        unit.OccupiedTile.highlight.SetActive(false);
                    }
                }
            }
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
