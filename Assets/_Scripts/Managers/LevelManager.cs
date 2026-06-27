using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Button> _levelButtons;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Button button in _levelButtons)
        {
            if (ProgressManager.Instance.CheckLevelUnlock(button.GetComponentInChildren<TMP_Text>().text))
            {
                button.interactable = true;
            }
        }
    }
}
