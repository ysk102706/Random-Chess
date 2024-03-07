using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRenderer : MonoBehaviour
{
    public Image playerColor;
    public Image otherPlayerColor;

    public Image turnColor;

    public GameObject isPlayerCheck;
    public GameObject isOtherPlayerCheck;

    private void Awake()
    {
        PlayController.instance.CheckRender += () =>
        {
            isPlayerCheck.SetActive(PlayController.instance.isCheck);
            isOtherPlayerCheck.SetActive(PlayController.instance.sendCheckData);
        };

        PlayController.instance.myTurn += () =>
        {
            turnColor.color = PlayController.instance.turnColor == EColor.white ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);
        };
    }

    private void Start()
    {
        if (PlayController.instance.playerColor == EColor.white)
        {
            otherPlayerColor.color = new Color(64 / 255f, 64 / 255f, 64 / 255f, 1);
            playerColor.color = new Color(192 / 255f, 192 / 255f, 192 / 255f, 1);
        } 
        else
        {
            playerColor.color = new Color(64 / 255f, 64 / 255f, 64 / 255f, 1);
            otherPlayerColor.color = new Color(192 / 255f, 192 / 255f, 192 / 255f, 1);
        }
    }
}
