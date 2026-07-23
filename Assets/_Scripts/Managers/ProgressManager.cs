using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
 [System.Serializable]
public class ProgressData
{
    public List<string> unlockedLevels = new List<string>();
}
[System.Serializable]
public class LevelSaveData
{
    public List<UnitBattleData> enemies = new List<UnitBattleData>();
    public List<HeroBattleData> heroes = new List<HeroBattleData>();
    public GameState gameState;
}
[System.Serializable]
public class UnitBattleData
{
    public string unitName;
    public Faction faction;
    public int currentHealth;
    public int maxHealth;
    public int gridX;
    public int gridY;
    public bool alive;
}
[System.Serializable]
public class HeroBattleData : UnitBattleData
{
    public string guid;
    public string className;
    public int level;
    public int experience;
}
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;
    private ProgressData _progressData;
    public List<string> Levels;
    private string _savePath => Path.Combine(Application.persistentDataPath, "progress.json");
    private string _levelProgressSavePath(string levelName) => Path.Combine(Application.persistentDataPath, $"{levelName}.json");
    public bool LevelProgressSaved => File.Exists(_levelProgressSavePath(SceneManager.GetActiveScene().name)); 
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
        if (index == -1 || index >= Levels.Count)
        {
            Debug.Log("The level is not found in levels list");
            return;
        }
        if (index+1 == Levels.Count)
        {
            Debug.Log("There is no next level");
            return;
        }
        UnlockLevel(Levels[index+1]);
    }
    public void SaveLevelProgress()
    {
        if (GameManager.Instance.State == GameState.SpawnHeroes)
        {
            return;
        }
        string savePath = _levelProgressSavePath(SceneManager.GetActiveScene().name);
        List<UnitBattleData> enemyDatas = new List<UnitBattleData>();
        List<HeroBattleData> heroDatas = new List<HeroBattleData>();
        foreach (BaseUnit unit in UnitManager.Instance.GetRemaingUnits())
        {
            if (unit is BaseHero)
            {
                BaseHero hero = unit as BaseHero;
                HeroBattleData heroData = new HeroBattleData
                {
                    unitName = hero.name.Replace("(Clone)", ""),
                    faction = hero.Faction,
                    currentHealth = hero.CurrentHealth,
                    maxHealth = hero.maxHealth,
                    gridX = (int)hero.Position.x,
                    gridY = (int)hero.Position.y,
                    alive = hero.Alive,

                    guid = hero.guid,
                    className = hero.className,
                    level = hero.level,
                    experience = hero.experience
                };
                heroDatas.Add(heroData);
            }
            else
            {
                UnitBattleData enemyData = new UnitBattleData
                {
                    unitName = unit.name.Replace("(Clone)", ""),
                    faction = unit.Faction,
                    currentHealth = unit.CurrentHealth,
                    maxHealth = unit.maxHealth,
                    gridX = (int)unit.Position.x,
                    gridY = (int)unit.Position.y,
                    alive = unit.Alive
                };
                enemyDatas.Add(enemyData);
            }
            
        }
        LevelSaveData data = new LevelSaveData
        {
            enemies = enemyDatas,
            heroes = heroDatas,
            gameState = GameManager.Instance.State
        };
        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
    }
    public LevelSaveData LoadLevelProgress()
    {
        string savePath = _levelProgressSavePath(SceneManager.GetActiveScene().name);
        if (!File.Exists(savePath))
        {
            Debug.Log($"No save file exists at {savePath}");
            return null;
        }
        return JsonUtility.FromJson<LevelSaveData>(File.ReadAllText(savePath));
    }
    public void DeleteLevelSaveData()
    {
        string savePath = _levelProgressSavePath(SceneManager.GetActiveScene().name);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}
