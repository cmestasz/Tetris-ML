using UnityEngine;
using static BoardConstants;
using static Pieces;
using static Tile;
using static Utils;

public partial class BoardController : MonoBehaviour
{
    public enum RotateType { CW = 1, CCW = 3, _180 = 2 }

    private void LockCurrentPiece()
    {
        MaxFallCurrentPiece();
        ForEveryCurrentTile((x, y) =>
        {
            tiles[x, y].SetTileType(TileType.Locked);
        });

        timeBuffer = 0;
        extendedTimeBuffer = 0;
        canHold = true;
        activePiece = false;

        CheckForClears();
        lastMoveWasRotate = false;
        // Debug.Log("Locking");
    }
    //p

    private void HoldCurrentPiece()
    {
        if (!canHold)
        {
            return;
        }
        if (heldPiece == Piece.None)
        {
            heldPiece = currentPiece;
            currentPiece = bag.GetNext();
            FirstHoldPiece(heldPiece, bag.PeekAt(PREVIEWS - 1));
        }
        else
        {
            (currentPiece, heldPiece) = (heldPiece, currentPiece);
            HoldPiece(heldPiece);
        }
        activePiece = false;
        ClearCurrentPiece();
        ClearGhost();
        holdUsed = true;
        canHold = false;

    }

    private bool FallCurrentPiece()
    {
        if (CanCurrentFall(1))
        {
            ClearCurrentPiece();
            currentPiecePosition.y--;
            timeBuffer = 0;
            lastMoveWasRotate = false;
            // Debug.Log("Falling");
            DrawCurrentPiece();
            return true;
        }
        return false;
    }

    private void MoveCurrentPiece(int direction)
    {
        if (CanCurrentShift(direction))
        {
            ClearCurrentPiece();
            currentPiecePosition.x += direction;
            lastMoveWasRotate = false;
            // Debug.Log("Moving " + direction);
            DrawCurrentPiece();
        }
    }

    private void RotateCurrentPiece(RotateType type)
    {
        Vector2Int[] newStructure = PieceStructures.RotateStructure(currentPieceStructure, currentPieceSize, (int)type);
        ClearCurrentPiece();
        if (KickCurrentPiece(newStructure, type))
        {
            currentPieceStructure = newStructure;
            // Debug.Log("Rotating " + times);
            lastMoveWasRotate = true;
        }
        DrawCurrentPiece();
    }

    private Vector2Int[] GetKickData(int rotation, RotateType type)
    {
        int dir = type == RotateType.CCW ? (rotation == 3 ? 0 : rotation + 1) : rotation;
        // int dir = rotation;

        return type switch
        {
            RotateType.CW => NegGet(currentPieceKicks, dir),
            RotateType.CCW => InverseKickData(currentPieceKicks[dir]),
            RotateType._180 => NegGet(currentPieceKicks180, dir),
            _ => null,
        };
    }

    private Vector2Int[] InverseKickData(Vector2Int[] kicks)
    {
        Vector2Int[] inverse = new Vector2Int[kicks.Length];
        for (int i = 0; i < kicks.Length; i++)
        {
            inverse[i] = kicks[i] * -1;
        }
        return inverse;
    }

    private bool KickCurrentPiece(Vector2Int[] structure, RotateType type)
    {
        int newRotation = (currentPieceRotation + (int)type) % 4;
        // Debug.Log(newRotation);
        Vector2Int[] kickData = GetKickData(newRotation, type);
        int kickIdx = -1;
        bool wasKick = false;
        do
        {
            kickIdx++;
            Vector2Int currentKick = kickData[kickIdx];
            if (CanKick(structure, currentKick))
            {
                currentPiecePosition += currentKick;
                wasKick = true;
                break;
            }
        } while (kickIdx < kickData.Length - 1);

        currentPieceRotation = newRotation;
        return wasKick;
    }



    private void MaxMoveCurrentPiece(int direction)
    {
        int distance = 1;
        while (CanCurrentShift(direction * distance))
        {
            distance++;
        }

        ClearCurrentPiece();
        currentPiecePosition.x += direction * (distance - 1);
        DrawCurrentPiece();
    }

    private void MaxFallCurrentPiece()
    {
        int distance = 1;
        while (CanCurrentFall(distance))
        {
            distance++;
        }

        if (distance > 1)
            lastMoveWasRotate = false;

        ClearCurrentPiece();
        currentPiecePosition.y -= distance - 1;
        DrawCurrentPiece();
    }
}