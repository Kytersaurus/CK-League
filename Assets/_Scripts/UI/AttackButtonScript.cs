using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Attacks attack;
    [SerializeField] private Toggle _toggle;

    void Awake()
    {
        _toggle.onValueChanged.AddListener(OnPress);
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
}
