using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pieces;

public class BoardController : MonoBehaviour
{
    private const float BOARD_SCALE = 0.45f;
    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 20;
    private const int BOARD_HEIGHT_BUFFER = 10;
    private const float TIME_BUFFER = 1f;
    private const float EXTENDED_TIME_BUFFER = 7f;
    private const float FALL_DELAY = 0.5f;
    private const float SPAWN_DELAY = 0.25f;
    [SerializeField] private TileDataScriptableObject tileTypes;
    [SerializeField] private GameObject tilePrefab;
    private readonly Bag bag = new();
    private readonly Tile[,] tiles = new Tile[BOARD_WIDTH, BOARD_HEIGHT + BOARD_HEIGHT_BUFFER];
    private Vector2Int currentPiecePosition;
    private Vector2Int[] currentPieceStructure;
    private Vector2Int[][] currentPieceOffsets;
    private int currentPieceRotation;
    private int currentPieceSize;
    private TileData currentType;
    private int lowestY;
    private int prevY;
    private float timeBuffer = 0;
    private float extendedTimeBuffer = 0;
    private float fallDelay = 0;
    private bool activePiece = false;

    // Start is called before the first frame update
    void Start()
    {
        tilePrefab.transform.localScale = new(BOARD_SCALE, BOARD_SCALE, 1);
        transform.GetChild(0).transform.localScale = new(BOARD_SCALE, BOARD_SCALE, 1);

        float offsetX = -BOARD_WIDTH * BOARD_SCALE / 2.0f + BOARD_SCALE / 2.0f;
        float offsetY = -BOARD_HEIGHT * BOARD_SCALE / 2.0f + BOARD_SCALE / 2.0f;
        for (int i = 0; i < BOARD_WIDTH; i++)
        {
            for (int j = 0; j < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER; j++)
            {
                float x = transform.position.x + i * BOARD_SCALE + offsetX;
                float y = transform.position.y + j * BOARD_SCALE + offsetY;
                GameObject instObject = Instantiate(tilePrefab, new(x, y, 0), Quaternion.identity, transform);
                tiles[i, j] = instObject.GetComponent<Tile>();
            }
        }

        StartCoroutine(BoardCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        HandleInput();
    }

    private IEnumerator BoardCoroutine()
    {
        yield return new WaitForSeconds(SPAWN_DELAY);
        while (true)
        {
            SpawnPiece();
            StartCoroutine(FallCoroutine());
            yield return LockCoroutine();
            yield return new WaitForSeconds(SPAWN_DELAY);
        }
    }

    private IEnumerator FallCoroutine()
    {
        while (activePiece)
        {
            FallCurrentPiece();
            yield return new WaitUntil(() => fallDelay >= FALL_DELAY || !activePiece);
            fallDelay = 0;
        }
    }

    private IEnumerator LockCoroutine()
    {
        yield return new WaitUntil(() => !activePiece || ForceLock());
        LockCurrentPiece();
    }

    // TODO: input manager
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCurrentPiece(-1);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCurrentPiece(1);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateCurrentPiece(1);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateCurrentPiece(2);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RotateCurrentPiece(3);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            FallCurrentPiece();
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activePiece = false;
            timeBuffer = 0;
        }
    }


    private void UpdateTimers()
    {
        timeBuffer += Time.deltaTime;
        extendedTimeBuffer += Time.deltaTime;
        fallDelay += Time.deltaTime;
    }

    private void SpawnPiece()
    {
        Piece piece = bag.GetNext();

        PieceStructure pieceStructure = PieceStructures.pieceStructures[piece];
        currentPiecePosition = new(pieceStructure.start.x, pieceStructure.start.y);
        currentPieceStructure = pieceStructure.structure.Clone() as Vector2Int[];
        currentPieceOffsets = pieceStructure.offsets;
        currentPieceRotation = 0;
        currentType = tileTypes.GetTileType(piece);
        currentPieceSize = pieceStructure.size;

        lowestY = currentPiecePosition.y;
        prevY = currentPiecePosition.y;
        activePiece = true;

        DrawCurrentPiece();
    }

    private void ClearCurrentPiece()
    {
        ForEveryCurrentTile((x, y) =>
        {
            tiles[x, y].SetTileType(tileTypes.Empty);
        });
    }

    private void DrawCurrentPiece()
    {
        ForEveryCurrentTile((x, y) =>
        {
            tiles[x, y].SetTileType(currentType);
        });
    }

    private void FallCurrentPiece()
    {
        if (CanFall(1))
        {
            ClearCurrentPiece();
            currentPiecePosition.y--;
            DrawCurrentPiece();
        }
    }

    private void MoveCurrentPiece(int direction)
    {
        // dont touch this, itll have to be like this when you add arr
        if (CanMove(direction))
        {
            ClearCurrentPiece();
            currentPiecePosition.x += direction;
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

    private bool CanKick(Vector2Int[] structure, Vector2Int kick)
    {
        bool canKick = true;
        ForEveryTileInStructure((x, y) =>
        {
            x += kick.x;
            y += kick.y;
            if (x < 0 || x >= BOARD_WIDTH || y < 0 || y >= BOARD_HEIGHT + BOARD_HEIGHT_BUFFER || tiles[x, y].locked)
            {
                canKick = false;
                return;
            }
        }, structure);

        return canKick;
    }

    private bool CanMove(int direction)
    {
        bool canMove = true;
        ForEveryCurrentTile((x, y) =>
        {
            x += direction;
            if (x < 0 || x >= BOARD_WIDTH || tiles[x, y].locked)
            {
                canMove = false;
                return;
            }
        });

        return canMove;
    }

    private bool CanFall(int distance)
    {
        bool canFall = true;

        ForEveryCurrentTile((x, y) =>
        {
            y -= distance;
            if (y < 0 || y >= BOARD_HEIGHT + BOARD_HEIGHT_BUFFER || tiles[x, y].locked)
            {
                canFall = false;
                return;
            }
        });

        return canFall;
    }

    private void LockCurrentPiece()
    {
        int distance = 1;
        while (CanFall(distance))
        {
            distance++;
        }

        ClearCurrentPiece();
        currentPiecePosition.y -= distance - 1;
        DrawCurrentPiece();

        foreach (Vector2Int tilePos in currentPieceStructure)
        {
            int x = currentPiecePosition.x + tilePos.x;
            int y = currentPiecePosition.y + tilePos.y;
            tiles[x, y].locked = true;
        }

        timeBuffer = 0;
        extendedTimeBuffer = 0;
    }

    private bool ForceLock()
    {
        // time_buffer resets with user input
        // store the lowest y, if it doesnt change in extended_time_buffer seconds land it
        // if lowest y changes, reset the timer


        if (currentPiecePosition.y < lowestY)
        {
            lowestY = currentPiecePosition.y;
            extendedTimeBuffer = 0;
        }
        prevY = currentPiecePosition.y;

        if (timeBuffer >= TIME_BUFFER || extendedTimeBuffer >= EXTENDED_TIME_BUFFER)
        {
            activePiece = false;
            return true;
        }
        return false;
    }

    private void ForEveryCurrentTile(System.Action<int, int> action)
    {
        ForEveryTileInStructure(action, currentPieceStructure);
    }

    private void ForEveryTileInStructure(System.Action<int, int> action, Vector2Int[] structure)
    {
        foreach (Vector2Int tilePos in structure)
        {
            int x = currentPiecePosition.x + tilePos.x;
            int y = currentPiecePosition.y + tilePos.y;
            action(x, y);
        }
    }
}
