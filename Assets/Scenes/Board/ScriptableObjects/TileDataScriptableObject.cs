using System;
using UnityEngine;
using static Pieces;

[CreateAssetMenu(fileName = "TileTypes", menuName = "ScriptableObjects/TileTypes", order = 1)]
public class TileDataScriptableObject : ScriptableObject
{
    public TileData I;
    public TileData J;
    public TileData L;
    public TileData O;
    public TileData S;
    public TileData T;
    public TileData Z;
    public TileData Garbage;
    public TileData Empty;

    public TileData GetTileType(Piece piece)
    {
        return piece switch
        {
            Piece.I => I,
            Piece.J => J,
            Piece.L => L,
            Piece.O => O,
            Piece.S => S,
            Piece.T => T,
            Piece.Z => Z,
            Piece.Garbage => Garbage,
            _ => Empty,
        };
    }
}

[Serializable]
public class TileData
{
    public Sprite sprite;
}
