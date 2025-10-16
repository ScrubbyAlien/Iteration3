using UnityEngine;

[RequireComponent(typeof(GeometryBody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private GeometryBody body;


    private void Awake() {
        body = GetComponent<GeometryBody>();
        body.SetXVelocity(speed);
    }
}