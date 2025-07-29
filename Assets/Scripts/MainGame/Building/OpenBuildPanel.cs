using UnityEngine;

public class OpenBuildPanel : MonoBehaviour
{
    [Header("References")]
    public RectTransform buildPanelTransform;  // assign your Build Panel (RectTransform)
    public GameObject buildPanelObject;    // optional: assign panel GameObject

    [Header("Target Positions")]
    public float openX = 350f;
    public float closedX = -600f;

    // Call this from a button OnClick()
    public void OpenBuildPanelUI()
    {
        if (buildPanelTransform != null)
        {
            Vector2 pos = buildPanelTransform.anchoredPosition;
            pos.x = openX;
            buildPanelTransform.anchoredPosition = pos;
        }
    }

    // Call this from another button OnClick()
    public void CloseBuildPanelUI()
    {
        if (buildPanelTransform != null)
        {
            Vector2 pos = buildPanelTransform.anchoredPosition;
            pos.x = closedX;
            buildPanelTransform.anchoredPosition = pos;
        }
    }
}
