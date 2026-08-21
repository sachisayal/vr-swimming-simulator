using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SwimLocomotion : MonoBehaviour
{
    [Header("XR Transforms")]
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Swim Tuning")]
    public float strokeGain = 1.6f;
    public float minStrokeSpeed = 0.25f;
    public float maxSpeed = 3.0f;
    public float drag = 2.0f;

    [Header("Water / Buoyancy")]
    [Tooltip("Y height of the water surface (your WaterSurface plane).")]
    public float waterLevelY = 0f;
    [Tooltip("How strongly you float upward when submerged.")]
    public float buoyancy = 0.6f;
    [Tooltip("How quickly vertical speed relaxes toward neutral (0).")]
    public float verticalDamping = 3.0f;
    [Tooltip("Small bob amplitude while underwater (meters).")]
    public float bobAmplitude = 0.04f;
    [Tooltip("Bob frequency while underwater (Hz).")]
    public float bobFrequency = 0.8f;

    [Header("Assist (optional)")]
    public float lateralAssist = 0.4f;
    public float upAssist = 0.2f;

    CharacterController cc;
    Vector3 velocity;
    Vector3 prevL, prevR;
    bool primed;

    public bool IsUnderwater => head != null && head.position.y < waterLevelY;

    void Awake() { cc = GetComponent<CharacterController>(); }
    void OnEnable() { primed = false; }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f || head == null || leftHand == null || rightHand == null) return;

        // Prime previous hand positions on first frame to avoid a spike
        if (!primed)
        {
            prevL = leftHand.position;
            prevR = rightHand.position;
            primed = true;
            return;
        }

        // Hand velocities (m/s)
        Vector3 vL = (leftHand.position - prevL) / dt;
        Vector3 vR = (rightHand.position - prevR) / dt;
        prevL = leftHand.position;
        prevR = rightHand.position;

        // Head-local horizontal axes
        Vector3 fwd = head.forward;  fwd.y = 0f; fwd.Normalize();
        Vector3 right = head.right;  right.y = 0f; right.Normalize();

        // Components in head space
        float vL_f = Vector3.Dot(vL, fwd);
        float vR_f = Vector3.Dot(vR, fwd);
        float vL_r = Vector3.Dot(vL, right);
        float vR_r = Vector3.Dot(vR, right);
        float vL_u = vL.y;
        float vR_u = vR.y;

        // Pulling hands backward (negative forward) => forward thrust
        float pullL = Mathf.Max(0f, -vL_f - minStrokeSpeed);
        float pullR = Mathf.Max(0f, -vR_f - minStrokeSpeed);
        float thrust = pullL + pullR;

        // Base acceleration from strokes
        Vector3 accel =
            fwd   * (thrust * strokeGain) +
            right * (lateralAssist * (-vL_r - vR_r) * 0.5f) +
            Vector3.up * (upAssist * (-vL_u - vR_u) * 0.5f);

        // Buoyancy while underwater
        if (IsUnderwater)
        {
            // Deeper -> a bit more lift (softly)
            float depth = Mathf.Clamp(waterLevelY - head.position.y, 0f, 1.5f); // meters
            float lift = buoyancy * (0.5f + 0.5f * (depth / 1.5f));
            accel += Vector3.up * lift;

            // Gentle bob
            float bob = bobAmplitude * Mathf.Sin(2f * Mathf.PI * bobFrequency * Time.time);
            velocity.y += bob * dt;
        }

        // Integrate & damp
        velocity += accel * dt;
        velocity *= Mathf.Exp(-drag * dt);                  // water drag on all axes
        velocity.y = Mathf.Lerp(velocity.y, 0f, verticalDamping * dt); // neutral buoyancy target

        // Clamp top speed
        float m = velocity.magnitude;
        if (m > maxSpeed) velocity *= (maxSpeed / m);

        // Move the rig
        cc.Move(velocity * dt);
    }
}
