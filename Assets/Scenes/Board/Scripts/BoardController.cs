using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pieces;
using static Tile;
using static BoardConstants;

public class BoardController : MonoBehaviour
{
    public TileDataScriptableObject tileDataSO;
    [SerializeField] private GameObject tilePrefab;
    public InfoController infoController;
    private ClearsController clearsController;
    private Bag bag = new();
    public readonly Tile[,] tiles = new Tile[BOARD_WIDTH, BOARD_HEIGHT + BOARD_HEIGHT_BUFFER];
    private Vector2Int currentGhostPosition;
    private Vector2Int[] currentGhostStructure;
    public Vector2Int currentPiecePosition;
    public Vector2Int[] currentPieceStructure;
    private Vector2Int[][] currentPieceOffsets;
    public int currentPieceRotation;
    public int currentPieceSize;
    public Piece currentPiece;
    private TileData currentData;
    private int lowestY;
    private float timeBuffer = 0;
    private float extendedTimeBuffer = 0;
    private float autoShiftTimer = 0;
    private float fallDelay = 0;
    private bool activePiece = false;
    private bool inputLock = false;
    private bool holdUsed = false;
    private bool canHold = true;
    private bool playing = false;
    public bool lastMoveWasRotate = false;
    private Piece heldPiece = Piece.None;

    // Start is called before the first frame update
    void Start()
    {
        clearsController = new ClearsController(this);
        tilePrefab.transform.localScale = new(BOARD_SCALE, BOARD_SCALE, 1);
        transform.Find("Background").transform.localScale = new(BOARD_SCALE, BOARD_SCALE, 1);

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

        Piece[] initialPreviews = new Piece[PREVIEWS];
        for (int i = 0; i < PREVIEWS; i++)
        {
            initialPreviews[i] = bag.PeekAt(i);
        }
        infoController.InitInfo(initialPreviews);

        StartCoroutine(GameLoop());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        HandleInput();
    }

