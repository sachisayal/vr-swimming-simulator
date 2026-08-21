using UnityEngine;
using UnityEngine.XR;

public class HandFXController : MonoBehaviour
{
    public Transform head;              // XR Origin ▸ Camera Offset ▸ Main Camera
    public Transform hand;              // this controller's transform
    public bool leftHand = true;        // set true on the left instance
    public ParticleSystem bubbles;      // hand bubble particle system (next section)
    public AudioSource whoosh;          // short one-shot
    public SwimLocomotion swim;         // to read IsUnderwater (optional)

    [Header("Stroke detection")]
    public float speedThreshold = 0.6f; // m/s backward to trigger
    public float hapticAmp = 0.4f;      // 0..1
    public float hapticDur = 0.06f;     // seconds

    Vector3 prev;
    float cooldown;
    InputDevice device;

    void Start()
    {
        prev = hand.position;
        device = InputDevices.GetDeviceAtXRNode(leftHand ? XRNode.LeftHand : XRNode.RightHand);
        if (whoosh) { whoosh.playOnAwake = false; whoosh.spatialBlend = 0f; }
    }

    void Update()
    {
        if (!device.isValid)
            device = InputDevices.GetDeviceAtXRNode(leftHand ? XRNode.LeftHand : XRNode.RightHand);

        float dt = Mathf.Max(Time.deltaTime, 1e-4f);
        Vector3 v = (hand.position - prev) / dt;
        prev = hand.position;

        // backward component relative to head
        Vector3 fwd = head.forward; fwd.y = 0f; fwd.Normalize();
        float backward = Mathf.Max(0f, -Vector3.Dot(v, fwd)); // >0 when pulling back
        bool under = swim ? swim.IsUnderwater : (head.position.y < 0f);

        // bubbles emission rate
        if (bubbles)
        {
            var em = bubbles.emission;
            em.enabled = under && backward > 0.1f;
            if (em.enabled) em.rateOverTime = Mathf.Lerp(2f, 25f, Mathf.Clamp01(backward));
        }

        // whoosh + haptics on strong pulls (with a small cooldown)
        cooldown -= dt;
        if (under && backward > speedThreshold && cooldown <= 0f)
        {
            if (device.isValid) device.SendHapticImpulse(0, Mathf.Clamp01(hapticAmp * backward), hapticDur);
            if (whoosh) { whoosh.volume = Mathf.Clamp01(0.15f + backward * 0.2f); whoosh.Play(); }
            cooldown = 0.12f; // avoid spamming
        }
    }
}
