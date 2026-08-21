using UnityEngine;

public class FollowSwimmerUI : MonoBehaviour
{
    public Transform swimmer;
    public float distance = 1.5f;    // how far in front of swimmer
    public Vector3 offset = Vector3.up * 0.5f;  // vertical offset
    public bool faceSwimmer = true;  // rotate toward swimmer

    void LateUpdate()
    {
        if (!swimmer) return;

        // Position panel in front of swimmer
        Vector3 forward = swimmer.forward;
        transform.position = swimmer.position + forward * distance + offset;

        // Rotate toward swimmer (optional)
        if (faceSwimmer)
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}
