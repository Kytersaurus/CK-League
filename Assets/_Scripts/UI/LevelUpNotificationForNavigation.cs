using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpNotificationForNavigation : MonoBehaviour
{
    [SerializeField] private Image _notification;
    void Update()
    {
        if(CheckForLevelUpForAll() == true)
        {
            _notification.enabled = true;
        }
        else
        {
            _notification.enabled = false;
        }
        
        
    }

    private bool CheckForLevelUpForAll()
    {
        List<UnitSaveData> allExistingUnits = TeamManager.Instance.GetAllUnitData();
        
        foreach(UnitSaveData unitData in allExistingUnits)
        {
            ScriptableUnit unit = TeamManager.Instance.AllUnitPrefabs.FirstOrDefault(u => u.name == unitData.unitName);
            if (unit == null)
            {
                continue;
            }
            if (unit.EvolvePathA != null && unit.EvolvePathB != null && unitData.level >= unit.EvolveLevel)
            {
                return true;
            }
            continue;
        }
        return false;
    }
}
