using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public bool matchRotation = true;

    void LateUpdate()
    {
        if (!target) return;
        transform.position = target.position;
        if (matchRotation) transform.rotation = target.rotation;
    }
}
