using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Exercise
{
    PushUp, Squat, LegRaise
}

public class ExerManager : MonoBehaviour
{
    const string CountTxtPrefix = "남은 횟수:";
    GameObject BluetoothManager;
    
    private Pair<int, int> exerCount;

    private Pair<int, int> setCount;
    private bool exerciseSetDone;
    private bool exerciseDone;
    private Exercise curExercise;

    private void Awake()
    {
        curExercise = Exercise.PushUp;
    }
    private void OnEnable()
    {
        exerciseSetDone = false;
        exerciseDone = false;
        BluetoothManager = GameObject.Find("BluetoothManager");
        BluetoothManager.GetComponent<BluetoothManager>().sendDataToBluetooth((int)curExercise);
        exerCount = new Pair<int, int>(BluetoothManager.GetComponent<BluetoothManager>().getExerLimitCount(),
                                        BluetoothManager.GetComponent<BluetoothManager>().getExerLimitCount());
    }

    private void Start()
    {
        setCount = new Pair<int, int>(2, 2);
        setExerInfo(exerCount.Full);
        
    }
    // Update is called once per frame
    void Update()
    {
        if(BluetoothManager.GetComponent<BluetoothManager>().notifyExerForEM())
        {
            BluetoothManager.GetComponent<BluetoothManager>().sttString = exerCount.Remain.ToString();
            exerCount.Remain = exerCount.Full - BluetoothManager.GetComponent<BluetoothManager>().getExerCount();

            if (exerCount.Remain <= 0)
            {
                setIsOver();
                if (setCount.Remain <= 0)
                {
                    resetSetCount();
                    exerciseDone = true;
                }
                else
                {
                    BluetoothManager.GetComponent<BluetoothManager>().sendDataToBluetooth((int)curExercise);
                }
            }
            setExerInfo(exerCount.Remain);
        }
    }
    private void setExerInfo(int c)
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = turnExerciseIntoString(curExercise);
        this.gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = CountTxtPrefix + c;
    }

    private string turnExerciseIntoString(Exercise exer)
    {
        switch(exer)
        {
            case Exercise.PushUp:
                return "PushUp";
            case Exercise.Squat:
                return "Squat";
            case Exercise.LegRaise:
                return "LegRaise";
        }

        return "None";
    }
    private void setIsOver()
    {
        exerCount.Remain = exerCount.Full;
        setCount.Remain--;
        setNextExercise();
        exerciseSetDone = true;
    }

    private void resetSetCount()
    {
        setCount.Remain = setCount.Full;
    }
    private void setNextExercise()
    {
        if (curExercise == Exercise.LegRaise)
            curExercise = Exercise.PushUp;
        else
            curExercise++;
    }

    public bool isSetOver()
    {
        bool ret = exerciseSetDone;
        exerciseSetDone = false;
        return ret;
    }

    public bool isExerciseDone()
    {
        bool ret = exerciseDone;
        exerciseDone = false;
        return ret;
    }
    public bool notifyExerForGameDirector()
    {
        return BluetoothManager.GetComponent<BluetoothManager>().notifyExerForGD();
    }

    public void resetExer()
    {
        BluetoothManager.GetComponent<BluetoothManager>().countReset();
    }
}
