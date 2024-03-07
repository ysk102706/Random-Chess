using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public EColor playerColor;
    public Image piece;
    public Image movePoint;
    public Button Select;

    private void Start()
    {
        Select.onClick.AddListener(OnTileClick);
    }

    private void OnTileClick()
    {
        if (PlayController.instance.turnColor != playerColor) return;

        string pos = gameObject.name;
        int y = pos[0] - 48;
        int x = pos[3] - 48;

        (Pieces piece, EColor color) piece = PlayController.instance.board[Swap(y), Swap(x)];
        if (playerColor != piece.color) Movement.instance.Move(y, x);
        else
        {
            Movement.instance.selectedPiece = piece;
            Movement.instance.selectedPos = (Swap(y), Swap(x));
            piece.piece.ShowMovePoint(playerColor, y, x);
        }
    }

    private int Swap(int val)
    {
        return playerColor == EColor.white ? val : 7 - val;
    }
}
