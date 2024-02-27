using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Pieces
{
    private int[] queen_y = { -1, -1, 0, 1, 1, 1, 0, -1 };
    private int[] queen_x = { 0, 1, 1, 1, 0, -1, -1, -1 };

    public override void SetMovePoint(EColor _color, int y, int x)
    {
        board = ReverseBoard(_color, PlayManager.instance.board);
        tempBoard = board;

        LineCheck((this, _color), y, x, queen_y, queen_x);
    }

    public override bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax)
    {
        return LineCheckPos(_color, py, px, ay, ax, queen_y, queen_x);
    }
}
