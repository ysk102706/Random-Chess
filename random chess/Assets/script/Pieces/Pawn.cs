using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : Pieces
{
    public static bool[,] EnPassant;

    public override void SetMovePoint(EColor _color, int y, int x)
    {
        board = ReverseBoard(_color, PlayManager.instance.board);
        bool[,] enPassant = ReverseEnPassant(_color, EnPassant);
        EColor _compareColor = EColor.none;

        if (OverCheck(y - 1, x) && board[y - 1, x].pieces == null)
        {
            movePoint[y - 1, x] = true;
            if (y == 6 && board[y - 2, x].pieces == null) movePoint[y - 2, x] = true;
        }

        if (OverCheck(y - 1, x - 1))
        {
            _compareColor = board[y - 1, x - 1].color;
            if (_compareColor != EColor.none && _compareColor != _color) movePoint[y - 1, x - 1] = true;
        }
        if (OverCheck(y - 1, x + 1))
        {
            _compareColor = board[y - 1, x + 1].color;
            if (_compareColor != EColor.none && _compareColor != _color) movePoint[y - 1, x + 1] = true;
        }

        if (y == 3)
        {
            if (OverCheck(y - 1, x - 1) && enPassant[y, x - 1]) movePoint[y - 1, x - 1] = true;
            if (OverCheck(y - 1, x + 1) && enPassant[y, x + 1]) movePoint[y - 1, x + 1] = true;
        }
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
}
