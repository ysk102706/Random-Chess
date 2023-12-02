using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class ChessPieces
{
    protected int num;
    protected int alp;
    protected char color;

    protected int x;
    protected int y;

    protected int SaveX;
    protected int SaveY;

    public void setPieces(int num, int alp, char color)
    {
        this.num = num;
        this.alp = alp;
        this.color = color;
    }

    public virtual void ShowPoint() {  }

    public virtual void Move(int num, int alp) {  }

    public virtual void setKMap(char color) {  }

    public virtual void isCheckmate(char color) {  }

    public virtual void isDefence(char color) {  }

    public void isInsufficientMaterials(GameManager gameManager)
    {
        int countQueen = 0;
        int countRook = 0;
        int countBishop = 0;
        int countKnight = 0;
        int BishopColor = -1;
        bool BishopCheck = true;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                switch(gameManager.getMap(i, j).Substring(1))
                {
                    case "Queen":
                        countQueen++;
                        break;
                    case "Rook":
                        countRook++;
                        break;
                    case "Bishop":
                        countBishop++;
                        if (BishopColor == -1)
                        {
                            BishopColor = (i + j) % 2;
                        }
                        else if (BishopColor == (i + j) % 2) 
                        {
                            BishopCheck = false;
                        }
                        break;
                    case "Knight":
                        countKnight++;
                        break;
                }
            }
        }

        if ((countQueen == 0 && countRook == 0) && ((countBishop == 0 && countKnight == 0) || (countBishop == 0 && countKnight == 1) || ((countBishop == 1 && countKnight == 0) || (countBishop > 1 && !BishopCheck && countKnight == 0))))
        {
            gameManager.setInsufficientMaterials(true);
        }
    }

}
