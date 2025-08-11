using UnityEngine;

/// <summary>
/// Bob tiles up/down with minimal per-frame work.
/// No distance culling. No visual changes. Zero GC per-frame.
/// </summary>

public class TileBobCPU : MonoBehaviour
{
    [Header("Target tiles (leave empty to use all direct children)")]
    public Transform[] tileRoots;

    [Header("Motion")]
    public float ampMin = 0.03f;
    public float ampMax = 0.10f;
    public float speedMin = 0.6f;
    public float speedMax = 1.6f;

    [Header("Variation")]
    [Tooltip("World-pos based variation added to phase. Higher == more desync. (computed once in Awake)")]
    public float noiseScale = 1.5f;        // how “tight” the Perlin is
    public float noisePhase = 1.2f;        // how much noise affects phase (radians)

    [Tooltip("If true, moves the tile root. If false, moves all children together.")]
    public bool moveParentInstead = true;

    // Cache
    Transform[] _roots;
    Vector3[] _baseRootPos;
    float[] _amp, _speed, _phase, _phaseNoise;

    // Only used if moveParentInstead == false
    Transform[][] _children;
    Vector3[][] _baseChildLocalPos;

    void Awake()
    {
        // Collect roots
        if (tileRoots == null || tileRoots.Length == 0)
        {
            int count = transform.childCount;
            _roots = new Transform[count];
            for (int i = 0; i < count; i++) _roots[i] = transform.GetChild(i);
        }
        else
        {
            _roots = tileRoots;
        }

        int n = _roots.Length;
        _baseRootPos = new Vector3[n];
        _amp = new float[n];
        _speed = new float[n];
        _phase = new float[n];
        _phaseNoise = new float[n];

        // Only allocate child caches if we need them
        if (!moveParentInstead)
        {
            _children = new Transform[n][];
            _baseChildLocalPos = new Vector3[n][];
        }

        // Deterministic-ish seed per scene object
        var rng = new System.Random(gameObject.GetInstanceID());

        for (int i = 0; i < n; i++)
        {
            var root = _roots[i];
            _baseRootPos[i] = root.position;

            // Per-tile params (randomized once)
            _amp[i] = Mathf.Lerp(ampMin, ampMax, (float)rng.NextDouble());
            _speed[i] = Mathf.Lerp(speedMin, speedMax, (float)rng.NextDouble());
            _phase[i] = (float)rng.NextDouble() * Mathf.PI * 2f;

            // PRECOMPUTE Perlin-based phase offset (no per-frame PerlinNoise)
            Vector3 wp = root.position;
            float nPerlin = Mathf.PerlinNoise(wp.x * noiseScale, wp.z * noiseScale) - 0.5f;
            _phaseNoise[i] = nPerlin * noisePhase;

            if (!moveParentInstead)
            {
                int c = root.childCount;
                var kids = new Transform[c];
                var bases = new Vector3[c];
                for (int j = 0; j < c; j++)
                {
                    var t = root.GetChild(j);
                    kids[j] = t;
                    bases[j] = t.localPosition;
                }
                _children[i] = kids;
                _baseChildLocalPos[i] = bases;
            }
        }
    }

    void Update()
    {
        // Single time sample for all tiles this frame
        float t = Time.time;

        // Tight for-loop, no allocations
        if (moveParentInstead)
        {
            for (int i = 0; i < _roots.Length; i++)
            {
                float y = Mathf.Sin(t * _speed[i] + _phase[i] + _phaseNoise[i]) * _amp[i];
                Vector3 p = _baseRootPos[i];
                p.y += y;
                _roots[i].position = p;
            }
        }
        else
        {
            for (int i = 0; i < _roots.Length; i++)
            {
                float y = Mathf.Sin(t * _speed[i] + _phase[i] + _phaseNoise[i]) * _amp[i];

                var kids = _children[i];
                var bases = _baseChildLocalPos[i];
                for (int j = 0; j < kids.Length; j++)
                {
                    Vector3 lp = bases[j];
                    lp.y += y;
                    kids[j].localPosition = lp;
                }
            }
        }
    }

    // Quick way to regenerate amplitudes/speeds/phases at runtime
    [ContextMenu("Reseed")]
    void Reseed() => Awake();
}
