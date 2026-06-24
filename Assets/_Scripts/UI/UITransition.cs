using UnityEngine;

public class UITransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void CloseUI(GameObject UI)
    {
        UI.SetActive(false);
    }
    public void OpenUI(GameObject UI)
    {
        UI.SetActive(true);
    }
}
