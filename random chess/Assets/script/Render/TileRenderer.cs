using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileRenderer : MonoBehaviour
{
    public GameObject TilePrefab;
    public Sprite None;

    public Tile[,] renderBoard;
    public EColor playerColor;

    private void Awake()
    {

        PlayController.instance.Init += () =>
        {
            playerColor = PlayController.instance.playerColor;
            InitBoard();
        };
        PlayController.instance.Render += () =>
        {
            Rendering();
        };
        PlayController.instance.MovePointRender += () =>
        {
            MovePointRendering();
        };
    }

    private void InitBoard()
    {
        renderBoard = new Tile[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject tile = Instantiate(TilePrefab, gameObject.transform);
                tile.name = i + ", " + j;
                tile.GetComponent<RectTransform>().anchorMin = new Vector2(j * 0.125f, (8 - (i + 1)) * 0.125f);
                tile.GetComponent<RectTransform>().anchorMax = new Vector2((j + 1) * 0.125f, (8 - i) * 0.125f);
                tile.GetComponent<Tile>().playerColor = playerColor;
                renderBoard[i, j] = tile.GetComponent<Tile>();

            }
        }
    }

    private void Rendering()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int ypos = Swap(i);
                int xpos = Swap(j);
                (Pieces piece, EColor color) piece = PlayController.instance.board[ypos, xpos];
                Image img = renderBoard[i, j].piece;
                img.color = new Color(1, 1, 1, piece.piece == null ? 0 : 1);
                if (piece.piece == null) img.sprite = None;
                else img.sprite = piece.piece.GetImage(piece.color);
            }
        }
    }

    private void MovePointRendering()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Image img = renderBoard[i, j].movePoint;
                img.color = new Color(1, 1, 1, Movement.instance.movePointBoard[i, j] ? 1 : 0);
            }
        }
    }

    private int Swap(int val)
    {
        return playerColor == EColor.white ? val : 7 - val;
    }
}