    private IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(START_TIME);
        playing = true;
        while (true)
        {
            SpawnPiece(holdUsed);
            if (CheckForGameOver())
            {
                RestartBoard();
                playing = false;
                yield return new WaitForSeconds(START_TIME);
                playing = true;
            }
            else
            {
                StartCoroutine(FallCoroutine());
                yield return WaitUntilLockOrHold();
                if (!holdUsed)
                    LockCurrentPiece();
                // yield return new WaitForSeconds(SPAWN_DELAY);
            }
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

    private IEnumerator WaitUntilLockOrHold()
    {
        yield return new WaitUntil(() => CanLockCurrentPiece() || holdUsed);
    }

    private void UpdateTimers()
    {
        if (playing)
        {
            timeBuffer += Time.deltaTime;
            extendedTimeBuffer += Time.deltaTime;
            fallDelay += Time.deltaTime;
            autoShiftTimer += Time.deltaTime;
        }
    }

    private void SpawnPiece(bool useCurrent)
    {
        Piece piece;
        if (useCurrent)
        {
            piece = currentPiece;
        }
        else
        {
            piece = bag.GetNext();
            infoController.UpdatePreview(bag.PeekAt(PREVIEWS - 1));
        }

        holdUsed = false;

        PieceStructure pieceStructure = PieceStructures.pieceStructures[piece];
        currentPiecePosition = new(pieceStructure.start.x, pieceStructure.start.y);
        currentPieceStructure = pieceStructure.structure.Clone() as Vector2Int[];
        currentPieceOffsets = pieceStructure.offsets;
        currentPieceRotation = 0;
        currentData = tileDataSO.GetTileType(piece);
        currentPiece = piece;
        currentPieceSize = pieceStructure.size;

        currentGhostPosition = new(currentPiecePosition.x, currentPiecePosition.y);
        currentGhostStructure = currentPieceStructure;

        lowestY = currentPiecePosition.y;
        activePiece = true;
    }

    private bool CheckForGameOver()
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


    private void ClearCurrentPiece()
    {
        ForEveryCurrentTile((x, y) =>
        {
            tiles[x, y].SetTileData(tileDataSO.Empty);
            tiles[x, y].SetTileType(TileType.Empty);
        });
    }

    private void DrawCurrentPiece()
    {
        ForEveryCurrentTile((x, y) =>
        {
            tiles[x, y].SetTileData(currentData);
            tiles[x, y].SetTileType(TileType.Active);
        });

        UpdateGhost();
    }

    private void RestartBoard()
    {
        bag = new Bag();
        heldPiece = Piece.None;
        activePiece = false;
        canHold = true;
        Piece[] initialPreviews = new Piece[PREVIEWS];
        for (int i = 0; i < PREVIEWS; i++)
        {
            initialPreviews[i] = bag.PeekAt(i);
        }
        infoController.RestartInfo(initialPreviews);
        RestartTimers();

        for (int i = 0; i < BOARD_WIDTH; i++)
        {
            for (int j = 0; j < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER; j++)
            {
                tiles[i, j].SetTileData(tileDataSO.Empty);
                tiles[i, j].SetTileType(TileType.Empty);
            }
        }
    }

    private void RestartTimers()
    {
        timeBuffer = 0;
        extendedTimeBuffer = 0;
        autoShiftTimer = 0;
        fallDelay = 0;
    }

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
            infoController.FirstHoldPiece(heldPiece, bag.PeekAt(PREVIEWS - 1));
        }
        else
        {
            (currentPiece, heldPiece) = (heldPiece, currentPiece);
            infoController.HoldPiece(heldPiece);
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
            Debug.Log("Falling");
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
            Debug.Log("Moving " + direction);
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
            Debug.Log("Rotating " + times);
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

    public bool IsTileInValidRange(int x, int y)
    {
        return x >= 0 && x < BOARD_WIDTH && y >= 0 && y < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER;
    }

    private bool CanCurrentFall(int distance)
    {
        return CanFall(currentPieceStructure, currentPiecePosition, distance);
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

    private void LockCurrentPiece()
    {
        MaxFallCurrentPiece();

        foreach (Vector2Int tilePos in currentPieceStructure)
        {
            int x = currentPiecePosition.x + tilePos.x;
            int y = currentPiecePosition.y + tilePos.y;
            tiles[x, y].SetTileType(TileType.Locked);
        }

        timeBuffer = 0;
        extendedTimeBuffer = 0;
        canHold = true;
        activePiece = false;

        clearsController.CheckForClears();
        lastMoveWasRotate = false;
        Debug.Log("Locking");
    }

    private void UpdateGhost()
    {
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
        ForEveryTileInStructure((x, y) =>
        {
            if (tiles[x, y].GetTileType() == TileType.Empty)
            {
                tiles[x, y].SetTileType(TileType.Ghost);
                tiles[x, y].SetTileData(currentData);
            }
        }, currentGhostStructure, currentGhostPosition);
    }

    private bool CanLockCurrentPiece()
    {
        // time_buffer resets with user input or falling
        // store the lowest y, if it doesnt change in extended_time_buffer seconds land it
        // if lowest y changes, reset the timer
        if (currentPiecePosition.y < lowestY)
        {
            lowestY = currentPiecePosition.y;
            extendedTimeBuffer = 0;
        }
        if (timeBuffer >= TIME_BUFFER || extendedTimeBuffer >= EXTENDED_TIME_BUFFER || inputLock)
        {
            activePiece = false;
            inputLock = false;
            return true;
        }
        return false;
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

    // TODO: input manager
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCurrentPiece(-1);
            autoShiftTimer = 0;
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCurrentPiece(1);
            autoShiftTimer = 0;
            timeBuffer = 0;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && autoShiftTimer >= DELAYED_AUTO_SHIFT)
        {
            MaxMoveCurrentPiece(-1);
            autoShiftTimer = 0;
            timeBuffer = 0;
        }
        if (Input.GetKey(KeyCode.RightArrow) && autoShiftTimer >= DELAYED_AUTO_SHIFT)
        {
            MaxMoveCurrentPiece(1);
            autoShiftTimer = 0;
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
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("Soft Dropping");
            MaxFallCurrentPiece();
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputLock = true;
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            HoldCurrentPiece();
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartBoard();
            timeBuffer = 0;
        }
    }
}
