using UnityEngine;

public class GridPath
{
    public int X;
    public int Y;
    public Vector3 Position;
    public TileType TileType;
    public GridPath(int x, int y, Vector3 position, TileType tileType){
        X = x;
        Y = y;
        Position = position;
        TileType = tileType;
    }
}
