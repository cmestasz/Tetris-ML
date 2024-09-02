using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BoardConstants;
using static Tile;
using static Pieces;

public class ClearsController
{
    private readonly BoardController boardController;
    private int combo = 0;
    private int b2b = 0;
    private readonly Vector2Int[] dirs = new Vector2Int[] {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    public ClearsController(BoardController boardController)
    {
        this.boardController = boardController;
    }

    public void CheckForClears()
    {
        List<int> toClear = new();
        for (int y = boardController.currentPiecePosition.y; y < boardController.currentPiecePosition.y + boardController.currentPieceSize; y++)
        {
            if (y < 0 || y >= BOARD_HEIGHT + BOARD_HEIGHT_BUFFER)
            {
                continue;
            }
            bool clear = true;
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (boardController.tiles[x, y].GetTileType() != TileType.Locked)
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

        bool allSpin = false;
        bool tSpin = false;
        if (toClear.Count > 0)
        {
            allSpin = CheckForAllSpin() || CheckForTSpinMini();
            tSpin = CheckForTSpin();
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

        boardController.infoController.UpdateClears(toClear.Count, "", b2b, combo);
    }

    private void ScoreClears(int b2b, int combo, List<int> toClear, bool allSpin, bool tSpin)
    {
        Debug.Log("Clears: " + toClear.Count);
        Debug.Log("B2B: " + b2b);
        Debug.Log("Combo: " + combo);
        Debug.Log("All Spin: " + allSpin);
        Debug.Log("T Spin: " + tSpin);
    }

    private void ClearLines(List<int> toClear)
    {
        for (int i = 0; i < toClear.Count; i++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                boardController.tiles[x, toClear[i] - i].SetTileData(boardController.tileDataSO.Empty);
                boardController.tiles[x, toClear[i] - i].SetTileType(TileType.Empty);
            }
            for (int y = toClear[i] - i; y < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER - 1; y++)
            {
                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    boardController.tiles[x, y].SetTileData(boardController.tiles[x, y + 1].tileData);
                    boardController.tiles[x, y].SetTileType(boardController.tiles[x, y + 1].GetTileType());
                }
            }
        }
    }

    private bool CheckForAllSpin()
    {
        if (boardController.currentPiece == Piece.T)
            return false;
        bool allSpin = true;
        foreach (Vector2Int dir in dirs)
        {
            if (!boardController.CanCurrentMove(dir))
            {
                allSpin = false;
                break;
            }
        }
        return allSpin;
    }

    private bool CheckForTSpin()
    {
        return false;
    }

    private bool CheckForTSpinMini()
    {
        return false;
    }

    private void CheckForPC()
    {
        bool pc = true;
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            if (boardController.tiles[x, 0].GetTileType() == TileType.Locked)
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
