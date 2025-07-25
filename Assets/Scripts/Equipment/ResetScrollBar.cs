using UnityEngine;
using UnityEngine.UI;

public class ResetScrollBar : MonoBehaviour
{
    public Scrollbar scrollBar;

    void Start()
    {
        if (scrollBar != null)
        {
            scrollBar.value = 1f; // 1 = top of scrollbar
        }
    }
}