using UnityEngine;
using static BoardConstants;

public partial class BoardController : MonoBehaviour
{
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
            RotateCurrentPiece(RotateType.CW);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateCurrentPiece(RotateType._180);
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RotateCurrentPiece(RotateType.CCW);
            timeBuffer = 0;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // Debug.Log("Soft Dropping");
            MaxFallCurrentPiece();
            timeBuffer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            forcedLock = true;
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