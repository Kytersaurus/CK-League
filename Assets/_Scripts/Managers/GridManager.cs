using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Clrain.Collections; //priority queue script

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
    private string SavePath => Path.Combine(Application.persistentDataPath, "level.json");

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
    private TileInfo[,] _levelLayout = new TileInfo[,]
    {
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Marsh, TileVariant.EdgeBL), new TileInfo(TileType.Marsh, TileVariant.Body1), new TileInfo(TileType.Marsh, TileVariant.EdgeTL), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Marsh, TileVariant.Body2), new TileInfo(TileType.Marsh, TileVariant.Body2), new TileInfo(TileType.Marsh, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Marsh, TileVariant.EdgeBR), new TileInfo(TileType.Marsh, TileVariant.Body1), new TileInfo(TileType.Marsh, TileVariant.EdgeTR), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Mountain, TileVariant.PillarBase), new TileInfo(TileType.Mountain, TileVariant.Pillar), new TileInfo(TileType.Mountain, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Water, TileVariant.EdgeBL), new TileInfo(TileType.Water, TileVariant.EdgeTL), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Water, TileVariant.EdgeBR), new TileInfo(TileType.Water, TileVariant.EdgeTR), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Tree, TileVariant.Body1), new TileInfo(TileType.Mountain, TileVariant.PillarBase), new TileInfo(TileType.Mountain, TileVariant.Pillar), new TileInfo(TileType.Mountain, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
        { new TileInfo(TileType.Mountain, TileVariant.PillarBase), new TileInfo(TileType.Mountain, TileVariant.Pillar), new TileInfo(TileType.Mountain, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1), new TileInfo(TileType.Grass, TileVariant.Body1) },
    };
    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                TileInfo info = _levelLayout[x, y];
                Tile spawnedTile = GenerateTile(x, y, info.type, info.variant);
                spawnedTile.Init(x, y);
            }
        }
        _cam.position = new Vector3(_width / 2f - 0.5f, _height / 2f - 0.5f, -10);
        GameManager.Instance.UpdateGameState(GameState.SpawnEnemies);
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

    public void LoadGrid()
    {
        if (!File.Exists(SavePath))
        {
            GenerateGrid();
            return;
        }

        var levelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(SavePath));
        _width = levelData.width;
        _height = levelData.height;
        _tiles = new Dictionary<Vector2, Tile>();

        foreach (var data in levelData.tileList)
        {
            var tile = GenerateTile(data.posX, data.posY, data.tileType, data.tileVariant);
            tile.Init(data.posX, data.posY);
        }

        _cam.position = new Vector3(_width / 2f - 0.5f, _height / 2f - 0.5f, -10);
        GameManager.Instance.UpdateGameState(GameState.SpawnEnemies);
    }

    public void SaveGrid()
    {
        var levelData = new LevelData { width = _width, height = _height };
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
        File.WriteAllText(SavePath, JsonUtility.ToJson(levelData, true));
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles
            .Where(t => t.Key.x > _width / 2 && t.Value.Walkable)
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
        Vector2 rootPos = root.gridPos;
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