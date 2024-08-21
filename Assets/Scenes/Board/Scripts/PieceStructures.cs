using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pieces;

public static class PieceStructures
{
    private static readonly Vector2Int[][] offsetJLSTZ = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) },
        new Vector2Int[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0), new(0, 0) },
        new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) }
    };

    private static readonly Vector2Int[][] offsetI = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0), new(-1, 0), new(2, 0), new(-1, 0), new(2, 0) },
        new Vector2Int[] { new(-1, 0), new(0, 0), new(0, 0), new(0, 1), new(0, -2) },
        new Vector2Int[] { new(-1, 1), new(1, 1), new(-2, 1), new(1, 0), new(-2, 0) },
        new Vector2Int[] { new(0, 1), new(0, 1), new(0, 1), new(0, -1), new(0, 2) }
    };

    private static readonly Vector2Int[][] offsetO = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0) },
        new Vector2Int[] { new(0, 1) },
        new Vector2Int[] { new(1, 1) },
        new Vector2Int[] { new(1, 0) }
    };

    private static readonly PieceStructure I = new()
    {
        start = new Vector2Int(2, 19),
        structure = new Vector2Int[]
        {
            new(1, 2),
            new(2, 2),
            new(3, 2),
            new(4, 2)
        },
        offsets = offsetI,
        size = 5
    };

    private static readonly PieceStructure J = new()
    {
        start = new Vector2Int(3, 20),
        structure = new Vector2Int[]
        {
            new(0, 2),
            new(0, 1),
            new(1, 1),
            new(2, 1)
        },
        offsets = offsetJLSTZ,
        size = 3
    };

    private static readonly PieceStructure L = new()
    {
        start = new Vector2Int(3, 20),
        structure = new Vector2Int[]
        {
            new(0, 1),
            new(1, 1),
            new(2, 1),
            new(2, 2)
        },
        offsets = offsetJLSTZ,
        size = 3
    };

    private static readonly PieceStructure O = new()
    {
        start = new Vector2Int(3, 20),
        structure = new Vector2Int[]
        {
            new(1, 2),
            new(2, 2),
            new(1, 1),
            new(2, 1)
        },
        offsets = offsetO,
        size = 3
    };

    private static readonly PieceStructure S = new()
    {
        start = new Vector2Int(3, 20),
        structure = new Vector2Int[]
        {
            new(0, 1),
            new(1, 1),
            new(1, 2),
            new(2, 2)
        },
        offsets = offsetJLSTZ,
        size = 3
    };

    private static readonly PieceStructure T = new()
    {
        start = new Vector2Int(3, 20),
        structure = new Vector2Int[]
        {
            new(1, 2),
            new(0, 1),
            new(1, 1),
            new(2, 1)
        },
        offsets = offsetJLSTZ,
        size = 3
    };

    private static readonly PieceStructure Z = new()
    {
        start = new Vector2Int(3, 20),
        structure = new Vector2Int[]
        {
            new(0, 2),
            new(1, 2),
            new(1, 1),
            new(2, 1)
        },
        offsets = offsetJLSTZ,
        size = 3
    };

    public static Dictionary<Piece, PieceStructure> pieceStructures = new()
    {
        { Piece.I, I },
        { Piece.J, J },
        { Piece.L, L },
        { Piece.O, O },
        { Piece.S, S },
        { Piece.T, T },
        { Piece.Z, Z }
    };

    public static Vector2Int[] RotateStructure(Vector2Int[] structure, int size, int times)
    {
        Vector2Int[] newStructure = structure.Clone() as Vector2Int[];
        for (int i = 0; i < times; i++)
        {
            RotateStructure(newStructure, size);
        }
        return newStructure;
    }

    private static void RotateStructure(Vector2Int[] structure, int size)
    {
        Vector2Int center = new(size / 2, size / 2);
        for (int i = 0; i < structure.Length; i++)
        {
            Vector2Int diff = structure[i] - center;
            int relX = Mathf.RoundToInt(diff.x * Mathf.Cos(-Mathf.PI / 2) - diff.y * Mathf.Sin(-Mathf.PI / 2));
            int relY = Mathf.RoundToInt(diff.x * Mathf.Sin(-Mathf.PI / 2) + diff.y * Mathf.Cos(-Mathf.PI / 2));
            structure[i] = new Vector2Int(center.x + relX, center.y + relY);
        }
    }
}

public class PieceStructure
{
    public Vector2Int start;
    public Vector2Int[] structure;
    public Vector2Int[][] offsets;
    public int size;
}