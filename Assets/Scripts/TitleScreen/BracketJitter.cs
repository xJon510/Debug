using System.Collections;
using UnityEngine;

public class BracketJitter : MonoBehaviour
{
    public float glitchIntervalMin = 2f;     // Time between glitches
    public float glitchIntervalMax = 5f;

    public float jitterAmount = 2f;          // Max pixel offset in any direction
    public int jitterCount = 3;              // How many times it jitters during a flicker
    public float jitterDuration = 0.05f;     // How long each jitter lasts

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(GlitchLoop());
    }

    private IEnumerator GlitchLoop()
    {
        while (true)
        {
            // Wait a random time between glitches
            yield return new WaitForSeconds(Random.Range(glitchIntervalMin, glitchIntervalMax));

            for (int i = 0; i < jitterCount; i++)
            {
                // Apply random offset
                Vector3 offset = new Vector3(
                    Random.Range(-jitterAmount, jitterAmount),
                    Random.Range(-jitterAmount, jitterAmount),
                    0f
                );

                transform.localPosition = originalPosition + offset;

                yield return new WaitForSeconds(jitterDuration);
            }

            // Snap back
            transform.localPosition = originalPosition;
        }
    }
}
