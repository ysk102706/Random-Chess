using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListManager : MonoBehaviourPunCallbacks
{
    GameManager gameManager;

    [SerializeField]
    private GameObject BListObj;
    [SerializeField]
    private GameObject WListObj;

    private static GameObject WListC;
    private static GameObject BListC;

    private RectTransform BListPos;
    private RectTransform WListPos;

    public static int BPos;
    public static int WPos;

    private void Start()
    {
        gameManager = GameManager.Instance;

        WListC = WListObj;
        BListC = BListObj;

        BPos = -1;
        WPos = -1;
        BListObj.SetActive(false);
        WListObj.SetActive(false);
        BListPos = BListObj.GetComponent<RectTransform>();
        WListPos = WListObj.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 8; i++) {
            if (gameManager.getMap(0, i) == "WPawn") {
                WListPos.anchorMin = new Vector2(0.125f * i, -3);
                WListPos.anchorMax = new Vector2(0.125f * (i + 1), 1);
                WListObj.SetActive(true);
                WPos = i;
            }
        }
        for (int i = 0; i < 8; i++)
        {
            if (gameManager.getMap(7, i) == "BPawn")
            {
                BListPos.anchorMin = new Vector2(1 - (0.125f * (i + 1)), -3);
                BListPos.anchorMax = new Vector2(1 - (0.125f * i), 1);
                BListObj.SetActive(true);
                BPos = i;
            }
        }
    }

    public static GameObject getList(char color)
    {
        if (color == 'W') 
        {
            return WListC;
        }
        else
        {
            return BListC;
        }
    }
}
