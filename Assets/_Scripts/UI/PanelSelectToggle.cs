using UnityEngine;
using UnityEngine.UI;

public class PanelSelectToggle : MonoBehaviour
{
    private Toggle _toggle;
    [SerializeField] private Image _image;
    [SerializeField] private Color _offColour;
    [SerializeField] private Color _onColour;
    [SerializeField] private int _panel;
    void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        _image.color = isOn ? _onColour : _offColour;
        TeamEditor.Instance.SetPanelActive(_panel);
    }
}
