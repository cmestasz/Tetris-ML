using System.Collections;
using UnityEngine;
using static BoardConstants;
using static Tile;

public partial class BoardController : MonoBehaviour
{
    public int pendingGarbage = 0;

    private IEnumerator GarbageTest()
    {
        while (true)
        {
            pendingGarbage += 4;
            Debug.Log("Garbage: " + pendingGarbage);
            yield return new WaitForSeconds(5);
        }
    }


    private void SpawnGarbage()
    {
        int toClear = Mathf.Min(pendingGarbage, MAX_GARBAGE);
        if (toClear == 0)
        {
            return;
        }
        pendingGarbage -= toClear;

        // shift current up
        for (int y = BOARD_HEIGHT + BOARD_HEIGHT_BUFFER - toClear - 1; y >= 0; y--)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (tiles[x, y].GetTileType() != TileType.Active)
                {
                    tiles[x, y + toClear].SetTileType(tiles[x, y].GetTileType());
                    tiles[x, y + toClear].SetTileData(tiles[x, y].GetTileData());
                }
            }
        }

        int row = Random.Range(0, BOARD_WIDTH);
        Debug.Log("Row: " + row);
        // spawn new garbage
        for (int y = 0; y < toClear; y++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (x != row)
                {
                    tiles[x, y].SetTileType(TileType.Locked);
                    tiles[x, y].SetTileData(tileDataSO.Garbage);
                }
                else
                {
                    tiles[x, y].SetTileType(TileType.Empty);
                    tiles[x, y].SetTileData(tileDataSO.Empty);
                }
            }
        }
    }
}