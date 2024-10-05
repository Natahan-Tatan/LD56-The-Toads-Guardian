using System;
using Godot;

public static class TileMapExtension
{
    public static int GetTileAtPos(this TileMap map, Vector2 pos)
    {
        return map.GetCell(Mathf.FloorToInt(pos.x / map.CellSize.x), Mathf.FloorToInt(pos.y / map.CellSize.y));
    }
}
