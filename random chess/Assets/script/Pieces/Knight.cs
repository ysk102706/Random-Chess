using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Pieces
{
    private int[] knight_y = { -2, -1, 1, 2, 2, 1, -1, -2 };
    private int[] knight_x = { 1, 2, 2, 1, -1, -2, -2, -1 };

    public override int SetMovePoint(EColor _color, int y, int x)
    {
        int _movementCount = 0;
        board = ReverseBoard(_color, PlayController.instance.board);
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
                    _movementCount++;
                    movePoint[ny, nx] = true;
                    if (tempPiece.pieces == PlayController.instance._king) PlayController.instance.sendCheckData = true;
                }
                TempRecovery((this, _color), y, x, ny, nx);
            }
        }

        return _movementCount;
    }

    public override bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax)
    {
        for (int idx = 0; idx < 8; idx++)
        {
            int ny = knight_y[idx];
            int nx = knight_x[idx];

            if (!OverCheck(py + ny, px + nx)) continue;
            if (py + ny == ay && px + nx == ax) return true;
            if (tempBoard[py + ny, px + nx] == (PlayController.instance._king, _color)) return true;
        }

        return false;
    }
}
