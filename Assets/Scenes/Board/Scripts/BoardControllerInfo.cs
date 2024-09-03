using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BoardConstants;
using static Pieces;

public partial class BoardController : MonoBehaviour
{
    [SerializeField] private GameObject I, J, L, O, S, T, Z;
    [SerializeField] private TMP_Text clearLines, clearModifier, clearB2B, clearCombo;
    private Transform holdParent;
    private Transform queueParent;
    private Dictionary<Piece, GameObject> piecePrefabs;
    private const float SPACING = 3f;
    private readonly Queue<GameObject> previewPieces = new();
    private Vector3 up = new(0, SPACING * BOARD_SCALE, 0);
    private readonly string[] clears = { "Single", "Double", "Triple", "Quad" };

    public void InitInfo(Piece[] pieces)
    {
        if (cleanMode)
            return;

        holdParent = transform.Find("Hold");
        queueParent = transform.Find("Queue");
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
        if (cleanMode)
            return;

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
            float x = queueParent.position.x;
            float y = queueParent.position.y - i * SPACING * BOARD_SCALE;
            GameObject piece = Instantiate(piecePrefabs[pieces[i]], new(x, y, 0), Quaternion.identity, queueParent);
            previewPieces.Enqueue(piece);
        }
    }

    public void UpdatePreview(Piece piece)
    {
        if (cleanMode)
            return;

        Destroy(previewPieces.Dequeue());
        foreach (GameObject previewPiece in previewPieces)
        {
            previewPiece.transform.position += up;
        }
        float x = queueParent.position.x;
        float y = queueParent.position.y - previewPieces.Count * SPACING * BOARD_SCALE;
        GameObject newPiece = Instantiate(piecePrefabs[piece], new(x, y, 0), Quaternion.identity, queueParent);
        previewPieces.Enqueue(newPiece);
    }

    public void FirstHoldPiece(Piece piece, Piece nextPiece)
    {
        if (cleanMode)
            return;

        UpdatePreview(nextPiece);
        Instantiate(piecePrefabs[piece], holdParent);
    }

    public void HoldPiece(Piece piece)
    {
        if (cleanMode)
            return;

        Destroy(holdParent.GetChild(0).gameObject);
        Instantiate(piecePrefabs[piece], holdParent);
    }


    public void UpdateClears(int lines, string modifier, int b2b, int combo)
    {
        if (cleanMode)
            return;

        SetClearLinesText(lines);
        SetClearModifierText(modifier);
        SetClearB2BText(b2b);
        SetClearComboText(combo);
    }

    private void SetClearLinesText(int lines)
    {
        if (lines == 0)
        {
            clearLines.text = "";
            return;
        }

        clearLines.text = clears[lines - 1];
    }

    private void SetClearModifierText(string modifier)
    {
        clearModifier.text = modifier;
    }

    private void SetClearB2BText(int b2b)
    {
        clearB2B.text = b2b >= 3 ? $"B2B x {b2b}" : "";
    }

    private void SetClearComboText(int combo)
    {
        clearCombo.text = combo >= 2 ? $"Combo x {combo}" : "";
    }
}