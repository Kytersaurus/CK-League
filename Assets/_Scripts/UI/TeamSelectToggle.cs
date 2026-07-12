using UnityEngine;
using UnityEngine.UI;

public class TeamSelectToggle : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] private Image _selectBorder;
    void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleChanged);
    }
    public void SelectTeam(int i)
    {
        TeamEditor.Instance.SwitchTeam(i);
    }
    private void OnToggleChanged(bool isOn)
    {
        _selectBorder.gameObject.SetActive(isOn);
    }
}
