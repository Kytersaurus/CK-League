using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int x, y;
    public bool isWalkable;
    public bool isOffset;
}
public class LevelData
{
    public int _width, _height;
    public List<TileData> tiles = new List<TileData>();
}
