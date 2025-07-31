using UnityEngine;

public class BitMiner : Building
{
    [Header("Stats")]
    public int capacity = 1000;
    public int productionPerHour = 100;
    public int upgradeCost = 200;
    public int upgradeTime = 5;
    [TextArea]
    public string description =
        "An automated process that digs into the digital substrate, extracting stray \"bits\" from memory leaks. Essential for fueling your defenses against the Bugs.";

    private CanvasGroup infoCanvas;

    private void Awake()
    {
        buildingName = "Bit Miner";

        GameObject panel = GameObject.Find("BitMinerInfoPanel");
        if (panel != null)
            infoCanvas = panel.GetComponent<CanvasGroup>();
    }

    public override void OpenInfoPanel()
    {
        if (infoCanvas != null)
        {
            // Enable visibility + interactivity
            infoCanvas.alpha = 1f;
            infoCanvas.interactable = true;
            infoCanvas.blocksRaycasts = true;

            BitMinerInfoUI ui = infoCanvas.GetComponent<BitMinerInfoUI>();
            if (ui != null)
                ui.SetInfo(this);
        }
    }

    public void CloseInfoPanel()
    {
        if (infoCanvas != null)
        {
            // Hide but keep object active
            infoCanvas.alpha = 0f;
            infoCanvas.interactable = false;
            infoCanvas.blocksRaycasts = false;
        }
    }
}
