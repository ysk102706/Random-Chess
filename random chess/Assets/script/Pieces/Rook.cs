using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rook : Pieces
{
    private int[] rook_y = { -1, 0, 1, 0 };
    private int[] rook_x = { 0, 1, 0, -1 };

    public static Dictionary<(int y, int x), bool> moveCheck = new()
    {
        { (7, 0), true },
        { (7, 7), true }
    };

    public override int SetMovePoint(EColor _color, int y, int x)
    {
        board = ReverseBoard(_color, PlayController.instance.board);
        tempBoard = board;

        return LineCheck((this, _color), y, x, rook_y, rook_x);
    }

    public override bool PiecesConfirmPos(EColor _color, int py, int px, int ay, int ax)
    {
        return LineCheckPos(_color, py, px, ay, ax, rook_y, rook_x);
    }
}