using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BitMinerInfoUI : MonoBehaviour
{
    [Header("Title")]
    public TMP_Text titleText;
    public TMP_Text levelText;

    [Header("Info Tab")]
    public TMP_Text descriptionText;
    public TMP_Text capacityText;
    public TMP_Text productionText;
    public TMP_Text stateText;
    public Slider capacitySlider;

    [Header("Upgrade Tab")]
    public TMP_Text upgradeCostBitsText;
    public TMP_Text upgradeCostScrapText;
    public TMP_Text upgradeTimeText;
    public TMP_Text upgradeCapacityText;       
    public TMP_Text upgradeCapacityGainText;
    public TMP_Text upgradeProductionRateText;   
    public TMP_Text upgradeProductionGainText;

    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button collectButton;

    private BitMiner trackedMiner;

    private bool lastIsProcessing;
    private TMP_EllipsisAnimator ellipsis;
    private Coroutine upgradeCountdownCo;

    [SerializeField] private CostHighlighter costHL;

    public void SetInfo(BitMiner miner)
    {
        trackedMiner = miner;

        if (costHL == null) costHL = GetComponent<CostHighlighter>();

        if (titleText) titleText.text = miner.buildingName;
        if (levelText) levelText.text = $"Lvl {miner.currentLevel}";
        if (descriptionText) descriptionText.text = miner.description;

        // Capacity / current stored
        if (capacitySlider != null)
        {
            capacitySlider.maxValue = miner.CurrentCapacity;
            capacitySlider.value = miner.currentStored;
        }

        // Production
        if (productionText)
        {
            productionText.text =
                $"Production: <color=#00FFFF>+{miner.CurrentProductionPerHour} Bits / Hr</color>";
        }

        // Upgrade costs (split)
        if (miner.IsMaxLevel)
        {
            if (costHL) costHL.Suspend(true);

            if (upgradeCostBitsText) upgradeCostBitsText.text = "MAX";
            if (upgradeCostScrapText) upgradeCostScrapText.text = "MAX";
            if (upgradeTimeText) upgradeTimeText.text = "Upgrade Time: <color=#00FFFF>—</color>";

            if (upgradeCapacityText) upgradeCapacityText.text = $"Capacity: <color=#00FFFF>{miner.CurrentCapacity}</color>";
            if (upgradeCapacityGainText) upgradeCapacityGainText.text = "";

            if (upgradeProductionRateText)
                upgradeProductionRateText.text = $"Production: <color=#00FFFF>{miner.CurrentProductionPerHour} Bits / Hr</color>";
            if (upgradeProductionGainText) upgradeProductionGainText.text = "";
        }
        else
        {
            if (costHL)
            {
                costHL.Suspend(false);
                costHL.SetCosts(miner.NextUpgradeCostBits, miner.NextUpgradeCostScrap);
            }

            // Bits
            if (upgradeCostBitsText)
                upgradeCostBitsText.text = $"{miner.NextUpgradeCostBits}";

            // Scrap (hide if 0)
            if (upgradeCostScrapText)
            {
                int scrap = miner.NextUpgradeCostScrap;
                if (scrap > 0)
                {
                    upgradeCostScrapText.gameObject.SetActive(true);
                    upgradeCostScrapText.text = $"{scrap}";
                }
                else
                {
                    // Either hide or show a dash — pick your preference:
                    upgradeCostScrapText.gameObject.SetActive(false);
                    // upgradeCostScrapText.text = "<color=#00FFFF>—</color>";
                }
            }

            // Time
            if (upgradeTimeText)
            {
                string pretty = FormatMMSS(miner.NextUpgradeTimeSeconds);
                upgradeTimeText.text = $"Upgrade Time: <color=#00FFFF>{pretty}</color>";
            }

            // Capacity current -> next
            if (upgradeCapacityText)
            {
                int currentCap = miner.CurrentCapacity;
                int nextCap = miner.NextStats.capacity;
                upgradeCapacityText.text =
                    $"Capacity: <color=#00FFFF>{currentCap} -> {nextCap}</color>";
            }

            // Capacity gain
            if (upgradeCapacityGainText)
            {
                int currentCap = miner.CurrentCapacity;
                int nextCap = miner.NextStats.capacity;
                int gain = nextCap - currentCap;
                upgradeCapacityGainText.text = $"+{gain}";
            }

            // Production current -> next
            if (upgradeProductionRateText)
            {
                int currentProd = miner.CurrentProductionPerHour;
                int nextProd = miner.NextStats.productionPerHour;
                upgradeProductionRateText.text = $"Production: <color=#00FFFF>{currentProd} -> {nextProd} Bits/Hr</color>";
            }

            // Production gain
            if (upgradeProductionGainText)
            {
                int currentProd = miner.CurrentProductionPerHour;
                int nextProd = miner.NextStats.productionPerHour;
                int gain = nextProd - currentProd;
                upgradeProductionGainText.text = $"+{gain}";
            }
        }

        ellipsis = stateText ? stateText.GetComponent<TMP_EllipsisAnimator>() : null;

        // Kick countdown if upgrading; otherwise show planned time once
        if (trackedMiner.isUpgrading)
        {
            if (costHL) costHL.Suspend(true);
            upgradeButton.interactable = false;
            collectButton.interactable = false;
            StartUpgradeCountdownUI();
        }
        else
        {
            StopUpgradeCountdownUI();

            upgradeButton.interactable = true;
            collectButton.interactable = true;

            // Not upgrading -> show planned time (NextUpgradeTimeSeconds) once
            if (upgradeTimeText)
            {
                int secs = Mathf.Max(0, trackedMiner.NextUpgradeTimeSeconds);
                upgradeTimeText.text = $"Upgrade Time: <color=#00FFFF>{FormatMMSS(secs)}</color>";
            }

            // Force overwrite of any leftover "Upgrading" text
            lastIsProcessing = !(
                trackedMiner.currentStored < trackedMiner.CurrentCapacity
            ); // flip so UpdateStateAndCapacityUI rewrites
        }

        // Initial state label
        UpdateStateAndCapacityUI();
    }

    private void Update()
    {
        if (trackedMiner == null) return;

        // Dynamic UI (stored amount, slider, FULL vs Processing)
        UpdateStateAndCapacityUI();

        // If you ever change level at runtime, you may also want to
        // refresh static fields (capacity slider max, production line, etc.)
        // by calling SetInfo(trackedMiner) after the upgrade completes.
    }

    private void UpdateStateAndCapacityUI()
    {
        if (trackedMiner == null) return;

        int stored = Mathf.FloorToInt(trackedMiner.currentStored);
        int cap = trackedMiner.CurrentCapacity;

        if (capacityText) capacityText.text = $"{stored}/{cap}";

        if (capacitySlider != null)
        {
            capacitySlider.maxValue = cap;
            capacitySlider.value = trackedMiner.currentStored;
        }

        bool isProcessing = trackedMiner.currentStored < trackedMiner.CurrentCapacity;

        if (isProcessing != lastIsProcessing)
        {
            lastIsProcessing = isProcessing;

            if (isProcessing)
            {
                // Write the base once (no dots); animator will animate it
                if (stateText) stateText.SetText("State: <color=#00FFFF>Processing</color>");
                if (ellipsis) ellipsis.SetProcessing(true);
            }
            else
            {
                if (ellipsis) ellipsis.SetProcessing(false);
                if (stateText) stateText.SetText("State: <color=#FF0000>STORAGE FULL</color>");
            }
        }
    }

    public void CollectButtonPressed()
    {
        if (trackedMiner != null)
        {
            trackedMiner.Collect();
            // Immediately reflect the new stored amount in UI
            UpdateStateAndCapacityUI();
        }
    }

    private void ShakeTMP(TMP_Text t, float magnitude = 6f, float duration = 0.18f)
    {
        if (!t) return;
        var sh = t.GetComponent<BkRndShake>();
        if (!sh) sh = t.gameObject.AddComponent<BkRndShake>();

        // Optional: tweak per-use
        sh.shakeMagnitude = magnitude;
        sh.shakeDuration = duration;
        sh.TriggerShake();
    }

    public void OnUpgradeButtonClicked()
    {
        if (trackedMiner == null || !trackedMiner.CanUpgrade()) return;

        int needBits = trackedMiner.NextUpgradeCostBits;
        int needScrap = trackedMiner.NextUpgradeCostScrap;

        int haveBits = GetBits();
        int haveScrap = GetScrap();

        bool okBits = haveBits >= needBits;
        bool okScrap = haveScrap >= needScrap;

        if (!okBits && needBits > 0) ShakeTMP(upgradeCostBitsText, 6f, 0.18f);
        if (!okScrap && needScrap > 0) ShakeTMP(upgradeCostScrapText, 6f, 0.18f);
        if (!okBits || !okScrap) return;

        // (Optional) collect before upgrade if you want, otherwise delete this line
        // trackedMiner.Collect();

        SpendBits(needBits);
        if (needScrap > 0) SpendScrap(needScrap);

        trackedMiner.BeginUpgradeTimer();

        // Immediately switch UI into “Upgrading” mode with countdown & dashes
        StartUpgradeCountdownUI();
    }

    private int GetBits() => BitManager.Instance ? BitManager.Instance.GetCurrentBits() : 0;
    private int GetScrap() => ScrapManager.Instance ? ScrapManager.Instance.GetCurrentScrap() : 0;

    private void SpendBits(int amount) { if (BitManager.Instance) BitManager.Instance.SpendBits(amount); }
    private void SpendScrap(int amount) { if (ScrapManager.Instance) ScrapManager.Instance.SpendScrap(amount); }

    private void StartUpgradeCountdownUI()
    {
        if (upgradeCountdownCo != null) StopCoroutine(upgradeCountdownCo);
        upgradeCountdownCo = StartCoroutine(UpgradeCountdownRoutine());
    }

    private void StopUpgradeCountdownUI()
    {
        if (upgradeCountdownCo != null)
        {
            StopCoroutine(upgradeCountdownCo);
            upgradeCountdownCo = null;
        }
    }

    private System.Collections.IEnumerator UpgradeCountdownRoutine()
    {
        // capture which miner we started for
        var minerRef = trackedMiner;

        var wait = new WaitForSeconds(0.1f);
        while (minerRef != null
               && trackedMiner == minerRef              // still the same miner bound to this UI
               && minerRef.isUpgrading)                 // and it’s still upgrading
        {
            // Status: Upgrading (no ellipsis during upgrade)
            if (ellipsis) ellipsis.SetProcessing(false);
            if (stateText) stateText.SetText("State: <color=#FFC107>Upgrading</color>");

            // Costs: show plain dashes while upgrading (no color touches)
            if (upgradeCostBitsText) upgradeCostBitsText.text = "-";
            if (upgradeCostScrapText) upgradeCostScrapText.text = "-";

            // Countdown
            if (upgradeTimeText)
            {
                float rem = Mathf.Max(0f, minerRef.upgradeTimeRemaining);
                int secs = Mathf.CeilToInt(rem);
                upgradeTimeText.text = $"Upgrade Time: <color=#00FFFF>{FormatMMSS(secs)}</color>";
            }

            yield return wait;
        }

        // Finished or switched: refresh to whatever the current miner state is
        if (trackedMiner != null)
            SetInfo(trackedMiner);

        upgradeCountdownCo = null;
    }

    // mm:ss for any seconds value (00:00 when secs <= 0)
    private string FormatMMSS(int secs)
    {
        if (secs <= 0) return "00:00";
        int m = secs / 60;
        int s = secs % 60;
        return $"{m:D2}:{s:D2}";
    }
}
