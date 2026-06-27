using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
 [System.Serializable]
public class ProgressData
{
    public List<string> unlockedLevels = new List<string>();
}
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;
    private ProgressData _progressData;
    public List<string> Levels;
    private string _savePath => Path.Combine(Application.persistentDataPath, "progress.json");
    void Awake()
    {
        Instance = this;
        LoadProgress();
        Levels = new List<string> {"Tutorial", "Level 1", "Level 2"};
    }

    
    public bool CheckLevelUnlock(string levelName)
    {
        return _progressData.unlockedLevels.Contains(levelName);
    }
    public void UnlockLevel(string levelName)
    {
        if (CheckLevelUnlock(levelName))
        {
            return;
        }
        _progressData.unlockedLevels.Add(levelName);
        SaveProgress();
    }
    private void SaveProgress()
    {
        File.WriteAllText(_savePath, JsonUtility.ToJson(_progressData, true));
    }
    private void LoadProgress()
    {
        if (File.Exists(_savePath))
        {
            _progressData = JsonUtility.FromJson<ProgressData>(File.ReadAllText(_savePath));
        }
        else
        {
            _progressData = new ProgressData();
            _progressData.unlockedLevels.Add("Tutorial");
            SaveProgress();
        }
    }
    public void LevelComplete()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        int index = Levels.IndexOf(currentLevel);
        if (index == -1 || index > Levels.Count)
        {
            Debug.LogError("The level is not found in levels list");
            return;
        }
        UnlockLevel(Levels[index+1]);
    }
}
