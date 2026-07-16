using TMPro;
using UnityEngine;

public class PopUpScript : MonoBehaviour
{
    [SerializeField] GameObject _popUp;
    [SerializeField] TMP_Text _popUpText;
    
    public void ShowPopUpAndSetText(string text)
    {
        _popUpText.text = text;
        ShowPopup();
    }
    public void ShowPopup()
    {
        _popUp.SetActive(true);
    }
    public void DismissPopup()
    {
        _popUp.SetActive(false);
    }
}
