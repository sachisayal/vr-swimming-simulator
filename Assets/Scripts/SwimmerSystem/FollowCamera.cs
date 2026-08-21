using UnityEngine;
public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(-3f, 1.2f, 0f);
    public float smooth = 4f;
    void LateUpdate()
    {
        if (!target) return;
        var desired = target.position + target.transform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
        transform.LookAt(target.position + Vector3.up * 0.2f);
    }
}
