using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class GeometryBody : MonoBehaviour
{
    [SerializeField]
    private float gravityConstant;
    public float g => gravityConstant;
    private new BoxCollider2D collider;
    private Vector2 velocity;

    [SerializeField]
    private GeometryBodyEvents events;
    
    
    private void Awake() {
        collider = GetComponent<BoxCollider2D>();
    }
    
    private void Update() {
        if (Physics2D.simulationMode != SimulationMode2D.Script) return;
        Physics2D.Simulate(Time.deltaTime);

        // Apply gravity
        SetYVelocity(velocity.y + g * Time.deltaTime);
        
        List<Collider2D> others = new();
        if (collider.Overlap(others) > 0) others.ForEach(other => {
            events.OnOverlap?.Invoke(other);
            OnOverlap(other);
        });
        
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void ApplyGravity() {
        
    }

    private void OnOverlap(Collider2D other) {
        ColliderDistance2D distance = collider.Distance(other);
        Vector3 offset = distance.normal * distance.distance;
        SetPosition(transform.position + offset);
        SetYVelocity(0);
    }

    public void SetGravity(float newGravityConstant) {
        gravityConstant = newGravityConstant;
    }

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

    [Serializable]
    private struct GeometryBodyEvents
    {
        public UnityEvent<Collider2D> OnOverlap;
    }
}