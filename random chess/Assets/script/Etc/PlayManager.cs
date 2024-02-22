using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;
    public PhotonView photonView;
    public EColor playerColor;

    public GameObject promotionPrefab;

    #region Pieces
    public King _king;
    public Queen _queen;
    public Rook _rook;
    public Bishop _bishop;
    public Knight _knight;
    public Pawn _pawn;
    #endregion

    #region Map
    public (Pieces pieces, EColor color)[,] board;

    private EColor N;
    private EColor W;
    private EColor B;
    #endregion

    #region Action
    public Action Init;
    public Action Render;
    public Action MovePointRender;
    public Action<int> promotion;
    #endregion

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        photonView = gameObject.GetComponent<PhotonView>();

        N = EColor.none;
        W = EColor.white;
        B = EColor.black;

        board = new(Pieces, EColor)[8,8] {
            { (_rook,B), (_knight,B), (_bishop,B), (_queen,B), (_king,B), (_bishop,B), (_knight,B), (_rook,B) },
            { (_pawn,B), (_pawn,B), (_pawn,B), (_pawn,B), (_pawn,B), (_pawn,B), (_pawn,B), (_pawn,B) },
            { (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N) },
            { (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N) },
            { (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N) },
            { (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N), (null, N) },
            { (_pawn,W), (_pawn,W), (_pawn,W), (_pawn,W), (_pawn,W), (_pawn,W), (_pawn,W), (_pawn,W) },
            { (_rook,W), (_knight,W), (_bishop,W), (_queen,W), (_king,W), (_bishop,W), (_knight,W), (_rook,W) }
        };

        playerColor = PhotonNetwork.IsMasterClient ? EColor.white : EColor.black;
    }

    private void Start()
    {
        Init?.Invoke();
        Render?.Invoke();
    }

    public void SetMovePointBoard(bool[,] _movePoint)
    {
        Movement.instance.movePointBoard = _movePoint;
        MovePointRender?.Invoke();
    }

    public void MoveRender(int y, int x)
    {
        board[y, x] = Movement.instance.selectedPiece;
        (int y, int x) pos = Movement.instance.selectedPos;
        board[pos.y, pos.x] = (null, EColor.none);

        #region Pawn
        if (board[y, x].pieces == _pawn)
        {
            if (Pawn.EnPassant != null && Pawn.EnPassant[playerColor == EColor.white ? y + 1 : y - 1, x]) board[playerColor == EColor.white ? y + 1 : y - 1, x] = (null, EColor.none);

            Pawn.EnPassant = new bool[8, 8];

            if (Mathf.Abs(y - pos.y) == 2) SendEnPassant(y, x);

            if (Swap(y) == 0)
            {
                promotionPrefab.SetActive(true);
                promotion?.Invoke(x);
            }
        }
        #endregion

        Render?.Invoke();
        SendBoard();
    }

    public void ChangePieces(int y, int x, string piece)
    {
        if (piece == "Queen") board[y, x] = (_queen, playerColor);
        else if (piece == "Rook") board[y, x] = (_rook, playerColor);
        else if (piece == "Bishop") board[y, x] = (_bishop, playerColor);
        else if (piece == "Knight") board[y, x] = (_knight, playerColor);

        Render?.Invoke();
        SendBoard();
    }

    private int Swap(int val)
    {
        return playerColor == EColor.white ? val : 7 - val;
    }

    #region SyncBoard
    public Pieces GetPiece(string  name)
    {
        if (name == "King (King)") return _king;
        else if (name == "Queen (Queen)") return _queen;
        else if (name == "Rook (Rook)") return _rook;
        else if (name == "Bishop (Bishop)") return _bishop;
        else if (name == "Knight (Knight)") return _knight;
        else if (name == "Pawn (Pawn)") return _pawn;
        else return null;
    }

    [PunRPC]
    public void SyncBoard(int y, int x, string piece, int color)
    {
        board[y, x] = (GetPiece(piece), (EColor)color);

        if (y == 7 && x == 7)
        {
            Render?.Invoke();
            Movement.instance.Clear();
            MovePointRender?.Invoke();
        }
    }

    public void SendBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Pieces piece = board[i, j].pieces;
                photonView.RPC("SyncBoard", RpcTarget.Others, i, j, piece == null ? "null" : piece.ToString(), (int)board[i, j].color);
            }
        }
    }
    #endregion

    #region SyncEnPassant
    [PunRPC]
    public void SyncEnPassant(int y, int x)
    {
        Pawn.EnPassant = new bool[8, 8];
        Pawn.EnPassant[y, x] = true;
    }

    public void SendEnPassant(int y, int x)
    {
        photonView.RPC("SyncEnPassant", RpcTarget.Others, y, x);
    }
    #endregion
}
