using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Promotion : MonoBehaviour
{
    public EColor playerColor;

    public GameObject queen;
    public GameObject rook;
    public GameObject bishop;
    public GameObject knight;

    public Queen _queen;
    public Rook _rook;
    public Bishop _bishop;
    public Knight _knight;
    
    private int x_pos;

    private void Start()
    {
        playerColor = PlayManager.instance.playerColor;

        queen.GetComponent<Image>().sprite = _queen.GetImage(playerColor);
        rook.GetComponent<Image>().sprite = _rook.GetImage(playerColor);
        bishop.GetComponent<Image>().sprite = _bishop.GetImage(playerColor);
        knight.GetComponent<Image>().sprite = _knight.GetImage(playerColor);

        PlayManager.instance.promotion += (x) =>
        {
            x_pos = x;
            SetPos(Swap(x)); 
        };

        queen.GetComponent<Button>().onClick.AddListener(() => { ChangePieces("Queen"); });
        rook.GetComponent<Button>().onClick.AddListener(() => { ChangePieces("Rook"); });
        bishop.GetComponent<Button>().onClick.AddListener(() => { ChangePieces("Bishop"); });
        knight.GetComponent<Button>().onClick.AddListener(() => { ChangePieces("Knight"); });

        gameObject.SetActive(false);
    }

    public void ChangePieces(string PromotionTarget)
    {
        PlayManager.instance.ChangePawn(Swap(0), x_pos, PromotionTarget);
        gameObject.SetActive(false);
    }

    public void SetPos(int x)
    {
        GetComponent<RectTransform>().anchorMin = new Vector2(x * 0.125f, 0.5f);
        GetComponent<RectTransform>().anchorMax = new Vector2((x + 1) * 0.125f, 1);
    }

    private int Swap(int val)
    {
        return playerColor == EColor.white ? val : 7 - val;
    }
}
