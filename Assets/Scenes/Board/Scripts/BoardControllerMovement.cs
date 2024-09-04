using UnityEngine;
using static BoardConstants;
using static Pieces;
using static Tile;

public partial class BoardController : MonoBehaviour
{
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

    private void RotateCurrentPiece(int times)
    {
        Vector2Int[] newStructure = PieceStructures.RotateStructure(currentPieceStructure, currentPieceSize, times);
        ClearCurrentPiece();
        if (KickCurrentPiece(newStructure, times))
        {
            currentPieceStructure = newStructure;
            // Debug.Log("Rotating " + times);
            lastMoveWasRotate = true;
        }
        DrawCurrentPiece();
    }

    private bool KickCurrentPiece(Vector2Int[] structure, int times)
    {
        int newRotation = (currentPieceRotation + times) % 4;
        int kickIdx = -1;
        Vector2Int currentKick;
        bool wasKick = false;
        do
        {
            kickIdx++;
            currentKick = currentPieceOffsets[currentPieceRotation][kickIdx] - currentPieceOffsets[newRotation][kickIdx];
            if (CanKick(structure, currentKick))
            {
                currentPiecePosition += currentKick;
                wasKick = true;
                break;
            }
        } while (kickIdx < currentPieceOffsets[newRotation].Length - 1);

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