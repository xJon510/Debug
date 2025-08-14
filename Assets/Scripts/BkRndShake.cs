using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BkRndShake : MonoBehaviour
{
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 10f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void TriggerShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            rectTransform.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }
}