using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RetryBtn : MonoBehaviour
{
    public GameObject BluetoothManager;
    public GameObject EscapeManager;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(retry);
        BluetoothManager = GameObject.Find("BluetoothManager");
        EscapeManager = GameObject.Find("Escape");
    }

    private void retry()
    {
        Destroy(BluetoothManager);
        Destroy(EscapeManager);
        SceneManager.LoadScene("StartScene");
    }
}
