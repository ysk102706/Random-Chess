using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private GameObject loading;

    // Start is called before the first frame update
    void Start()
    {
        loading.SetActive(false);
    }

    public void ClickButton()
    {
        loading.SetActive(true);
    }
}
