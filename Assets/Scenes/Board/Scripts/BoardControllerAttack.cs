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
            yield return new WaitForSeconds(3);
            pendingGarbage += 4;
            CreateGarbage();
        }
    }


    private void CreateGarbage()
    {
        int toClear = Mathf.Min(pendingGarbage, MAX_GARBAGE);
        pendingGarbage -= toClear;

        int row = Random.Range(0, BOARD_WIDTH);
        // shift current up
        for (int y = 0; y < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER - toClear; y++)
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
            {
                if (tiles[x, y].GetTileType() == TileType.Locked)
                {
                    tiles[x, y + toClear].SetTileType(tiles[x, y].GetTileType());
                    tiles[x, y + toClear].SetTileData(tiles[x, y].GetTileData());
                }
            }
        }
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
            }
        }
    }
}