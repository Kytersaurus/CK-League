using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UnitSaveData
{
    public string guid;
    public string unitName;
    public string className;
    public int level;
    public int experience; 
    public string[] attackNames;
}

[System.Serializable]
public class TeamSaveData
{
    public int teamSlot;
    public List<string> unitGuids = new List<string>();
    public int TeamSize;
}

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    private string TeamSavePath(int slot) => Path.Combine(Application.persistentDataPath, $"team_{slot}.json");

    private string UnitSavePath(string guid) => Path.Combine(Application.persistentDataPath, $"unit_{guid}.json");
    public List<Attacks> AllAttacks;
    public List<ScriptableUnit> AllUnitPrefabs;
    public int ActiveTeamSlot = 0;
    public bool SavedTeamExists => File.Exists(TeamSavePath(ActiveTeamSlot));
    public bool SavedUnitExists(string guid) => File.Exists(UnitSavePath(guid));
    public void OnEnable()
    {
        UnitManager.OnExperienceAdded += UpdateUnitData;
    }
    public void OnDisable()
    {
        UnitManager.OnExperienceAdded -= UpdateUnitData;
    }
    void Awake()
    {
        Instance = this;
        AllAttacks = Resources.LoadAll<Attacks>("Attacks").ToList();
        AllUnitPrefabs = Resources.LoadAll<ScriptableUnit>("Units")
            .Where(u => u.Faction == Faction.Hero)
            .ToList();
        
    }
    public void SaveTeam(List<UnitSaveData> units)
    {
        TeamSaveData teamData = new TeamSaveData
        {
            unitGuids = units.Select(u => u.guid).ToList(),
            TeamSize = units.Count()
        };
        File.WriteAllText(TeamSavePath(ActiveTeamSlot), JsonUtility.ToJson(teamData, true));
    }
    public List<UnitSaveData> LoadTeamData()
    {
        string path = TeamSavePath(ActiveTeamSlot);
       if (!File.Exists(path))
        {
            Debug.Log($"No team save found at {path}");
            return null;
        }
        TeamSaveData teamData = JsonUtility.FromJson<TeamSaveData>(File.ReadAllText(path));
        List<UnitSaveData> team = new List<UnitSaveData>();
        foreach (string guid in teamData.unitGuids)
        {
            UnitSaveData unitData = LoadUnitData(guid);
            if (unitData != null)
            {
                team.Add(unitData);    
            }
        }
        return team;
    }
    public List<(ScriptableUnit, UnitSaveData)> LoadTeamUnits()
    {
        List<UnitSaveData> teamUnitData = LoadTeamData();
        var ret = new List<(ScriptableUnit, UnitSaveData)>();
        foreach (UnitSaveData unitData in teamUnitData)
        {
            ScriptableUnit unit = AllUnitPrefabs.FirstOrDefault(u => u.name == unitData.unitName);
            if (unit == null)
            {
                Debug.LogError($"No scriptableunit prefab found for {unitData.unitName}");
                continue;
            }
            ret.Add((unit, unitData));
        }
        return ret;
    }

    public UnitSaveData LoadUnitData(string guid)
    {
        string savePath = UnitSavePath(guid);
        if (!File.Exists(savePath))
        {
            Debug.Log($"No hero exists at {savePath}");
            return null;
        }
        return JsonUtility.FromJson<UnitSaveData>(File.ReadAllText(savePath));
    }

    public void SaveUnitData(UnitSaveData data)
    {
        File.WriteAllText(UnitSavePath(data.guid), JsonUtility.ToJson(data, true));
    }

    public void UpdateUnitData(BaseHero hero)
    {
        UnitSaveData data = new UnitSaveData
        {
            guid = hero.guid,
            unitName = hero.unitName,
            className = hero.className,
            level = hero.level,
            experience = hero.experience,
            attackNames = hero.moveSet.Select(a => a.attackName).ToArray()
        };
        SaveUnitData(data);
    }

    public List<UnitSaveData> GetAllUnitData()
    {
        List<UnitSaveData> allUnits = new List<UnitSaveData>();

        string[] files = Directory.GetFiles(Application.persistentDataPath, "unit_*.json");

        foreach (string file in files)
        {
            UnitSaveData data = JsonUtility.FromJson<UnitSaveData>(File.ReadAllText(file));
            if (data != null)
            {
                allUnits.Add(data);
            }
        }
        return allUnits;
    }
    public void DeleteTeam()
    {
        if (File.Exists(TeamSavePath(ActiveTeamSlot)))
        {
            File.Delete(TeamSavePath(ActiveTeamSlot));
        }
    }

    public void DeleteUnit(string guid)
    {
        if (File.Exists(UnitSavePath(guid)))
        {
            File.Delete(UnitSavePath(guid));
        }
    }
    public UnitSaveData CreateAndSaveUnit(ScriptableUnit unit)
    {
        BaseHero heroPrefab = unit.UnitPrefab as BaseHero;

        UnitSaveData data = new UnitSaveData
        {
            guid = System.Guid.NewGuid().ToString(),
            unitName = unit.name,
            level = 1,
            experience = 0,
            attackNames = unit.UnitPrefab.moveSet
                .Select(a => a.attackName)
                .ToArray()
        };
        SaveUnitData(data);
        return data;
    }
}
