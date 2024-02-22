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

        for (int idx = 0; idx < 4; idx++) LineCheck(_color, idx, y, x);
    }

    private void LineCheck(EColor _color, int idx, int y, int x)
    {
        EColor _compareColor = EColor.none;

        for (int pos = 1; pos < 8; pos++)
        {
            int ny = bishop_y[idx] * pos;
            int nx = bishop_x[idx] * pos;

            if (!OverCheck(y + ny, x + nx)) return;
            _compareColor = board[y + ny, x + nx].color;
            if (_compareColor != _color)
            {
                movePoint[y + ny, x + nx] = true;
                if (_compareColor != EColor.none) return;
            }
            else return;
        }
    }
}
