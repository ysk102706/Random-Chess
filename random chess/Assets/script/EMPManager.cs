using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMPManager : MonoBehaviour
{
    GameManager gameManager;

    private Image EMPMark;

    private int alp;
    private int num;
    private char color;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        color = transform.parent.transform.parent.name[0];

        gameManager.WhoAmI(color, name, ref num, ref alp);

        EMPMark = GetComponent<Image>();
        EMPMark.sprite = gameManager.getImage("EMP");
        EMPMark.raycastTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        ShowEnableMap();
    }

    void ShowEnableMap()
    {
        if (color == 'W')
        {
            if (gameManager.getWEMap(num, alp))
            {
                EMPMark.color = new Color(255, 255, 255, 1);
            }
            else
            {
                EMPMark.color = new Color(255, 255, 255, 0);
            }
        }
        else
        {
            if (gameManager.getBEMap(num, alp))
            {
                EMPMark.color = new Color(255, 255, 255, 1);
            }
            else
            {
                EMPMark.color = new Color(255, 255, 255, 0);
            }
        }
    }
}
