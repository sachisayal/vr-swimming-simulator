using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[AddComponentMenu("XR/Locomotion/Vertical Move Provider")]
public class VerticalMoveProvider : MonoBehaviour
{
    [Header("Input (1D Axis: -1..+1)")]
    [Tooltip("Bind this to the 'Translate Vertical' action (Q/E, T/Y) from the XR Device Controller Controls asset.")]
    public InputActionProperty verticalAction;

    [Header("Motion")]
    [Tooltip("Units per second to move along Up/Down.")]
    public float speed = 2f;

    [Tooltip("What transform to move (usually the XROrigin root). If null, this GameObject is moved.")]
    public Transform rootToMove;

    [Tooltip("Use world up (Vector3.up). If false, uses rootToMove.up.")]
    public bool worldUp = true;

    CharacterController _cc;
    Transform _root;

    void Awake()
    {
        _root = rootToMove != null ? rootToMove : transform;

        // Use a CharacterController if present (common on the XR Origin root)
        _cc = _root.GetComponent<CharacterController>();
        if (_cc == null && _root != transform)
            _cc = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        if (verticalAction.action != null)
            verticalAction.action.Enable();
    }

    void OnDisable()
    {
        if (verticalAction.action != null)
            verticalAction.action.Disable();
    }

    void Update()
    {
        if (verticalAction.action == null) return;

        float axis = verticalAction.action.ReadValue<float>(); // -1 (down) .. +1 (up)
        if (Mathf.Abs(axis) < 0.001f) return;

        Vector3 up = worldUp ? Vector3.up : _root.up;
        Vector3 delta = up * (axis * speed * Time.deltaTime);

        if (_cc != null && _cc.enabled)
            _cc.Move(delta);
        else
            _root.position += delta;
    }
}
