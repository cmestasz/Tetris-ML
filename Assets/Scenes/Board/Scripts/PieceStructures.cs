using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pieces;

public static class PieceStructures
{
    private static readonly Vector2Int[][] kicksNotI = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) },
        new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2) }
    };

    private static readonly Vector2Int[][] kicksI = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(-2, 0), new(1, 2), new(-2, -1) },
        new Vector2Int[] { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1) },
        new Vector2Int[] { new(0, 0), new(-1, 0), new(2, 0), new(-1, -2), new(2, 1) }
    };

    private static readonly Vector2Int[][] kicksNotI180 = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0), new(0, -1), new(-1, -1), new(1, -1), new(-1, 0), new(1, 0) },
        new Vector2Int[] { new(0, 0), new(-1, 0), new(-1, 2), new(-1, 1), new(0, 2), new(0, 1) },
        new Vector2Int[] { new(0, 0), new(0, 1), new(1, 1), new(-1, 1), new(1, 0), new(-1, 0) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, 2), new(1, 1), new(0, 2), new(0, 1) }
    };

    private static readonly Vector2Int[][] kicksI180 = new Vector2Int[][]
    {
        new Vector2Int[] { new(0, 0), new(0, -1) },
        new Vector2Int[] { new(0, 0), new(-1, 0) },
        new Vector2Int[] { new(0, 0), new(0, 1) },
        new Vector2Int[] { new(0, 0), new(1, 0) }
    };

    private static readonly PieceStructure I = new()
    {
        start = new Vector2Int(3, 19),
        structure = new Vector2Int[]
        {
            new(0, 2),
            new(1, 2),
            new(2, 2),
            new(3, 2)
        },
        kicks = kicksI,
        kicks180 = kicksI180,
        size = 4
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
        kicks = kicksNotI,
        kicks180 = kicksNotI180,
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
        kicks = kicksNotI,
        kicks180 = kicksNotI180,
        size = 3
    };

    private static readonly PieceStructure O = new()
    {
        start = new Vector2Int(4, 20),
        structure = new Vector2Int[]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        },
        kicks = kicksNotI,
        kicks180 = kicksNotI180,
        size = 2
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
        kicks = kicksNotI,
        kicks180 = kicksNotI180,
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
        kicks = kicksNotI,
        kicks180 = kicksNotI180,
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
        kicks = kicksNotI,
        kicks180 = kicksNotI180,
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

    private static readonly Vector2 half = new(0.5f, 0.5f);
    private const int NEG90SIN = -1;
    private static void RotateStructure(Vector2Int[] structure, int size)
    {
        float c = size / 2f;
        Vector2 center = new(c, c);
        for (int i = 0; i < structure.Length; i++)
        {
            Vector2 diff = structure[i] + half - center;
            float relX = -diff.y * NEG90SIN;
            float relY = diff.x * NEG90SIN;
            structure[i] = new Vector2Int(Mathf.RoundToInt(relX - half.x + center.x), Mathf.RoundToInt(relY - half.y + center.y));
        }
    }
}

public class PieceStructure
{
    public Vector2Int start;
    public Vector2Int[] structure;
    public Vector2Int[][] kicks;
    public Vector2Int[][] kicks180;
    public int size;
}