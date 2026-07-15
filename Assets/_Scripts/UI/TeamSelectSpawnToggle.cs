using UnityEngine;
using UnityEngine.UI;

public class TeamSelectSpawnToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Toggle _toggle;
    [SerializeField] private Image _selectBorder;
    void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleChanged);
    }
    public void SelectTeam(int i)
    {
        UnitManager.Instance.LoadTeam(i);
    }
    private void OnToggleChanged(bool isOn)
    {
        _selectBorder.gameObject.SetActive(isOn);
    }
}
