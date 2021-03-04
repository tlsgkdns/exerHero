using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;


public enum arduinoFlow
{
    Reset=4, Pause, Resume
}

public class BluetoothManager : MonoBehaviour
{
    private int exerciseLimit;
    private int count;
    private BluetoothHelper helper;
    private string deviceName;
    public string sttString;
    private bool didExerForExerManager;
    private bool didExerForGameDirector;

    GameObject canvas;
    GameObject stateObj;
    // Start is called before the first frame update
    void Start()
    {
        exerciseLimit = 0;
        count = 0;
        canvas = GameObject.Find("Canvas");
        stateObj = canvas.transform.Find("connectTxt").gameObject;
        deviceName = "ESP32_train";
        didExerForExerManager = false;
        didExerForGameDirector = false;

        connectBluetooth();

        DontDestroyOnLoad(gameObject);
    }

    private void OnConnected()
    {
        try
        {
            helper.StartListening();
        } catch (Exception)
        {
            sttString = "cannot listen";
          
        }
        sttString = "connected";
       
    }

    private void connectBluetooth()
    {
        try
        {
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.setFixedLengthBasedStream(1);
            helper.Connect();
        }
        catch (BluetoothHelper.BlueToothNotEnabledException) { sttString = "not enable"; }
        catch (BluetoothHelper.BlueToothNotReadyException) { sttString = "not ready"; }
        catch (BluetoothHelper.BlueToothNotSupportedException) { sttString = "not support"; }
        catch (Exception) { stateObj.GetComponent<Text>().text = "not connected"; }
    }
    private void OnConnectionFailed()
    {
        sttString = "none connected";
    }

   
    public void setExerCount(int c)
    {
        exerciseLimit = c;
     
        sendDataToBluetooth(exerciseLimit);
    }

    public int getExerLimitCount()
    {
        return exerciseLimit;
    }
    void Update()
    {
        connectBluetooth();

        canvas = GameObject.Find("Canvas");
        stateObj = canvas.transform.Find("connectTxt").gameObject;
        stateObj.GetComponent<Text>().text = sttString;

        if (helper == null)
            return;

        if (helper.Available)
        {
            string count = helper.Read();
            if (helper.Available)
                count += helper.Read();

            this.count = Convert.ToInt32(count);
            didExerForExerManager = true;
            didExerForGameDirector = true;
            
        }
    }

    public void sendDataToBluetooth(int sendData)
    {
        try
        {
            byte[] b = new byte[1];
            b[0] = Convert.ToByte(sendData);
            helper.SendData(b);
            sttString = exerciseLimit.ToString();
        }
        catch (Exception e)
        {
            sttString = e.Message;
            stateObj.GetComponent<Text>().text = e.Message;
        }
    }

    public bool notifyExerForEM()
    {
        if(didExerForExerManager)
        {
            didExerForExerManager = false;
           
            return true;
        }

        return false;
    }

    public bool notifyExerForGD()
    {
        if (didExerForGameDirector)
        {
            didExerForGameDirector = false;

            return true;
        }

        return false;
    }

    public int getExerCount()
    {
        return count;
    }

    public void countReset()
    {
        sendDataToBluetooth((int)arduinoFlow.Reset);
    }

    public void pauseArduino()
    {
        sendDataToBluetooth((int)arduinoFlow.Pause);
    }

    public void resumeArduino()
    {
        sendDataToBluetooth((int)arduinoFlow.Resume);
    }
}
