using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Button> _levelButtons;
    [SerializeField] private List<string> _levelButtonTexts;
    [SerializeField] private Button _teamEditorButton;
    void Start()
    {
        for (int i = 0 ; i < _levelButtons.Count; i++)
        {
            string levelName = _levelButtonTexts[i];
            _levelButtons[i].interactable = ProgressManager.Instance.CheckLevelUnlock(levelName);
        }
        _teamEditorButton.gameObject.SetActive(ProgressManager.Instance.CheckLevelUnlock("Tutorial"));
    }

    void Update()
    {
        
    }
}
