using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class GeometryBody : MonoBehaviour
{
    public event Action<Collider2D> OnCollide;

    [SerializeField, Range(0, 0.3f)]
    private float maxDeltaTime;
    private float deltaTime => Mathf.Min(Time.deltaTime, maxDeltaTime);
    [SerializeField]
    private float gravityConstant;
    private float baseG;
    public float g => gravityConstant;
    private new BoxCollider2D collider;
    private Vector2 velocity;
    
    [SerializeField]
    private GeometryBodyEvents events;
    [Serializable]
    private struct GeometryBodyEvents
    {
        public UnityEvent<Collider2D> OnOverlap;
    }

    private bool ignoreCollision;

    private Vector2 lastPosition;
    private Vector2 nextPosition;
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
        baseG = gravityConstant;
        lastPosition = transform.position;
        nextPosition = transform.position;
    }
    
    private void Update() {
        if (Physics2D.simulationMode != SimulationMode2D.Script) return;
        Physics2D.Simulate(deltaTime);

        // Apply gravity
        SetYVelocity(velocity.y + g * deltaTime);
        
        // todo maybe check for 
        if (!ignoreCollision) {
            List<Collider2D> others = new();
            if (collider.Overlap(others) > 0) others.ForEach(other => {
                events.OnOverlap?.Invoke(other);
                OnOverlap(other);
            });
        }
        
        transform.position += (Vector3)velocity * deltaTime;

        ignoreCollision = false;
    }

    // private void InterpolatePosition() {
    //     nextPosition = transform.position + (Vector3)velocity * deltaTime;
    //     
    //     RaycastHit2D hit = Physics2D.Linecast(lastPosition, nextPosition);
    //     if (nextPosition == lastPosition || !hit) {
    //         transform.position = nextPosition;
    //         lastPosition = transform.position;
    //         return;
    //     }
    //     
    //     Vector2 diff = nextPosition - lastPosition;
    //     float h = collider.bounds.extents.y;
    //     if (diff.x == 0) {
    //         transform.position = hit.point + new Vector2(0, h);
    //     }
    //     else {
    //         float slope = diff.y / diff.x;
    //         transform.position = hit.point + new Vector2(slope / h, h);
    //     }
    //     
    //     lastPosition = transform.position;
    //     return;
    // }
    
    private void OnOverlap(Collider2D other) {
        ColliderDistance2D distance = collider.Distance(other);
        Vector3 offset = distance.normal * distance.distance;
        SetPosition(transform.position + offset);
        SetYVelocity(0);
        OnCollide?.Invoke(other);
    }

    public void IgnoreCollisionsFor1Frame() {
        ignoreCollision = true;
    }
    
    public void SetGravity(float newGravityConstant) {
        gravityConstant = newGravityConstant;
    }
    public void ResetGravity() => gravityConstant = baseG;
    
    public void SetXVelocity(float xVelocity) {
        velocity = new Vector2(xVelocity, velocity.y);
    }

    public void SetYVelocity(float yVelocity) {
        velocity = new Vector2(velocity.x, yVelocity);
    }

    public void SetVelocity(Vector2 newVelocity) {
        velocity = newVelocity;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }
    
    public void SetParameters(float newG = 0, Vector3 position = default(Vector3), float newX = 0, float newY = 0) {
        SetGravity(newG);
        SetPosition(position);
        SetXVelocity(newX);
        SetYVelocity(newY);
    }
}