using UnityEngine;
using static Tile;

public partial class BoardController : MonoBehaviour
{
    private Vector2Int currentGhostPosition;
    private Vector2Int[] currentGhostStructure;


    private void UpdateGhost()
    {
        if (cleanMode)
            return;

        int distance = 1;
        while (CanCurrentFall(distance))
        {
            distance++;
        }

        ClearGhost();
        currentGhostPosition.x = currentPiecePosition.x;
        currentGhostPosition.y = currentPiecePosition.y - distance + 1;
        currentGhostStructure = currentPieceStructure;
        DrawGhost();
    }

    private void ClearGhost()
    {
        if (cleanMode)
            return;

        ForEveryTileInStructure((x, y) =>
        {
            if (tiles[x, y].GetTileType() == TileType.Ghost)
            {
                tiles[x, y].SetTileType(TileType.Empty);
                tiles[x, y].SetTileData(tileDataSO.Empty);
            }
        }, currentGhostStructure, currentGhostPosition);
    }

    private void DrawGhost()
    {
        if (cleanMode)
            return;

        ForEveryTileInStructure((x, y) =>
        {
            if (tiles[x, y].GetTileType() == TileType.Empty)
            {
                tiles[x, y].SetTileType(TileType.Ghost);
                tiles[x, y].SetTileData(currentData);
            }
        }, currentGhostStructure, currentGhostPosition);
    }
}