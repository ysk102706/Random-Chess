using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayController : MonoBehaviour
{
    public static PlayController instance;
    public PhotonView photonView;
    public EColor playerColor;

    public GameObject promotionPrefab;

    public GameObject drawRequestWindow;
    public GameObject waitResponse;

    public GameObject resultWindow;
    public Text resultWindowText;
    public Text causeWindowText;

    public bool isCheck = false;
    public bool sendCheckData = false;

    public bool isCheckMate = false;
    public bool isStailMate = false;
    public bool isInsufficientMaterials = false;

    public bool DrawRequest = false;

    public int turnCount;
    public EColor turnColor;

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
    public Action CheckRender;
    public Action myTurn;
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
        CheckRender?.Invoke();

        turnColor = EColor.white;

        myTurn?.Invoke();

        myTurn += () =>
        {
            ConfirmMate();
        };
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

        #region Rook
        if (board[y, x].pieces == _rook)
        {
            (int y, int x) swapPos = (Swap(pos.y), Swap(pos.x));
            if (Rook.moveCheck.ContainsKey(swapPos)) Rook.moveCheck[swapPos] = false;
        }
        #endregion

        #region King
        if (board[y, x].pieces == _king)
        {
            if (!King.isMove) {
                int dir = x - pos.x;
                if (Mathf.Abs(dir) == 2)
                {
                    if (dir < 0)
                    {
                        board[y, x + 1] = (_rook, playerColor);
                        board[y, 0] = (null, EColor.none);
                    }
                    else
                    {
                        board[y, x - 1] = (_rook, playerColor);
                        board[y, 7] = (null, EColor.none);
                    }
                }
            }

            King.isMove = true;
        }
        #endregion

        #region Sync
        Render?.Invoke();
        SendBoard();

        if (isCheck) isCheck = false;

        board[y, x].pieces.SetMovePoint(playerColor, Swap(y), Swap(x));
        SendCheckData();
        CheckRender?.Invoke();

        turnCount++;
        turnColor = (EColor)((int)turnColor % 2 + 1);
        SendTurn();

        InsufficienMaterialsCheck();
        SendInsufficien();
        #endregion

        if (GameManager.instance.mode == "R" && turnCount % 10 == 0) RandomMap();
    }

    public void ChangePawn(int y, int x, string piece)
    {
        if (piece == "Queen") board[y, x] = (_queen, playerColor);
        else if (piece == "Rook") board[y, x] = (_rook, playerColor);
        else if (piece == "Bishop") board[y, x] = (_bishop, playerColor);
        else if (piece == "Knight") board[y, x] = (_knight, playerColor);

        Render?.Invoke();
        SendBoard();
    }

    public void ConfirmMate()
    {
        int sum = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j].color != playerColor) continue;
                sum += board[i, j].pieces.SetMovePoint(playerColor, Swap(i), Swap(j));
            }
        }

        if (sum == 0)
        {
            if (isCheck) isCheckMate = true;
            else isStailMate = true;
            SendMate();
        }
    }

    private int Swap(int val)
    {
        return playerColor == EColor.white ? val : 7 - val;
    }

    private void InsufficienMaterialsCheck()
    {
        int queen = 0;
        int rook = 0;
        int bishop = 0;
        int knight = 0;
        int pawn = 0;

        int bishopColor = -1;
        bool bishopCheck = false;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j].pieces == _queen) queen++;
                else if (board[i, j].pieces == _rook) rook++;
                else if (board[i, j].pieces == _bishop)
                {
                    if (bishopColor == -1) bishopColor = i % 2 == 1 ? 1 - (j % 2) : j % 2;
                    else if (bishopColor != (i % 2 == 1 ? 1 - (j % 2) : j % 2)) bishopCheck = true;

                    if (!bishopCheck) bishop++;
                    else bishop = -1;
                }
                else if (board[i, j].pieces == _knight) knight++;
                else if (board[i, j].pieces == _pawn) pawn++;
            }
        }

        if (queen == 0 && rook == 0 && bishop == 0 && knight == 0 && pawn == 0) isInsufficientMaterials = true;
        if (queen == 0 && rook == 0 && bishop > 0 && knight == 0 && pawn == 0) isInsufficientMaterials = true;
        if (queen == 0 && rook == 0 && bishop == 0 && knight > 0 && pawn == 0) isInsufficientMaterials = true;
    }

    public void PrintResult(string _result, string _cause)
    {
        resultWindow.SetActive(true);
        resultWindowText.text = _result;
        causeWindowText.text = _cause;
        StartCoroutine(MoveToTitle());
    }

    private IEnumerator MoveToTitle()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("Start");
    }

    private IEnumerator SendDrawRequestAccept()
    {
        yield return new WaitForSeconds(1.5f);
        waitResponse.SetActive(false);
        PrintResult("Draw", "DrawRequest");
    }

    private IEnumerator SendDrawRequestRefuse()
    {
        yield return new WaitForSeconds(1.5f);
        waitResponse.SetActive(false);
    }

    private void RandomMap()
    {
        int pawn = 50;
        int knight = 64;
        int bishop = 78;
        int rook = 92;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j].pieces == _king || board[i, j].pieces == null) continue;

                EColor color = board[i, j].color;
                int r = UnityEngine.Random.Range(1, 101);

                if (r <= pawn) board[i, j] = (_pawn, color);
                else if (r <= knight) board[i, j] = (_knight, color);
                else if (r <= bishop) board[i, j] = (_bishop, color);
                else if (r <= rook) board[i, j] = (_rook, color);
                else  board[i, j] = (_queen, color);
            }
        }

        Render?.Invoke();
        SendBoard();
    }

    #region Sync
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

    #region SyncCheck
    [PunRPC]
    public void SyncCheckData(bool _checkData, bool _otherCheckData)
    {
        isCheck = _checkData;
        sendCheckData = _otherCheckData;
        CheckRender?.Invoke();
    }

    public void SendCheckData()
    {
        photonView.RPC("SyncCheckData", RpcTarget.Others, sendCheckData, isCheck);
    }
    #endregion

    #region SyncTurn
    [PunRPC]
    public void SyncTurn(int _turnCount, int _color)
    {
        turnCount = _turnCount;
        turnColor = (EColor)_color;
        myTurn?.Invoke();
    }

    public void SendTurn()
    {
        photonView.RPC("SyncTurn", RpcTarget.Others, turnCount, (int)turnColor);
        myTurn?.Invoke();
    }
    #endregion

    #region SyncMate
    [PunRPC]
    public void SyncMate(bool _isCheckMate, bool _isStailMate)
    {
        isCheckMate = _isCheckMate;
        isStailMate = _isStailMate;
        if (isCheckMate) PrintResult("Win", "CheckMate");
        if (isStailMate) PrintResult("Draw", "StailMate");
    }

    public void SendMate()
    {
        photonView.RPC("SyncMate", RpcTarget.Others, isCheckMate, isStailMate);
        if (isCheckMate) PrintResult("Lose", "CheckMate");
        if (isStailMate) PrintResult("Draw", "StailMate");
    }
    #endregion

    #region SyncInsufficien
    [PunRPC]
    public void SyncInsufficien(bool _value)
    {
        if (_value) PrintResult("Draw", "InsufficienMaterials");
        isInsufficientMaterials = _value;
    }

    public void SendInsufficien()
    {
        photonView.RPC("SyncInsufficien", RpcTarget.Others, isInsufficientMaterials);
        if (isInsufficientMaterials) PrintResult("Draw", "InsufficienMaterials");
    }
    #endregion

    #region SendSurrender
    [PunRPC]
    public void SendSurrender() => PrintResult("Win", "Surrender");
    #endregion

    #region SendDrawRequest
    [PunRPC]
    public void SendDrawRequest()
    {
        drawRequestWindow.SetActive(true);
    }

    [PunRPC]
    public void SendDrawResponse(bool _val)
    {
        if (_val) {
            waitResponse.GetComponentInChildren<Text>().text = "Accept";
            StartCoroutine(SendDrawRequestAccept());
        }
        else
        {
            waitResponse.GetComponentInChildren<Text>().text = "Refuse";
            StartCoroutine(SendDrawRequestRefuse());
        }
    }
    #endregion
    #endregion
}
