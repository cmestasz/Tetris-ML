using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pieces
{
    public enum State
    {
        P,
        N,
        C
    }

    private const State P = State.P;
    private const State N = State.N;
    private const State C = State.C;

    public static Vector2Int I_START = new(3, 20); 
    public static Vector2Int J_START = new(3, 20);
    public static Vector2Int L_START = new(2, 20);
    public static Vector2Int O_START = new(3, 20);
    public static Vector2Int S_START = new(3, 20);
    public static Vector2Int T_START = new(3, 20);
    public static Vector2Int Z_START = new(3, 20);
    public static State[,] I = new State[5, 5]
    {
        {N, N, P, N, N},
        {N, N, P, N, N},
        {N, N, C, N, N},
        {N, N, P, N, N},
        {N, N, N, N, N}
    };

    public static State[,] J = new State[3, 3]
    {
        {N, P, N},
        {N, C, N},
        {P, P, N}
    };

    public static State[,] L = new State[3, 3]
    {
        {P, P, N},
        {N, C, N},
        {N, P, N}
    };

    public static State[,] O = new State[3, 3]
    {
        {P, P, N},
        {P, C, N},
        {N, N, N}
    };

    public static State[,] S = new State[3, 3]
    {
        {P, N, N},
        {P, C, N},
        {N, P, N}
    };

    public static State[,] T = new State[3, 3]
    {
        {N, P, N},
        {P, C, N},
        {N, P, N}
    };

    public static State[,] Z = new State[3, 3]
    {
        {N, P, N},
        {P, C, N},
        {P, N, N}
    };

    public static State[,] RotatePiece(State[,] piece)
    {
        State[,] rotatedPiece = new State[piece.GetLength(0), piece.GetLength(1)];
        for (int i = 0; i < piece.GetLength(0); i++)
        {
            for (int j = 0; j < piece.GetLength(1); j++)
            {
                rotatedPiece[i, j] = piece[j, piece.GetLength(0) - 1 - i];
            }
        }
        return rotatedPiece;
    }

    public static State[,] GetPiece(string piece)
    {
        return piece switch
        {
            "I" => I,
            "J" => J,
            "L" => L,
            "O" => O,
            "S" => S,
            "T" => T,
            "Z" => Z,
            _ => null,
        };
    }

    public static Vector2Int GetPieceStart(string piece)
    {
        return piece switch
        {
            "I" => I_START,
            "J" => J_START,
            "L" => L_START,
            "O" => O_START,
            "S" => S_START,
            "T" => T_START,
            "Z" => Z_START,
            _ => new Vector2Int(0, 0),
        };
    }
}