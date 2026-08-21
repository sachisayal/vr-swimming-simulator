using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class VRStrokeController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputAction leftHandMove;
    public InputAction rightHandMove;

    [Header("References")]
    public SwimmerController swimmer;
    public SwimStroke currentStroke = SwimStroke.Freestyle;

    [Header("Controllers (Haptics)")]
    public HapticImpulsePlayer leftController;
    public HapticImpulsePlayer rightController;

    private void OnEnable()
    {
        leftHandMove?.Enable();
        rightHandMove?.Enable();
    }

    private void OnDisable()
    {
        leftHandMove?.Disable();
        rightHandMove?.Disable();
    }

    void Update()
    {
        Vector3 leftVel = leftHandMove.ReadValue<Vector3>();
        Vector3 rightVel = rightHandMove.ReadValue<Vector3>();

        // Average stroke velocity magnitude
        float combinedSpeed = (leftVel.magnitude + rightVel.magnitude) * 0.5f;

        // Threshold to trigger stroke motion
        float strokeThreshold = 0.3f;

        if (combinedSpeed > strokeThreshold)
        {
            // Trigger stroke animation if moving
            swimmer.ApplyStroke(currentStroke);

            // Smoothly scale swimming speed based on velocity magnitude
            float scaledSpeed = Mathf.Lerp(0.6f, 2f, Mathf.Clamp01(combinedSpeed / 3f));
            swimmer.ApplySpeedScale(scaledSpeed);

            // Trigger haptics proportional to velocity
            float hapticStrength = Mathf.Clamp01(combinedSpeed / 3f);
            PlayHaptic(leftController, hapticStrength, 0.1f);
            PlayHaptic(rightController, hapticStrength, 0.1f);
        }
    }

    private void PlayHaptic(HapticImpulsePlayer player, float amplitude, float duration)
    {
        if (player == null)
            return;

        try
        {
            player.SendHapticImpulse(amplitude, duration);
        }
        catch
        {
            var method = player.GetType().GetMethod("PlayHapticImpulse");
            if (method != null)
                method.Invoke(player, new object[] { amplitude, duration });
        }
    }
}
