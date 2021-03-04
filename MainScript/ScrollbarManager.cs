using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarManager : MonoBehaviour
{
    public bool alsoDisableScrolling;

    private float disableRange = 0.99f;
    private ScrollRect scrollRect;
    private ScrollbarClass scrollbarVertical = null;
    private float scrollBarSize;
    private class ScrollbarClass
    {
        public Scrollbar bar;
        public bool active;
    }


    void Start()
    {
        scrollRect = gameObject.GetComponent<ScrollRect>();
        if (scrollRect.verticalScrollbar != null)
            scrollbarVertical = new ScrollbarClass() { bar = scrollRect.verticalScrollbar, active = true };

        if (scrollbarVertical == null)
            Debug.LogWarning("Must have a vertical scrollbar attached to the Scroll Rect for AutoHideUIScrollbar to work");

        scrollBarSize = scrollbarVertical.bar.size;
    }

    void Update()
    {
        if (scrollbarVertical != null)
            SetScrollBar(scrollbarVertical);
        if(scrollbarVertical.bar.size != scrollBarSize)
        {
            scrollBarSize = scrollbarVertical.bar.size;
            scrollbarVertical.bar.value = 0;
        }
    }

    void SetScrollBar(ScrollbarClass scrollbar)
    {
        if (scrollbar.active && scrollbar.bar.size > disableRange)
        {
            SetBar(scrollbar, false);
        }

        else if (!scrollbar.active && scrollbar.bar.size < disableRange)
        {
            SetBar(scrollbar, true);
        }
    }

    void SetBar(ScrollbarClass scrollbar, bool active)
    {
        
        scrollbar.bar.gameObject.SetActive(active);
        scrollbar.active = active;

        if (alsoDisableScrolling)
        {
          scrollRect.vertical = active;
        }
    }

    public void setScrollBarDown()
    {
        scrollbarVertical.bar.value = 0;
    }
}
