using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonsManager : MonoBehaviour
{
    private int selection;
    private const int buttonCount = 3;
    private bool isClicked;
    Button[] btnList;
    // Start is called before the first frame update
    void Start()
    {
        selection = 0;
        isClicked = false;
        btnList = new Button[buttonCount];
        for (int i = 0; i < buttonCount; i++)
        {
            btnList[i] = getButtonFromChild(i);
            btnList[i].onClick.AddListener(() => returnSelection());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Button getButtonFromChild(int num)
    {
        return this.gameObject.transform.GetChild(num).gameObject.GetComponent<Button>();
    }
    private void returnSelection()
    {
        if (isClicked)
            return;
        isClicked = true;
        try
        {
            int select = Int32.Parse(EventSystem.current.currentSelectedGameObject.name);
            selection = select;
        } catch (FormatException e)
        {
            Debug.Log(selection);
            Debug.Log(e.Message); 
        }
    }

    public int getBtnSelection()
    {
        return selection;
    }

    public void initBtnSelection()
    {
        selection = 0;
    }
    public bool isBtnClicked()
    {
        return isClicked;
    }
    public void setBtnInActive()
    {
        isClicked = false;
        this.gameObject.SetActive(false);
    }
}
