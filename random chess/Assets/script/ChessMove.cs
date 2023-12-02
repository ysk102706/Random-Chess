using JetBrains.Annotations;
using System;
using System.Globalization;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChessMove : MonoBehaviour
{
    GameManager gameManager;

    private int alp;
    private int num;
    private char color;
    private Image Tile;

    private float time;

    private ChessPieces pieces;
    private static ChessPieces prepieces;
    private King king;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        Tile = GetComponent<Image>();
        color = transform.parent.transform.parent.name[0];
        gameManager.WhoAmI(color, name, ref num, ref alp);
        king = new King();
        gameManager.getLoading().SetActive(true);
    }

    //Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (gameManager.getMap(num, alp) != " ")
        {
            Tile.sprite = gameManager.getImage(gameManager.getMap(num, alp));
            Tile.color = new Color(255, 255, 255, 1);
        }
        else
        {
            Tile.sprite = gameManager.getImage(gameManager.getMap(num, alp));
            Tile.color = new Color(255, 255, 255, 0);
        }

        if (color == 'W')
        {
            pieces = gameManager.getPieces(gameManager.getMap(num, alp).Substring(1));
            pieces.setPieces(num, alp, 'W');
            pieces.isInsufficientMaterials(gameManager);
            if (pieces.GetType() == typeof(King))
            {
                if (gameManager.getMap(num, alp)[0] == 'W')
                {
                    pieces.isCheckmate('W');
                    king.setKMap('W');
                    gameManager.sentCheck();
                }
                else
                {
                    king.setKMap('B');
                    gameManager.sentCheck();
                }
                gameManager.sentInsufficientMaterials();
            }
        }
        else
        {
            pieces = gameManager.getPieces(gameManager.getMap(num, alp).Substring(1));
            pieces.setPieces(num, alp, 'B');
            pieces.isInsufficientMaterials(gameManager);
            if (pieces.GetType() == typeof(King))
            {
                if (gameManager.getMap(num, alp)[0] == 'B')
                {
                    pieces.isCheckmate('B');
                    king.setKMap('B');
                    gameManager.sentCheck();
                }
                else
                {
                    king.setKMap('W');
                    gameManager.sentCheck();
                }
            }
        }

        if (time > 2)
        {
            gameManager.getLoading().SetActive(false);
        }
        else
        {
            if (pieces.GetType() == typeof(King))
            {
                if (color == 'W' && gameManager.getMap(num, alp)[0] == 'W')
                {
                    king.isDefence('W');
                }
                else if (color == 'B' && gameManager.getMap(num, alp)[0] == 'B')
                {
                    king.isDefence('B');
                }
                gameManager.sentDefence();
            }
        }

        if (gameManager.getisWCheckmate())
        {
            if (pieces.GetType() == typeof(King))
            {
                if (color == 'W' && gameManager.getMap(num, alp)[0] == 'W')
                {
                    king.isDefence('W');
                }
                gameManager.sentDefence();
            }
        }
        if (gameManager.getisBCheckmate())
        {
            if (pieces.GetType() == typeof(King))
            {
                if (color == 'B' && gameManager.getMap(num, alp)[0] == 'B')
                {
                    king.isDefence('B');
                }
                gameManager.sentDefence();
            }
        }

        if ((gameManager.getisWCheck() && !gameManager.getisWDefence()) && gameManager.getisWCheckmate())
        {
            gameManager.ShowResult(true);
            gameManager.GameEnd(new Color(0, 0, 0), "Checkmate");
        }
        else if ((gameManager.getisBCheck() && !gameManager.getisBDefence()) && gameManager.getisBCheckmate())
        {
            gameManager.ShowResult(true);
            gameManager.GameEnd(new Color(255, 255, 255), "Checkmate");
        }
        else if ((!gameManager.getisWDefence() && gameManager.getisWCheckmate()) || (!gameManager.getisBDefence() && gameManager.getisBCheckmate()))
        {
            gameManager.ShowResult(true);
            gameManager.GameEnd(new Color(50, 50, 50), "Stalemate");
        }
        else
        {
            gameManager.ShowResult(false);
        }

        if (gameManager.getInsufficientMaterials())
        {
            gameManager.ShowResult(true);
            gameManager.GameEnd(new Color(50, 50, 50), "Insufficient Materials");
        }
        else
        {
            gameManager.ShowResult(false);
        }

        if (gameManager.getisWCheck())
        {
            gameManager.WCheck(true);
        }
        else
        {
            gameManager.WCheck(false);
        }
        if (gameManager.getisBCheck())
        {
            gameManager.BCheck(true);
        }
        else
        {
            gameManager.BCheck(false);
        }
    }

    public void Clicked()
    {
        if (color == 'B')
        {
            if (gameManager.getBEMap(num, alp))
            {
                prepieces.Move(num, alp);
                gameManager.ResetBEMap();
            }
            else
            {
                gameManager.ResetBEMap();
                if (gameManager.getMap(num, alp)[0] == 'B')
                {
                    ChessClicked('B');
                }
            }
        }
        else
        {
            if (gameManager.getWEMap(num, alp))
            {
                prepieces.Move(num, alp);
                gameManager.ResetWEMap();
            }
            else
            {
                gameManager.ResetWEMap();
                if (gameManager.getMap(num, alp)[0] == 'W')
                {
                    ChessClicked('W');
                }
            }
        }

        king.setKMap('W');
        king.setKMap('B');
        gameManager.sentMap();
        gameManager.sentTurn();
        gameManager.sentCheck();

        if (pieces.GetType() == typeof(ChessPieces))
        {
            if (color == 'W') king.isDefence('W');
            else king.isDefence('B');
            gameManager.sentDefence();
            king.setKMap('W');
            king.setKMap('B');
            gameManager.sentCheck();
        }
    }

    void ChessClicked(char color)
    {
        pieces.ShowPoint();
        prepieces = pieces;
        gameManager.setP(gameManager.getMap(num, alp), num, alp);
    }
}

