using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    private static GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void DrawRequest()
    {
        if (this.name[0] == 'W')
        {
            gameManager.setisBDraw(true);
        }
        else
        {
            gameManager.setisWDraw(true);
        }
        gameManager.sentDraw();
    }

    public void Accept()
    {
        if(this.name[0] == 'W')
        {
            gameManager.setisWDraw(false);
        }
        else
        {
            gameManager.setisBDraw(false);
        }
        gameManager.setisDraw(true);
        gameManager.sentDraw();
    }
    public void Refuse()
    {
        if (this.name[0] == 'W')
        {
            gameManager.setisWDraw(false);
        }
        else
        {
            gameManager.setisBDraw(false);
        }
        gameManager.sentDraw();
    }

    public void Surrender()
    {
        gameManager.setSurrender(this.name[0], true);
        gameManager.sentSurrender();
    }
}
