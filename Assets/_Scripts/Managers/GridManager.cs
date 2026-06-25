using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Clrain.Collections;
using UnityEngine.Rendering;
using UnityEditor.PackageManager.UI;
using NUnit.Framework; //priority queue script

[System.Serializable]
public class TileEntry
{
    public TileType tileType;
    public TileVariant tileVariant;
    public Tile prefab;
}

[System.Serializable]
public class TileData
{
    public int posX, posY;
    public bool walkable;
    public TileType tileType;
    public TileVariant tileVariant;
}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public int width, height;
    public List<TileData> tileList = new List<TileData>();
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int _width, _height;
    [SerializeField] private TileEntry[] _tilePrefabs;
    [SerializeField] private Transform _cam;
    private Dictionary<Vector2, Tile> _tiles;
    [SerializeField] private string _levelName;
    void Awake()
    {
        Instance = this;
    }
    //Struct constructor for level creation
    public struct TileInfo
    {
        public TileType type;
        public TileVariant variant;
        public TileInfo(TileType type, TileVariant variant)
        {
            this.type = type;
            this.variant = variant;
        }
    }
    //Level
    
    public void SetupGrid()
    {
        if (string.IsNullOrEmpty(_levelName))
        {
            GenerateGrid();
        }
        else
        {
            LoadGrid(_levelName);
        }
    }
    public void GenerateGrid()
    {
        LoadGrid("AllGrass");
    }

    public Tile GenerateTile(int x, int y, TileType type, TileVariant variant)
    {
        Tile prefab = GetTilePrefab(type, variant);
        Tile spawnedTile = Instantiate(prefab, new Vector3(x, y), Quaternion.identity);
        spawnedTile.name = $"Tile {x} {y} ({type} {variant})";
        _tiles[new Vector2(x, y)] = spawnedTile;
        return spawnedTile;
    }

    public Tile GetTilePrefab(TileType type, TileVariant variant)
    {
        foreach (TileEntry entry in _tilePrefabs)
        {
            if (entry.tileType == type && entry.tileVariant == variant)
            {    
                return entry.prefab;
            }
        }
        return _tilePrefabs[0].prefab;
    }

    
    private string GetSavePath(string levelName)
    {
        return Path.Combine(Application.streamingAssetsPath, $"Levels/{levelName}.json");
    }

    #if UNITY_EDITOR
    public void SaveGrid(string levelName)
    {
        var levelData = new LevelData 
        { 
            width = _width, 
            height = _height 
        };
        foreach (var kvp in _tiles)
        {
            levelData.tileList.Add(new TileData
            {
                posX = (int)kvp.Key.x,
                posY = (int)kvp.Key.y,
                walkable = kvp.Value.Walkable,
                tileType = kvp.Value.TileType,
                tileVariant = kvp.Value.TileVariant
            });
        }
        File.WriteAllText(GetSavePath(levelName), JsonUtility.ToJson(levelData, true));
        UnityEditor.AssetDatabase.Refresh();
    }
    #endif

    public void LoadGrid(string levelName)
    {
        string path = GetSavePath(levelName);
        if (!File.Exists(path))
        {
            Debug.LogError($"Level {levelName} not found at {path}, defaulting to sample grid");
            GenerateGrid();
            return;
        }

        var levelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
        _width = levelData.width;
        _height = levelData.height;
        _tiles = new Dictionary<Vector2, Tile>();

        foreach (var data in levelData.tileList)
        {
            var tile = GenerateTile(data.posX, data.posY, data.tileType, data.tileVariant);
            tile.Init(data.posX, data.posY);
        }

        _cam.position = new Vector3(_width / 2f - 0.5f, _height / 2f - 0.5f, -10);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.SpawnEnemies);    
        }
    }
    
    public void PlaceTile(Vector2 pos, TileType type, TileVariant variant)
    {
        if (!_tiles.TryGetValue(pos, out var oldTile))
        {
            return;
        }
        int x = (int)pos.x;
        int y = (int)pos.y;
        Destroy(oldTile.gameObject);
        var newTile = GenerateTile(x, y, type, variant);
        newTile.Init(x, y);
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles
            .Where(t => t.Key.x > _width / 2 && t.Value.Walkable)
            .OrderBy(_ => Random.value)
            .First().Value;
    }
    public Tile GetHeroSpawnTile()
    {
        return _tiles
            .Where(t => t.Key.x < _width / 2 && t.Value.Walkable)
            .OrderBy(_ => Random.value)
            .First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
            return tile;
        return null;
    }

    public List<Tile> GetNeighbourTiles(Tile root)
    {
        var nbs = new List<Tile>();
        Vector2 rootPos = root.GridPos;
        Vector2[] directions =
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };
        foreach (Vector2 dir in directions)
        {
            Tile nb = GetTileAtPosition(rootPos + dir);
            if (nb != null)
            {
                nbs.Add(nb);
            }
        }
        return nbs;
    }
    public List<Tile> GetTileInAOERAnge(Tile origin, int range)
    {
        var dists = new Dictionary<Tile, int>();
        var vis = new HashSet<Tile>();
        var q = new Queue<Tile>();

        foreach (Tile tile in _tiles.Values)
        {
            dists[tile] = int.MaxValue;
        }
        dists[origin] = 0;
        q.Enqueue(origin);
        while (q.Count > 0)
        {
            Tile curr = q.Dequeue();
            if (!vis.Add(curr))
            {
                continue;
            }
            List<Tile> nbs = GetNeighbourTiles(curr);
            foreach(Tile nb in nbs)
            {
                if (vis.Contains(nb))
                {
                    continue;
                }
                if (dists[curr] + 1 <= range)
                {
                    dists[nb] = dists[curr] + 1;
                    q.Enqueue(nb);
                }
            }
        }
        return dists
            .Where(kvp => kvp.Value <= range && kvp.Key != origin)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    public List<Tile> GetReachableTiles(Tile from, int moveRange)
    {
        var dists = new Dictionary<Tile, int>();
        var vis = new HashSet<Tile>(); 
        var pq = new PriorityQueue<Tile, int>(); //using pq script from internet as unity does not support it natively
        
        foreach (Tile tile in _tiles.Values)
        {
            dists[tile] = int.MaxValue;
        }
        dists[from] = 0;
        pq.Enqueue(from, 0);

        while (pq.Count > 0)
        {
            Tile curr = pq.Dequeue();
            List<Tile> nbs = GetNeighbourTiles(curr);
            if (!vis.Add(curr))
            {
                continue;
            }
            if (dists[curr] > moveRange)
            {
                continue;
            }
            foreach (Tile nb in nbs)
            {
                if (vis.Contains(nb) || !nb.Walkable)
                {
                    continue;
                }
                int newDist = dists[curr] + nb.MoveCost;
                if (newDist <= moveRange && newDist < dists[nb])
                {
                    dists[nb] = newDist;
                    pq.Enqueue(nb, newDist);
                }
            }
        }
        return dists
        //only return list of tiles in range
            .Where(kvp => kvp.Value <= moveRange && kvp.Key != from)
            .Select(kvp => kvp.Key)
            .ToList();
    }
}