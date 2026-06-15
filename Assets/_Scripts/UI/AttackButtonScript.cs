using UnityEngine;

public class AttackButtonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Attacks attack;
    public void SetSelectedAttack()
    {
        UnitManager.Instance.SelectedHero.SelectedAttack = attack;
    }
}
