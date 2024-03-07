using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public static Movement instance;

    public EColor playerColor;
    public (Pieces pieces, EColor color) selectedPiece;
    public (int y, int x) selectedPos;
    public bool[,] movePointBoard;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerColor = PlayController.instance.playerColor;
    }

    public void Clear()
    {
        movePointBoard = new bool[8, 8];
    }

    public void Move(int y, int x)
    {
        if (movePointBoard[y, x]) PlayController.instance.MoveRender(Swap(y), Swap(x));
        
        Clear();
        PlayController.instance.MovePointRender?.Invoke();
    }

    private int Swap(int val)
    {
        return playerColor == EColor.white ? val : 7 - val;
    }
}
