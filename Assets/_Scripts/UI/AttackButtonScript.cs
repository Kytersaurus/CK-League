using System;
using TMPro;
using Unity.VisualScripting;
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
        if (pressed)
        {
            UnitManager.Instance.SelectedHero.SelectedAttack = attack;
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
