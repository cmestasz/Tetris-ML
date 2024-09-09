using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BoardConstants;
using static Tile;

public partial class BoardController : MonoBehaviour
{
    private static Dictionary<string, int[]> attackTable = new()
    {
        { "s", new int[] { 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 } },
        { "d", new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6 } },
        { "t", new int[] { 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12 } },
        { "q", new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 } },
        { "tsms", new int[] { 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 } },
        { "tss", new int[] { 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12 } },
        { "tsmd", new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6 } },
        { "tsd", new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 } },
        { "tst", new int[] { 6, 7, 9, 10, 12, 13, 15, 16, 18, 19, 21, 22, 24, 25, 27, 28, 30, 31, 33, 34, 36 } },
    };

    public int pendingGarbage = 0;
    private void ScoreClears(int b2b, int combo, int clears, bool allSpin, int tSpin, int surge, bool pc)
    {
        if (clears == 0)
        {
            return;
        }

        int[] attackArray;
        switch (clears)
        {
            case 1:
                if (tSpin == 1)
                    attackArray = attackTable["tss"];
                else if (tSpin == 0)
                    attackArray = attackTable["tsms"];
                else
                    attackArray = attackTable["s"];
                break;
            case 2:
                if (tSpin == 1)
                    attackArray = attackTable["tsd"];
                else if (tSpin == 0)
                    attackArray = attackTable["tsmd"];
                else
                    attackArray = attackTable["d"];
                break;
            case 3:
                if (tSpin == 1)
                    attackArray = attackTable["tst"];
                else
                    attackArray = attackTable["t"];
                break;
            case 4:
                attackArray = attackTable["q"];
                break;
            default:
                attackArray = attackTable["q"];
                break;
        }

        int attack = attackArray[combo];
        attack += surge;
        if (b2b > 3)
            attack += 1;
        if (pc)
            attack += 5;

        int toBlock = Mathf.Min(attack, pendingGarbage);
        pendingGarbage -= toBlock;
        attack -= toBlock;

        GameManager.Instance.SendAttack(attack, targetId);

        string debugText = "Clears: " + clears + "\n";
        debugText += "B2B: " + b2b + "\n";
        debugText += "Combo: " + combo + "\n";
        debugText += "All Spin: " + allSpin + "\n";
        debugText += "T Spin: " + tSpin + "\n";
        // Debug.Log(debugText);

        Debug.Log("Sent " + attack + " garbage to " + targetId);
    }

    private IEnumerator GarbageTest()
    {
        while (true)
        {
            pendingGarbage += 1;
            yield return new WaitForSeconds(2);
        }
    }

    public IEnumerator ReceiveAttack(int attack)
    {
        yield return new WaitForSeconds(PASSTHROUGH_TIME);
        pendingGarbage += attack;
        UpdatePendingLine();
    }

    private void UpdatePendingLine()
    {
        pendingLine.SetPositions(new Vector3[] { new(0, 0, 0), new(0, pendingGarbage * BOARD_SCALE, 0) });
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

        UpdatePendingLine();

    }
}