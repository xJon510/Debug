using UnityEngine;
using UnityEngine.UI;

public class CloseUIButton : MonoBehaviour
{
    [Header("Assign the CanvasGroup of the panel to close")]
    public CanvasGroup panelToClose;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(ClosePanel);
        else
            Debug.LogWarning($"{name} has CloseUIButton but no Button component!");
    }

    private void ClosePanel()
    {
        if (panelToClose != null)
        {
            panelToClose.alpha = 0f;
            panelToClose.interactable = false;
            panelToClose.blocksRaycasts = false;
        }
        else
        {
            Debug.LogWarning($"{name} CloseUIButton has no panel assigned!");
        }
    }
}
