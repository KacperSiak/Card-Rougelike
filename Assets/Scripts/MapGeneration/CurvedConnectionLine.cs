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
        Rebuild();
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
    lr.sharedMaterial = material;
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
        var p = start.position; p.y = yOffset; return p;
    }

    Vector3 GetEnd()
    {
        var p = end.position; p.y = yOffset; return p;
    }

    public void Rebuild()
    {
        if (lr == null) Setup();

    if (lr.sharedMaterial != material) lr.sharedMaterial = material;
        lr.positionCount = Mathf.Max(2, segments + 1);

        lr.startWidth = width;
        lr.endWidth = width;

        var p0 = GetStart();
        var p3 = GetEnd();

        Vector3 dir = (p3 - p0);
        Vector3 mid = (p0 + p3) * 0.5f;
        Vector3 ortho = Vector3.Cross(dir.normalized, Vector3.up);
        float arc = curvature * dir.magnitude * 0.25f;
        Vector3 c1 = Vector3.Lerp(p0, mid, 0.5f) + ortho * arc;
        Vector3 c2 = Vector3.Lerp(mid, p3, 0.5f) + ortho * -arc;
        c1.y = yOffset; c2.y = yOffset;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
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
