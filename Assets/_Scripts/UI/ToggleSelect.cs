using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSelect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UnitSaveData _unitData;
    private ScriptableUnit _unit;
    private Attacks _attack;
    private Toggle _toggle;
    [SerializeField] private Image _selectBorder;
    void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void SetExistingUnit(UnitSaveData unit)
    {
        _unitData = unit;
    }
    public void SetNewUnit(ScriptableUnit unit)
    {
        _unit = unit;
    }
    public void SetAttack(Attacks attack)
    {
        _attack = attack;
    }
    private void OnToggleChanged(bool isOn)
    {
        _selectBorder.gameObject.SetActive(isOn);
        if (!isOn)
        {
            return;
        }
        if (_unitData != null)
        {
            TeamEditor.Instance.SetSelectedUnit(_unitData);
        }
        else if (_unit != null)
        {
            TeamEditor.Instance.SetSelectedNewUnit(_unit);
        }
        else if (_attack != null)
        {
            TeamEditor.Instance.SetAttack(_attack);
        }
    }
}
