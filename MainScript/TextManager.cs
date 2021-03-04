using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    private readonly List<string> bgName = new List<string>() { "Cliff", "Cave", "Desert" };

    Touch tempTouch;
    private List<GameObject> textList;
    private List<string> listSentences = new List<string>();
    private bool texting, skip, textEnd;
    private bool isFileRead;
    private int count;
    private bool touchOn;
    private bool isBtnClicked;

    private void initVar()
    {
        textList = new List<GameObject>();
        textEnd = false;
        texting = false;
        skip = false;
        isBtnClicked = false;
        isFileRead = false;
        touchOn = false;
        count = 0;
    }

    private void Start()
    {
        initVar();
        StartCoroutine(StartDialogueCoroutine());
    }

    private void Update()
    {
        touchOn = false;
        if(Input.touchCount > 0)
        {
            tempTouch = Input.GetTouch(0);
            if (tempTouch.phase == TouchPhase.Began)
                touchOn = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) || isBtnClicked || touchOn)
        {
            isBtnClicked = false;

            if(texting)
            {
                skip = true;
            }

            else if (getSentencesCount() > count)
            {
                textEnd = false;
                StartCoroutine(StartDialogueCoroutine());
            }
        }

       if(getSentencesCount() <= count && !textEnd)
       {
            textEnd = true;
       }
    }

    IEnumerator StartDialogueCoroutine()
    {
        InstanceTxtGo();
        texting = true;
        float speed = 0.05f;

        foreach (char c in getSentenceByindex(count))
        {
            GameObject tL = textList[textList.Count - 1];
            tL.GetComponent<Text>().text += c;
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
            if (skip)
                continue;
                
            yield return new WaitForSeconds(speed);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        speed = 0.05f;
        skip = false;
        texting = false;
        count++;
    }
    void InstanceTxtGo()
    {
        GameObject txtOB = new GameObject("Txt");
        txtOB.AddComponent<Text>();
        txtOB.GetComponent<Text>().fontSize = (Screen.width / 240 * 10);
        txtOB.AddComponent<ContentSizeFitter>();
        txtOB.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        txtOB.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 80f, txtOB.GetComponent<RectTransform>().rect.height);
        txtOB.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        txtOB.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        txtOB.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);

        GameObject txtInstance = Instantiate(txtOB);
        txtInstance.transform.SetParent(transform);
        textList.Add(txtInstance);
        Destroy(txtOB);
    }

    public bool checkTextEnded()
    {
        bool ret = textEnd;
        if (ret)    textEnd = false;

        return ret;
    }

    

    public void addSentence(string sen)
    {
        textEnd = false;
        listSentences.Add(sen);
    }

    public int getSentencesCount()
    {
        return listSentences.Count;
    }

    public string getSentenceByindex(int index)
    {
        return listSentences[index];
    }

    public void notifyToReadList()
    {
        isBtnClicked = true;
    }

    public void addSentenceFromFile(string fileName, bool splitLine)
    {
        if (isFileRead)
            return;
        TextAsset asset = Resources.Load(fileName) as TextAsset;
        string str = asset.text;

        TextReader txtReader = new StringReader(str);
        string lines = string.Empty;
        string readLine;

        if (splitLine) // 텍스트를 따로 만들 것인가 아니면, 한 번에 만들것인가 여부.
            while ((readLine = txtReader.ReadLine()) != null)
            {
                addSentence(readLine);
            }
        else
        {
            while ((readLine = txtReader.ReadLine()) != null)
                lines = lines + readLine + '\n';
            addSentence(lines);
        }

        isFileRead = true;
    }

    public void resetFileReadFlag()
    {
        isFileRead = false;
    }

    public void addSentenceFromFileByStage(string fileName, Stage s)
    {
        if (isFileRead)
            return;
        TextAsset asset = Resources.Load(fileName) as TextAsset;
        string str = asset.text;

        TextReader txtReader = new StringReader(str);
        string readLine;
        string lines = string.Empty;
        while ((readLine = txtReader.ReadLine()) != null)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(readLine, bgName[(int)s]))
            {
                while (true)
                {
                    readLine =txtReader.ReadLine();
                    if (System.Text.RegularExpressions.Regex.IsMatch(readLine, "end")) break;
                    addSentence(readLine);
                }
                    break;
            }
        }

        isFileRead = true;
    }
}
