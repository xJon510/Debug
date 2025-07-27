using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text damageText;
    public float lifetime = 1f;
    public float floatSpeed = 2f;
    public float fadeSpeed = 2f;

    private float timer = 0f;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(float damageAmount, Color textColor)
    {
        damageText.text = damageAmount.ToString("F0");
        damageText.color = textColor;
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
