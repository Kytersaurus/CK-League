using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UnitSaveData
{
    public string unitName;
    public string[] attackNames;
}

[System.Serializable]
public class TeamData
{
    public List<UnitSaveData> units = new List<UnitSaveData>();
    public int TeamSize;
}

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    private string SavePath => Path.Combine(Application.persistentDataPath, "team.json");

    private List<Attacks> _allAttacks;
    private List<ScriptableUnit> _allUnitPrefabs;
    void Awake()
    {
        Instance = this;
        _allAttacks = Resources.LoadAll<Attacks>("Attacks").ToList();
        _allUnitPrefabs = Resources.LoadAll<ScriptableUnit>("Units")
            .Where(u => u.Faction == Faction.Hero)
            .ToList();
    }
    public void SaveTeam(List<ScriptableUnit> units)
    {
        TeamData teamData = new TeamData();

        foreach (ScriptableUnit unit in units)
        {
            UnitSaveData unitData = new UnitSaveData
            {
                unitName = unit.name,
                attackNames = unit.UnitPrefab.moveSet.Select(a => a.attackName).ToArray(),
            };
            teamData.units.Add(unitData);
        }
        teamData.TeamSize = teamData.units.Count();
        File.WriteAllText(SavePath, JsonUtility.ToJson(teamData, true));
        Debug.Log($"Team saved to {SavePath}");
    }
    public List<ScriptableUnit> LoadTeam(string SavePath)
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log($"No saved team found at {SavePath}");
            return null;
        }
        var Team = new List<ScriptableUnit>();
        TeamData teamData = JsonUtility.FromJson<TeamData>(File.ReadAllText(SavePath));
        if (teamData.TeamSize <= 0)
        {
            Debug.Log("No units in team");
            return null;
        }
        foreach (UnitSaveData unitData in teamData.units)
        {
            ScriptableUnit unit = _allUnitPrefabs.FirstOrDefault(p => p.name == unitData.unitName);
            unit.UnitPrefab.moveSet = unitData.attackNames
                .Select(name => _allAttacks.FirstOrDefault(a => a.attackName == name))
                .Where(a => a != null)
                .ToList();
            Team.Add(unit);
        }
        Debug.Log($"Team loaded from {SavePath}");
        return Team;
    }
    
}
