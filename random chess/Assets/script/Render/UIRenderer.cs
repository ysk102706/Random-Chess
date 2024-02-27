using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRenderer : MonoBehaviour
{
    public GameObject isPlayerCheck;
    public GameObject isOtherPlayerCheck;

    private void Awake()
    {
        PlayManager.instance.CheckRender += () =>
        {
            isPlayerCheck.SetActive(PlayManager.instance.isCheck);
            isOtherPlayerCheck.SetActive(PlayManager.instance.sendCheckData);
        };
    }
}
