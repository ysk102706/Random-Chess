using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Toggle modeToggle;
    public Button startButton;

    private const string VERSION = "1";
    private const string GAME_MODE = "gm";
    private string mode;

    private void Awake()
    {
        PhotonNetwork.GameVersion = VERSION;
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        else
        {
            startButton.enabled = false;
            PhotonNetwork.ConnectUsingSettings(); // 마스터 서버에 접속
        }
    }

    public override void OnConnectedToMaster()
    {
        startButton.enabled = true;
        modeToggle.isOn = true;
        mode = "N";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings(); // 서버 접속 재시도
    }

    public void Matching()
    {
        Hashtable roomProperties = new Hashtable { { GAME_MODE, mode == "N" ? 1 : 2 } };
        PhotonNetwork.JoinRandomRoom(roomProperties, 2);
    }

    // 방이 없으면 방 생성
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions { 
            MaxPlayers = 2, // 최대 인원
            CustomRoomPropertiesForLobby = new string[] { GAME_MODE }, // 방의 속성
            CustomRoomProperties = new Hashtable { { GAME_MODE, mode == "N" ? 1 : 2 } } // 속성의 값
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        GameManager.instance.mode = mode;
        StartCoroutine("StartCheck");
    }

    private IEnumerator StartCheck()
    {
        yield return null;
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) PhotonNetwork.LoadLevel("Play");
        else StartCoroutine("StartCheck");
    }

    private void Update()
    {
        mode = modeToggle.isOn ? "N" : "R";
    }
}
