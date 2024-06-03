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
            PhotonNetwork.ConnectUsingSettings(); // ������ ������ ����
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
        PhotonNetwork.ConnectUsingSettings(); // ���� ���� ��õ�
    }

    public void Matching()
    {
        Hashtable roomProperties = new Hashtable { { GAME_MODE, mode == "N" ? 1 : 2 } };
        PhotonNetwork.JoinRandomRoom(roomProperties, 2);
    }

    // ���� ������ �� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions { 
            MaxPlayers = 2, // �ִ� �ο�
            CustomRoomPropertiesForLobby = new string[] { GAME_MODE }, // ���� �Ӽ�
            CustomRoomProperties = new Hashtable { { GAME_MODE, mode == "N" ? 1 : 2 } } // �Ӽ��� ��
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        GameManager.instance.mode = mode;
        StartCoroutine(StartCheck());
    }

    private IEnumerator StartCheck()
    {
        while (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers) yield return null;
        PhotonNetwork.LoadLevel("Play");
    }

    private void Update()
    {
        mode = modeToggle.isOn ? "N" : "R";
    }
}
