using System.Linq;
using UnityEngine;

// Stroke enum
public enum SwimStroke { Freestyle = 0, Backstroke = 1, Breaststroke = 2, Butterfly = 3 }

public class SwimmerController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;                   // assign the Animator on the child model
    public string[] clipNames = {               // exact clip names in your controller
        "WS_freestyle", "WS_backstroke", "WS_breaststroke", "WS_butterfly"
    };

    [Header("Stroke (meters per animation cycle)")]
    public SwimStroke stroke = SwimStroke.Freestyle;
    public float freestyleMeters  = 2.0f;
    public float backstrokeMeters = 1.8f;
    public float breastMeters     = 1.2f;
    public float butterflyMeters  = 1.5f;

    [Header("Surface Placement")]
    public float waterLevelY = 0f;              // set to your water plane Y
    public float swimDepth   = 0.06f;           // how deep to sit below surface
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 1.2f;

    [Header("Walls (Trigger-based)")]
    [Tooltip("BoxCollider with IsTrigger=ON at one end (e.g., West)")]
    public Collider wallA;
    [Tooltip("BoxCollider with IsTrigger=ON at the opposite end (e.g., East)")]
    public Collider wallB;
    [Tooltip("Optional empties at each end (surface height) for clean facing after turns.")]
    public Transform endA;
    public Transform endB;

    [Header("Turn Behaviour")]
    public bool stopAtWall = true;
    public float wallPauseSec = 0.6f;

    [Header("Playback & Control")]
    public bool running = true;                   // master run/pause
    [Range(0.25f, 2f)] public float speedScale = 1f; // scales animation + movement

    // ------------ internal ------------
    RuntimeAnimatorController _rac;
    Vector3 _dirNorm = Vector3.right;             // travel direction along the lane
    float _cycleTime = 1f;
    float _metersPerCycle = 1.5f;
    float _baseSpeedMS = 1f;                      // meters/sec at speedScale = 1
    float _speedMS = 1f;                          // meters/sec after scaling
    float _bobPhase;
    bool _turnCooldown;

    void Reset()
    {
        // Ensure we have a Rigidbody + Capsule for triggers
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (!GetComponent<CapsuleCollider>())
        {
            var cc = gameObject.AddComponent<CapsuleCollider>();
            cc.radius = 0.25f; cc.height = 1.6f; // tweak for your model
        }
    }

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        _rac = animator ? animator.runtimeAnimatorController : null;

        if (endA && endB) _dirNorm = (endB.position - endA.position).normalized;

        _bobPhase = Random.Range(0f, Mathf.PI * 2f);

        ApplyStroke(stroke);        // computes _baseSpeedMS
        ApplySpeedScale(speedScale); // sets _speedMS and animator.speed
    }

    void Update()
    {
        // quick hotkeys for testing (optional)
        if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyStroke(SwimStroke.Freestyle);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyStroke(SwimStroke.Backstroke);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyStroke(SwimStroke.Breaststroke);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyStroke(SwimStroke.Butterfly);

        if (!running) return;

        // 1) translate forward along lane (movement uses scaled speed)
        transform.position += _dirNorm * (_speedMS * Time.deltaTime);

        // 2) keep at water level with gentle bob + desired depth (THIS IS THE SNIPPET)
        var p = transform.position;
        float bob = bobAmplitude * Mathf.Sin((Time.time + _bobPhase) * (bobFrequency * 2f * Mathf.PI));
        p.y = waterLevelY - swimDepth + bob;   // surface height minus how deep to sit
        transform.position = p;

        // 3) face travel direction (horizontal only)
        Vector3 face = _dirNorm; face.y = 0f;
        if (face.sqrMagnitude > 0.0001f)
            transform.forward = face.normalized;

        // (In trigger mode, turns handled in OnTriggerEnter)
    }

    // -------- TRIGGER TURN LOGIC --------
    void OnTriggerEnter(Collider other)
    {
        if (_turnCooldown) return;

        if (other == wallA)
        {
            Vector3 dir = endB ? (endB.position - transform.position) : (-_dirNorm);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f) _dirNorm = dir.normalized;
            transform.forward = _dirNorm;
            StartCoroutine(TurnPauseCoroutine());
        }
        else if (other == wallB)
        {
            Vector3 dir = endA ? (endA.position - transform.position) : (-_dirNorm);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f) _dirNorm = dir.normalized;
            transform.forward = _dirNorm;
            StartCoroutine(TurnPauseCoroutine());
        }
    }

    System.Collections.IEnumerator TurnPauseCoroutine()
    {
        _turnCooldown = true;

        // Stop + pause animation
        bool restoreRun = running;
        if (stopAtWall) running = false;

        float oldAnimSpeed = animator ? animator.speed : 1f;
        if (animator) animator.speed = 0f;

        yield return new WaitForSeconds(wallPauseSec);

        // resume
        if (animator) animator.speed = oldAnimSpeed;
        running = restoreRun || !stopAtWall;

        // brief cooldown to avoid immediate retrigger inside the trigger volume
        yield return new WaitForSeconds(0.25f);
        _turnCooldown = false;
    }

    // ------------- PUBLIC API -------------
    public void ApplyStroke(SwimStroke s)
    {
        stroke = s;
        if (animator) animator.SetInteger("Stroke", (int)stroke);

        _metersPerCycle = stroke switch
        {
            SwimStroke.Freestyle    => freestyleMeters,
            SwimStroke.Backstroke   => backstrokeMeters,
            SwimStroke.Breaststroke => breastMeters,
            SwimStroke.Butterfly    => butterflyMeters,
            _ => 1.5f
        };

        _cycleTime   = GetClipLengthSeconds((int)stroke);
        _baseSpeedMS = (_cycleTime > 0.01f) ? _metersPerCycle / _cycleTime : _metersPerCycle;

        ApplySpeedScale(speedScale); // recompute _speedMS and animator speed
    }

    public void ApplySpeedScale(float m)
    {
        speedScale = Mathf.Clamp(m, 0.05f, 3f);
        _speedMS   = _baseSpeedMS * speedScale;                // movement scales
        if (animator) animator.speed = running ? speedScale : 0f; // stroke rate scales
    }

    public void SetRunning(bool on)
    {
        running = on;
        if (animator) animator.speed = on ? speedScale : 0f;
    }

    public void SetEnds(Transform a, Transform b)
    {
        endA = a; endB = b;
        if (endA && endB) _dirNorm = (endB.position - endA.position).normalized;
    }

    // ------------- Helpers -------------
    float GetClipLengthSeconds(int strokeIndex)
    {
        if (_rac == null || clipNames == null || clipNames.Length == 0) return 1f;
        int i = Mathf.Clamp(strokeIndex, 0, clipNames.Length - 1);
        string want = clipNames[i];
        var clip = _rac.animationClips.FirstOrDefault(c => c && c.name == want);
        return clip ? clip.length : 1f;
    }
}
