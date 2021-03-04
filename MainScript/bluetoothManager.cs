using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;

public class bluetoothManager : MonoBehaviour
{
    private BluetoothHelper helper;

    private string deviceName;

    // Start is called before the first frame update
    void Start()
    {
        deviceName = "ESP32";

        try
        {
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.Connect();
        }
        catch (BluetoothHelper.BlueToothNotEnabledException) { GetComponent<Text>().text = "not enable"; }
        catch (BluetoothHelper.BlueToothNotReadyException) { GetComponent<Text>().text = "not ready"; }
        catch (BluetoothHelper.BlueToothNotSupportedException) { GetComponent<Text>().text = "not support"; }
    }

    private void OnConnected()
    {
        helper.StartListening();
        GetComponent<Text>().text = "connected";
        helper.SendData("a");
    }

    void OnConnFailed()
    {
        GetComponent<Text>().text = "unconnected";
    }

    // Update is called once per frame
    void Update()
    {
        if (helper.isConnected())
            GetComponent<Text>().text = "connected";
        else
            GetComponent<Text>().text = "unconnected";
        if (helper.Available)
        {
            string msg = helper.Read();
        }
    }

    private void OnDestroy()
    {

    }
}
