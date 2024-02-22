using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Pieces
{
    private int[] knight_y = { -2, -1, 1, 2, 2, 1, -1, -2 };
    private int[] knight_x = { 1, 2, 2, 1, -1, -2, -2, -1 };

    public override void SetMovePoint(EColor _color, int y, int x)
    {
        board = ReverseBoard(_color, PlayManager.instance.board);
        EColor _compareColor = EColor.none;

        for (int idx = 0; idx < 8; idx++)
        {
            int ny = knight_y[idx];
            int nx = knight_x[idx];

            if (!OverCheck(y + ny, x + nx)) continue;

            _compareColor = board[y + ny, x + nx].color;
            if (_compareColor != _color) movePoint[y + ny, x + nx] = true;
        }
    }
}
