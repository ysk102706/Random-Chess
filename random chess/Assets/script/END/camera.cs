using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject White;
    [SerializeField]
    private GameObject Black;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            White.SetActive(true);
            Black.SetActive(false);
        }
        else
        {
            Black.SetActive(true);
            White.SetActive(false);
        }
    }
}
