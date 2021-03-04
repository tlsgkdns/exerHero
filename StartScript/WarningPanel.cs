using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    float timer;
    int waitingTime;

    private void OnEnable()
    {
        timer = 0.0f;
        waitingTime = 3;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > waitingTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
