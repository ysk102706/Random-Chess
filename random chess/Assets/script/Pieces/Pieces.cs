using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public bool[,] movePoint;
    public (Pieces pieces, EColor color)[,] board;

    public Sprite GetImage(EColor _color)
    {
        return _color == EColor.white ? _whiteImage : _blackImage;
    }

    public void ShowMovePoint(EColor _color, int y, int x)
    {
        Movement.instance.Clear();
        movePoint = new bool[8, 8];
        SetMovePoint(_color, y, x);
        PlayManager.instance.SetMovePointBoard(movePoint);
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

    public virtual void SetMovePoint(EColor _color, int y, int x) { }

    public bool OverCheck(int y, int x)
    {
        return y >= 0 && y < 8 && x >= 0 && x < 8;
    }
}