using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun.Demo.Cockpit;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private int NormalRoomCount;
    private int RandomRoomCount;
    private int RoomNumber;

    [SerializeField]
    private Toggle modeToggle;

    private string mode;

    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        PhotonNetwork.ConnectUsingSettings();
        modeToggle.isOn = true;
        NormalRoomCount = 0;
        RandomRoomCount = 0;
        RoomNumber = 0;
    }

    public void ClickButton()
    {
        if (mode == "Normal")
        {
            RoomNumber = Random.Range(0, NormalRoomCount+1);
        }
        else
        {
            RoomNumber = Random.Range(0, RandomRoomCount + 1);
        }

        PhotonNetwork.JoinRoom(mode + RoomNumber);
    }

    //public override void OnConnectedToMaster() => 

    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    PhotonNetwork.ConnectUsingSettings();
    //}

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (mode == "Normal")
        {
            PhotonNetwork.CreateRoom("Normal" + NormalRoomCount, new RoomOptions { MaxPlayers = 2 });
            NormalRoomCount++;
        }
        else
        {
            PhotonNetwork.CreateRoom("Random" + RandomRoomCount, new RoomOptions { MaxPlayers = 2 });
            RandomRoomCount++;
        }
    }

    private void Update()
    {
        if (modeToggle.isOn)
        {
            mode = "Normal";
        }
        else
        {
            mode = "Random";
        }

        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.Players.Count == 2)
        {
            if (mode == "Normal")
            {
                SceneManager.LoadScene("NormalOnLine");
            }
            else
            {
                SceneManager.LoadScene("RandomOnLine");
            }
        }
    }

    private void Awake()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.RemovedFromList = true;
            PhotonNetwork.LeaveRoom();
        }
    }
}
