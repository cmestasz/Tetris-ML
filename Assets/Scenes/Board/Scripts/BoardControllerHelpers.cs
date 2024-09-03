using UnityEngine;
using static Pieces;
using static Tile;
using static BoardConstants;

public partial class BoardController : MonoBehaviour
{
    public bool IsTileInValidRange(int x, int y)
    {
        return x >= 0 && x < BOARD_WIDTH && y >= 0 && y < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER;
    }

    private void ForEveryCurrentTile(System.Action<int, int> action)
    {
        ForEveryTileInStructure(action, currentPieceStructure, currentPiecePosition);
    }

    private void ForEveryTileInStructure(System.Action<int, int> action, Vector2Int[] structure, Vector2Int position)
    {
        foreach (Vector2Int tilePos in structure)
        {
            int x = position.x + tilePos.x;
            int y = position.y + tilePos.y;
            action(x, y);
        }
    }
}