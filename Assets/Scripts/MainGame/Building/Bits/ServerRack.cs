using UnityEngine;

public class ServerRack : Building
{
    [Header("Stats")]
    public int capacity = 2000;        // how much storage this rack adds
    public int upgradeCost = 500;
    public int upgradeTime = 5;       // minutes 

    [TextArea]
    public string description = "A towering server rack that boosts your bit storage capacity. More space means more room for mining profits.";

    private CanvasGroup infoCanvas;

    private void Awake()
    {
        buildingName = "Server Rack";

        // (Optional) if you have a dedicated info panel prefab like BitMiner
        GameObject panel = GameObject.Find("ServerRackInfoPanel");
        if (panel != null)
            infoCanvas = panel.GetComponent<CanvasGroup>();
    }

    public override void OnPlaced()
    {
        if (BitManager.Instance != null)
        {
            int newMax = BitManager.Instance.GetMaxBits() + capacity;
            BitManager.Instance.SetMaxBits(newMax, keepRatio: false);
        }
    }

    public override void OpenInfoPanel()
    {
        if (infoCanvas != null)
        {
            infoCanvas.alpha = 1f;
            infoCanvas.interactable = true;
            infoCanvas.blocksRaycasts = true;

            var ui = infoCanvas.GetComponent<ServerRackInfoUI>();
            if (ui != null) ui.SetInfo(this);
        }
    }

    public void CloseInfoPanel()
    {
        if (infoCanvas != null)
        {
            infoCanvas.alpha = 0f;
            infoCanvas.interactable = false;
            infoCanvas.blocksRaycasts = false;
        }
    }

}
