using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Clrain.Collections;
using UnityEngine.UI; //priority queue script

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
    public List<Tile> SpawnTiles {get; private set;}
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
    public void SpawnCamPosition()
    {
        _cam.position = new Vector3(_width / 2f - 1.5f, _height / 2f - 1f, -10);
    }
    public void DefaultCamPosition()
    {
        _cam.position = new Vector3(_width / 2f - 0.5f, _height / 2f - 1f, -10);
    }
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
    public void GetSpawnTiles()
    {
        SpawnTiles = _tiles
                        .Where(kvp => kvp.Key.x <= UnitManager.Instance.SpawnBox.x && kvp.Key.y >= UnitManager.Instance.SpawnBox.y)
                        .Select(kvp => kvp.Value)
                        .ToList();
    }
    public void HighlightSpawnTiles(bool isActive)
    {
        if (SpawnTiles == null || SpawnTiles.Count == 0)
        {
            return;
        }
        foreach (Tile tile in SpawnTiles)
        {
            tile.highlight.SetActive(isActive);
        }
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
        DefaultCamPosition();
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
    public Tile GetSpecificSpawnTile(int x, int y, bool isHero)
    {
        var key = new Vector2(x, y);
        if (!_tiles.TryGetValue(key, out Tile tile))
        {
            Debug.Log("Tile for unit spawn does not exist");
            return isHero ? GetHeroSpawnTile() : GetEnemySpawnTile();
        }
        if (!tile.Walkable)
        {
            Debug.Log("Tile for unit spawn is blocked by an obstacle");
            return isHero ? GetHeroSpawnTile() : GetEnemySpawnTile();
        }
        return tile;
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

    public List<Tile> GetReachableTiles(BaseUnit unit, int moveRange)
    {
        Tile from = unit.OccupiedTile;
        var dists = new Dictionary<Tile, int>();
        var vis = new HashSet<Tile>(); 
        var pq = new PriorityQueue<Tile, int>(); //using pq script from internet as unity does not support it natively
        var previousTile = new Dictionary<Tile, Tile>();
        
        foreach (Tile tile in _tiles.Values)
        {
            dists[tile] = int.MaxValue;
            previousTile[tile] = null;
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
                    previousTile[nb] = curr;
                    pq.Enqueue(nb, newDist);
                }
            }
        }
        unit.PathDictionary = previousTile;
        return dists
        //only return list of tiles in range
            .Where(kvp => kvp.Value <= moveRange && kvp.Key != from)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    public Tile GetEnemyPath(BaseUnit unit, List<BaseUnit> remainingHeroes)
    {
        int difficulty = GameManager.Instance.EnemyDifficulty;
        Tile startingTile = unit.OccupiedTile;
        var dists = new Dictionary<Tile, int>();
        var vis = new HashSet<Tile>(); 
        var pq = new PriorityQueue<Tile, int>(); //using pq script from internet as unity does not support it natively
        var previousTile = new Dictionary<Tile, Tile>();
        
        //Djikstra's Algorithm for entire map
        foreach (Tile tile in _tiles.Values)
        {
            dists[tile] = int.MaxValue;
            previousTile[tile] = null;
        }
        dists[startingTile] = 0;
        pq.Enqueue(startingTile, 0);

        while (pq.Count > 0)
        {
            Tile curr = pq.Dequeue();
            List<Tile> nbs = GetNeighbourTiles(curr);
            if (!vis.Add(curr))
            {
                continue;
            }
            foreach (Tile nb in nbs)
            {
                if (vis.Contains(nb) || (!nb.Walkable && nb.OccupiedUnit == null))
                {
                    continue;
                }
                int newDist = dists[curr] + nb.MoveCost;
                if (newDist < dists[nb])
                {
                    dists[nb] = newDist;
                    previousTile[nb] = curr;
                    if(nb.OccupiedUnit == null || nb.OccupiedUnit.Faction == Faction.Enemy)
                    {
                        pq.Enqueue(nb, newDist);
                    }
                }
            }
        }

        //Retreat if surrounded (difficulty 2 and above only)
        if(difficulty >= 2)
        {
            List<Tile> neighbouringTiles = GetNeighbourTiles(startingTile);
            neighbouringTiles.OrderBy(o=>o.transform.position.x).ToList(); //left before right
            int neighbouringHeroesCount = 0;
            Tile escapeTile = startingTile;
            foreach(Tile tile in neighbouringTiles)
            {
                if(tile.OccupiedUnit != null && tile.OccupiedUnit.Faction == Faction.Hero)
                {
                    neighbouringHeroesCount++;
                }
                else if(tile.OccupiedUnit == null && tile.MoveCost == 1)
                {
                    escapeTile = tile; //neighbouringTiles is sorted by left, up, down, right
                }
            }

            if(neighbouringHeroesCount > 1)
            {
                return escapeTile;
            }
        }

        //Find closest hero
        BaseHero closestHero = null;
        int closestDistance = int.MaxValue;
        Tile targetTile = null;
        foreach(BaseHero hero in remainingHeroes)
        {
            if(dists[hero.OccupiedTile] < closestDistance)
            {
                closestHero = hero;
                closestDistance = dists[hero.OccupiedTile];
                targetTile = hero.OccupiedTile; 
            }
        }
        if(difficulty >= 3)//override if empty tile next to hero is found
        {
            closestDistance = int.MaxValue;
            foreach(BaseHero hero in remainingHeroes)
            {
                List<Tile> neighbourTiles = GetNeighbourTiles(hero.OccupiedTile);
                foreach(Tile tile in neighbourTiles)
                {
                    if(dists[tile] < closestDistance && (tile.Walkable || tile.OccupiedUnit == unit))
                    {
                        targetTile = tile;
                        closestDistance = dists[tile];
                        closestHero = hero;
                    }
                }
            }
        }
        
        //Find tile in enemy movement range that is in the path towards closest hero
        if(closestHero == null)
        {
            return unit.OccupiedTile;
        }
        Tile validTile = targetTile;
        var reachableTiles = GetReachableTiles(unit, unit.moveRange);
        while (!reachableTiles.Contains(validTile) && validTile != unit.OccupiedTile)
        {
            validTile = previousTile[validTile];
        }
        return validTile;
                                                       
    }
    public void ShowUnitDest(BaseUnit unit, bool show)
    {
        if (unit.DestinationTile == null)
        {
            return;
        }
        unit.DestinationTile.Objective.SetActive(show);
        if (!show)
        {
            return;
        }
        SpriteRenderer ghostUnitSR = unit.DestinationTile.Objective.GetComponent<SpriteRenderer>();
        SpriteRenderer unitSR = unit.GetComponent<SpriteRenderer>();
        ghostUnitSR.sprite = unit.UnitIcon;
        ghostUnitSR.transform.localScale = unitSR.transform.localScale;

        Color ghostColour = ghostUnitSR.color;
        ghostColour.a = 0.6f;
        ghostUnitSR.color = ghostColour;
    }
}