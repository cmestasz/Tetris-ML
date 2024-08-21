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
    [SerializeField] private TileTypesScriptableObject tileTypes;
    [SerializeField] private GameObject tilePrefab;
    private Tile[,] tiles = new Tile[BOARD_WIDTH, BOARD_HEIGHT + BOARD_HEIGHT_BUFFER];
    private string[] bag = new string[7] { "S", "Z", "L", "J", "I", "O", "T" };
    private List<string> currentBag;
    private Vector2Int currentPiecePosition;
    private State[,] currentPieceStructure;
    private TileType currentType;
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

        currentBag = new(bag);
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

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCurrentPiece(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCurrentPiece(1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //RotateCurrentPiece();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            FallCurrentPiece();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activePiece = false;
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
        string piece = GetNextPiece();
        State[,] pieceStructure = GetPiece(piece);
        Vector2Int pieceStart = GetPieceStart(piece);
        TileType tileType = tileTypes.GetTileType(piece);
        currentPiecePosition = new(pieceStart.x, pieceStart.y);
        currentPieceStructure = pieceStructure.Clone() as State[,];
        currentType = tileType;
        lowestY = currentPiecePosition.y;
        prevY = currentPiecePosition.y;
        activePiece = true;

        DrawCurrentPiece();
    }

    private void ClearCurrentPiece()
    {
        ForEveryTileRelativeToBoard((i, j) =>
        {
            if (!tiles[i, j].locked)
            {
                tiles[i, j].SetTileType(tileTypes.Empty);
            }
        });
    }

    private void DrawCurrentPiece()
    {
        ForEveryTileRelativeToBoard((i, j) =>
        {
            if (!tiles[i, j].locked && currentPieceStructure[i - currentPiecePosition.x, j - currentPiecePosition.y] != State.N)
            {
                tiles[i, j].SetTileType(currentType);
            }
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

    private bool CanMove(int direction)
    {
        bool canMove = true;
        ForEveryTileRelativeToStructure((i, j) =>
        {
            if (currentPieceStructure[i, j] != State.N)
            {
                if (currentPiecePosition.x + i + direction < 0 ||
                currentPiecePosition.x + i + direction >= BOARD_WIDTH ||
                tiles[currentPiecePosition.x + i + direction, currentPiecePosition.y + j].locked)
                    canMove = false;
            }
        });
        return canMove;
    }

    private bool CanFall(int distance)
    {
        bool canFall = true;
        ForEveryTileRelativeToStructure((i, j) =>
        {
            if (currentPieceStructure[i, j] != State.N)
            {
                if (currentPiecePosition.y + j - distance < 0 ||
                currentPiecePosition.y + j - distance >= BOARD_HEIGHT + BOARD_HEIGHT_BUFFER ||
                tiles[currentPiecePosition.x + i, currentPiecePosition.y + j - distance].locked)
                    canFall = false;
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

        ForEveryTileRelativeToBoard((i, j) =>
        {
            if (currentPieceStructure[i - currentPiecePosition.x, j - currentPiecePosition.y] != State.N)
            {
                tiles[i, j].locked = true;
            }
        });

        timeBuffer = 0;
        extendedTimeBuffer = 0;
    }

    private bool ForceLock()
    {
        // innerly if y doesnt change in time_buffer seconds, land it
        // store the lowest y, if it doesnt change in extended_time_buffer seconds land it
        // if lowest y changes, reset the timer
        // if it lands, lock the tiles

        if (currentPiecePosition.y != prevY)
        {
            timeBuffer = 0;
            if (currentPiecePosition.y < lowestY)
            {
                lowestY = currentPiecePosition.y;
                extendedTimeBuffer = 0;
            }
        }
        prevY = currentPiecePosition.y;

        if (timeBuffer >= TIME_BUFFER || extendedTimeBuffer >= EXTENDED_TIME_BUFFER)
        {
            activePiece = false;
            return true;
        }
        return false;
    }

    private string GetNextPiece()
    {
        if (currentBag.Count == 0)
        {
            currentBag = new(bag);
        }
        int idx = Random.Range(0, currentBag.Count);
        string piece = currentBag[idx];
        currentBag.RemoveAt(idx);
        return piece;
    }

    private void ForEveryTileRelativeToBoard(System.Action<int, int> action)
    {
        for (int i = currentPiecePosition.x; i < currentPiecePosition.x + currentPieceStructure.GetLength(0); i++)
        {
            for (int j = currentPiecePosition.y; j < currentPiecePosition.y + currentPieceStructure.GetLength(1); j++)
            {
                if (i >= 0 && i < BOARD_WIDTH && j >= 0 && j < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER)
                {
                    action(i, j);
                }
            }
        }
    }

    private void ForEveryTileRelativeToStructure(System.Action<int, int> action)
    {
        for (int i = 0; i < currentPieceStructure.GetLength(0); i++)
        {
            for (int j = 0; j < currentPieceStructure.GetLength(1); j++)
            {
                if (currentPiecePosition.x + i >= 0 && currentPiecePosition.x + i < BOARD_WIDTH && currentPiecePosition.y + j >= 0 && currentPiecePosition.y + j < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER)
                {
                    action(i, j);
                }
            }
        }
    }
}
