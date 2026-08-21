using System.Collections.Generic;
using UnityEngine;

public class SwimTrainer : MonoBehaviour
{
    [Header("XR Refs")]
    public Transform rig;        // XR Origin (XR Rig)
    public Transform head;       // Main Camera
    public Transform leftHand;   // Left Controller
    public Transform rightHand;  // Right Controller

    [Header("Stroke detection")]
    public float strokeThreshold = 0.6f;   // m/s backward
    public float strokeCooldown = 0.22f;   // seconds to avoid double-counts

    [Header("Session")]
    public bool autoStart = true;

    // public stats you can read from other scripts
    public float SessionTime { get; private set; }     // seconds
    public float DistanceMeters { get; private set; }  // horizontal path length
    public int Strokes { get; private set; }           // total (L+R)
    public float StrokeRateSPM { get; private set; }   // strokes per minute

    Vector3 _prevRig;
    Vector3 _prevL, _prevR;
    float _coolL, _coolR;
    bool _primed;
    readonly Queue<float> _lastStrokeTimes = new Queue<float>(); // for SPM

    void OnEnable() { _primed = false; SessionTime = 0; DistanceMeters = 0; Strokes = 0; StrokeRateSPM = 0; }

    void Update()
    {
        if (!autoStart) return;

        float dt = Time.deltaTime;
        if (dt <= 0f || rig == null || head == null || leftHand == null || rightHand == null) return;

        if (!_primed)
        {
            _prevRig = head.position;
            _prevL = leftHand.position;
            _prevR = rightHand.position;
            _primed = true;
            return;
        }

        SessionTime += dt;

        // Distance: accumulate planar displacement of the rig
        var nowHead = head.position;
        Vector3 d = nowHead - _prevRig; d.y = 0f;
        DistanceMeters += d.magnitude;
        _prevRig = nowHead;

        // Hand velocities
        Vector3 vL = (leftHand.position - _prevL) / dt;
        Vector3 vR = (rightHand.position - _prevR) / dt;
        _prevL = leftHand.position; _prevR = rightHand.position;

        Vector3 fwd = head.forward; fwd.y = 0f; fwd.Normalize();
        float backL = Mathf.Max(0f, -Vector3.Dot(vL, fwd));
        float backR = Mathf.Max(0f, -Vector3.Dot(vR, fwd));

        _coolL -= dt; _coolR -= dt;

        if (backL > strokeThreshold && _coolL <= 0f) RegisterStroke();
        if (backR > strokeThreshold && _coolR <= 0f) RegisterStroke();

        void RegisterStroke()
        {
            Strokes++;
            _lastStrokeTimes.Enqueue(SessionTime);
            while (_lastStrokeTimes.Count > 0 && SessionTime - _lastStrokeTimes.Peek() > 10f)
                _lastStrokeTimes.Dequeue(); // keep last 10s window
            StrokeRateSPM = _lastStrokeTimes.Count * 6f; // strokes per minute
            // reset the correct cooldown
            if (backL > backR) _coolL = strokeCooldown; else _coolR = strokeCooldown;
        }
    }

    // Helper for HUD formatting
    public string TimeMMSS()
    {
        int t = Mathf.FloorToInt(SessionTime);
        return $"{t/60:00}:{t%60:00}";
    }
}
