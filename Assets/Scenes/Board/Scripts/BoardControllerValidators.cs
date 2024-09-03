using UnityEngine;
using static BoardConstants;
using static Tile;

public partial class BoardController : MonoBehaviour
{
    private bool IsGameOver()
    {
        bool gameOver = false;
        ForEveryCurrentTile((x, y) =>
        {
            if (tiles[x, y].GetTileType() == TileType.Locked)
            {
                gameOver = true;
                return;
            }
        });
        return gameOver;
    }

    private bool CanKick(Vector2Int[] structure, Vector2Int kick)
    {
        bool canKick = true;
        ForEveryTileInStructure((x, y) =>
        {
            x += kick.x;
            y += kick.y;
            if (!IsTileInValidRange(x, y) || tiles[x, y].GetTileType() == TileType.Locked)
            {
                canKick = false;
                return;
            }
        }, structure, currentPiecePosition);

        return canKick;
    }

    public bool CanShift(Vector2Int[] structure, Vector2Int position, int direction)
    {
        bool canMove = true;
        ForEveryTileInStructure((x, y) =>
        {
            x += direction;
            if (!IsTileInValidRange(x, y) || tiles[x, y].GetTileType() == TileType.Locked)
            {
                canMove = false;
                return;
            }
        }, structure, position);

        return canMove;
    }

    public bool CanMove(Vector2Int[] structure, Vector2Int position, Vector2Int dir)
    {
        bool canMove = true;
        ForEveryTileInStructure((x, y) =>
        {
            x += dir.x;
            y += dir.y;
            if (!IsTileInValidRange(x, y) || tiles[x, y].GetTileType() == TileType.Locked)
            {
                canMove = false;
                return;
            }
        }, structure, position);

        return canMove;
    }

    public bool CanCurrentMove(Vector2Int dir)
    {
        return CanMove(currentPieceStructure, currentPiecePosition, dir);
    }

    public bool CanCurrentShift(int direction)
    {
        return CanShift(currentPieceStructure, currentPiecePosition, direction);
    }

    public bool CanFall(Vector2Int[] structure, Vector2Int position, int distance)
    {
        bool canFall = true;
        ForEveryTileInStructure((x, y) =>
        {
            y -= distance;
            if (y < 0 || y >= BOARD_HEIGHT + BOARD_HEIGHT_BUFFER || tiles[x, y].GetTileType() == TileType.Locked)
            {
                canFall = false;
                return;
            }
        }, structure, position);

        return canFall;
    }

    private bool CanCurrentFall(int distance)
    {
        return CanFall(currentPieceStructure, currentPiecePosition, distance);
    }

    private bool CanCurrentLock()
    {
        // time_buffer resets with user input or falling
        // store the lowest y, if it doesnt change in extended_time_buffer seconds land it
        // if lowest y changes, reset the timer
        if (currentPiecePosition.y < lowestY)
        {
            lowestY = currentPiecePosition.y;
            extendedTimeBuffer = 0;
        }
        if (timeBuffer >= TIME_BUFFER || extendedTimeBuffer >= EXTENDED_TIME_BUFFER || forcedLock)
        {
            activePiece = false;
            forcedLock = false;
            return true;
        }
        return false;
    }
}