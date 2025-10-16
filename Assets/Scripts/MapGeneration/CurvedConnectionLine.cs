using UnityEngine;


[ExecuteAlways]
public class CurvedConnectionLine : MonoBehaviour
{
    [System.Serializable]
    public struct Config
    {
        public Transform start;
        public Transform end;
        public float yOffset;
        public float width;
        public int segments;
        public float curvature;
        public Material material;
        public float pulseSpeed;
        public float pulseWidthAmp;
        public bool pulseOnlyWhenReachable;
    }

    public Transform start;
    public Transform end;
    public float yOffset = 0.1f;
    public float width = 0.1f;
    public int segments = 24;
    [Range(0f, 1f)] public float curvature = 0.35f;
    public Material material;

    public float pulseSpeed = 2.5f;
    [Range(0f, 0.5f)] public float pulseWidthAmplitude = 0.02f;
    public bool pulseOnlyWhenReachable = true;

    private LineRenderer lr;
    private Vector3 lastStartPos, lastEndPos;
    private float lastYOffset;
    private int _lastSegments;
    private float _lastWidth;

    public void Initialize(Config cfg)
    {
        start = cfg.start;
        end = cfg.end;
        yOffset = cfg.yOffset;
        width = cfg.width;
        segments = Mathf.Max(2, cfg.segments);
        curvature = Mathf.Clamp01(cfg.curvature);
        material = cfg.material;
        pulseSpeed = cfg.pulseSpeed;
        pulseWidthAmplitude = cfg.pulseWidthAmp;
        pulseOnlyWhenReachable = cfg.pulseOnlyWhenReachable;

        Setup();
        Rebuild();
    }

    void OnEnable()
    {
        Setup();
        if (start != null && end != null)
        {
            Rebuild();
        }
    }

    void Setup()
    {
        if (lr == null)
        {
            lr = gameObject.GetComponent<LineRenderer>();
            if (lr == null) lr = gameObject.AddComponent<LineRenderer>();
        }
        lr.useWorldSpace = true;
        lr.alignment = LineAlignment.View;
        lr.numCapVertices = 4;
        lr.numCornerVertices = 2;
        if (lr.sharedMaterial != material) lr.sharedMaterial = material;
        lr.textureMode = LineTextureMode.Stretch;
        lr.sortingOrder = -1;
        lr.receiveShadows = false;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    void Update()
    {
        if (start == null || end == null)
        {
            if (!Application.isPlaying) return;
            Destroy(gameObject);
            return;
        }

        bool reachable = (NodeClickHandler.currentPlayerNode != null 
                          && (NodeClickHandler.currentPlayerNode == start 
                              || NodeClickHandler.currentPlayerNode == end));

    float effWidthAmp = (pulseOnlyWhenReachable && !reachable) ? 0f : pulseWidthAmplitude;
    if (Application.isPlaying && effWidthAmp > 0f)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float baseW = Mathf.Max(0.001f, width);
            float wPulse = baseW + effWidthAmp * (0.5f - Mathf.Abs(t - 0.5f)) * 2f;
            lr.startWidth = wPulse;
            lr.endWidth = wPulse;
        }
    else { lr.startWidth = width; lr.endWidth = width; }

        if (PositionsChanged())
        {
            Rebuild();
        }
    }

    bool PositionsChanged()
    {
        var sp = GetStart();
        var ep = GetEnd();
        if (sp != lastStartPos || ep != lastEndPos || !Mathf.Approximately(lastYOffset, yOffset))
        {
            lastStartPos = sp;
            lastEndPos = ep;
            lastYOffset = yOffset;
            return true;
        }
        return false;
    }

    Vector3 GetStart()
    {
    if (start == null) return lastStartPos;
    var p = start.position; p.y = yOffset; return p;
    }

    Vector3 GetEnd()
    {
    if (end == null) return lastEndPos;
    var p = end.position; p.y = yOffset; return p;
    }

    public void Rebuild()
    {
        if (lr == null) Setup();
    if (start == null || end == null) return;

    if (lr.sharedMaterial != material) lr.sharedMaterial = material;
        var p0 = GetStart();
        var p3 = GetEnd();

        // Adaptive segments by distance (respects a higher explicit 'segments')
        float dist = Vector3.Distance(p0, p3);
        int adaptive = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(8, 48, Mathf.InverseLerp(3f, 30f, dist))), 8, 64);
        int segCount = Mathf.Max(2, Mathf.Max(segments, adaptive));
        if (segCount != _lastSegments)
        {
            lr.positionCount = segCount + 1;
            _lastSegments = segCount;
        }

        if (!Mathf.Approximately(_lastWidth, width))
        {
            lr.startWidth = width;
            lr.endWidth = width;
            _lastWidth = width;
        }

        Vector3 dir = (p3 - p0);
        Vector3 mid = (p0 + p3) * 0.5f;
        Vector3 ortho = Vector3.Cross(dir.normalized, Vector3.up);
        float arc = curvature * dir.magnitude * 0.25f;
        Vector3 c1 = Vector3.Lerp(p0, mid, 0.5f) + ortho * arc;
        Vector3 c2 = Vector3.Lerp(mid, p3, 0.5f) + ortho * -arc;
        c1.y = yOffset; c2.y = yOffset;

        int loopSegments = _lastSegments > 0 ? _lastSegments : Mathf.Max(2, segments);
        for (int i = 0; i <= loopSegments; i++)
        {
            float t = i / (float)loopSegments;
            Vector3 pt = Bezier(p0, c1, c2, p3, t);
            lr.SetPosition(i, pt);
        }
    }

    static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u =  t;
        float uu = u * u;
        float uuu = uu * u;
        float v = 1f - u;
        float vv = v * v;
        float vvv = vv * v;
        return vvv * p0 + 3f * vv * u * p1 + 3f * v * uu * p2 + uuu * p3;
    }
}