class King : ChessPieces
{
    GameManager gameManager = GameManager.Instance;

    private bool[,] KMap = new bool[8, 8];

    private static bool WKsC = true;
    private static bool WQsC = true;

    private static bool BKsC = true;
    private static bool BQsC = true;

    private int Wcount, Bcount;

    public override void ShowPoint()
    {
        if (color == 'W')
        {
            setKMap('W');
            if (gameManager.getisWKsC())
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!KMap[num, alp + i])
                    {
                        if (i != 0 && gameManager.getMap(num, alp + i) != " ")
                        {
                            WKsC = false;
                            break;
                        }
                    }
                    else
                    {
                        WKsC = false;
                        break;
                    }
                    WKsC = true;
                }
            }
            else
            {
                WKsC = false;
            }
            if (gameManager.getisWQsC())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!KMap[num, alp - i])
                    {
                        if (i != 0 && gameManager.getMap(num, alp - i) != " ")
                        {
                            WQsC = false;
                            break;
                        }
                    }
                    else
                    {
                        WQsC = false;
                        break;
                    }
                    WQsC = true;
                }
            }
            else
            {
                WQsC = false;
            }
            if (WKsC)
            {
                gameManager.setWEMap(num, alp + 2, true);
            }
            if (WQsC)
            {
                gameManager.setWEMap(num, alp - 2, true);
            }
        }
        else
        {
            setKMap('B');
            if (gameManager.getisBKsC())
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!KMap[num, alp + i])
                    {
                        if (i != 0 && gameManager.getMap(num, alp + i) != " ")
                        {
                            BKsC = false;
                            break;
                        }
                    }
                    else
                    {
                        BKsC = false;
                        break;
                    }
                    BKsC = true;
                }
            }
            else
            {
                BKsC = false;
            }
            if (gameManager.getisBQsC())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!KMap[num, alp - i])
                    {
                        if (i != 0 && gameManager.getMap(num, alp - i) != " ")
                        {
                            BQsC = false;
                            break;
                        }
                    }
                    else
                    {
                        BQsC = false;
                        break;
                    }
                    BQsC = true;
                }
            }
            else
            {
                BQsC = false;
            }
            if (BKsC)
            {
                gameManager.setBEMap(num, alp + 2, true);
            }
            if (BQsC)
            {
                gameManager.setBEMap(num, alp - 2, true);
            }
        }
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (((num + i < 8 && alp + j < 8) && (num + i >= 0 && alp + j >= 0)))
                {
                    if (color == 'W')
                    {
                        if (gameManager.getMap(num + i, alp + j)[0] != 'W' && !KMap[num + i, alp + j])
                        {
                            string name = gameManager.getMap(num + i, alp + j);
                            gameManager.PrePMove(num + i, alp + j, "WKing", num, alp);
                            this.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i, alp + j, true);
                            }
                            gameManager.PrePMove(num, alp, "WKing", num + i, alp + j);
                            gameManager.setMap(num + i, alp + j, name);
                        }
                    }
                    else
                    {
                        if (gameManager.getMap(num + i, alp + j)[0] != 'B' && !KMap[num + i, alp + j])
                        {
                            string name = gameManager.getMap(num + i, alp + j);
                            gameManager.PrePMove(num + i, alp + j, "BKing", num, alp);
                            this.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i, alp + j, true);
                            }
                            gameManager.PrePMove(num, alp, "BKing", num + i, alp + j);
                            gameManager.setMap(num + i, alp + j, name);
                        }
                    }
                }
            }
        }
        gameManager.sentCheck();
        KMap = new bool[8, 8];
    }

    public override void Move(int num, int alp)
    {
        gameManager.PMove(num, alp);
        if (gameManager.getWenpL() || gameManager.getWenpR())
        {
            gameManager.setWenpL(false);
            gameManager.setWenpR(false);
        }
        if (gameManager.getBenpL() || gameManager.getBenpR())
        {
            gameManager.setBenpL(false);
            gameManager.setBenpR(false);
        }
        if (color == 'W')
        {
            if (WKsC && (num == 7 && alp == 6))
            {
                gameManager.setMap(7, 7, " ");
                gameManager.setMap(7, 5, "WRook");
            }
            if (WQsC && (num == 7 && alp == 2))
            {
                gameManager.setMap(7, 0, " ");
                gameManager.setMap(7, 3, "WRook");
            }
            gameManager.setisWKsC();
            gameManager.setisWQsC();
        }
        else
        {
            if (BKsC && (num == 0 && alp == 6))
            {
                gameManager.setMap(0, 7, " ");
                gameManager.setMap(0, 5, "BRook");
            }
            if (BQsC && (num == 0 && alp == 2))
            {
                gameManager.setMap(0, 0, " ");
                gameManager.setMap(0, 3, "BRook");
            }
            gameManager.setisBKsC();
            gameManager.setisBQsC();
        }
        gameManager.sentData();
    }

    public override void setKMap(char color)
    {
        if (color == 'W')
        {
            gameManager.setisWCheck(false);
        }
        else
        {
            gameManager.setisBCheck(false);
        }

        for (int num = 0; num < 8; num++)
        {
            for (int alp = 0; alp < 8; alp++)
            {
                if (color == 'W')
                {
                    switch (gameManager.getMap(num, alp))
                    {
                        case "BKing":
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    if (((num + i < 8 && alp + j < 8) && (num + i >= 0 && alp + j >= 0)))
                                    {
                                        KMap[num + i, alp + j] = true;
                                    }
                                }
                            }
                            break;
                        case "BQueen":
                            x = -1;
                            y = 2;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "WKing")
                                        {
                                            gameManager.setisWCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            x = -1;
                            y = 1;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "WKing")
                                        {
                                            gameManager.setisWCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "BRook":
                            x = -1;
                            y = 2;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "WKing")
                                        {
                                            gameManager.setisWCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "BBishop":
                            x = -1;
                            y = 1;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "WKing")
                                        {
                                            gameManager.setisWCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "BKnight":
                            for (int i = -2; i < 3; i++)
                            {
                                for (int j = -2; j < 3; j++)
                                {
                                    if (((Math.Abs(i) == 1 && Math.Abs(j) == 2) || (Math.Abs(i) == 2 && Math.Abs(j) == 1)))
                                    {
                                        if ((num + i >= 0 && num + i < 8) && (alp + j >= 0 && alp + j < 8))
                                        {
                                            KMap[num + i, alp + j] = true;
                                            if (gameManager.getMap(num + i, alp + j) == "WKing")
                                            {
                                                gameManager.setisWCheck(true);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "BPawn":
                            if (num + 1 < 8 && alp - 1 >= 0)
                            {
                                KMap[num + 1, alp - 1] = true;
                                if (gameManager.getMap(num + 1, alp - 1) == "WKing")
                                {
                                    gameManager.setisWCheck(true);
                                }
                            }
                            if (num + 1 < 8 && alp + 1 < 8)
                            {
                                KMap[num + 1, alp + 1] = true;
                                if (gameManager.getMap(num + 1, alp + 1) == "WKing")
                                {
                                    gameManager.setisWCheck(true);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (gameManager.getMap(num, alp))
                    {
                        case "WKing":
                            for (int i = -1; i < 2; i++)
                            {
                                for (int j = -1; j < 2; j++)
                                {
                                    if (((num + i < 8 && alp + j < 8) && (num + i >= 0 && alp + j >= 0)))
                                    {
                                        KMap[num + i, alp + j] = true;
                                    }
                                }
                            }
                            break;
                        case "WQueen":
                            x = -1;
                            y = 2;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "BKing")
                                        {
                                            gameManager.setisBCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            x = -1;
                            y = 1;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "BKing")
                                        {
                                            gameManager.setisBCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "WRook":
                            x = -1;
                            y = 2;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "BKing")
                                        {
                                            gameManager.setisBCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "WBishop":
                            x = -1;
                            y = 1;
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x) == "BKing")
                                        {
                                            gameManager.setisBCheck(true);
                                        }
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W' || gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                            break;
                                        }
                                        else
                                        {
                                            KMap[num + i * y, alp + i * x] = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "WKnight":
                            for (int i = -2; i < 3; i++)
                            {
                                for (int j = -2; j < 3; j++)
                                {
                                    if (((Math.Abs(i) == 1 && Math.Abs(j) == 2) || (Math.Abs(i) == 2 && Math.Abs(j) == 1)))
                                    {
                                        if ((num + i >= 0 && num + i < 8) && (alp + j >= 0 && alp + j < 8))
                                        {
                                            KMap[num + i, alp + j] = true;
                                            if (gameManager.getMap(num + i, alp + j) == "BKing")
                                            {
                                                gameManager.setisBCheck(true);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "WPawn":
                            if (num - 1 < 8 && alp - 1 >= 0)
                            {
                                KMap[num - 1, alp - 1] = true;
                                if (gameManager.getMap(num - 1, alp - 1) == "BKing")
                                {
                                    gameManager.setisBCheck(true);
                                }
                            }
                            if (num - 1 < 8 && alp + 1 < 8)
                            {
                                KMap[num - 1, alp + 1] = true;
                                if (gameManager.getMap(num - 1, alp + 1) == "BKing")
                                {
                                    gameManager.setisBCheck(true);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    public override void isDefence(char color)
    {
        if (color == 'W')
        {
            gameManager.setisWDefence(false);
        }
        else
        {
            gameManager.setisBDefence(false);
        }

        for (int num = 0; num < 8; num++)
        {
            for (int alp = 0; alp < 8; alp++)
            {
                switch (gameManager.getMap(num, alp).Substring(1))
                {
                    case "Queen":
                        x = -1;
                        y = 2;
                        if (gameManager.getMap(num, alp)[0] == 'W' && color == 'W')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "WQueen", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisWCheck())
                                            {
                                                gameManager.setisWDefence(true);
                                                gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (gameManager.getMap(num, alp)[0] == 'B' && color == 'B')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "BQueen", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisBCheck())
                                            {
                                                gameManager.setisBDefence(true);
                                                gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        x = -1;
                        y = 1;
                        if (gameManager.getMap(num, alp)[0] == 'W' && color == 'W')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "WQueen", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisWCheck())
                                            {
                                                gameManager.setisWDefence(true);
                                                gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (gameManager.getMap(num, alp)[0] == 'B' && color == 'B')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "BQueen", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisBCheck())
                                            {
                                                gameManager.setisBDefence(true);
                                                gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "Rook":
                        x = -1;
                        y = 2;
                        if (gameManager.getMap(num, alp)[0] == 'W' && color == 'W')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "WRook", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisWCheck())
                                            {
                                                gameManager.setisWDefence(true);
                                                gameManager.PrePMove(num, alp, "WRook", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "WRook", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (gameManager.getMap(num, alp)[0] == 'B' && color == 'B')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f > 1)
                                {
                                    x -= 1;
                                }
                                else
                                {
                                    x += 1;
                                }
                                if (f < 3)
                                {
                                    y -= 1;
                                }
                                else
                                {
                                    y += 1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "BRook", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisBCheck())
                                            {
                                                gameManager.setisBDefence(true);
                                                gameManager.PrePMove(num, alp, "BRook", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "BRook", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "Bishop":
                        x = -1;
                        y = 1;
                        if (gameManager.getMap(num, alp)[0] == 'W' && color == 'W')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "WBishop", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisWCheck())
                                            {
                                                gameManager.setisWDefence(true);
                                                gameManager.PrePMove(num, alp, "WBishop", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "WBishop", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (gameManager.getMap(num, alp)[0] == 'B' && color == 'B')
                        {
                            for (int f = 0; f < 4; f++)
                            {
                                if (f % 2 == 0)
                                {
                                    x *= -1;
                                }
                                else if (f % 2 == 1)
                                {
                                    y *= -1;
                                }
                                for (int i = 1; i < 8; i++)
                                {
                                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                                    {
                                        if (gameManager.getMap(num + i * y, alp + i * x)[0] != color)
                                        {
                                            string name = gameManager.getMap(num + i * y, alp + i * x);
                                            gameManager.PrePMove(num + i * y, alp + i * x, "BBishop", num, alp);
                                            SaveX = x;
                                            SaveY = y;
                                            this.setKMap(color);
                                            x = SaveX;
                                            y = SaveY;
                                            gameManager.sentCheck();
                                            if (!gameManager.getisBCheck())
                                            {
                                                gameManager.setisBDefence(true);
                                                gameManager.PrePMove(num, alp, "BBishop", num + i * y, alp + i * x);
                                                gameManager.setMap(num + i * y, alp + i * x, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "BBishop", num + i * y, alp + i * x);
                                            gameManager.setMap(num + i * y, alp + i * x, name);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "Knight":
                        for (int i = -2; i < 3; i++)
                        {
                            for (int j = -2; j < 3; j++)
                            {
                                if (((Math.Abs(i) == 1 && Math.Abs(j) == 2) || (Math.Abs(i) == 2 && Math.Abs(j) == 1)))
                                {
                                    if ((num + i >= 0 && num + i < 8) && (alp + j >= 0 && alp + j < 8))
                                    {
                                        if (gameManager.getMap(num, alp)[0] == 'W' && color == 'W')
                                        {
                                            if (gameManager.getMap(num + i, alp + j)[0] != color)
                                            {
                                                string name = gameManager.getMap(num + i, alp + j);
                                                gameManager.PrePMove(num + i, alp + j, "WKnight", num, alp);
                                                this.setKMap(color);
                                                gameManager.sentCheck();
                                                if (!gameManager.getisWCheck())
                                                {
                                                    gameManager.setisWDefence(true);
                                                    gameManager.PrePMove(num, alp, "WKnight", num + i, alp + j);
                                                    gameManager.setMap(num + i, alp + j, name);
                                                    break;
                                                }
                                                gameManager.PrePMove(num, alp, "WKnight", num + i, alp + j);
                                                gameManager.setMap(num + i, alp + j, name);
                                            }
                                        }
                                        else if (gameManager.getMap(num, alp)[0] == 'B' && color == 'B')
                                        {
                                            if (gameManager.getMap(num + i, alp + j)[0] != color)
                                            {
                                                string name = gameManager.getMap(num + i, alp + j);
                                                gameManager.PrePMove(num + i, alp + j, "BKnight", num, alp);
                                                this.setKMap(color);
                                                gameManager.sentCheck();
                                                if (!gameManager.getisBCheck())
                                                {
                                                    gameManager.setisBDefence(true);
                                                    gameManager.PrePMove(num, alp, "BKnight", num + i, alp + j);
                                                    gameManager.setMap(num + i, alp + j, name);
                                                    break;
                                                }
                                                gameManager.PrePMove(num, alp, "BKnight", num + i, alp + j);
                                                gameManager.setMap(num + i, alp + j, name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "Pawn":
                        if (gameManager.getMap(num, alp)[0] == 'W' && color == 'W')
                        {
                            if (!gameManager.getisWDefence())
                            {
                                if (alp - 1 >= 0 && gameManager.getMap(num - 1, alp - 1)[0] == 'B')
                                {
                                    if (gameManager.getMap(num - 1, alp - 1)[0] != color)
                                    {
                                        string name = gameManager.getMap(num - 1, alp - 1);
                                        gameManager.PrePMove(num - 1, alp - 1, "WPawn", num, alp);
                                        this.setKMap(color);
                                        gameManager.sentCheck();
                                        if (!gameManager.getisWCheck())
                                        {
                                            gameManager.setisWDefence(true);
                                            gameManager.PrePMove(num, alp, "WPawn", num - 1, alp - 1);
                                            gameManager.setMap(num - 1, alp - 1, name);
                                            break;
                                        }
                                        gameManager.PrePMove(num, alp, "WPawn", num - 1, alp - 1);
                                        gameManager.setMap(num - 1, alp - 1, name);
                                    }
                                }
                                if (alp + 1 < 8 && gameManager.getMap(num - 1, alp + 1)[0] == 'B')
                                {
                                    if (gameManager.getMap(num - 1, alp + 1)[0] != color)
                                    {
                                        string name = gameManager.getMap(num - 1, alp + 1);
                                        gameManager.PrePMove(num - 1, alp + 1, "WPawn", num, alp);
                                        this.setKMap(color);
                                        gameManager.sentCheck();
                                        if (!gameManager.getisWCheck())
                                        {
                                            gameManager.setisWDefence(true);
                                            gameManager.PrePMove(num, alp, "WPawn", num - 1, alp + 1);
                                            gameManager.setMap(num - 1, alp + 1, name);
                                            break;
                                        }
                                        gameManager.PrePMove(num, alp, "WPawn", num - 1, alp + 1);
                                        gameManager.setMap(num - 1, alp + 1, name);
                                    }
                                }
                                if (num == 6)
                                {
                                    for (int i = -1; i >= -2; i--)
                                    {
                                        if (gameManager.getMap(num + i, alp) == " ")
                                        {
                                            string name = gameManager.getMap(num + i, alp);
                                            gameManager.PrePMove(num + i, alp, "WPawn", num, alp);
                                            this.setKMap(color);
                                            gameManager.sentCheck();
                                            if (!gameManager.getisWCheck())
                                            {
                                                gameManager.setisWDefence(true);
                                                gameManager.PrePMove(num, alp, "WPawn", num + i, alp);
                                                gameManager.setMap(num + i, alp, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "WPawn", num + i, alp);
                                            gameManager.setMap(num + i, alp, name);
                                        }
                                    }
                                }
                                else
                                {
                                    if (gameManager.getMap(num - 1, alp) == " ")
                                    {
                                        string name = gameManager.getMap(num - 1, alp);
                                        gameManager.PrePMove(num - 1, alp, "WPawn", num, alp);
                                        this.setKMap(color);
                                        gameManager.sentCheck();
                                        if (!gameManager.getisWCheck())
                                        {
                                            gameManager.setisWDefence(true);
                                            gameManager.PrePMove(num, alp, "WPawn", num - 1, alp);
                                            gameManager.setMap(num - 1, alp, name);
                                            break;
                                        }
                                        gameManager.PrePMove(num, alp, "WPawn", num - 1, alp);
                                        gameManager.setMap(num - 1, alp, name);
                                    }
                                }
                            }
                        }
                        else if (gameManager.getMap(num, alp)[0] == 'B' && color == 'B')
                        {
                            if (!gameManager.getisBDefence())
                            {
                                if (alp + 1 < 8 && gameManager.getMap(num + 1, alp + 1)[0] == 'W')
                                {
                                    if (gameManager.getMap(num + 1, alp + 1)[0] != color)
                                    {
                                        string name = gameManager.getMap(num + 1, alp + 1);
                                        gameManager.PrePMove(num + 1, alp + 1, "BPawn", num, alp);
                                        this.setKMap(color);
                                        gameManager.sentCheck();
                                        if (!gameManager.getisBCheck())
                                        {
                                            gameManager.setisBDefence(true);
                                            gameManager.PrePMove(num, alp, "BPawn", num + 1, alp + 1);
                                            gameManager.setMap(num + 1, alp + 1, name);
                                            break;
                                        }
                                        gameManager.PrePMove(num, alp, "BPawn", num + 1, alp + 1);
                                        gameManager.setMap(num + 1, alp + 1, name);
                                    }
                                }
                                if (alp - 1 >= 0 && gameManager.getMap(num - 1, alp - 1)[0] == 'W')
                                {
                                    if (gameManager.getMap(num + 1, alp - 1)[0] != color)
                                    {
                                        string name = gameManager.getMap(num + 1, alp - 1);
                                        gameManager.PrePMove(num + 1, alp - 1, "BPawn", num, alp);
                                        this.setKMap(color);
                                        gameManager.sentCheck();
                                        if (gameManager.getisBCheck())
                                        {
                                            gameManager.setisBDefence(true);
                                            gameManager.PrePMove(num, alp, "BPawn", num + 1, alp - 1);
                                            gameManager.setMap(num + 1, alp - 1, name);
                                            break;
                                        }
                                        gameManager.PrePMove(num, alp, "BPawn", num + 1, alp - 1);
                                        gameManager.setMap(num + 1, alp - 1, name);
                                    }
                                }
                                if (num == 1)
                                {
                                    for (int i = 1; i <= 2; i++)
                                    {
                                        if (gameManager.getMap(num + i, alp) == " ")
                                        {
                                            string name = gameManager.getMap(num + i, alp);
                                            gameManager.PrePMove(num + i, alp, "BPawn", num, alp);
                                            this.setKMap(color);
                                            gameManager.sentCheck();
                                            if (!gameManager.getisWCheck())
                                            {
                                                gameManager.setisWDefence(true);
                                                gameManager.PrePMove(num, alp, "BPawn", num + i, alp);
                                                gameManager.setMap(num + i, alp, name);
                                                break;
                                            }
                                            gameManager.PrePMove(num, alp, "BPawn", num + i, alp);
                                            gameManager.setMap(num + i, alp, name);
                                        }
                                    }
                                }
                                else
                                {
                                    if (gameManager.getMap(num + 1, alp) == " ")
                                    {
                                        string name = gameManager.getMap(num + 1, alp);
                                        gameManager.PrePMove(num + 1, alp, "BPawn", num, alp);
                                        this.setKMap(color);
                                        gameManager.sentCheck();
                                        if (!gameManager.getisWCheck())
                                        {
                                            gameManager.setisWDefence(true);
                                            gameManager.PrePMove(num, alp, "BPawn", num + 1, alp);
                                            gameManager.setMap(num + 1, alp, name);
                                            break;
                                        }
                                        gameManager.PrePMove(num, alp, "BPawn", num + 1, alp);
                                        gameManager.setMap(num + 1, alp, name);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        this.setKMap('W');
        this.setKMap('B');
        gameManager.sentCheck();
    }

    public override void isCheckmate(char color)
    {
        if (color == 'W')
        {
            Wcount = 0;
            gameManager.setisWCheckmate(false);
            this.setKMap('W');
            gameManager.sentCheck();
        }
        else
        {
            Bcount = 0;
            gameManager.setisBCheckmate(false);
            this.setKMap('B');
            gameManager.sentCheck();
        }

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (((num + i < 8 && alp + j < 8) && (num + i >= 0 && alp + j >= 0)))
                {
                    if (color == 'W')
                    {
                        if (gameManager.getMap(num + i, alp + j)[0] != 'W' && !KMap[num + i, alp + j])
                        {
                            string name = gameManager.getMap(num + i, alp + j);
                            gameManager.PrePMove(num + i, alp + j, "WKing", num, alp);
                            this.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                Wcount++;
                            }
                            gameManager.PrePMove(num, alp, "WKing", num + i, alp + j);
                            gameManager.setMap(num + i, alp + j, name);
                        }
                    }
                    else
                    {
                        if (gameManager.getMap(num + i, alp + j)[0] != 'B' && !KMap[num + i, alp + j])
                        {
                            string name = gameManager.getMap(num + i, alp + j);
                            gameManager.PrePMove(num + i, alp + j, "BKing", num, alp);
                            this.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                Bcount++;
                            }
                            gameManager.PrePMove(num, alp, "BKing", num + i, alp + j);
                            gameManager.setMap(num + i, alp + j, name);
                        }
                    }
                }
            }
        }

        if (color == 'W' && Wcount == 0)
        {
            gameManager.setisWCheckmate(true);
        }
        if (color == 'B' && Bcount == 0)
        {
            gameManager.setisBCheckmate(true);
        }

        gameManager.sentCheck();
    }
}

class Queen : ChessPieces
{
    GameManager gameManager = GameManager.Instance;
    private King king = new King();

    public override void ShowPoint()
    {
        x = -1;
        y = 2;
        if (color == 'W')
        {
            for (int f = 0; f < 4; f++)
            {
                if (f > 1)
                {
                    x -= 1;
                }
                else
                {
                    x += 1;
                }
                if (f < 3)
                {
                    y -= 1;
                }
                else
                {
                    y += 1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WQueen", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WQueen", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            x = -1;
            y = 1;
            for (int f = 0; f < 4; f++)
            {
                if (f % 2 == 0)
                {
                    x *= -1;
                }
                else if (f % 2 == 1)
                {
                    y *= -1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WQueen", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WQueen", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int f = 0; f < 4; f++)
            {
                if (f > 1)
                {
                    x -= 1;
                }
                else
                {
                    x += 1;
                }
                if (f < 3)
                {
                    y -= 1;
                }
                else
                {
                    y += 1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BQueen", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BQueen", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            x = -1;
            y = 1;
            for (int f = 0; f < 4; f++)
            {
                if (f % 2 == 0)
                {
                    x *= -1;
                }
                else if (f % 2 == 1)
                {
                    y *= -1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BQueen", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BQueen", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BQueen", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public override void Move(int num, int alp)
    {
        gameManager.PMove(num, alp);
        if (gameManager.getWenpL() || gameManager.getWenpR())
        {
            gameManager.setWenpL(false);
            gameManager.setWenpR(false);
        }
        if (gameManager.getBenpL() || gameManager.getBenpR())
        {
            gameManager.setBenpL(false);
            gameManager.setBenpR(false);
        }
        gameManager.sentData();
    }
}

class Rook : ChessPieces
{
    GameManager gameManager = GameManager.Instance;
    private King king = new King();

    public override void ShowPoint()
    {
        x = -1;
        y = 2;
        if (color == 'W')
        {
            for (int f = 0; f < 4; f++)
            {
                if (f > 1)
                {
                    x -= 1;
                }
                else
                {
                    x += 1;
                }
                if (f < 3)
                {
                    y -= 1;
                }
                else
                {
                    y += 1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WRook", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WRook", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WRook", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WRook", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int f = 0; f < 4; f++)
            {
                if (f > 1)
                {
                    x -= 1;
                }
                else
                {
                    x += 1;
                }
                if (f < 3)
                {
                    y -= 1;
                }
                else
                {
                    y += 1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BRook", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BRook", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BRook", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BRook", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public override void Move(int num, int alp)
    {
        gameManager.PMove(num, alp);
        if (gameManager.getWenpL() || gameManager.getWenpR())
        {
            gameManager.setWenpL(false);
            gameManager.setWenpR(false);
        }
        if (gameManager.getBenpL() || gameManager.getBenpR())
        {
            gameManager.setBenpL(false);
            gameManager.setBenpR(false);
        }
        if (color == 'W')
        {
            if (alp == 0)
            {
                gameManager.setisWQsC();
            }
            else if (alp == 7)
            {
                gameManager.setisWKsC();
            }
        }
        else
        {
            if (alp == 0)
            {
                gameManager.setisBQsC();
            }
            else if (alp == 7)
            {
                gameManager.setisBKsC();
            }
        }
        gameManager.sentData();
    }
}

class Bishop : ChessPieces
{
    GameManager gameManager = GameManager.Instance;
    private King king = new King();

    public override void ShowPoint()
    {
        x = -1;
        y = 1;
        if (color == 'W')
        {
            for (int f = 0; f < 4; f++)
            {
                if (f % 2 == 0)
                {
                    x *= -1;
                }
                else if (f % 2 == 1)
                {
                    y *= -1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WBishop", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WBishop", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "WBishop", num, alp);
                            king.setKMap('W');
                            gameManager.sentCheck();
                            if (!gameManager.getisWCheck())
                            {
                                gameManager.setWEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "WBishop", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int f = 0; f < 4; f++)
            {
                if (f % 2 == 0)
                {
                    x *= -1;
                }
                else if (f % 2 == 1)
                {
                    y *= -1;
                }
                for (int i = 1; i < 8; i++)
                {
                    if ((num + i * y >= 0 && num + i * y < 8) && (alp + i * x >= 0 && alp + i * x < 8))
                    {
                        if (gameManager.getMap(num + i * y, alp + i * x)[0] == 'W')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BBishop", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BBishop", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                            break;
                        }
                        else if (gameManager.getMap(num + i * y, alp + i * x)[0] != 'B')
                        {
                            string name = gameManager.getMap(num + i * y, alp + i * x);
                            gameManager.PrePMove(num + i * y, alp + i * x, "BBishop", num, alp);
                            king.setKMap('B');
                            gameManager.sentCheck();
                            if (!gameManager.getisBCheck())
                            {
                                gameManager.setBEMap(num + i * y, alp + i * x, true);
                            }
                            gameManager.PrePMove(num, alp, "BBishop", num + i * y, alp + i * x);
                            gameManager.setMap(num + i * y, alp + i * x, name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public override void Move(int num, int alp)
    {
        gameManager.PMove(num, alp);
        if (gameManager.getWenpL() || gameManager.getWenpR())
        {
            gameManager.setWenpL(false);
            gameManager.setWenpR(false);
        }
        if (gameManager.getBenpL() || gameManager.getBenpR())
        {
            gameManager.setBenpL(false);
            gameManager.setBenpR(false);
        }
        gameManager.sentData();
    }
}

class Knight : ChessPieces
{
    GameManager gameManager = GameManager.Instance;
    King king = new King();

    public override void ShowPoint()
    {
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                if (((Math.Abs(i) == 1 && Math.Abs(j) == 2) || (Math.Abs(i) == 2 && Math.Abs(j) == 1)))
                {
                    if ((num + i >= 0 && num + i < 8) && (alp + j >= 0 && alp + j < 8))
                    {
                        if (color == 'W')
                        {
                            if (gameManager.getMap(num + i, alp + j)[0] != 'W')
                            {
                                string name = gameManager.getMap(num + i, alp + j);
                                gameManager.PrePMove(num + i, alp + j, "WKnight", num, alp);
                                king.setKMap('W');
                                gameManager.sentCheck();
                                if (!gameManager.getisWCheck())
                                {
                                    gameManager.setWEMap(num + i, alp + j, true);
                                }
                                gameManager.PrePMove(num, alp, "WKnight", num + i, alp + j);
                                gameManager.setMap(num + i, alp + j, name);
                            }
                        }
                        else
                        {
                            if (gameManager.getMap(num + i, alp + j)[0] != 'B')
                            {
                                string name = gameManager.getMap(num + i, alp + j);
                                gameManager.PrePMove(num + i, alp + j, "BKnight", num, alp);
                                king.setKMap('B');
                                gameManager.sentCheck();
                                if (!gameManager.getisBCheck())
                                {
                                    gameManager.setBEMap(num + i, alp + j, true);
                                }
                                gameManager.PrePMove(num, alp, "BKnight", num + i, alp + j);
                                gameManager.setMap(num + i, alp + j, name);
                            }
                        }
                    }
                }
            }
        }
    }

    public override void Move(int num, int alp)
    {
        gameManager.PMove(num, alp);
        if (gameManager.getWenpL() || gameManager.getWenpR())
        {
            gameManager.setWenpL(false);
            gameManager.setWenpR(false);
        }
        if (gameManager.getBenpL() || gameManager.getBenpR())
        {
            gameManager.setBenpL(false);
            gameManager.setBenpR(false);
        }
        gameManager.sentData();
    }
}

class Pawn : ChessPieces
{
    GameManager gameManager = GameManager.Instance;
    private King king = new King();

    public override void ShowPoint()
    {
        if (color == 'W')
        {
            if (num == 3)
            {
                if (gameManager.getWenpL() && gameManager.getMap(num, alp - 1) == "BPawn")
                {
                    string name = gameManager.getMap(num - 1, alp - 1);
                    gameManager.PrePMove(num - 1, alp - 1, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp - 1, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp - 1);
                    gameManager.setMap(num - 1, alp - 1, name);
                }
                if (gameManager.getWenpR() && gameManager.getMap(num, alp + 1) == "BPawn")
                {
                    string name = gameManager.getMap(num - 1, alp + 1);
                    gameManager.PrePMove(num - 1, alp + 1, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp + 1, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp + 1);
                    gameManager.setMap(num - 1, alp + 1, name);
                }
            }
            if (alp == 0)
            {
                if (gameManager.getMap(num - 1, alp + 1)[0] == 'B')
                {
                    string name = gameManager.getMap(num - 1, alp + 1);
                    gameManager.PrePMove(num - 1, alp + 1, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp + 1, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp + 1);
                    gameManager.setMap(num - 1, alp + 1, name);
                }
            }
            else if (alp == 7)
            {
                if (gameManager.getMap(num - 1, alp - 1)[0] == 'B')
                {
                    string name = gameManager.getMap(num - 1, alp - 1);
                    gameManager.PrePMove(num - 1, alp - 1, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp - 1, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp - 1);
                    gameManager.setMap(num - 1, alp - 1, name);
                }
            }
            else
            {
                if (gameManager.getMap(num - 1, alp - 1)[0] == 'B')
                {
                    string name = gameManager.getMap(num - 1, alp - 1);
                    gameManager.PrePMove(num - 1, alp - 1, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp - 1, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp - 1);
                    gameManager.setMap(num - 1, alp - 1, name);
                }
                if (gameManager.getMap(num - 1, alp + 1)[0] == 'B')
                {
                    string name = gameManager.getMap(num - 1, alp + 1);
                    gameManager.PrePMove(num - 1, alp + 1, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp + 1, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp + 1);
                    gameManager.setMap(num - 1, alp + 1, name);
                }
            }
            if (num == 6)
            {
                for (int i = -1; i >= -2; i--)
                {
                    if (gameManager.getMap(num + i, alp) == " ")
                    {
                        string name = gameManager.getMap(num + i, alp);
                        gameManager.PrePMove(num + i, alp, "WPawn", num, alp);
                        king.setKMap('W');
                        gameManager.sentCheck();
                        if (!gameManager.getisWCheck())
                        {
                            gameManager.setWEMap(num + i, alp, true);
                        }
                        gameManager.PrePMove(num, alp, "WPawn", num + i, alp);
                        gameManager.setMap(num + i, alp, name);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (gameManager.getMap(num - 1, alp) == " ")
                {
                    string name = gameManager.getMap(num - 1, alp);
                    gameManager.PrePMove(num - 1, alp, "WPawn", num, alp);
                    king.setKMap('W');
                    gameManager.sentCheck();
                    if (!gameManager.getisWCheck())
                    {
                        gameManager.setWEMap(num - 1, alp, true);
                    }
                    gameManager.PrePMove(num, alp, "WPawn", num - 1, alp);
                    gameManager.setMap(num - 1, alp, name);
                }
            }
        }
        else
        {
            if (num == 4)
            {
                if (gameManager.getBenpL() && gameManager.getMap(num, alp + 1) == "WPawn")
                {
                    string name = gameManager.getMap(num + 1, alp + 1);
                    gameManager.PrePMove(num + 1, alp + 1, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp + 1, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp + 1);
                    gameManager.setMap(num + 1, alp + 1, name);
                }
                if (gameManager.getBenpR() && gameManager.getMap(num, alp - 1) == "WPawn")
                {
                    string name = gameManager.getMap(num + 1, alp - 1);
                    gameManager.PrePMove(num + 1, alp - 1, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp - 1, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp - 1);
                    gameManager.setMap(num + 1, alp - 1, name);
                }
            }
            if (alp == 7)
            {
                if (gameManager.getMap(num + 1, alp - 1)[0] == 'W')
                {
                    string name = gameManager.getMap(num + 1, alp - 1);
                    gameManager.PrePMove(num + 1, alp - 1, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp - 1, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp - 1);
                    gameManager.setMap(num + 1, alp - 1, name);
                }
            }
            else if (alp == 0)
            {
                if (gameManager.getMap(num + 1, alp + 1)[0] == 'W')
                {
                    string name = gameManager.getMap(num + 1, alp + 1);
                    gameManager.PrePMove(num + 1, alp + 1, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp + 1, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp + 1);
                    gameManager.setMap(num + 1, alp + 1, name);
                }
            }
            else
            {
                if (gameManager.getMap(num + 1, alp + 1)[0] == 'W')
                {
                    string name = gameManager.getMap(num + 1, alp + 1);
                    gameManager.PrePMove(num + 1, alp + 1, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp + 1, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp + 1);
                    gameManager.setMap(num + 1, alp + 1, name);
                }
                if (gameManager.getMap(num + 1, alp - 1)[0] == 'W')
                {
                    string name = gameManager.getMap(num + 1, alp - 1);
                    gameManager.PrePMove(num + 1, alp - 1, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp - 1, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp - 1);
                    gameManager.setMap(num + 1, alp - 1, name);
                }
            }
            if (num == 1)
            {
                for (int i = 1; i <= 2; i++)
                {
                    if (gameManager.getMap(num + i, alp) == " ")
                    {
                        string name = gameManager.getMap(num + i, alp);
                        gameManager.PrePMove(num + i, alp, "BPawn", num, alp);
                        king.setKMap('B');
                        gameManager.sentCheck();
                        if (!gameManager.getisBCheck())
                        {
                            gameManager.setBEMap(num + i, alp, true);
                        }
                        gameManager.PrePMove(num, alp, "BPawn", num + i, alp);
                        gameManager.setMap(num + i, alp, name);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (gameManager.getMap(num + 1, alp) == " ")
                {
                    string name = gameManager.getMap(num + 1, alp);
                    gameManager.PrePMove(num + 1, alp, "BPawn", num, alp);
                    king.setKMap('B');
                    gameManager.sentCheck();
                    if (!gameManager.getisBCheck())
                    {
                        gameManager.setBEMap(num + 1, alp, true);
                    }
                    gameManager.PrePMove(num, alp, "BPawn", num + 1, alp);
                    gameManager.setMap(num + 1, alp, name);
                }
            }
        }
    }

    public override void Move(int num, int alp)
    {
        if (color == 'W')
        {
            if (gameManager.getWenpL() && gameManager.getWenpA() == alp)
            {
                gameManager.setMap(num + 1, alp, " ");
                gameManager.setWenpL(false);
                gameManager.setWenpR(false);
            }
            if (gameManager.getWenpR() && gameManager.getWenpA() == alp)
            {
                gameManager.setMap(num + 1, alp, " ");
                gameManager.setWenpL(false);
                gameManager.setWenpR(false);
            }
            if (num == 4 && gameManager.getMap(num + 2, alp) == "WPawn")
            {
                if (alp + 1 < 8 && gameManager.getMap(num, alp + 1) == "BPawn")
                {
                    gameManager.setBenpR(true);
                    gameManager.setBenpA(alp);
                }
                if (alp - 1 >= 0 && gameManager.getMap(num, alp - 1) == "BPawn")
                {
                    gameManager.setBenpL(true);
                    gameManager.setBenpA(alp);
                }
            }
            gameManager.PMove(num, alp);
            if (gameManager.getWenpL())
            {
                gameManager.setWenpL(false);
            }
            if (gameManager.getWenpR())
            {
                gameManager.setWenpR(false);
            }
        }
        else
        {
            if (gameManager.getBenpL() && gameManager.getBenpA() == alp)
            {
                gameManager.setMap(num - 1, alp, " ");
                gameManager.setBenpL(false);
                gameManager.setBenpR(false);
            }
            if (gameManager.getBenpR() && gameManager.getBenpA() == alp)
            {
                gameManager.setMap(num - 1, alp, " ");
                gameManager.setBenpL(false);
                gameManager.setBenpR(false);
            }
            if (num == 3 && gameManager.getMap(num - 2, alp) == "BPawn")
            {
                if (alp - 1 >= 0 && gameManager.getMap(num, alp - 1) == "WPawn")
                {
                    gameManager.setWenpR(true);
                    gameManager.setWenpA(alp);
                }
                if (alp + 1 < 8 && gameManager.getMap(num, alp + 1) == "WPawn")
                {
                    gameManager.setWenpL(true);
                    gameManager.setWenpA(alp);
                }
            }
            gameManager.PMove(num, alp);
            if (gameManager.getBenpL())
            {
                gameManager.setBenpL(false);
            }
            if (gameManager.getBenpR())
            {
                gameManager.setBenpR(false);
            }
        }
        gameManager.sentData();
    }
}
