using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pieces;

public static class PieceStructures
{
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
}

public class PieceStructure
{
    public Vector2Int start;
    public Vector2Int[] structure;
    public int size;
}