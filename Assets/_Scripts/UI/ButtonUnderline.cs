using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI buttonText;
    private FontStyles defaultText;
    void Start()
    {
        defaultText = buttonText.fontStyle;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.fontStyle = defaultText | FontStyles.Underline;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.fontStyle = defaultText;
    }
}
