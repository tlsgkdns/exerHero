using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage
{
    Cliff, Cave, Desert
}

public class StageManager : MonoBehaviour
{
    Stage stage;
    // Start is called before the first frame update
    void Start()
    {
        stage = Stage.Cliff;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Stage returnCurStage()
    {
        return stage;
    }

    public void goToNextStage()
    {
        stage++;
    }

    public bool isLastStage()
    {
        if (stage == Stage.Desert)
            return true;
        else
            return false;
    }
}
