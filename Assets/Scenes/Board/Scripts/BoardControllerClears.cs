using System.Collections.Generic;
using UnityEngine;
using static BoardConstants;
using static Tile;
using static Pieces;

public partial class BoardController : MonoBehaviour
{
    private int combo = 0;
    private int b2b = 0;
    private readonly Vector2Int[] dirs = new Vector2Int[] {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    };
    private readonly Vector2Int[] cornersT = new Vector2Int[] {
        new(0, 2),
        new(2, 2),
        new(2, 0),
        new(0, 0)
    };


    public void CheckForClears()
    {
        List<int> toClear = new();
        for (int y = currentPiecePosition.y; y < currentPiecePosition.y + currentPieceSize; y++)
        {
            if (y < 0 || y >= BOARD_HEIGHT + BOARD_HEIGHT_BUFFER)
            {
                continue;
            }
            bool clear = true;
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (tiles[x, y].GetTileType() != TileType.Locked)
                {
                    clear = false;
                    break;
                }
            }
            if (clear)
            {
                toClear.Add(y);
            }
        }

        int tSpin = CheckForTSpin();
        bool allSpin = CheckForAllSpin() || tSpin == 0;
        if (toClear.Count > 0)
        {
            ClearLines(toClear);
        }

        ScoreClears(b2b, combo, toClear, allSpin, tSpin);


        if (toClear.Count == 0)
        {
            combo = 0;
        }
        else
        {
            combo++;
        }

        if (toClear.Count == 4)
        {
            b2b++;
        }
        else if (toClear.Count > 0)
        {
            b2b = 0;
        }

        string clearMod = "";
        if (tSpin == 1)
        {
            clearMod = "T Spin";
        }
        else if (tSpin == 0)
        {
            clearMod = "Mini T Spin";
        }
        else if (allSpin)
        {
            clearMod = currentPiece + " Spin";
        }

        UpdateClears(toClear.Count, clearMod, b2b, combo);
    }

    private void ScoreClears(int b2b, int combo, List<int> toClear, bool allSpin, int tSpin)
    {
        string debugText = "Clears: " + toClear.Count + "\n";
        debugText += "B2B: " + b2b + "\n";
        debugText += "Combo: " + combo + "\n";
        debugText += "All Spin: " + allSpin + "\n";
        debugText += "T Spin: " + tSpin + "\n";
        Debug.Log(debugText);
    }

    private void ClearLines(List<int> toClear)
    {
        for (int i = 0; i < toClear.Count; i++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                tiles[x, toClear[i] - i].SetTileData(tileDataSO.Empty);
                tiles[x, toClear[i] - i].SetTileType(TileType.Empty);
            }
            for (int y = toClear[i] - i; y < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER - 1; y++)
            {
                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    tiles[x, y].SetTileData(tiles[x, y + 1].GetTileData());
                    tiles[x, y].SetTileType(tiles[x, y + 1].GetTileType());
                }
            }
        }
    }

    private bool CheckForAllSpin()
    {
        if (!lastMoveWasRotate || currentPiece == Piece.T)
            return false;
        bool allSpin = true;
        foreach (Vector2Int dir in dirs)
        {
            if (CanCurrentMove(dir))
            {
                allSpin = false;
                break;
            }
        }
        return allSpin;
    }

    // 0: mini, 1: regular
    private int CheckForTSpin()
    {
        if (!lastMoveWasRotate || currentPiece != Piece.T)
            return -1;

        int idx1 = currentPieceRotation;
        int idx2 = (idx1 + 1) % 4;

        Vector2Int corner1 = cornersT[idx1];
        Vector2Int corner2 = cornersT[idx2];
        Debug.Log(corner1 + " " + corner2);

        Vector2Int pos1 = currentPiecePosition + corner1;
        Vector2Int pos2 = currentPiecePosition + corner2;

        Tile tile1 = IsTileInValidRange(pos1.x, pos1.y) ? tiles[pos1.x, pos1.y] : null;
        Tile tile2 = IsTileInValidRange(pos2.x, pos2.y) ? tiles[pos2.x, pos2.y] : null;

        int count = -1;
        if (tile1 != null && tile1.GetTileType() == TileType.Locked)
        {
            count++;
        }
        if (tile2 != null && tile2.GetTileType() == TileType.Locked)
        {
            count++;
        }
        return count;
    }

    private void CheckForPC()
    {
        bool pc = true;
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            if (tiles[x, 0].GetTileType() == TileType.Locked)
            {
                pc = false;
                break;
            }
        }
        if (pc)
        {
            Debug.Log("PC");
        }
    }
}