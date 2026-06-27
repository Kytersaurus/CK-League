using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has closed");
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LevelComplete()
    {
        List<string> levels = ProgressManager.Instance.Levels;
        string currentLevel = SceneManager.GetActiveScene().name;
        int index = levels.IndexOf(currentLevel);
        ProgressManager.Instance.UnlockLevel(levels[index+1]);
    }
}
