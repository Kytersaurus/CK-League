using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;

public class TestEnemyAI
{
    /*[UnityTest]
    public IEnumerator TestEnemyAIWithEnumeratorPasses()
    {        
        GameObject gridManagerObject = new GameObject("GridManager");
        GridManager gridManager = gridManagerObject.AddComponent<GridManager>();
        yield return null;

        TileEntry grass = new TileEntry();
        grass.tileType = TileType.Grass;
        grass.tileVariant = TileVariant.Body1;
        string assetPath = "Assets/Prefabs/Terrain tiles/Grass Tile.prefab";
        GameObject grassPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        Tile grassTile = grassPrefab.GetComponent<GrassTile>();
        grass.prefab = grassTile;
        TileEntry[] testMap = {grass};
        gridManager.SetTestMapPrefabs(testMap);
        yield return null;

        GameObject unitManagerObject = new GameObject("UnitManager");
        UnitManager unitManager = unitManagerObject.AddComponent<UnitManager>();
        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
        BaseUnit hero = MonoBehaviour.Instantiate(Resources.Load<ScriptableUnit>("Units/Heroes/Warrior").UnitPrefab);
        gridManager.GenerateGrid();
        Tile startTile = gridManager.GetTileAtPosition(new Vector2(0,0));
        //startTile.SetUnit(hero);

        yield return null;
        yield return null;
    }*/

    
    [UnityTest, Order(0)]
    public IEnumerator InitTestSuite()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/TestScene.unity");
        yield return null;
    }

    [UnityTest, Order(1)]
    public IEnumerator TestPathFinding()
    {
        GameObject heroObject = new GameObject();
        BaseHero hero = heroObject.AddComponent<BaseHero>();
        GameObject enemyObject = new GameObject();
        BaseEnemy enemy = enemyObject.AddComponent<BaseEnemy>();
        //Tile heroTile = GridManager.Instance.GetTileAtPosition(new Vector2(0,0));
        //heroTile.SetUnit(hero);
        yield return null;
    }

    /*[TearDown]
    public void ResetEnvironment()
    {
        // Reverts the test runner back to a clean, default empty scene state
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }*/
    
}
