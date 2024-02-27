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
        tempBoard = board;
        
        for (int idx = 0; idx < 8; idx++)
        {
            int ny = knight_y[idx] + y;
            int nx = knight_x[idx] + x;

            if (!OverCheck(ny, nx)) continue;

            if (board[ny, nx].color != _color)
            {
                TempMove((this, _color), y, x, ny, nx);
                if (AllConfirmPos(_color, 8, 8))
                {
                    movePoint[ny, nx] = true;
                    if (board[ny, nx].pieces.GetType() == typeof(King)) PlayManager.instance.sendCheckData = true;
                }
                TempRecovery((this, _color), y, x, ny, nx);
            }
        }
    }

    public override bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax)
    {
        for (int idx = 0; idx < 8; idx++)
        {
            int ny = knight_y[idx];
            int nx = knight_x[idx];

            if (!OverCheck(py + ny, px + nx)) continue;
            if (py + ny == ay && px + nx == ax) return true;
            if (tempBoard[py + ny, px + nx] == (PlayManager.instance._king, _color)) return true;
        }

        return false;
    }
}
