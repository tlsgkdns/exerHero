using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeScript : MonoBehaviour
{
    public GameObject BluetoothManager;
    private bool bPaused;

    // Start is called before the first frame update
    void Start()
    {
        bPaused = false;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BluetoothManager.GetComponent<BluetoothManager>().countReset();
            Application.Quit();
        }
    }

    private void OnApplicationQuit()
    {
        BluetoothManager.GetComponent<BluetoothManager>().countReset();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            bPaused = true;
            BluetoothManager.GetComponent<BluetoothManager>().pauseArduino();
        }     
        else if (bPaused)
        {
            bPaused = false;
            BluetoothManager.GetComponent<BluetoothManager>().resumeArduino();
        }
            
    }
}
