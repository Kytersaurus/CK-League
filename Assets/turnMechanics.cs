using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class turnMechanics : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button FireBallButton;
    public Button HealButton;
    public playerScript player;
    public OgreScript ogre;
    public void OnFireBallButton()
    {
        StartCoroutine(battleTurn(1));
    }
    public void OnHealButton()
    {
        StartCoroutine(battleTurn(2));
    }
    IEnumerator battleTurn(int move)
    {
        FireBallButton.interactable = false;
        HealButton.interactable = false;
        if (move == 1)
        {
            player.playerfireballAttack();
        }
        else if (move == 2)
        {
            player.healHealth(10);
        }
        yield return new WaitForSeconds(2f);
        ogre.ogreAttack();
        FireBallButton.interactable = true;
        HealButton.interactable = true;
    }
    void Update()
    {
        if (player.isDead) {

        }
    }
}
