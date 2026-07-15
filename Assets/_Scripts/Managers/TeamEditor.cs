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
    [SerializeField] private Toggle _unitToggle, _newUnitPanelToggle, _attacksPanelToggle;
    [SerializeField] private List<Toggle> _teamSelectToggles;
    [SerializeField] private Button _deselectUnitButton, _deselectAttackButton, _addUnitToTeamButton, _deleteUnitButton, _saveUnitButton, _addAttackButton, _removeAttackButton, _saveTeamButton, _deleteTeamButton, _removeUnitFromTeamButton;
    private List<Toggle> _newUnitsList, _existingUnitsList, _attacksList, _unitAttacksList, _teamUnitsList;
    public bool TeamSaved;
    [SerializeField] GameObject _popUp, _popUpConfirmSwitchNoSave, _popUpConfirmDeleteTeam, _popUpConfirmDeleteUnit;
    private PopUpScript _popUpScript;
    private int _prevTeamSlot, _teamSlot;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        _popUpScript = _popUp.GetComponent<PopUpScript>();
        RefreshNewUnitsList();
        RefreshExistingUnitsList();
        RefreshAttacksList();
        TeamManager.Instance.ActiveTeamSlot = 0;
        _teamSelectToggles[0].isOn = true;
        SwitchTeam(0);
        SetPanelActive(0);
        PopUpScript popScript = _popUpConfirmSwitchNoSave.GetComponent<PopUpScript>();
        popScript.DismissPopup();
        _newUnitPanelToggle.isOn = true;
        ShowUnitInfo();
        ShowAttackInfo();
        RefreshTeam(false);
    }

    void Update()
    {
        if (_selectedUnit == null && _newSelectedUnit == null)
        {
            _attacksPanelToggle.interactable = false;
        }
        if (_selectedUnit != null || _newSelectedUnit != null)
        {
            _attacksPanelToggle.interactable = true;
        }
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
        }
        if (_team != null && _team.Count() > 0 && !TeamSaved)
        {
            _saveTeamButton.gameObject.SetActive(true);
        }
        if (!TeamManager.Instance.SavedTeamExists)
        {
            _deleteTeamButton.gameObject.SetActive(false);
        }
        if (TeamManager.Instance.SavedTeamExists)
        {
            _deleteTeamButton.gameObject.SetActive(true);
        }
        if (!_team.Contains(_selectedUnit))
        {
            _removeUnitFromTeamButton.gameObject.SetActive(false);
        }
        if (_team.Contains(_selectedUnit))
        {
            _removeUnitFromTeamButton.gameObject.SetActive(true);
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
            unitStats.text = $"{unit.name}";

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
        _selectedUnitImage.gameObject.SetActive(isActive);
        _selectedUnitDesc.gameObject.SetActive(isActive);
        if (!isActive)
        {
            return;
        }

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
        if (_selectedUnit != null)
        {
            List<string> attacks = _selectedUnit.attackNames.ToList();
            if (attacks.Contains(_selectedAttack.attackName))
            {
                _addAttackButton.gameObject.SetActive(false);
                _removeAttackButton.gameObject.SetActive(true);
            }
            else
            {
                _addAttackButton.gameObject.SetActive(true);
                _removeAttackButton.gameObject.SetActive(false);
            }
        }
        if (_newSelectedUnit != null)
        {
            if (_newSelectedUnit.UnitPrefab.moveSet.Contains(_selectedAttack))
            {
                _addAttackButton.gameObject.SetActive(false);
                _removeAttackButton.gameObject.SetActive(true);
            }
            else
            {
                _addAttackButton.gameObject.SetActive(true);
                _removeAttackButton.gameObject.SetActive(false);
            }
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
        SetAllTogglesOff();
        ShowUnitInfo();
        ShowAttackInfo();
        if (_attacksPanelToggle.isOn)
        {
            SetPanelActive(0);
            _newUnitPanelToggle.isOn = true;
        }
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
            _popUpScript.ShowPopUpAndSetText($"Team is full {_team.Count}/{_teamSize}");
            return;
        }
        if (_selectedUnit != null)
        {
            if (_team.Any(u => u.guid == _selectedUnit.guid))
            {
                _popUpScript.ShowPopUpAndSetText("The unit is already on the team");
                return;
            }
            _team.Add(_selectedUnit);
            _selectedUnit = null;
            SetAllTogglesOff();
            TeamSaved = false;
            RefreshTeam(true);
            ShowUnitInfo();
            RefreshUnitAttackList();
            return;
        }
        if (_newSelectedUnit != null)
        {
            UnitSaveData unit = CreateAndSaveUnit();
            _team.Add(unit);
            _newSelectedUnit = null;
            SetAllTogglesOff();
            TeamSaved = false;
            RefreshTeam(true);
            ShowUnitInfo();
            RefreshUnitAttackList();
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
        _selectedUnit = null;
        ShowUnitInfo();
        RefreshUnitAttackList();
        SetAllTogglesOff();
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
            ScriptableUnit unit = _allNewUnits.FirstOrDefault(u => u.name == _selectedUnit.unitName);
            if (_selectedAttack is MeleeAttack && (unit.UnitPrefab is BaseArcher || unit.UnitPrefab is BaseMage))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit cannot take Melee Attacks");
                return;
            }
            if (!(_selectedAttack is MeleeAttack && !(_selectedAttack is Heals)) && (unit.UnitPrefab is BaseWarrior || unit.UnitPrefab is BaseCavalry))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit cannot take ranged attacks");
                return;
            }
            if (_selectedAttack is MagicAttack && !(unit.UnitPrefab is BaseMage))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit cannot take magic attacks");
                return;
            }
            List<string> attacks = _selectedUnit.attackNames.ToList();
            if (attacks.Count() >= 4)
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit has 4 attacks already");
                return;
            }
            if (attacks.Contains(_selectedAttack.attackName))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit already has this attack equipped");
                return;
            }
            attacks.Add(_selectedAttack.attackName);
            _selectedUnit.attackNames = attacks.ToArray();
            RefreshUnitAttackList();
            _selectedAttack = null;
            SetAllAttackTogglesOff();
        }
        if (_newSelectedUnit != null)
        {
            ScriptableUnit unit = _newSelectedUnit;
            if (_selectedAttack is MeleeAttack && (unit.UnitPrefab is BaseArcher || unit.UnitPrefab is BaseMage))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit cannot take Melee Attacks");
                return;
            }
            if (!(_selectedAttack is MeleeAttack && !(_selectedAttack is Heals)) && (unit.UnitPrefab is BaseWarrior || unit.UnitPrefab is BaseCavalry))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit cannot take ranged attacks");
                return;
            }
            if (_selectedAttack is MagicAttack && !(unit.UnitPrefab is BaseMage))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit cannot take magic attacks");
                return;
            }
            if (_newSelectedUnit.UnitPrefab.moveSet.Count() >= 4)
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit has 4 attacks already");
                return;
            }
            if (_newSelectedUnit.UnitPrefab.moveSet.Contains(_selectedAttack))
            {
                _popUpScript.ShowPopUpAndSetText("Selected Unit already has this attack equipped");
                return;
            }
            _newSelectedUnit.UnitPrefab.moveSet.Add(_selectedAttack);
            RefreshUnitAttackList();
            _selectedAttack = null;
            SetAllAttackTogglesOff();
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
            ShowAttackInfo();
            SetAllAttackTogglesOff();
        }
        if (_newSelectedUnit != null)
        {
            _newSelectedUnit.UnitPrefab.moveSet.Remove(_selectedAttack);
            RefreshUnitAttackList();
            _selectedAttack = null;
            ShowAttackInfo();
            SetAllAttackTogglesOff();
        }
    }
    public void ConfirmSwitchTeam(int slot)
    {
        _prevTeamSlot = TeamManager.Instance.ActiveTeamSlot;
        _teamSlot = slot;
        if (TeamManager.Instance.ActiveTeamSlot == slot)
        {
            return;
        }
        if (!TeamSaved)
        {
            PopUpScript popUpConfirmScript = _popUpConfirmSwitchNoSave.GetComponent<PopUpScript>();
            popUpConfirmScript.ShowPopUpAndSetText("Team is not saved, switching will void all changes");
            return;
        }
        else
        {
            SwitchTeam(slot);
        }
    }
    public void SwitchTeam(int slot)
    {
        if (slot == -1)
        {
            slot = _teamSlot;
        }
        if (slot == -2)
        {
            slot = _prevTeamSlot;
            _teamSelectToggles[slot].isOn = true;
            return;
        }
        TeamManager.Instance.ActiveTeamSlot = slot;
        RefreshTeam(false);
        TeamSaved = true;
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
    public void ConfirmDeleteTeam()
    {
        PopUpScript popUpConfirmScript = _popUpConfirmDeleteTeam.GetComponent<PopUpScript>();
        popUpConfirmScript.ShowPopUpAndSetText("Confirm Deletion?");
    }
    public void DeleteTeam()
    {
        TeamManager.Instance.DeleteTeam();
        _team = null;
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
    public void ConfirmDeleteUnit()
    {
        PopUpScript popUpConfirmScript = _popUpConfirmDeleteUnit.GetComponent<PopUpScript>();
        popUpConfirmScript.ShowPopUpAndSetText("Confirm Deletion?");
    }
    public void DeleteUnit()
    {
        if (_newSelectedUnit != null)
        {
            Debug.Log("Cannot delete prefab");
            return;
        }
        if(_selectedUnit != null)
        {
            TeamManager.Instance.DeleteUnit(_selectedUnit.guid);
        }
        _selectedUnit = null;
        ShowUnitInfo();
        RefreshUnitAttackList();
        ShowAttackInfo();
        RefreshExistingUnitsList();
    }
    public void DeselectAttack()
    {
        SetAttack(null);
        ShowAttackInfo();
        SetAllAttackTogglesOff();
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
        if (_newUnitsList != null)
        {
            foreach (Toggle toggle in _newUnitsList)
            {
                toggle.isOn = false;
            }
        }
        if (_existingUnitsList != null)
        {
            foreach (Toggle toggle in _existingUnitsList)
            {
            toggle.isOn = false;
            }
        }
        if (_attacksList != null)
        {
            foreach (Toggle toggle in _attacksList)
            {
                toggle.isOn = false;
            }
        }
        if (_unitAttacksList != null)
        {
            foreach (Toggle toggle in _unitAttacksList)
            {
                toggle.isOn = false;
            }
        }
        if (_teamUnitsList != null)
        {
            foreach (Toggle toggle in _teamUnitsList)
            {
                toggle.isOn = false;
            }
        }
    }
    public void SetAllAttackTogglesOff()
    {
        if (_attacksList != null)
        {
            foreach (Toggle toggle in _attacksList)
            {
                toggle.isOn = false;
            }
        }
        if (_unitAttacksList != null)
        {
            foreach (Toggle toggle in _unitAttacksList)
            {
                toggle.isOn = false;
            }
        }
    }
    
}
