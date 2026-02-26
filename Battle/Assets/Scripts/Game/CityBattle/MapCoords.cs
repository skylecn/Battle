using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCoords
{
    public static float GRID_SIZE = 1;
    // 实际地图大小
    public static int MAP_SIZE = 150;

    public static int GetIndex(int x, int y)
    {
        return y * MAP_SIZE + x;
    }

    public static int GetIndex(Vector2Int coor)
    {
        return GetIndex(coor.x, coor.y);
    }

    public static Vector3 CoorToWorldPos(Vector2Int coor)
    {
        return new Vector3(coor.x * GRID_SIZE, 0, coor.y * GRID_SIZE);
    }

    public static Vector3 CoorToWorldPos(Vector2 coor)
    {
        return new Vector3(coor.x * GRID_SIZE, 0, coor.y * GRID_SIZE);
    }

    public static Vector2Int WorldPosToCoor(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x / GRID_SIZE), (int)(pos.z / GRID_SIZE));
    }

    public static Vector2 WorldPosToCoorFloat(Vector3 pos)
    {
        return new Vector2(pos.x / GRID_SIZE, pos.z / GRID_SIZE);
    }
}
