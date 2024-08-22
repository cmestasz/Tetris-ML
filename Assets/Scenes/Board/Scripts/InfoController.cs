using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BoardConstants;
using static Pieces;

public class InfoController : MonoBehaviour
{
    [SerializeField] private GameObject I, J, L, O, S, T, Z;
    private Transform holdParent;
    private Dictionary<Piece, GameObject> piecePrefabs;
    private const float SPACING = 3f;
    private readonly Queue<GameObject> previewPieces = new();
    private Vector3 up = new(0, SPACING * BOARD_SCALE, 0);

    public void InitInfo(Piece[] pieces)
    {
        holdParent = transform.Find("Hold");
        piecePrefabs = new()
        {
            { Piece.I, I },
            { Piece.J, J },
            { Piece.L, L },
            { Piece.O, O },
            { Piece.S, S },
            { Piece.T, T },
            { Piece.Z, Z }
        };
        foreach (GameObject piece in piecePrefabs.Values)
        {
            piece.transform.localScale = new Vector3(BOARD_SCALE, BOARD_SCALE, 1);
        }
        BuildPreviewPieces(pieces);
    }

    public void RestartInfo(Piece[] pieces)
    {
        foreach (GameObject previewPiece in previewPieces)
        {
            Destroy(previewPiece);
        }
        previewPieces.Clear();
        BuildPreviewPieces(pieces);
        if (holdParent.childCount > 0)
            Destroy(holdParent.GetChild(0).gameObject);
    }

    private void BuildPreviewPieces(Piece[] pieces)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            float x = transform.position.x;
            float y = transform.position.y - i * SPACING * BOARD_SCALE;
            GameObject piece = Instantiate(piecePrefabs[pieces[i]], new(x, y, 0), Quaternion.identity, transform);
            previewPieces.Enqueue(piece);
        }
    }

    public void UpdatePreview(Piece piece)
    {
        Destroy(previewPieces.Dequeue());
        foreach (GameObject previewPiece in previewPieces)
        {
            previewPiece.transform.position += up;
        }
        float x = transform.position.x;
        float y = transform.position.y - previewPieces.Count * SPACING * BOARD_SCALE;
        GameObject newPiece = Instantiate(piecePrefabs[piece], new(x, y, 0), Quaternion.identity, transform);
        previewPieces.Enqueue(newPiece);
    }

    public void FirstHoldPiece(Piece piece, Piece nextPiece)
    {
        UpdatePreview(nextPiece);
        Instantiate(piecePrefabs[piece], holdParent);
    }

    public void HoldPiece(Piece piece)
    {
        Destroy(holdParent.GetChild(0).gameObject);
        Instantiate(piecePrefabs[piece], holdParent);
    }
}
