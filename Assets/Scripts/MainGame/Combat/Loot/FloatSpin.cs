using UnityEngine;

public class FloatSpin : MonoBehaviour
{
    [Header("Hover")]
    public float amplitude = 0.15f;
    public float frequency = 1.2f;

    [Header("Spin")]
    public Vector3 spinDegPerSec = new Vector3(0, 45f, 0);

    float _phase;           // random offset so they don't sync
    float _baseY;           // starting height
    Transform _t;

    void OnEnable()
    {
        _t = transform;
        _baseY = _t.localPosition.y;
        _phase = Random.value * 10f; // de-sync
    }

    void Update()
    {
        // Spin (cheap)
        _t.Rotate(spinDegPerSec * Time.deltaTime, Space.Self);

        // Hover (cheap sine)
        var p = _t.localPosition;
        p.y = _baseY + Mathf.Sin((Time.time + _phase) * frequency) * amplitude;
        _t.localPosition = p;
    }
}
