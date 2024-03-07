using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum EColor
{
    none,
    white,
    black
}

public class Pieces : MonoBehaviour
{
    public Sprite _whiteImage;
    public Sprite _blackImage;

    public bool[,] movePoint = new bool[8, 8];
    public (Pieces pieces, EColor color)[,] board;
    public static (Pieces pieces, EColor color)[,] tempBoard;
    public static (Pieces pieces, EColor color) tempPiece;

    public Sprite GetImage(EColor _color)
    {
        return _color == EColor.white ? _whiteImage : _blackImage;
    }

    public void ShowMovePoint(EColor _color, int y, int x)
    {
        Movement.instance.Clear();
        movePoint = new bool[8, 8];
        SetMovePoint(_color, y, x);
        PlayController.instance.SetMovePointBoard(movePoint);
    }

    public (Pieces pieces, EColor color)[,] ReverseBoard(EColor _color, (Pieces pieces, EColor color)[,] board)
    {
        (Pieces pieces, EColor color)[,] temp;
        if (_color == EColor.black)
        {
            temp = new (Pieces pieces, EColor color)[8, 8];
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

    public virtual int SetMovePoint(EColor _color, int y, int x) { return 0; }

    public bool OverCheck(int y, int x)
    {
        return y >= 0 && y < 8 && x >= 0 && x < 8;
    }

    public int LineCheck((Pieces piece, EColor color) piece, int y, int x, int[] move_y, int[] move_x)
    {
        int _movementCount = 0;
        EColor _compareColor = EColor.none;

        for (int idx = 0; idx < move_y.Length; idx++)
        {
            for (int pos = 1; pos < 8; pos++)
            {
                int ny = move_y[idx] * pos + y;
                int nx = move_x[idx] * pos + x;

                if (!OverCheck(ny, nx)) break;
                
                _compareColor = board[ny, nx].color;
                if (_compareColor != piece.color)
                {
                    TempMove(piece, y, x, ny, nx);
                    if (AllConfirmPos(piece.color, 8, 8))
                    {
                        _movementCount++;
                        movePoint[ny, nx] = true;
                        if (tempPiece.pieces == PlayController.instance._king) PlayController.instance.sendCheckData = true;
                    }
                    TempRecovery(piece, y, x, ny, nx);
                    if (_compareColor != EColor.none) break;
                }
                else break;
            }
        }

        return _movementCount;
    }

    public bool LineCheckPos(EColor _color, int y, int x, int ay, int ax, int[] move_y, int[] move_x)
    {
        for (int idx = 0; idx < move_y.Length; idx++)
        {
            for (int pos = 1; pos < 8; pos++)
            {
                int ny = move_y[idx] * pos + y;
                int nx = move_x[idx] * pos + x;

                if (!OverCheck(ny, nx)) break;
                if (ny == ay && nx == ax) return true;
                if (tempBoard[ny, nx] == (PlayController.instance._king, _color)) return true;
                if (tempBoard[ny, nx].color != EColor.none) break;
            }

        }
        return false;
    }

    public bool AllConfirmPos(EColor _color, int ay, int ax)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j].color != _color && board[i, j].color != EColor.none && board[i, j].pieces.PiecesConfirmPos(_color, i, j, ay, ax)) return false;
            }
        }

        return true;
    }

    public virtual bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax) { return false; }

    public void TempMove((Pieces piece, EColor color) piece, int post_y, int post_x, int pre_y, int pre_x)
    {
        tempPiece = board[pre_y, pre_x];
        tempBoard[pre_y, pre_x] = piece;
        tempBoard[post_y, post_x] = (null, EColor.none);
    }

    public void TempRecovery((Pieces piece, EColor color) piece, int post_y, int post_x, int pre_y, int pre_x)
    {
        tempBoard[pre_y, pre_x] = tempPiece;
        tempBoard[post_y, post_x] = piece;
    }

}