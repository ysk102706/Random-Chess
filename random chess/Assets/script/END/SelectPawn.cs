using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPawn : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void WClicked() {
        gameManager.setMap(0, ListManager.WPos, "W" + this.name);
        ListManager.getList('W').SetActive(false);
    }

    public void BClicked() {
        gameManager.setMap(7, ListManager.BPos, "B" + this.name);
        ListManager.getList('B').SetActive(false);
    }
}
    