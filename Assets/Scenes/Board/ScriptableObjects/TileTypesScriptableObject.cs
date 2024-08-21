using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileTypes", menuName = "ScriptableObjects/TileTypes", order = 1)]
public class TileTypesScriptableObject : ScriptableObject
{
    public TileType S;
    public TileType Z;
    public TileType L;
    public TileType J;
    public TileType I;
    public TileType O;
    public TileType T;
    public TileType Empty;

    public TileType GetTileType(string type)
    {
        return type switch
        {
            "S" => S,
            "Z" => Z,
            "L" => L,
            "J" => J,
            "I" => I,
            "O" => O,
            "T" => T,
            _ => Empty,
        };
    }
}

[Serializable]
public class TileType
{
    public Sprite sprite;
}
