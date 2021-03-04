using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBtn : MonoBehaviour
{
    public GameObject Panel;

    private void Start()
    {
        Panel.SetActive(false);
    }
    public void StartAction()
    {
        this.gameObject.SetActive(false);
        Panel.SetActive(true);
    }
}


