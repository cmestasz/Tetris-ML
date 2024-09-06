using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pieces;
using static Tile;
using static BoardConstants;
using System;

public partial class BoardController : MonoBehaviour
{
    public TileDataScriptableObject tileDataSO;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private bool cleanMode;
    private Bag bag;
    public readonly Tile[,] tiles = new Tile[BOARD_WIDTH, BOARD_HEIGHT + BOARD_HEIGHT_BUFFER];
    public Vector2Int currentPiecePosition;
    public Vector2Int[] currentPieceStructure;
    private Vector2Int[][] currentPieceKicks;
    private Vector2Int[][] currentPieceKicks180;
    public int currentPieceRotation;
    public int currentPieceSize;
    public Piece currentPiece;
    private Piece heldPiece = Piece.None;
    private TileData currentData;
    private int lowestY;
    private int id;
    private int targetId;
    private float timeBuffer = 0;
    private float extendedTimeBuffer = 0;
    private float autoShiftTimer = 0;
    private float fallDelay = 0;
    private bool activePiece = false;
    private bool forcedLock = false;
    private bool holdUsed = false;
    private bool canHold = true;
    private bool playing = false;
    public bool allowInput = false;
    public bool lastMoveWasRotate = false;

    public void Init(int id, int bagSeed)
    {
        this.id = id;
        targetId = GameManager.Instance.GetTargetId(id);
        bag = new Bag(bagSeed);

        tilePrefab.transform.localScale = new(BOARD_SCALE, BOARD_SCALE, 1);
        transform.Find("Background").transform.localScale = new(BOARD_SCALE, BOARD_SCALE, 1);
        Transform tilesParent = transform.Find("Tiles");

        float offsetX = -BOARD_WIDTH * BOARD_SCALE / 2.0f + BOARD_SCALE / 2.0f;
        float offsetY = -BOARD_HEIGHT * BOARD_SCALE / 2.0f + BOARD_SCALE / 2.0f;
        for (int i = 0; i < BOARD_WIDTH; i++)
        {
            for (int j = 0; j < BOARD_HEIGHT + BOARD_HEIGHT_BUFFER; j++)
            {
                float x = transform.position.x + i * BOARD_SCALE + offsetX;
                float y = transform.position.y + j * BOARD_SCALE + offsetY;
                GameObject instObject = Instantiate(tilePrefab, new(x, y, 0), Quaternion.identity, tilesParent);
                tiles[i, j] = instObject.GetComponent<Tile>();
                tiles[i, j].Init();
            }
        }

        Piece[] initialPreviews = new Piece[PREVIEWS];
        for (int i = 0; i < PREVIEWS; i++)
        {
            initialPreviews[i] = bag.PeekAt(i);
        }
        InitInfo(initialPreviews);

        StartCoroutine(GameLoop());
        // StartCoroutine(GarbageTest());
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
            if (IsGameOver())
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
        yield return new WaitUntil(() => CanCurrentLock() || holdUsed);
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
            UpdatePreview(bag.PeekAt(PREVIEWS - 1));
        }

        holdUsed = false;

        PieceStructure pieceStructure = PieceStructures.pieceStructures[piece];
        currentPiecePosition = new(pieceStructure.start.x, pieceStructure.start.y);
        currentPieceStructure = pieceStructure.structure.Clone() as Vector2Int[];
        currentPieceKicks = pieceStructure.kicks;
        currentPieceKicks180 = pieceStructure.kicks180;
        currentPieceRotation = 0;
        currentData = tileDataSO.GetTileType(piece);
        currentPiece = piece;
        currentPieceSize = pieceStructure.size;

        currentGhostPosition = new(currentPiecePosition.x, currentPiecePosition.y);
        currentGhostStructure = currentPieceStructure;

        lowestY = currentPiecePosition.y;
        activePiece = true;
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
        bag = new Bag(0);
        heldPiece = Piece.None;
        activePiece = false;
        canHold = true;
        Piece[] initialPreviews = new Piece[PREVIEWS];
        for (int i = 0; i < PREVIEWS; i++)
        {
            initialPreviews[i] = bag.PeekAt(i);
        }
        RestartInfo(initialPreviews);
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
}
