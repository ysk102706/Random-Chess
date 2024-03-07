using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour
{
    public GameObject SurrenderBox;

    public void DrawRequest()
    {
        PlayController.instance.photonView.RPC("SendDrawRequest", Photon.Pun.RpcTarget.Others);
        PlayController.instance.waitResponse.SetActive(true);
        PlayController.instance.waitResponse.GetComponentInChildren<Text>().text = "wait response...";
    }

    public void DrawAccept()
    {
        PlayController.instance.photonView.RPC("SendDrawResponse", Photon.Pun.RpcTarget.Others, true);
        PlayController.instance.drawRequestWindow.SetActive(false);
        PlayController.instance.PrintResult("Draw", "DrawRequest");
    }

    public void DrawCancle()
    {
        PlayController.instance.photonView.RPC("SendDrawResponse", Photon.Pun.RpcTarget.Others, false);
        PlayController.instance.drawRequestWindow.SetActive(false);
    }

    public void Surrender()
    {
        SurrenderBox.SetActive(true);
    }

    public void SurrenderAccept()
    {
        PlayController.instance.PrintResult("Lose", "Surrender");
        PlayController.instance.photonView.RPC("SendSurrender", Photon.Pun.RpcTarget.Others);
        SurrenderBox.SetActive(false);
    }

    public void SurrenderCancle()
    {
        SurrenderBox.SetActive(false);
    }
}
