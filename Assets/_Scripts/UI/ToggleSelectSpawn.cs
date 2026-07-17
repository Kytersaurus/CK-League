using UnityEngine;
using UnityEngine.UI;

public class ToggleSelectSpawn : MonoBehaviour
{
    private UnitSaveData _unitData;
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
    
    private void OnToggleChanged(bool isOn)
    {
        _selectBorder.gameObject.SetActive(isOn);
        if (!isOn)
        {
            _unitData = null;
            return;
        }
        UnitManager.Instance.SelectedHero = null;
        if (_unitData != null)
        {
            UnitManager.Instance.UnitToSpawn = _unitData;
            GridManager.Instance.HighlightSpawnTiles(true);
        }
    }
}