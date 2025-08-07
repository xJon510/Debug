using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonJitter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float jitterAmount = 2f;
    public float jitterSpeed = 0.05f;
    public int jitterCount = 3;

    private Vector3 originalPosition;
    private bool isHovered = false;
    private Coroutine jitterRoutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        jitterRoutine = StartCoroutine(JitterLoop());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StopCoroutine(jitterRoutine);
        transform.localPosition = originalPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        jitterRoutine = StartCoroutine(JitterLoop());
    }

    private IEnumerator JitterLoop()
    {
        while (!isHovered)
        {
            for (int i = 0; i < jitterCount; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-jitterAmount, jitterAmount), 0f, 0f);
                transform.localPosition = originalPosition + offset;
                yield return new WaitForSeconds(jitterSpeed);
            }

            transform.localPosition = originalPosition;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
