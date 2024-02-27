using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Pieces
{
    private int[] bishop_y = { -1, 1, 1, -1 };
    private int[] bishop_x = { 1, 1, -1, -1 };

    public override void SetMovePoint(EColor _color, int y, int x)
    {
        board = ReverseBoard(_color, PlayManager.instance.board);
        tempBoard = board;

        LineCheck((this, _color), y, x, bishop_y, bishop_x);
    }

    public override bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax)
    {
        return LineCheckPos(_color, py, px, ay, ax, bishop_y, bishop_x);
    }
}
