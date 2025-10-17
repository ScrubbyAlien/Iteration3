using NUnit.Framework;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    private Transform follow;

    [SerializeField]
    private FollowBehaviour behaviour;

    [SerializeField, UnityEngine.Range(0, 1),
     Tooltip("0: Will not follow any movements, 1: will follow movements exactly")]
    private float parallax;

    [SerializeField, UnityEngine.Range(0f, 1f)]
    private float dampening;

    [SerializeField]
    private bool x, y;
    [SerializeField]
    private float minX, maxX;
    [SerializeField]
    private float minY, maxY;

    private Vector3 targetPosition;

    private Rigidbody2D body;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        if (body) {
            body.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void SetFollow(Transform newFollow) {
        Debug.Assert(newFollow != null, "Don't disable follow behaviour by passing null to SetFollow," +
                                        "use SetFollowDisabled() instead.");
        follow = newFollow;
    }

    public void SetFollowDisabled() {
        follow = null;
    }

    private void OnValidate() {
        if (maxX < minX) maxX = minX;
        if (maxY < minY) maxY = minY;
    }

    private void LateUpdate() {
        if (!follow) return;

        targetPosition = GetTargetPosition();

        switch (behaviour) {
            case FollowBehaviour.Dampened:
                transform.position = Vector3.Lerp(transform.position, targetPosition, dampening);
                break;
            case FollowBehaviour.Parallax:
                transform.position = GetParallaxPosition();
                break;
            case FollowBehaviour.Normal:
                transform.position = targetPosition;
                break;
            default: break;
        }
    }

    private Vector3 GetTargetPosition() {
        float newX = x ? Mathf.Clamp(follow.position.x, minX, maxX) : transform.position.x;
        float newY = y ? Mathf.Clamp(follow.position.y, minY, maxY) : transform.position.y;

        return new Vector3(newX, newY, transform.position.z);
    }

    private Vector3 GetParallaxPosition() {
        float newX = x ? Mathf.Clamp(follow.position.x * parallax, minX, maxX) : transform.position.x;
        float newY = y ? Mathf.Clamp(follow.position.y * parallax, minY, maxY) : transform.position.y;

        return new Vector3(newX, newY, transform.position.z);
    }

    public void SetFollowXAxis(bool value) => x = value;
    public void SetFollowyAxis(bool value) => y = value;

    public void SetMaxX(float value) => maxX = value;
    public void SetMaxY(float value) => maxY = value;

    public void SetMinX(float value) => minX = value;
    public void SetMinY(float value) => minY = value;

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        if (x) {
            Gizmos.DrawLine(new Vector3(minX, 1000, 0), new Vector3(minX, -1000, 0));
            Gizmos.DrawLine(new Vector3(maxX, 1000, 0), new Vector3(maxX, -1000, 0));
        }

        if (y) {
            Gizmos.DrawLine(new Vector3(1000, minY, 0), new Vector3(-1000, minY, 0));
            Gizmos.DrawLine(new Vector3(1000, maxY, 0), new Vector3(-1000, maxY, 0));
        }
    }

    private enum FollowBehaviour
    {
        Normal = 0,
        Dampened = 1,
        Parallax = 2
    }
}