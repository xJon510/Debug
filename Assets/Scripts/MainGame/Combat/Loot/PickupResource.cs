using UnityEngine;
using System.Collections;

public class PickupResource : MonoBehaviour
{
    public int resourceAmount = 1;

    [Header("Pickup Animation")]
    public float raiseHeight = 0.6f;
    public float raiseDuration = 0.15f;
    public float flySpeed = 10;

    // Add/replace these fields
    [Header("Fly-in (Homing)")]
    public float flyLerpStrength = 15f;   // higher = snappier tracking
    public float flyMaxDuration = 0.75f;  // safety cap
    public float collectDistance = 0.15f; // slightly larger to avoid jitter
    public float shrinkDuration = 0.3f;

    private Transform target;
    private Vector3 startScale;
    private bool isCollecting;

    void Awake()
    {
        startScale = transform.localScale;
    }

    public void BeginCollect(Transform targetTransform)
    {
        if (isCollecting) return;
        isCollecting = true;
        target = targetTransform;

        // Disable idle float/spin while animating
        var floatSpin = GetComponent<FloatSpin>();
        if (floatSpin) floatSpin.enabled = false;

        StartCoroutine(AnimateAndCollect());
    }

    private IEnumerator AnimateAndCollect()
    {
        // Step 1: Raise up briefly
        Vector3 startPos = transform.position;
        Vector3 raisedPos = startPos + Vector3.up * raiseHeight;
        float t = 0f;
        while (t < raiseDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, raisedPos, t / raiseDuration);
            yield return null;
        }

        // Step 2: Fly toward target while shrinking (follows moving target)
        float shrinkTime = 0f;
        float elapsed = 0f;

        while (elapsed < flyMaxDuration)
        {
            elapsed += Time.deltaTime;

            // If target disappeared, bail out cleanly
            Vector3 targetPos = target ? target.position : transform.position;

            // Exponential chase toward CURRENT target position
            float k = 1f - Mathf.Exp(-flyLerpStrength * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, k);

            // Shrink over time
            shrinkTime += Time.deltaTime;
            float shrinkLerp = Mathf.Clamp01(shrinkTime / shrinkDuration);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, shrinkLerp);

            // Close enough?
            if ((transform.position - targetPos).sqrMagnitude <= collectDistance * collectDistance)
                break;

            yield return null;
        }

        // Step 3: Actually collect
        Collect();
    }

    public void Collect()
    {
        // Add to BitManager
        if (BitManager.Instance != null)
        {
            BitManager.Instance.AddBits(resourceAmount);
        }

        // Destroy or disable the pickup
        Destroy(gameObject); // Pool later
    }
}
