using TMPro;
using UnityEngine;
using System.Collections;

public class CostHighlighter : MonoBehaviour
{
    [SerializeField] TMP_Text bitsLabel;
    [SerializeField] int bitsCost;

    [SerializeField] TMP_Text scrapLabel;
    [SerializeField] int scrapCost;

    [SerializeField] Color okColor = Color.white;
    [SerializeField] Color notEnoughColor = Color.red;

    Coroutine _waitRoutine;

    void OnEnable()
    {
        _waitRoutine = StartCoroutine(WaitAndSubscribe());
    }

    IEnumerator WaitAndSubscribe()
    {
        // Wait until singletons exist (covers script execution order issues)
        while (BitManager.Instance == null || ScrapManager.Instance == null) yield return null;

        BitManager.Instance.OnBitsChanged += CheckBits;
        ScrapManager.Instance.OnScrapChanged += CheckScrap;

        // Initial paint
        CheckBits(BitManager.Instance.GetCurrentBits());
        CheckScrap(ScrapManager.Instance.GetCurrentScrap());

        Debug.Log("[CostHighlighter] Subscribed & painted.");
    }

    void OnDisable()
    {
        if (_waitRoutine != null) { StopCoroutine(_waitRoutine); _waitRoutine = null; }
        if (BitManager.Instance != null) BitManager.Instance.OnBitsChanged -= CheckBits;
        if (ScrapManager.Instance != null) ScrapManager.Instance.OnScrapChanged -= CheckScrap;
    }

    void CheckBits(int currentBits)
    {
        if (bitsLabel != null)
            bitsLabel.color = currentBits >= bitsCost ? okColor : notEnoughColor;
    }

    void CheckScrap(int currentScrap)
    {
        if (scrapLabel != null)
            scrapLabel.color = currentScrap >= scrapCost ? okColor : notEnoughColor;
    }
}
