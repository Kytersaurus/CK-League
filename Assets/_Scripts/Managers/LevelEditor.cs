using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor Instance;
    public bool EditingMode = false;
    public TileType selectedType;
    public TileVariant selectedVariant;
    private string _saveName;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GridManager.Instance.LoadGrid($"AllGrass");
        EditingMode = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GUIUtility.keyboardControl != 0)
        {
            return;
        }
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            EditingMode = !EditingMode;
        }
        if (Keyboard.current.sKey.wasPressedThisFrame && EditingMode)
        {
            if (string.IsNullOrEmpty(_saveName))
            {
                Debug.LogError("Please input level name");
                return;
            }
            Debug.Log($"Level {_saveName} saved");
            GridManager.Instance.SaveGrid(_saveName);
        }
        if (Keyboard.current.lKey.wasPressedThisFrame && EditingMode)
        {
            GridManager.Instance.LoadGrid(_saveName);
        }
    }
    void OnGUI()
    {
        if (!EditingMode)
        {
            return;
        }
        GUILayout.BeginArea(new Rect(10, 10, 100, 600));
        GUILayout.Label($"Level Editor, press E to toggle editing mode");
        GUILayout.Label("Level Name: ");
        _saveName = GUILayout.TextField(_saveName);
        GUILayout.Label("S to save, L to load");
        GUILayout.Space(10);

        GUILayout.Label("Tile Types:");
        foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
        {
            if (GUILayout.Button(selectedType == type ? $"[{type}]" : type.ToString()))
            {
                selectedType = type;
            }
        }
        GUILayout.Space(10);

        GUILayout.Label("Tile Variants:");
        foreach (TileVariant variant in System.Enum.GetValues(typeof(TileVariant)))
        {
            if (GUILayout.Button(selectedVariant == variant ? $"[{variant}]" : variant.ToString()))
            {
                selectedVariant = variant;
            }    
        }            
        
        GUILayout.EndArea();
    }
}
