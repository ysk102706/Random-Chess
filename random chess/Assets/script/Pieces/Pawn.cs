using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : Pieces
{
    public static bool[,] EnPassant;

    public override int SetMovePoint(EColor _color, int y, int x)
    {
        int _movementCount = 0;
        board = ReverseBoard(_color, PlayController.instance.board);
        tempBoard = board;
        bool[,] enPassant = ReverseEnPassant(_color, EnPassant);
        EColor _compareColor = EColor.none;

        if (OverCheck(y - 1, x) && board[y - 1, x].pieces == null)
        {
            TempMove((this, _color), y, x, y - 1, x);
            if (AllConfirmPos(_color, 8, 8))
            {
                _movementCount++;
                movePoint[y - 1, x] = true;
                if (y == 6 && board[y - 2, x].pieces == null)
                {
                    TempMove((this, _color), y - 1, x, y - 2, x);
                    if (AllConfirmPos(_color, 8, 8))
                    {
                        _movementCount++;
                        movePoint[y - 2, x] = true;
                    }
                    TempRecovery((this, _color), y - 1, x, y - 2, x);
                }
            }
            TempRecovery((this, _color), y, x, y - 1, x);
        }

        if (OverCheck(y - 1, x - 1))
        {
            _compareColor = board[y - 1, x - 1].color;
            if (_compareColor != EColor.none && _compareColor != _color)
            {
                TempMove((this, _color), y, x, y - 1, x - 1);
                if (AllConfirmPos(_color, 8, 8))
                {
                    _movementCount++;
                    movePoint[y - 1, x - 1] = true;
                    if (tempPiece.pieces == PlayController.instance._king) PlayController.instance.sendCheckData = true;
                }
                TempRecovery((this, _color), y, x, y - 1, x - 1);
            }
        }
        if (OverCheck(y - 1, x + 1))
        {
            _compareColor = board[y - 1, x + 1].color;
            if (_compareColor != EColor.none && _compareColor != _color)
            {
                TempMove((this, _color), y, x, y - 1, x + 1);
                if (AllConfirmPos(_color, 8, 8))
                {
                    _movementCount++;
                    movePoint[y - 1, x + 1] = true;
                    if (tempPiece.pieces == PlayController.instance._king) PlayController.instance.sendCheckData = true;
                }
                TempRecovery((this, _color), y, x, y - 1, x + 1);
            }

        }

        if (y == 3)
        {
            if (OverCheck(y - 1, x - 1) && enPassant[y, x - 1])
            {
                TempMove((this, _color), y, x, y - 1, x - 1);
                if (AllConfirmPos(_color, 8, 8))
                {
                    _movementCount++;
                    movePoint[y - 1, x - 1] = true;
                }
                TempRecovery((this, _color), y, x, y - 1, x - 1);
            }
            if (OverCheck(y - 1, x + 1) && enPassant[y, x + 1])
            {
                TempMove((this, _color), y, x, y - 1, x + 1);
                if (AllConfirmPos(_color, 8, 8))
                {
                    _movementCount++;
                    movePoint[y - 1, x + 1] = true;
                }
                TempRecovery((this, _color), y, x, y - 1, x + 1);
            }
        }

        return _movementCount;
    }

    public bool[,] ReverseEnPassant(EColor _color, bool[,] board)
    {
        if (board == null) return new bool[8, 8];

        bool[,] temp;
        if (_color == EColor.black)
        {
            temp = new bool[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    temp[i, j] = board[7 - i, 7 - j];
                }
            }
        }
        else temp = board;

        return temp;
    }

    public override bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax)
    {
        if (OverCheck(py + 1, px - 1))
        {
            if (py + 1 == ay && px - 1 == ax) return true;
            if (tempBoard[py + 1, px - 1] == (PlayController.instance._king, _color)) return true;
        }

        if (OverCheck(py + 1, px + 1))
        {
            if (py + 1 == ay && px + 1 == ax) return true;
            if (tempBoard[py + 1, px + 1] == (PlayController.instance._king, _color)) return true;
        }

        return false;
    }
}
