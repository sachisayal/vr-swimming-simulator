using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AutoRecenterOnStart : MonoBehaviour
{
    [Tooltip("Delay before recenter (seconds) to ensure XR subsystems are running.")]
    public float recenterDelay = 0.5f;

    private void Start()
    {
        // Optionally wait a short moment to ensure the XR system is fully initialized
        if (recenterDelay > 0f)
            Invoke(nameof(DoRecenter), recenterDelay);
        else
            DoRecenter();
    }

    private void DoRecenter()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        foreach (var xrInput in subsystems)
        {
            if (xrInput != null && xrInput.running)
            {
                xrInput.TryRecenter();
                Debug.Log("XR recentered on start using XRInputSubsystem.");
                break;
            }
        }
    }
}
