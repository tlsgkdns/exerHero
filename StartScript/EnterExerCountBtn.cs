using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnterExerCountBtn : MonoBehaviour
{
    public GameObject ExerCount;
    public GameObject WarningPanel;
    public GameObject cc;

    private string notEnoughMessage;
    private string formatErrorMessage;
    private string overCountMessage;

    private int minCount;
    private int maxCount;

    private void Start()
    {
        notEnoughMessage = "운동횟수가 부족합니다";
        formatErrorMessage = "숫자를 입력하세요";
        overCountMessage = "운동횟수가 너무 많습니다.";
        minCount = 9;
        maxCount = 100;
        WarningPanel.SetActive(false);   
    }
    public void getExerCount()
    {
        string exerCountTxt = ExerCount.GetComponent<Text>().text;

        int exerCount = 0;

        try
        {
            exerCount = Int32.Parse(exerCountTxt);
        } catch (FormatException)
        {
            setErrorMessage(formatErrorMessage);
            setWarningPanelActive();
            return;
        }

        if (checkExerCount(exerCount))
        {
            cc.GetComponent<BluetoothManager>().setExerCount(exerCount);
            SceneManager.LoadScene("MainScene");
        }
    }

    private void setWarningPanelActive()
    {
        WarningPanel.SetActive(true);
        ExerCount.GetComponent<Text>().text = "";
    }

    private bool checkExerCount(int count)
    {
        if (count < minCount)
        {
            setErrorMessage(notEnoughMessage);
            setWarningPanelActive();
            return false;
        }
        if (count > maxCount)
        {
            setErrorMessage(overCountMessage);
            setWarningPanelActive();
            return false;
        }

        return true;
            
    }

    private void setErrorMessage(string msg)
    {
        WarningPanel.gameObject.transform.GetChild(0).GetComponent<Text>().text = msg;
    }
}
