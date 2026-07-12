using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamEditor : MonoBehaviour
{
    public static TeamEditor Instance;
    private List<ScriptableUnit> _allNewUnits = new List<ScriptableUnit>();
    private List<UnitSaveData> _allExistingUnits = new List<UnitSaveData>();
    private List<Attacks> _allAttacks = new List<Attacks>();
    private List<UnitSaveData> _team = new List<UnitSaveData>();
    [SerializeField] private int _teamSize;
    private UnitSaveData _selectedUnit;
    private ScriptableUnit _newSelectedUnit;
    private Attacks _selectedAttack;
    [SerializeField] private Transform _newUnitPanel, _existingUnitPanel, _attacksPanel, _infoPanel, _teamPanel;
    [SerializeField] private Image _selectedUnitImage,  _selectedAttackImage;
    [SerializeField] private TextMeshProUGUI _selectedUnitDesc,_selectedAttackDesc;
    [SerializeField] private ToggleGroup _newUnitToggleGroup, _existingUnitToggleGroup, _attacksToggleGroup;
    [SerializeField] private Toggle _unitToggle;
    [SerializeField] private Button _deselectUnitButton, _deselectAttackButton, _addUnitToTeamButton, _deleteUnitButton, _saveUnitButton, _addAttackButton, _removeAttackButton, _saveTeamButton, _deleteTeamButton, _removeUnitFromTeamButton;
    private List<Toggle> _newUnitsList, _existingUnitsList, _attacksList, _unitAttacksList, _teamUnitsList;
    public bool TeamSaved;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //Generate Lists
        RefreshNewUnitsList();
        RefreshExistingUnitsList();
        RefreshAttacksList();
        TeamManager.Instance.ActiveTeamSlot = 0;
        RefreshTeam(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_selectedUnit == null && _newSelectedUnit == null && _selectedAttack == null)
        {
            _deselectUnitButton.gameObject.SetActive(false);
        }
        if (_selectedUnit != null || _newSelectedUnit != null || _selectedAttack != null)
        {
            _deselectUnitButton.gameObject.SetActive(true);
        }
        if (_selectedAttack == null)
        {
            _deselectAttackButton.gameObject.SetActive(false);
            _addAttackButton.gameObject.SetActive(false);
            _removeAttackButton.gameObject.SetActive(false);
        }
        if (_selectedAttack != null)
        {
            _deselectAttackButton.gameObject.SetActive(true);
            _addAttackButton.gameObject.SetActive(true);
            _removeAttackButton.gameObject.SetActive(true);
        }
        if (_selectedUnit == null && _newSelectedUnit == null)
        {
            _addUnitToTeamButton.gameObject.SetActive(false);
            _deleteUnitButton.gameObject.SetActive(false);
            _saveUnitButton.gameObject.SetActive(false);
        }
        if (_selectedUnit != null || _newSelectedUnit != null)
        {
            _addUnitToTeamButton.gameObject.SetActive(true);
            _deleteUnitButton.gameObject.SetActive(_selectedUnit != null);
            _saveUnitButton.gameObject.SetActive(true);
        }
        if (_team == null || _team.Count() == 0 || TeamSaved)
        {
            _saveTeamButton.gameObject.SetActive(false);
            _deleteTeamButton.gameObject.SetActive(false);
        }
        if (_team != null && _team.Count() > 0 && !TeamSaved)
        {
            _saveTeamButton.gameObject.SetActive(true);
            _deleteTeamButton.gameObject.SetActive(true);
        }
    }
    public void RefreshNewUnitsList()
    {
        _allNewUnits = TeamManager.Instance.AllUnitPrefabs;
        if (_newUnitsList == null)
        {
            _newUnitsList = new List<Toggle>();
        }
        else
        {
            foreach (Toggle unitSelect in _newUnitsList)
            {
                Destroy(unitSelect.gameObject);
            }
            _newUnitsList.Clear();
        }
        int x = -550, y = 180;
        foreach (ScriptableUnit unit in _allNewUnits)
        {
            if (x > 550)
            {
                y -= 140;
                x = -550;
            }
            if (y < -100)
            {
                Debug.Log("List is full");
                return;
            }
            Toggle unitSelect = Instantiate(_unitToggle, _newUnitPanel);
            
            RectTransform rect = unitSelect.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);
            unitSelect.group = _newUnitToggleGroup;
            
            ToggleSelect toggleScript = unitSelect.GetComponent<ToggleSelect>();
            toggleScript.SetNewUnit(unit);

            BaseHero hero = unit.UnitPrefab as BaseHero;
            Image unitSprite = unitSelect.GetComponentInChildren<Image>();
            unitSprite.sprite = hero.UnitIcon;

            TMP_Text unitStats = unitSelect.GetComponentInChildren<TMP_Text>();
            unitStats.text = $"{unit.name}";
            
            unitSelect.isOn = false;
            x += 100;
            _newUnitsList.Add(unitSelect);
        }
    }
    public void RefreshExistingUnitsList()
    {
        _allExistingUnits = TeamManager.Instance.GetAllUnitData();
        if (_existingUnitsList == null)
        {
            _existingUnitsList = new List<Toggle>();
        }
        else
        {
            foreach (Toggle unitSelect in _existingUnitsList)
            {
                Destroy(unitSelect.gameObject);
            }
            _existingUnitsList.Clear();
        }
        int x = -550, y = 180;
        foreach (UnitSaveData unitData in _allExistingUnits)
        {
            if (x > 550)
            {
                y -= 140;
                x = -550;
            }
            if (y < -100)
            {
                Debug.Log("List is full");
                return;
            }
            Toggle unitSelect = Instantiate(_unitToggle, _existingUnitPanel);
            RectTransform rect = unitSelect.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);
            unitSelect.group = _existingUnitToggleGroup;
            ScriptableUnit unit = _allNewUnits.FirstOrDefault(u => u.name == unitData.unitName);
            ToggleSelect toggleScript = unitSelect.GetComponent<ToggleSelect>();
            toggleScript.SetExistingUnit(unitData);

            BaseHero hero = unit.UnitPrefab as BaseHero;
            Image unitSprite = unitSelect.GetComponentInChildren<Image>();
            unitSprite.sprite = hero.UnitIcon;

            TMP_Text unitStats = unitSelect.GetComponentInChildren<TMP_Text>();
            unitStats.text = $"Unit name: {unit.name} \n";

            unitSelect.isOn = false;
            x += 100;
            _existingUnitsList.Add(unitSelect);
        }
    }
    public void RefreshAttacksList()
    {
        _allAttacks = TeamManager.Instance.AllAttacks;
        if (_attacksList == null)
        {
            _attacksList = new List<Toggle>();
        }
        else
        {
            foreach (Toggle attackSelect in _attacksList)
            {
                Destroy(attackSelect.gameObject);
            }
            _attacksList.Clear();
        }
        int x = -550, y = 180;
        foreach (Attacks attack in _allAttacks)
        {
            if (x > 550)
            {
                y -= 140;
                x = -550;
            }
            if (y < -100)
            {
                Debug.Log("List is full");
                return;
            }
            Toggle unitSelect = Instantiate(_unitToggle, _attacksPanel);
            RectTransform rect = unitSelect.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);

            unitSelect.group = _attacksToggleGroup;
            ToggleSelect toggleScript = unitSelect.GetComponent<ToggleSelect>();
            toggleScript.SetAttack(attack);

            Image attackSprite = unitSelect.GetComponentInChildren<Image>();
            attackSprite.sprite = attack.icon;

            TMP_Text unitStats = unitSelect.GetComponentInChildren<TMP_Text>();
            unitStats.text = $"{attack.attackName}";

            unitSelect.isOn = false;
            x += 100;
            _attacksList.Add(unitSelect);
        }
    }
    public void RefreshUnitAttackList()
    {
        if (_unitAttacksList == null)
        {
            _unitAttacksList = new List<Toggle>();
        }
        if (_unitAttacksList != null && _unitAttacksList.Count() > 0)
        {
            foreach(Toggle attacksSelect in _unitAttacksList)
            {
                Destroy(attacksSelect.gameObject);
            }
            _unitAttacksList.Clear();
        }
        if (_selectedUnit != null)
        {
            List<Attacks> unitAttacks = _selectedUnit.attackNames
                .Select(name => _allAttacks.FirstOrDefault(a => a.attackName == name))
                .Where(a => a != null)
                .ToList();
            int x = -85, y = -25;
            foreach (Attacks attack in unitAttacks)
            {
                Toggle attacksSelect = Instantiate(_unitToggle,  _infoPanel);
                RectTransform rect = attacksSelect.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(x, y);
                Image attackIcon = attacksSelect.GetComponentInChildren<Image>();
                attackIcon.sprite = attack.icon;

                TMP_Text attackName = attacksSelect.GetComponentInChildren<TMP_Text>();
                attackName.text = attack.attackName;
                attacksSelect.group = _attacksToggleGroup;
                ToggleSelect toggleScript = attacksSelect.GetComponent<ToggleSelect>();
                toggleScript.SetAttack(attack);

                _unitAttacksList.Add(attacksSelect);
                x += 100;
            }
        }
        if (_newSelectedUnit != null)
        {
            List<Attacks> unitAttacks = _newSelectedUnit.UnitPrefab.moveSet;
            int x = -85, y = -25;
            foreach (Attacks attack in unitAttacks)
            {
                Toggle attacksSelect = Instantiate(_unitToggle,  _infoPanel);
                RectTransform rect = attacksSelect.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(x, y);
                Image attackIcon = attacksSelect.GetComponentInChildren<Image>();
                attackIcon.sprite = attack.icon;
                TMP_Text attackName = attacksSelect.GetComponentInChildren<TMP_Text>();
                attackName.text = attack.attackName;

                attacksSelect.group = _attacksToggleGroup;
                ToggleSelect toggleScript = attacksSelect.GetComponent<ToggleSelect>();
                toggleScript.SetAttack(attack);

                _unitAttacksList.Add(attacksSelect);
                x += 100;
            }
        }
    }
    public void ShowUnitInfo()
    {
        bool isActive = _selectedUnit != null || _newSelectedUnit != null;
        if (!isActive)
        {
            return;
        }
        _selectedUnitImage.gameObject.SetActive(isActive);
        _selectedUnitDesc.gameObject.SetActive(isActive);

        if (_selectedUnit != null)
        {
            ScriptableUnit unit = _allNewUnits.FirstOrDefault(u => u.name == _selectedUnit.unitName);
            _selectedUnitImage.sprite = unit.UnitPrefab.UnitIcon;
            _selectedUnitImage.preserveAspect = true;
            _selectedUnitDesc.text = $"Name: {unit.name}\n" +
                                    GetUnitStats(unit.UnitPrefab as BaseHero) +
                                    $"Level: {_selectedUnit.level}\n" +
                                    $"Experience: {_selectedUnit.experience}\n" +
                                    $"Unit Description: {unit.UnitPrefab.UnitDescription}\n";
            RefreshUnitAttackList();
        }
        if (_newSelectedUnit != null)
        {
            ScriptableUnit unit = _newSelectedUnit;
            _selectedUnitImage.sprite = unit.UnitPrefab.UnitIcon;
            _selectedUnitImage.preserveAspect = true;
            _selectedUnitDesc.text = $"Name: {unit.name}\n" +
                                    GetUnitStats(unit.UnitPrefab as BaseHero) +
                                    "Level: 1\n" +
                                    "Experience: 0\n" +
                                    $"Unit Description: {unit.UnitPrefab.UnitDescription}\n";
            RefreshUnitAttackList();
        }
    }
    public void ShowAttackInfo()
    {
        bool isActive = _selectedAttack != null;
        _selectedAttackImage.gameObject.SetActive(isActive);
        _selectedAttackDesc.gameObject.SetActive(isActive);
        if (!isActive)
        {
            return;
        }
        _selectedAttackImage.sprite = _selectedAttack.icon;
        _selectedAttackImage.preserveAspect = true;
        _selectedAttackDesc.text = $"Name: {_selectedAttack.attackName}\n" +
                                   $"Damage: {_selectedAttack.damage}\n" +
                                   $"Attack Description: {_selectedAttack.attackDesc}\n";
    }
    public string GetUnitStats(BaseHero unit)
    {
        string stats = $"Health: {unit.maxHealth}\n" +
                       $"Attack Range: {unit.AttackRange}\n" +
                       $"Attack Speed: {unit.AttackSpeed}\n" +
                       $"Move Range: {unit.moveRange}\n" +
                       $"Class: {unit.className}\n";
        
        return stats;
    }
    public void UpdateExistingUnits()
    {
        List<UnitSaveData> allExistingUnitData = TeamManager.Instance.GetAllUnitData();
        _allExistingUnits.Clear();
        foreach (UnitSaveData unitdata in allExistingUnitData)
        {
            _allExistingUnits.Add(unitdata);
        }
    }
    public BaseHero GetUnitPrefab(UnitSaveData data)
    {
        ScriptableUnit unit = _allNewUnits.FirstOrDefault(u => u.name == data.unitName);
            if (unit == null)
            {
                Debug.Log("No unit found");
                return null;
            }
        return unit.UnitPrefab as BaseHero;
    }
    public void SetSelectedUnit(UnitSaveData unit)
    {
        _selectedUnit = unit;
        _newSelectedUnit = null;
        _selectedAttack = null;
        RefreshUnitAttackList();
        ShowUnitInfo();
    }
    public void SetSelectedNewUnit(ScriptableUnit unit)
    {
        _newSelectedUnit = unit;
        _selectedUnit = null;
        _selectedAttack = null;
        RefreshUnitAttackList();
        ShowUnitInfo();
    }
    public void SetAttack(Attacks attack)
    {
        _selectedAttack = attack;
        ShowAttackInfo();
    }
    public void DeselectEverything()
    {
       SetSelectedNewUnit(null);
       SetSelectedNewUnit(null);
       SetAttack(null);
    }
    public void AddUnitToTeam()
    {
        if (_selectedUnit == null && _newSelectedUnit == null)
        {
            Debug.Log("No selected Unit");
            return;
        }
        if (_team == null)
        {
            _team = new List<UnitSaveData>();
        }
        if (_team.Count >= _teamSize)
        {
            Debug.Log("Team is full");
            return;
        }
        if (_selectedUnit != null)
        {
            if (_team.Any(u => u.guid == _selectedUnit.guid))
            {
                Debug.Log("Unit already on team");
                return;
            }
            _team.Add(_selectedUnit);
            _selectedUnit = null;
            TeamSaved = false;
            RefreshTeam(true);
            return;
        }
        if (_newSelectedUnit != null)
        {
            UnitSaveData unit = CreateAndSaveUnit();
            if (_team.Any(u => u.guid == unit.guid))
            {
                Debug.Log("Unit already on team");
                return;
            }
            _team.Add(unit);
            _newSelectedUnit = null;
            TeamSaved = false;
            RefreshTeam(true);
            return;
        }
        
    }
    public void RemoveUnitFromTeam()
    {
        if (_team == null || _team.Count() <= 0)
        {
            return;
        }
        _team.Remove(_selectedUnit);
        TeamSaved = false;
        RefreshTeam(true);
    }
    public void AddAttackToUnit()
    {
        if (_selectedAttack == null)
        {
            Debug.Log("No attack is selected");
            return;
        }
        if (_selectedUnit != null)
        {
            List<string> attacks = _selectedUnit.attackNames.ToList();
            if (attacks.Count() >= 4)
            {
                Debug.Log("Unit has 4 attacks already");
                return;
            }
            attacks.Add(_selectedAttack.attackName);
            _selectedUnit.attackNames = attacks.ToArray();
            RefreshUnitAttackList();
            _selectedAttack = null;
        }
        if (_newSelectedUnit != null)
        {
            if (_newSelectedUnit.UnitPrefab.moveSet.Count() >= 4)
            {
                Debug.Log("Unit has 4 attacks already");
                return;
            }
            _newSelectedUnit.UnitPrefab.moveSet.Add(_selectedAttack);
            RefreshUnitAttackList();
            _selectedAttack = null;
        }
    }
    public void RemoveAttackFromUnit()
    {
        if (_selectedAttack == null)
        {
            Debug.Log("No attack is selected");
            return;
        }
        if (_selectedUnit != null)
        {
            List<string> attacks = _selectedUnit.attackNames.ToList();
            attacks.Remove(_selectedAttack.attackName);
            _selectedUnit.attackNames = attacks.ToArray();
            RefreshUnitAttackList();
            _selectedAttack = null;
        }
        if (_newSelectedUnit != null)
        {
            _newSelectedUnit.UnitPrefab.moveSet.Remove(_selectedAttack);
            RefreshUnitAttackList();
            _selectedAttack = null;
        }
    }
    public void SwitchTeam(int slot)
    {
        TeamManager.Instance.ActiveTeamSlot = slot;
        RefreshTeam(false);
        Debug.Log($"Switched to team {slot+1}");
    }
    public void SaveTeam()
    {
        if (_team == null || _team.Count() == 0)
        {
            Debug.Log("No team to save");
            return;
        }
        TeamManager.Instance.SaveTeam(_team);
        TeamSaved = true;
    }
    public void DeleteTeam()
    {
        TeamManager.Instance.DeleteTeam();
        RefreshTeam(true);
    }
    public void RefreshTeam(bool newTeam)
    {
        if (!newTeam)
        {
            _team = TeamManager.Instance.LoadTeamData();
        }
        if (_team == null)
        {
            _team = new List<UnitSaveData>();
        }
        if (_teamUnitsList == null)
        {
            _teamUnitsList = new List<Toggle>();
        }
        
        if (_teamUnitsList.Count() > 0)
        {
            foreach (Toggle teamUnitSelect in _teamUnitsList)
            {
                Destroy(teamUnitSelect.gameObject);
            }
            _teamUnitsList.Clear();
        }
        int x = 300, y = 80;
        foreach (UnitSaveData unitData in _team)
        {
            if (x > 600)
            {
                y -= 110;
                x = 300;
            }
            
            Toggle unitSelect = Instantiate(_unitToggle, _teamPanel);
            RectTransform rect = unitSelect.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(x, y);
            unitSelect.group = _existingUnitToggleGroup;
            ScriptableUnit unit = _allNewUnits.FirstOrDefault(u => u.name == unitData.unitName);
            ToggleSelect toggleScript = unitSelect.GetComponent<ToggleSelect>();
            toggleScript.SetExistingUnit(unitData);

            TMP_Text attackName = unitSelect.GetComponentInChildren<TMP_Text>();
            attackName.text = unitData.unitName;

            BaseHero hero = unit.UnitPrefab as BaseHero;
            Image unitSprite = unitSelect.GetComponentInChildren<Image>();
            unitSprite.sprite = hero.UnitIcon;


            _teamUnitsList.Add(unitSelect);
            x += 150;
        }
    }
    public void SaveUnit()
    {
        CreateAndSaveUnit();
    }
    public void DeselectAttack()
    {
        SetAttack(null);
    }
    public UnitSaveData CreateAndSaveUnit()
    {
        if (_selectedUnit != null)
        {
            TeamManager.Instance.SaveUnitData(_selectedUnit);
            RefreshExistingUnitsList();
            return null;
        }
        if (_newSelectedUnit != null)
        {
            UnitSaveData data = TeamManager.Instance.CreateAndSaveUnit(_newSelectedUnit);
            RefreshExistingUnitsList();
            return data;
        }
        return null;
    }
    public void SetPanelActive(int panel)
    {
        if (panel == 0)
        {
            Debug.Log("Switched to new units");
            _newUnitPanel.gameObject.SetActive(true);
            _existingUnitPanel.gameObject.SetActive(false);
            _attacksPanel.gameObject.SetActive(false);
        }
        if (panel == 1)
        {
            Debug.Log("Switched to existing units");
            _newUnitPanel.gameObject.SetActive(false);
            _existingUnitPanel.gameObject.SetActive(true);
            _attacksPanel.gameObject.SetActive(false);
        }
        if (panel == 2)
        {
            Debug.Log("Switched to attacks");
            if (_selectedUnit == null && _newSelectedUnit == null)
            {
                Debug.Log("Select Unit First!");
                return;
            }
            _newUnitPanel.gameObject.SetActive(false);
            _existingUnitPanel.gameObject.SetActive(false);
            _attacksPanel.gameObject.SetActive(true);
        }
        SetAllTogglesOff();
    }
    public void SetAllTogglesOff()
    {
        foreach (Toggle toggle in _newUnitsList)
        {
            toggle.isOn = false;
        }
        foreach (Toggle toggle in _existingUnitsList)
        {
            toggle.isOn = false;
        }
        foreach (Toggle toggle in _attacksList)
        {
            toggle.isOn = false;
        }
        foreach (Toggle toggle in _unitAttacksList)
        {
            toggle.isOn = false;
        }
        foreach (Toggle toggle in _teamUnitsList)
        {
            toggle.isOn = false;
        }
    }
}
